namespace DocumentProcessingSample.Views;

using DocumentProcessingSample.Services;

public partial class PdfViewerFilePickerView : ContentView
{
    private List<string> pdfFiles = new();
    public event EventHandler<string>? FileSelected;
    public event EventHandler? CloseRequested;

    public PdfViewerFilePickerView()
    {
        InitializeComponent();
        LoadAvailablePdfs();
    }

    private void LoadAvailablePdfs()
    {
        pdfFiles.Clear();
        
        // Add PDF files from the DocumentProcessingSample resources
        pdfFiles.Add("PDF_Succinctly");
        pdfFiles.Add("Rotated_document.pdf");
        pdfFiles.Add("Password_protected_document.pdf");
        pdfFiles.Add("Single_page_document.pdf");
        pdfFiles.Add("Annotations_document.pdf");
        pdfFiles.Add("form_document.pdf");
        pdfFiles.Add("Browse files on this device");

        FileListView.ItemsSource = pdfFiles;
    }

    private void FileListView_ItemTapped(object? sender, Syncfusion.Maui.ListView.ItemTappedEventArgs? e)
    {
        if (e?.DataItem is string displayName)
        {
            string fileName = string.Empty;
            int tappedIndex = pdfFiles.IndexOf(displayName);
            
            if (tappedIndex == 0)
                fileName = "PDF_Succinctly1.pdf";
            else if (tappedIndex == 1)
                fileName = "rotated_document.pdf";
            else if (tappedIndex == 2)
                fileName = "password_protected_document.pdf";
            else if (tappedIndex == 3)
                fileName = "Invoice.pdf";
            else if (tappedIndex == 4)
                fileName = "Annotations.pdf";
            else if (tappedIndex == 5)
                fileName = "form_document.pdf";
            else if (tappedIndex == 6)
            {
                // Browse device
                BrowseDevice_Clicked(null, EventArgs.Empty);
                FileListView.SelectedItem = null;
                return;
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                FileSelected?.Invoke(this, fileName);
            }
            
            FileListView.SelectedItem = null;
        }
    }

    private async void BrowseDevice_Clicked(object? sender, EventArgs e)
    {
        PdfFileData? fileData = await FileService.OpenFile("pdf");
        if (fileData != null)
        {
            FileSelected?.Invoke(this, fileData.FileName);
        }
    }

    private void Close_Clicked(object sender, EventArgs e)
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}
