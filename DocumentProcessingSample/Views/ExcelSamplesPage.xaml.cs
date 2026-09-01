using DocumentProcessingSample.Services;
using Syncfusion.Pdf;
using Syncfusion.XlsIO;
using Syncfusion.XlsIORenderer;
using System;
using System.Data;
using System.IO;
using Color = Syncfusion.Drawing.Color;
using IApplication = Syncfusion.XlsIO.IApplication;

namespace DocumentProcessingSample.Views
{
    public partial class ExcelSamplesPage : ContentPage
    {
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public ExcelSamplesPage()
        {
            InitializeComponent();
        }

    // 1. Create Invoice
    private void OnCreateInvoiceClicked(object sender, EventArgs e)
    {
        try
        {
            using ExcelEngine excelEngine = new();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Xlsx;

            IWorkbook workbook = CreateInvoiceWorkbook(application);

            using MemoryStream ms = new();
            workbook.SaveAs(ms);
            workbook.Close();
            ms.Position = 0;

            SaveService saveService = new();
            saveService.SaveAndView("Invoice.xlsx", ExcelContentType, ms);
        }
        catch (Exception ex)
        {
            DisplayAlert("Excel Invoice Error", ex.Message, "OK");
        }
    }

    /// <summary>
    /// Builds the sample invoice workbook shared by the Create Invoice,
    /// Excel to PDF, and Excel to Image samples.
    /// </summary>
    private static IWorkbook CreateInvoiceWorkbook(IApplication application)
    {
        IWorkbook workbook = application.Workbooks.Create(1);
        IWorksheet worksheet = workbook.Worksheets[0];
        worksheet.Name = "Invoice";

        // Title
        worksheet.Range["A1"].Text = "INVOICE";
        worksheet.Range["A1:E1"].Merge();
        worksheet.Range["A1"].CellStyle.Font.Bold = true;
        worksheet.Range["A1"].CellStyle.Font.Size = 20;
        worksheet.Range["A1"].CellStyle.Font.Color = ExcelKnownColors.White;
        worksheet.Range["A1"].CellStyle.Color = Color.FromArgb(33, 115, 70);
        worksheet.Range["A1"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
        worksheet.Range["A1"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
        worksheet.Range["A1"].RowHeight = 30;

        // Invoice metadata
        worksheet.Range["A3"].Text = "Invoice Number: 2058557939";
        worksheet.Range["A4"].Text = "Date: " + DateTime.Now.ToString("dddd dd, MMMM yyyy");
        worksheet.Range["A6"].Text = "Bill To:";
        worksheet.Range["A7"].Text = "Abraham Swearegin,";
        worksheet.Range["A8"].Text = "United States, California, San Mateo,";
        worksheet.Range["A9"].Text = "9920 BridgePointe Parkway,";
        worksheet.Range["A10"].Text = "9365550136";
        for (int row = 3; row <= 10; row++)
        {
            worksheet.Range["A" + row].CellStyle.Font.Bold = row == 3 || row == 6;
        }

        // Header row of product table
        worksheet.Range["A12"].Text = "Product ID";
        worksheet.Range["B12"].Text = "Product Name";
        worksheet.Range["C12"].Text = "Price ($)";
        worksheet.Range["D12"].Text = "Quantity";
        worksheet.Range["E12"].Text = "Total ($)";
        IStyle headerStyle = workbook.Styles.Add("HeaderStyle");
        headerStyle.BeginUpdate();
        headerStyle.Color = Color.FromArgb(33, 115, 70);
        headerStyle.Font.Bold = true;
        headerStyle.Font.Color = ExcelKnownColors.White;
        headerStyle.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
        headerStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
        headerStyle.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
        headerStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
        headerStyle.EndUpdate();
        worksheet.Range["A12:E12"].CellStyle = headerStyle;

        // Product rows (matching the PDF invoice sample products)
        string[,] products = new string[,]
        {
            { "CA-1098", "AWC Logo Cap", "8.99", "2" },
            { "LJ-0192", "Long-Sleeve Logo Jersey,M", "49.99", "3" },
            { "So-B909-M", "Mountain Bike Socks,M", "9.50", "2" },
            { "LJ-0192", "Long-Sleeve Logo Jersey,M", "49.99", "4" },
            { "FK-5136", "ML Fork", "175.49", "6" },
            { "HL-U509", "Sports-100 Helmet,Black", "34.99", "1" }
        };
        for (int i = 0; i < products.GetLength(0); i++)
        {
            int row = 13 + i;
            worksheet.Range["A" + row].Text = products[i, 0];
            worksheet.Range["B" + row].Text = products[i, 1];
            worksheet.Range["C" + row].Number = double.Parse(products[i, 2], System.Globalization.CultureInfo.InvariantCulture);
            worksheet.Range["D" + row].Number = int.Parse(products[i, 3]);
            worksheet.Range["E" + row].Formula = "C" + row + "*D" + row;
            worksheet.Range["C" + row + ":E" + row].NumberFormat = "$#,##0.00";
            if (i % 2 == 1)
            {
                worksheet.Range["A" + row + ":E" + row].CellStyle.Color = Color.FromArgb(234, 244, 238);
            }
            else
            {
                worksheet.Range["A" + row + ":E" + row].CellStyle.Color = Color.FromArgb(255, 255, 255);
            }
        }

        // Grand total row (row after last product)
        int totalRow = 13 + products.GetLength(0);
        worksheet.Range["D" + totalRow].Text = "Grand Total:";
        worksheet.Range["D" + totalRow].CellStyle.Font.Bold = true;
        worksheet.Range["E" + totalRow].Formula = "SUM(E13:E" + (totalRow - 1) + ")";
        worksheet.Range["E" + totalRow].NumberFormat = "$#,##0.00";
        worksheet.Range["E" + totalRow].CellStyle.Font.Bold = true;

        // Footer thank-you note
        worksheet.Range["A" + (totalRow + 2)].Text = "Thank you for your business!";
        worksheet.Range["A" + (totalRow + 2)].CellStyle.Font.Italic = true;
        worksheet.Range["A" + (totalRow + 2) + ":E" + (totalRow + 2)].Merge();
        worksheet.Range["A" + (totalRow + 2)].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

        // Footer payee block
        worksheet.Range["A" + (totalRow + 4)].Text = "800 Interchange Blvd.";
        worksheet.Range["A" + (totalRow + 5)].Text = "Suite 2501, Austin, TX 78721";
        worksheet.Range["A" + (totalRow + 6)].Text = "Any Questions? support@adventure-works.com";

        // Auto-fit columns for neat layout
        worksheet.UsedRange.AutofitColumns();
        worksheet.SetColumnWidth(4, 14);
        worksheet.SetColumnWidth(5, 14);

        return workbook;
    }

    // 2. Import Data from DataTable
    private void OnImportDataTableClicked(object sender, EventArgs e)
    {
        try
        {
            using ExcelEngine excelEngine = new();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Xlsx;

            IWorkbook workbook = application.Workbooks.Create(1);
            IWorksheet worksheet = workbook.Worksheets[0];
            worksheet.Name = "Employee Report";

            // Build the sample DataTable
            DataTable employeeTable = GetEmployeeDataTable();
            worksheet.ImportDataTable(employeeTable, true, 2, 1);

            // Format the header row (imported at row 2 with column headers)
            IStyle headerStyle = workbook.Styles.Add("TableHeaderStyle");
            headerStyle.BeginUpdate();
            headerStyle.Color = Color.FromArgb(33, 115, 70);
            headerStyle.Font.Bold = true;
            headerStyle.Font.Color = ExcelKnownColors.White;
            headerStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
            headerStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
            headerStyle.EndUpdate();
            worksheet.Range["A2:G2"].CellStyle = headerStyle;

            // Title above the table
            worksheet.Range["A1"].Text = "Sales Details";
            worksheet.Range["A1"].CellStyle.Font.Bold = true;
            worksheet.Range["A1"].CellStyle.Font.Size = 16;

            // Number formats for price and total amount columns
            worksheet.Range["E3:E10"].NumberFormat = "$#,##0.00";
            worksheet.Range["G3:G10"].NumberFormat = "$#,##0.00";

            // Auto-fit columns so all data is visible
            worksheet.UsedRange.AutofitColumns();

            using MemoryStream ms = new();
            workbook.SaveAs(ms);
            workbook.Close();
            ms.Position = 0;

            SaveService saveService = new();
            saveService.SaveAndView("ImportData.xlsx", ExcelContentType, ms);
        }
        catch (Exception ex)
        {
            DisplayAlert("Import DataTable Error", ex.Message, "OK");
        }
    }

    private static DataTable GetEmployeeDataTable()
    {
        DataTable table = new("SalesDetails");
        table.Columns.Add("SalesPerson", typeof(string));
        table.Columns.Add("Product", typeof(string));
        table.Columns.Add("Region", typeof(string));
        table.Columns.Add("UnitsSold", typeof(int));
        table.Columns.Add("Price", typeof(double));
        table.Columns.Add("OrderDate", typeof(DateTime));
        table.Columns.Add("TotalAmount", typeof(double));

        table.Rows.Add("Andy Bernard", "Bicycle", "East", 450, 450.00, new DateTime(2025, 1, 15), 304950.00);
        table.Rows.Add("Jim Halpert", "Helmet", "West", 910, 34.99, new DateTime(2025, 2, 10), 31840.90);
        table.Rows.Add("Karen Fillippelli", "Jersey", "North", 750, 49.99, new DateTime(2025, 3, 5), 37492.50);
        table.Rows.Add("Phyllis Lapin", "Bicycle", "South", 565, 450.00, new DateTime(2025, 4, 20), 254250.00);
        table.Rows.Add("Stanley Hudson", "Cap", "East", 1200, 8.99, new DateTime(2025, 5, 12), 10788.00);
        table.Rows.Add("Creed Bratton", "Socks", "West", 340, 9.50, new DateTime(2025, 6, 8), 3230.00);
        table.Rows.Add("Meredith Palmer", "Fork", "North", 89, 175.49, new DateTime(2025, 7, 25), 15618.61);
        table.Rows.Add("Oscar Martinez", "Jersey", "South", 465, 52.00, new DateTime(2025, 8, 30), 24180.00);
        return table;
    }

    // 3. Excel to PDF
    private void OnExcelToPdfClicked(object sender, EventArgs e)
    {
        try
        {
            using ExcelEngine excelEngine = new();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Xlsx;

            // Create a workbook with sample data to convert
            IWorkbook workbook = CreateInvoiceWorkbook(application);

            // Initialize XlsIO renderer
            XlsIORenderer renderer = new();

            // Convert the Excel workbook to PDF
            PdfDocument pdfDocument = renderer.ConvertToPDF(workbook);

            using MemoryStream ms = new();
            pdfDocument.Save(ms);
            pdfDocument.Close(true);
            ms.Position = 0;

            SaveService saveService = new();
            saveService.SaveAndView("ExcelToPDF.pdf", "application/pdf", ms);
        }
        catch (Exception ex)
        {
            DisplayAlert("Excel to PDF Error", ex.Message, "OK");
        }
    }

    // 4. Excel to Image
    private void OnExcelToImageClicked(object sender, EventArgs e)
    {
        try
        {
            using ExcelEngine excelEngine = new();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Xlsx;

            // Create a workbook with sample data to convert
            IWorkbook workbook = CreateInvoiceWorkbook(application);
            IWorksheet worksheet = workbook.Worksheets[0];

            // XlsIORenderer instance is mandatory to convert worksheet to image
            application.XlsIORenderer = new XlsIORenderer();

            // Convert the used range of the worksheet to a PNG image stream
            using MemoryStream ms = new();
            worksheet.ConvertToImage(worksheet.UsedRange, ms);
            worksheet.Workbook.Close();
            ms.Position = 0;

            SaveService saveService = new();
            saveService.SaveAndView("ExcelToImage.png", "image/png", ms);
        }
        catch (Exception ex)
        {
            DisplayAlert("Excel to Image Error", ex.Message, "OK");
        }
    }

    // 5. Encryption
    private void OnEncryptionClicked(object sender, EventArgs e)
    {
        try
        {
            using ExcelEngine excelEngine = new();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Xlsx;

            // Create and encrypt the sample workbook
            IWorkbook workbook = CreateInvoiceWorkbook(application);
            workbook.PasswordToOpen = "password@123";

            using MemoryStream ms = new();
            workbook.SaveAs(ms);
            workbook.Close();
            ms.Position = 0;

            SaveService saveService = new();
            saveService.SaveAndView("EncryptedWorkbook.xlsx", ExcelContentType, ms);
        }
        catch (Exception ex)
        {
            DisplayAlert("Encryption Error", ex.Message, "OK");
        }
    }
    }
}
