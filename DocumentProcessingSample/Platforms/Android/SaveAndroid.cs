using Android.Content;
using Android.OS;
using Java.IO;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DocumentProcessingSample.Services
{
    public partial class SaveService
    {
        public static async Task<string?> PlatformSaveAsAsync(string fileName, Stream fileStream)
        {
            string? root = null;

            if (Android.OS.Environment.IsExternalStorageEmulated)
            {
                root = Android.App.Application.Context!.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads)!.AbsolutePath;
            }
            else
            {
                root = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            }

            Java.IO.File myDir = new(root + "/Syncfusion");
            myDir.Mkdir();

            Java.IO.File file = new(myDir, fileName);

            if (file.Exists())
            {
                file.Delete();
            }

            try
            {
                FileOutputStream outs = new(file);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                outs.Write(buffer);
                outs.Flush();
                outs.Close();
                return file.AbsolutePath;
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Failed to save file: {ex.Message}", "OK");
                return null;
            }
        }

        public partial void SaveAndView(string filename, string contentType, MemoryStream stream)
        {
            string? root = null;

            if (Android.OS.Environment.IsExternalStorageEmulated)
            {
                root = Android.App.Application.Context!.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads)!.AbsolutePath;
            }
            else
            {
                root = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            }

            Java.IO.File myDir = new(root + "/Syncfusion");
            myDir.Mkdir();

            Java.IO.File file = new(myDir, filename);

            if (file.Exists())
            {
                file.Delete();
            }

            try
            {
                FileOutputStream outs = new(file);
                outs.Write(stream.ToArray());
                outs.Flush();
                outs.Close();
            }
            catch (Exception)
            {
            }

            if (file.Exists())
            {
                if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    var fileUri = AndroidX.Core.Content.FileProvider.GetUriForFile(Android.App.Application.Context, Android.App.Application.Context.PackageName + ".provider", file);
                    var intent = new Intent(Intent.ActionView);
                    intent.SetData(fileUri);
                    intent.AddFlags(ActivityFlags.NewTask);
                    intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                    Android.App.Application.Context.StartActivity(intent);
                }
                else
                {
                    var fileUri = Android.Net.Uri.Parse(file.AbsolutePath);
                    var intent = new Intent(Intent.ActionView);
                    intent.SetDataAndType(fileUri, contentType);
                    intent = Intent.CreateChooser(intent, "Open File");
                    intent!.AddFlags(ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intent);
                }
            }
        }
    }
}
