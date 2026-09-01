using DocumentProcessingSample.Services;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DocumentProcessingSample.Views;

public partial class PdfViewerSamplesPage : ContentPage, INotifyPropertyChanged
{
    private string currentFileName = "PDF_Succinctly1.pdf";
    Button? fileOpenButton, fileSaveButton;

    public PdfViewerSamplesPage()
    {
        InitializeComponent();
        LoadPdf("PDF_Succinctly1");
        PdfViewer.Tapped += PdfViewer_Tapped;
        AddFileOperationsToolbarItems();
    }

    private void PdfViewer_Tapped(object? sender, Syncfusion.Maui.PdfViewer.GestureEventArgs? e)
    {
        FilePickerFrame.IsVisible = false;
    }

    private void LoadPdf(string fileName)
    {
        var stream = GetType().Assembly.GetManifestResourceStream($"DocumentProcessingSample.Pdf.{fileName}.pdf");
        PdfViewer.LoadDocument(stream);
    }

    private void AddFileOperationsToolbarItems()
    {
        // Setup FilePickerView event handlers
        FilePickerView.FileSelected += FilePickerView_FileSelected;
        FilePickerView.FileSelectedFromBrowse += FilePickerView_FileSelectedFromBrowse;
        FilePickerView.CloseRequested += FilePickerView_CloseRequested;

        fileOpenButton = new Button
        {
            Text = "\ue712",
            FontSize = 20,
            FontFamily = "MauiMaterialAssets",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = Colors.Transparent,
            BorderColor = Colors.Transparent,
            Padding = 10,
            Margin = new Thickness(5, 0, 0, 0),
            TextColor = Colors.Black
        };
        fileOpenButton.Clicked += FileOpenButton_Clicked;

        fileSaveButton = new Button
        {
            Text = "\ue75f",
            FontSize = 20,
            FontFamily = "MauiMaterialAssets",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = Colors.Transparent,
            BorderColor = Colors.Transparent,
            Padding = 10,
            Margin = new Thickness(5, 0, 0, 0),
            TextColor = Colors.Black
        };
        fileSaveButton.Clicked += FileSaveButton_Clicked!;

#if !WINDOWS && !MACCATALYST
        fileOpenButton.CornerRadius = 5;
        fileSaveButton.CornerRadius = 5;
        PdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(0, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileOpenButton, "FileOpenButton"));
        PdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(1, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileSaveButton, "FileSaveButton"));
#else
        PdfViewer?.Toolbars?.GetByName("PrimaryToolbar")?.Items?.Insert(0, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileOpenButton, "FileOpenButton"));
        PdfViewer?.Toolbars?.GetByName("PrimaryToolbar")?.Items?.Insert(1, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileSaveButton, "FileSaveButton"));
#endif
    }

    private void FilePickerView_FileSelectedFromBrowse(object? sender, PdfFileData e)
    {
        currentFileName = e.FileName;
        PdfViewer.LoadDocument(e.Stream);
        FilePickerFrame.IsVisible = false;
    }

    private void PdfViewer_DocumentLoaded(object? sender, EventArgs? e)
    {
        if (fileOpenButton != null)
        {
            fileOpenButton.IsEnabled = true;
            fileOpenButton.Opacity = 1;
        }
        if (fileSaveButton != null)
        {
            fileSaveButton.IsEnabled = true;
            fileSaveButton.Opacity = 1;
        }
    }

    private void PdfViewer_DocumentUnloaded(object? sender, EventArgs? e)
    {
        if (fileOpenButton != null)
        {
            fileOpenButton.IsEnabled = false;
            fileOpenButton.Opacity = 0.5;
        }
        if (fileSaveButton != null)
        {
            fileSaveButton.IsEnabled = false;
            fileSaveButton.Opacity = 0.5;
        }
    }

    private void FileOpenButton_Clicked(object? sender, EventArgs e)
    {
        // Show file picker popup
        FilePickerFrame.IsVisible = true;
    }

    private void FilePickerView_FileSelected(object? sender, string fileName)
    {
        currentFileName = fileName;
        LoadPdfFile(fileName);
        FilePickerFrame.IsVisible = false;
    }

    private void FilePickerView_CloseRequested(object? sender, EventArgs e)
    {
        FilePickerFrame.IsVisible = false;
    }

    private void LoadPdfFile(string fileName)
    {
        try
        {
            var stream = GetType().Assembly.GetManifestResourceStream($"DocumentProcessingSample.Pdf.{fileName}");
            if (stream != null)
            {
                PdfViewer.LoadDocument(stream);
                currentFileName = fileName;
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                    await DisplayAlert("Error", $"PDF file '{fileName}' not found in resources.", "OK"));
            }
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
                await DisplayAlert("Error", $"Failed to load file: {ex.Message}", "OK"));
        }
    }

    private async void FileSaveButton_Clicked(object? sender, EventArgs e)
    {
        try
        {
            Stream savedStream = new MemoryStream();
            await PdfViewer.SaveDocumentAsync(savedStream);
            
            if (!string.IsNullOrEmpty(currentFileName))
            {
                try
                {
                    string? filePath = await FileService.SaveAsAsync(currentFileName, savedStream);
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        await DisplayAlert("Success", $"File saved successfully", "OK");
                    }
                }
                catch { }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save file: {ex.Message}", "OK");
        }
    }
}
