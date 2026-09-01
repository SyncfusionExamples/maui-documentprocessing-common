using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;

namespace DocumentProcessingSample.Services
{
    public partial class SaveService
    {
        public async partial void SaveAndView(string filename, string contentType, MemoryStream stream)
        {
            StorageFile? stFile = null;
            string extension = Path.GetExtension(filename);
            IntPtr windowHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

            if (!Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                FileSavePicker savePicker = new();
                if (extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    savePicker.DefaultFileExtension = ".pdf";
                    savePicker.SuggestedFileName = filename;
                    savePicker.FileTypeChoices.Add("PDF Document", new List<string>() { ".pdf" });
                }
                else if (extension.Equals(".docx", StringComparison.OrdinalIgnoreCase))
                {
                    savePicker.DefaultFileExtension = ".docx";
                    savePicker.SuggestedFileName = filename;
                    savePicker.FileTypeChoices.Add("Word Document", new List<string>() { ".docx" });
                }
                else if (extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    savePicker.DefaultFileExtension = ".xlsx";
                    savePicker.SuggestedFileName = filename;
                    savePicker.FileTypeChoices.Add("Excel Workbook", new List<string>() { ".xlsx" });
                }
                else if (extension.Equals(".pptx", StringComparison.OrdinalIgnoreCase))
                {
                    savePicker.DefaultFileExtension = ".pptx";
                    savePicker.SuggestedFileName = filename;
                    savePicker.FileTypeChoices.Add("PowerPoint Presentation", new List<string>() { ".pptx" });
                }
                else
                {
                    savePicker.DefaultFileExtension = extension;
                    savePicker.SuggestedFileName = filename;
                    savePicker.FileTypeChoices.Add("Document", new List<string>() { extension });
                }

                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, windowHandle);
                stFile = await savePicker.PickSaveFileAsync();
            }
            else
            {
                StorageFolder local = ApplicationData.Current.LocalFolder;
                stFile = await local.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            }

            if (stFile != null)
            {
                using (IRandomAccessStream zipStream = await stFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using Stream outstream = zipStream.AsStreamForWrite();
                    outstream.SetLength(0);
                    byte[] buffer = stream.ToArray();
                    outstream.Write(buffer, 0, buffer.Length);
                    outstream.Flush();
                }

                MessageDialog msgDialog = new("Do you want to view the document?", "File has been created successfully");
                UICommand yesCmd = new("Yes");
                msgDialog.Commands.Add(yesCmd);
                UICommand noCmd = new("No");
                msgDialog.Commands.Add(noCmd);

                WinRT.Interop.InitializeWithWindow.Initialize(msgDialog, windowHandle);

                IUICommand cmd = await msgDialog.ShowAsync();
                if (cmd.Label == yesCmd.Label)
                {
                    await Windows.System.Launcher.LaunchFileAsync(stFile);
                }
            }
        }
    }
}
