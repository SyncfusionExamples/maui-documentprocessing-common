# Syncfusion .NET MAUI Document Processing Samples

This repository showcases interactive cross-platform examples using the **Syncfusion .NET MAUI Document Processing SDK**. The SDK enables developers to create, read, edit, convert, and process Word, Excel, PDF, and PowerPoint documents programmatically—as well as view PDF files interactively—across **Android, iOS, macOS (Mac Catalyst), and Windows** using a single C# codebase.

---

## 🚀 Key Modules & Features Included

### 1. 📄 PDF Library
High-performance, non-UI .NET PDF library to create, read, and edit PDF documents without Adobe Acrobat dependencies.
* **Create PDF Invoice**: Generate complex PDF invoices with tables, headers/footers, logos, and custom styling.
* **Digital Signature**: Cryptographically sign PDFs using certificates (.pfx) with SHA-512 hashing and visual appearance.
* **Encryption**: Protect documents using 256-bit AES encryption with User and Owner passwords.
* **Image to PDF**: Convert raster images into multi-page PDF documents.
* **Watermark**: Stamp documents with customizable diagonal text watermarks.
* **Redaction**: Permanently black out sensitive text and graphic content.

### 2. 📝 Word Library (DocIO)
Standalone .NET Word library to create, read, and manipulate Word documents (DOCX, WordML, HTML, RTF, TXT).
* **Word to PDF Conversion**: Convert DOCX documents directly to high-fidelity PDF files.
* **Clone and Merge**: Import and combine content across multiple Word documents while preserving styles.
* **Mail Merge**: Generate personalized documents dynamically from data sources using merge fields.
* **DOCX to DOCX**: Read, edit, and duplicate Word document structure and content.
* **DOCX to HTML**: Export DOCX documents into HTML format.
* **DOCX to Markdown**: Convert Word documents into clean Markdown files.
* **Retrieve Bookmark Content**: Extract text and elements within specific document bookmarks.

### 3. 📊 Excel Library (XlsIO)
High-performance .NET Excel library to create, read, and manipulate Excel spreadsheets without Microsoft Office.
* **Create Invoice**: Build professional spreadsheet invoices with formulas, cell formatting, and styles.
* **Import Data from DataTable**: Populate worksheets dynamically from .NET DataTables.
* **Excel to PDF**: Convert full workbooks or worksheets to PDF.
* **Excel to Image**: Export worksheets or specific ranges to high-quality images (PNG/JPEG).
* **Encryption**: Protect sensitive financial worksheets with password encryption.

### 4. 🖥️ PowerPoint Library (Presentation)
High-performance .NET library to create, read, edit, and convert PowerPoint presentations (PPTX).
* **PowerPoint to PDF**: Convert PPTX presentations to PDF preserving formatting, layouts, and fonts.
* **Clone and Merge**: Extract, reorder, and merge slides across different presentations.
* **Find and Replace**: Perform batch search-and-replace for text across all slides.

### 5. 🔍 .NET MAUI PDF Viewer
A powerful, feature-rich UI control to view, navigate, and interact with PDF documents seamlessly on mobile and desktop platforms with built-in page virtualization and customizable toolbars.

---

## 📚 Documentation & Reference Links

* **PDF**: [About Syncfusion .NET PDF Library | Syncfusion](https://help.syncfusion.com/document-processing/pdf/pdf-library/net/overview)
* **Word**: [About Syncfusion Word Document Processing Solutions | Syncfusion](https://help.syncfusion.com/document-processing/word/overview)
* **Excel**: [About Syncfusion Excel Document Processing Solutions | Syncfusion](https://help.syncfusion.com/document-processing/excel/overview)
* **Presentation**: [About Syncfusion PowerPoint Processing Solutions | Syncfusion](https://help.syncfusion.com/document-processing/powerpoint/overview)
* **PdfViewer**: [About Syncfusion .NET MAUI PDF Viewer Control | Syncfusion](https://help.syncfusion.com/document-processing/pdf/pdf-viewer/maui/overview)

---

## 💻 Prerequisites & Requirements

* [.NET 10 SDK](https://dotnet.microsoft.com/download) or higher
* [Visual Studio](https://visualstudio.microsoft.com/) / [Code Studio](https://www.syncfusion.com/) with .NET MAUI workload installed
* Supported platforms:
  * Android 5.0 (API 21) or higher
  * iOS 15.0 or higher
  * macOS 15.0 or higher (Mac Catalyst)
  * Windows 10 build 17763 or higher

---

## 🛠️ How to Run the Sample

1. Clone or download this repository.
2. Open the solution `DocumentProcessingSample.slnx` or project `DocumentProcessingSample.csproj`.
3. Select your target device/platform (Android Emulator/Device, Windows Machine, iOS Simulator, or Mac Catalyst).
4. Build and run the project. Use the flyout navigation menu to explore each document processing module and execute the sample features.

