using Uri = Android.Net.Uri;
using Application = Android.App.Application;
using Environment = Android.OS.Environment;
using Android.Content;
using Android.Webkit;
using Android.Provider;
using System.Web;
using Java.IO;

namespace DocumentProcessingSample.Services
{
    public partial class FileService
    {
        internal static partial async Task<string?> PlatformSaveAsAsync(string fileName, Stream stream)
        {
            Uri? filePath = null;
            CancellationToken cancellationToken = CancellationToken.None;

            if (!OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                var status = await Permissions.RequestAsync<Permissions.StorageWrite>().WaitAsync(cancellationToken).ConfigureAwait(false);
                if (status is not PermissionStatus.Granted)
                {
                    throw new PermissionException("Storage permission is not granted.");
                }
            }

            var intent = new Intent(Intent.ActionCreateDocument);

            intent.AddCategory(Intent.CategoryOpenable);
            intent.SetType(MimeTypeMap.Singleton?.GetMimeTypeFromExtension(MimeTypeMap.GetFileExtensionFromUrl(fileName)) ?? "*/*");
            intent.PutExtra(Intent.ExtraTitle, fileName);
            await IntermediateActivity.StartAsync(intent, 2001, onResult: OnResult).WaitAsync(cancellationToken).ConfigureAwait(false);

            if (filePath is null)
            {
                throw new Exception("User canceled or error in saving.");
            }

            return await SaveDocument(filePath, stream, cancellationToken).ConfigureAwait(false);

            void OnResult(Intent resultIntent)
            {
                filePath = resultIntent.Data;
            }
        }

        static async Task<string?> SaveDocument(Uri uri, Stream stream, CancellationToken cancellationToken)
        {
            using var parcelFileDescriptor = Application.Context.ContentResolver?.OpenFileDescriptor(uri, "wt");
            using var fileOutputStream = new FileOutputStream(parcelFileDescriptor?.FileDescriptor);
            await using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
            await fileOutputStream.WriteAsync(memoryStream.ToArray()).WaitAsync(cancellationToken).ConfigureAwait(false);

            return ConvertToPhysicalPath(uri);
        }

        static string? ConvertToPhysicalPath(Uri uri)
        {
            const string uriSchemeFolder = "content";
            if (uri.Scheme is null || !uri.Scheme.Equals(uriSchemeFolder, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (uri.PathSegments?.Count < 2)
            {
                return null;
            }

            var path = uri.PathSegments?[1];

            if (path is null)
            {
                return null;
            }

            var pathSplit = path.Split(':');
            if (pathSplit.Length < 2)
            {
                if (pathSplit.Length == 1)
                {
                    return $"{Environment.ExternalStorageDirectory?.Path}/Downloads";
                }
                return null;
            }

            if (pathSplit[0].Equals("primary", StringComparison.OrdinalIgnoreCase))
            {
                return $"{Environment.ExternalStorageDirectory?.Path}/{pathSplit[1]}";
            }

            return $"/storage/{pathSplit[0]}/{pathSplit[1]}";
        }
    }
}
