using DocumentProcessingSample.Services;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;

namespace DocumentProcessingSample.Views;

/// <summary>
/// WordSamplesPage demonstrates seven key features of Syncfusion DocIO:
/// 1. Word to PDF conversion (converting DOCX documents to PDF format)
/// 2. Clone and merge (importing content from source document into destination document)
/// 3. Execute Mail merge (populating tables with employee data)
/// 4. DOCX to DOCX conversion (loading and saving DOCX documents)
/// 5. DOCX to HTML conversion (converting HTML files to Word format)
/// 6. DOCX to Markdown conversion (converting DOCX documents to Markdown format)
/// 7. Retrieve Bookmark Content (creating, finding, and extracting bookmark content)
/// 
/// All source documents are loaded from Resources/Word folder using embedded resources.
/// Reference: docio/Skill/SKILL.md and corresponding .md files in references/
/// </summary>
public partial class WordSamplesPage : ContentPage
{
    private const string ResourceBasePath = "DocumentProcessingSample.Resources.Word.";

    public WordSamplesPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Feature 1: DOCX to PDF conversion - convert DOCX documents to PDF format.
    /// Demonstrates using DocIORenderer to convert DOCX documents to PDF with proper formatting preservation.
    /// Reference: docx-to-pdf-conversion.md
    /// </summary>
    private async void OnWordToPdfClicked(object sender, EventArgs e)
    {
        try
        {
            // Load sample DOCX from embedded resources
            var assembly = GetType().Assembly;
            using (Stream? docStream = assembly.GetManifestResourceStream(ResourceBasePath + "Sample.docx"))
            {
                if (docStream == null)
                {
                    await DisplayAlert("Error", "Sample.docx not found in resources", "OK");
                    return;
                }

                // Load the DOCX document
                using (WordDocument document = new(docStream, FormatType.Docx))
                {
                    // Create a DocIORenderer instance to convert DOCX to PDF
                    DocIORenderer renderer = new();
                    
                    // Convert the DOCX document to PDF
                    PdfDocument pdfDocument = renderer.ConvertToPDF(document);
                    
                    // Save the PDF to a MemoryStream
                    MemoryStream outputStream = new();
                    pdfDocument.Save(outputStream);
                    outputStream.Position = 0;

                    new SaveService().SaveAndView("DocxToPdf_Converted.pdf", "application/pdf", outputStream);
                    
                    pdfDocument.Close();
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to convert DOCX to PDF: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Feature 2: Clone and merge - import content from source document into destination document.
    /// Demonstrates using ImportContent to merge multiple documents while preserving destination formatting.
    /// Reference: clone-merge.md
    /// </summary>
    private async void OnCloneAndMergeClicked(object sender, EventArgs e)
    {
        try
        {
            // Load source and destination documents from embedded resources
            var assembly = GetType().Assembly;
            using (Stream? sourceStream = assembly.GetManifestResourceStream(ResourceBasePath + "SourceDocument.docx"))
            using (Stream? destinationStream = assembly.GetManifestResourceStream(ResourceBasePath + "DestinationDocument.docx"))
            {
                if (sourceStream == null)
                {
                    await DisplayAlert("Error", "SourceDocument.docx not found in resources", "OK");
                    return;
                }

                if (destinationStream == null)
                {
                    await DisplayAlert("Error", "DestinationDocument.docx not found in resources", "OK");
                    return;
                }

                // Opens the source document from file stream through constructor of WordDocument class
                using (WordDocument sourceDocument = new(sourceStream, FormatType.Automatic))
                {
                    // Opens the destination document
                    WordDocument destinationDocument = new(destinationStream, FormatType.Docx);
                    
                    // Imports the contents of source document at the end of destination document
                    destinationDocument.ImportContent(sourceDocument, ImportOptions.UseDestinationStyles);
                    
                    // Saves and closes the destination document to a MemoryStream
                    MemoryStream outputStream = new();
                    destinationDocument.Save(outputStream, FormatType.Docx);
                    outputStream.Position = 0;

                    new SaveService().SaveAndView("CloneAndMerge_Result.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputStream);
                    
                    destinationDocument.Close();
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to clone and merge: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Feature 3: Mail merge - populate template with employee data.
    /// Demonstrates using MailMerge.Execute to populate merge fields (Emp_Id, Name, Phone, City).
    /// Reference: mail-merge.md
    /// </summary>
    private async void OnMailMergeClicked(object sender, EventArgs e)
    {
        try
        {
            // Load mail merge template from embedded resources
            var assembly = GetType().Assembly;
            using (Stream? mailmergeStream = assembly.GetManifestResourceStream(ResourceBasePath + "Mailmerge.docx"))
            {
                if (mailmergeStream == null)
                {
                    await DisplayAlert("Error", "Mailmerge.docx not found in resources", "OK");
                    return;
                }

                // Load the mail merge template document
                WordDocument document = new(mailmergeStream, FormatType.Docx);

                // Employee data
                var employees = new[]
                {
                    new { Emp_Id = "1001", Name = "John Smith", Phone = "555-0101", City = "New York" }
                };

                // Define field names and values for mail merge
                string[] fieldNames = new string[] { "Emp_Id", "Name", "Phone", "City" };

                // Create output document to collect all merged results
                WordDocument outputDoc = new();
                outputDoc.EnsureMinimal();

                // Perform mail merge for each employee record
                for (int i = 0; i < employees.Length; i++)
                {
                    // Reload document for each merge to get fresh copy
                    if (i > 0)
                    {
                        document.Close();
                        mailmergeStream.Seek(0, SeekOrigin.Begin);
                        document = new WordDocument(mailmergeStream, FormatType.Docx);
                    }

                    // Create field values array for current employee
                    string[] fieldValues = new string[]
                    {
                        employees[i].Emp_Id,
                        employees[i].Name,
                        employees[i].Phone,
                        employees[i].City
                    };

                    // Performs the mail merge
                    document.MailMerge.Execute(fieldNames, fieldValues);

                    // Copy the merged document content to output document
                    if (i > 0)
                        outputDoc.AddSection();

                    // Copy all body items from merged document to output
                    for (int j = 0; j < document.LastSection.Body.ChildEntities.Count; j++)
                        outputDoc.LastSection.Body.ChildEntities.Add(document.LastSection.Body.ChildEntities[j].Clone());
                }

                // Save the output document
                MemoryStream outputStream = new();
                outputDoc.Save(outputStream, FormatType.Docx);
                outputStream.Position = 0;

                new SaveService().SaveAndView("MailMerge_Employee.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputStream);
                
                outputDoc.Close();
                document.Close();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to perform mail merge: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Feature 4: Load DOCX document from resources and convert to DOCX.
    /// This demonstrates the basic load-and-save workflow for Word documents.
    /// Reference: document-structure.md
    /// </summary>
    private async void OnDocxToDocxClicked(object sender, EventArgs e)
    {
        try
        {
            // Load sample DOCX from embedded resources
            var assembly = GetType().Assembly;
            using (Stream? docStream = assembly.GetManifestResourceStream(ResourceBasePath + "Sample.docx"))
            {
                if (docStream == null)
                {
                    await DisplayAlert("Error", "sample.docx not found in resources", "OK");
                    return;
                }

                // Load the document using the constructor with FormatType
                WordDocument doc = new(docStream, FormatType.Docx);

                // Save to output stream
                MemoryStream outputStream = new();
                doc.Save(outputStream, FormatType.Docx);
                outputStream.Position = 0;

                new SaveService().SaveAndView("DocxToDocx_Converted.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputStream);
                doc.Close();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to convert DOCX: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Feature 5: Convert DOCX document to HTML format.
    /// Demonstrates converting a Word document to HTML format while preserving structure and formatting.
    /// Reference: docx-to-html-conversion.md
    /// </summary>
    private async void OnHtmlToDocxClicked(object sender, EventArgs e)
    {
        try
        {
            // Load sample DOCX from embedded resources
            var assembly = GetType().Assembly;
            using (Stream? docStream = assembly.GetManifestResourceStream(ResourceBasePath + "Sample.docx"))
            {
                if (docStream == null)
                {
                    await DisplayAlert("Error", "Sample.docx not found in resources", "OK");
                    return;
                }

                // Load the DOCX document
                WordDocument doc = new(docStream, FormatType.Docx);

                // Save to HTML format
                MemoryStream outputStream = new();
                doc.Save(outputStream, FormatType.Html);
                outputStream.Position = 0;

                new SaveService().SaveAndView("DocxToHtml_Converted.html", "text/html", outputStream);
                doc.Close();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to convert DOCX to HTML: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Feature 6: Convert DOCX document to Markdown format.
    /// Demonstrates converting a Word document to Markdown while preserving structure and formatting.
    /// Reference: docx-to-markdown-conversion.md
    /// </summary>
    private async void OnDocxToMarkdownClicked(object sender, EventArgs e)
    {
        try
        {
            // Load sample DOCX from embedded resources
            var assembly = GetType().Assembly;
            using (Stream? docStream = assembly.GetManifestResourceStream(ResourceBasePath + "Sample.docx"))
            {
                if (docStream == null)
                {
                    await DisplayAlert("Error", "Sample.docx not found in resources", "OK");
                    return;
                }

                // Load the DOCX document
                WordDocument doc = new(docStream, FormatType.Docx);

                // Save to Markdown format
                MemoryStream outputStream = new();
                doc.Save(outputStream, FormatType.Markdown);
                outputStream.Position = 0;

                new SaveService().SaveAndView("DocxToMarkdown_Converted.md", "text/markdown", outputStream);
                doc.Close();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to convert DOCX to Markdown: {ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Feature 7: Bookmark operations - create bookmarks and retrieve content.
    /// Demonstrates retrieving bookmark content using BookmarksNavigator.
    /// Reference: bookmarks.md
    /// </summary>
    private async void OnRetrieveBookmarkClicked(object sender, EventArgs e)
    {
        try
        {
            // Load bookmark document from embedded resources
            var assembly = GetType().Assembly;
            using (Stream? bookmarkStream = assembly.GetManifestResourceStream(ResourceBasePath + "Bookmark.docx"))
            {
                if (bookmarkStream == null)
                {
                    await DisplayAlert("Error", "Bookmark.docx not found in resources", "OK");
                    return;
                }

                // Load the bookmark document
                WordDocument document = new(bookmarkStream, FormatType.Docx);

                // Creates the bookmark navigator instance to access the bookmark
                BookmarksNavigator bookmarkNavigator = new BookmarksNavigator(document);
                
                // Moves the virtual cursor to the location before the end of the bookmark "Northwind"
                bookmarkNavigator.MoveToBookmark("Northwind");
                
                // Gets the bookmark content
                TextBodyPart part = bookmarkNavigator.GetBookmarkContent();
                
                // Create a new document to display the retrieved content
                WordDocument outputDoc = new();
                
                // Adds the retrieved content into the new document
                outputDoc.AddSection();
                for (int i = 0; i < part.BodyItems.Count; i++)
                    outputDoc.LastSection.Body.ChildEntities.Add(part.BodyItems[i].Clone());

                // Save the output document
                MemoryStream outputStream = new();
                outputDoc.Save(outputStream, FormatType.Docx);
                outputStream.Position = 0;

                new SaveService().SaveAndView("BookmarkRetrieve.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", outputStream);
                
                outputDoc.Close();
                document.Close();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to retrieve bookmark: {ex.Message}", "OK");
        }
    }

}
