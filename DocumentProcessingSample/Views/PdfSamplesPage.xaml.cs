using DocumentProcessingSample.Services;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Redaction;
using Syncfusion.Pdf.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Color = Syncfusion.Drawing.Color;
using PointF = Syncfusion.Drawing.PointF;
using SizeF = Syncfusion.Drawing.SizeF;

namespace DocumentProcessingSample.Views
{
    public partial class PdfSamplesPage : ContentPage
    {
        private const string ResourceBasePath = "DocumentProcessingSample.Resources.Pdf.";
        private RectangleF _totalPriceCellBounds = RectangleF.Empty;
        private RectangleF _quantityCellBounds = RectangleF.Empty;

        public PdfSamplesPage()
        {
            InitializeComponent();
        }

        #region 1. Create PDF Invoice
        private void OnCreateInvoiceClicked(object sender, EventArgs e)
        {
            try
            {
                // Create a new PDF document.
                PdfDocument document = new();
                PdfPage page = document.Pages.Add();
                PdfGraphics graphics = page.Graphics;

                float pageWidth = page.GetClientSize().Width;
                float pageHeight = page.GetClientSize().Height;

                float headerHeight = 90;
                PdfColor lightBlue = Color.FromArgb(255, 91, 126, 215);
                PdfBrush lightBlueBrush = new PdfSolidBrush(lightBlue);
                PdfColor darkBlue = Color.FromArgb(255, 65, 104, 209);
                PdfBrush darkBlueBrush = new PdfSolidBrush(darkBlue);
                PdfBrush whiteBrush = new PdfSolidBrush(Color.FromArgb(255, 255, 255, 255));

                Assembly assembly = typeof(PdfSamplesPage).GetTypeInfo().Assembly;

                PdfStandardFont headerFont = new(PdfFontFamily.Helvetica, 30, PdfFontStyle.Regular);
                PdfStandardFont regularFont = new(PdfFontFamily.Helvetica, 18, PdfFontStyle.Regular);
                PdfStandardFont boldFont = new(PdfFontFamily.Helvetica, 9, PdfFontStyle.Bold);

                PdfStringFormat format = new()
                {
                    Alignment = PdfTextAlignment.Center,
                    LineAlignment = PdfVerticalAlignment.Middle
                };

                float margin = 30;
                float lineSpace = 10;

                PdfColor borderColor = Color.FromArgb(255, 142, 170, 219);
                PdfPen borderPen = new(borderColor, 1f);
                graphics.DrawRectangle(borderPen, new RectangleF(0, 0, pageWidth, pageHeight));

                PdfGrid grid = new();
                grid.Columns.Add(5);

                PdfGridRow[] headerRow = grid.Headers.Add(1);
                headerRow[0].Style.BackgroundBrush = new PdfSolidBrush(new PdfColor(68, 114, 196));
                headerRow[0].Style.TextBrush = PdfBrushes.White;
                headerRow[0].Cells[0].Value = "Product ID";
                headerRow[0].Cells[0].StringFormat.Alignment = PdfTextAlignment.Center;
                headerRow[0].Cells[1].Value = "Product Name";
                headerRow[0].Cells[2].Value = "Price ($)";
                headerRow[0].Cells[3].Value = "Quantity";
                headerRow[0].Cells[4].Value = "Total ($)";

                AddProducts("CA-1098", "AWC Logo Cap", 8.99, 2, 17.98, grid);
                AddProducts("LJ-0192", "Long-Sleeve Logo Jersey,M", 49.99, 3, 149.97, grid);
                AddProducts("So-B909-M", "Mountain Bike Socks,M", 9.50, 2, 19, grid);
                AddProducts("LJ-0192", "Long-Sleeve Logo Jersey,M", 49.99, 4, 199.96, grid);
                AddProducts("FK-5136", "ML Fork", 175.49, 6, 1052.94, grid);
                AddProducts("HL-U509", "Sports-100 Helmet,Black", 34.99, 1, 34.99, grid);

                // Header
                graphics.DrawRectangle(lightBlueBrush, new RectangleF(0, 0, pageWidth, headerHeight));
                string title = "INVOICE";
                RectangleF headerTotalBounds = new(400, 0, pageWidth - 400, headerHeight);
                SizeF textSize = headerFont.MeasureString(title);
                graphics.DrawString(title, headerFont, whiteBrush, new RectangleF(0, 0, textSize.Width + 50, headerHeight), format);
                graphics.DrawRectangle(darkBlueBrush, headerTotalBounds);
                graphics.DrawString("$" + GetTotalAmount(grid).ToString("F2"), regularFont, whiteBrush, new RectangleF(400, 0, pageWidth - 400, headerHeight + 10), format);

                regularFont = new PdfStandardFont(PdfFontFamily.Helvetica, 9, PdfFontStyle.Regular);
                format.LineAlignment = PdfVerticalAlignment.Bottom;
                graphics.DrawString("Amount", regularFont, whiteBrush, new RectangleF(400, 0, pageWidth - 400, headerHeight / 2 - regularFont.Height), format);

                SizeF size = regularFont.MeasureString("Invoice Number: 2058557939");
                float y = headerHeight + margin;
                float x = (pageWidth - margin) - size.Width;
                graphics.DrawString("Invoice Number: 2058557939", regularFont, PdfBrushes.Black, new PointF(x, y));

                size = regularFont.MeasureString("Date: " + DateTime.Now.ToString("dddd dd, MMMM yyyy"));
                x = (pageWidth - margin) - size.Width;
                y += regularFont.Height + lineSpace;
                graphics.DrawString("Date: " + DateTime.Now.ToString("dddd dd, MMMM yyyy"), regularFont, PdfBrushes.Black, new PointF(x, y));

                y = headerHeight + margin;
                x = margin;
                graphics.DrawString("Bill To:", regularFont, PdfBrushes.Black, new PointF(x, y));
                y += regularFont.Height + lineSpace;
                graphics.DrawString("Abraham Swearegin,", regularFont, PdfBrushes.Black, new PointF(x, y));
                y += regularFont.Height + lineSpace;
                graphics.DrawString("United States, California, San Mateo,", regularFont, PdfBrushes.Black, new PointF(x, y));
                y += regularFont.Height + lineSpace;
                graphics.DrawString("9920 BridgePointe Parkway,", regularFont, PdfBrushes.Black, new PointF(x, y));
                y += regularFont.Height + lineSpace;
                graphics.DrawString("9365550136", regularFont, PdfBrushes.Black, new PointF(x, y));

                // Grid settings
                grid.Columns[0].Width = 110;
                grid.Columns[1].Width = 150;
                grid.Columns[2].Width = 110;
                grid.Columns[3].Width = 70;
                grid.Columns[4].Width = 100;

                for (int i = 0; i < grid.Headers.Count; i++)
                {
                    grid.Headers[i].Height = 20;
                    for (int j = 0; j < grid.Columns.Count; j++)
                    {
                        PdfStringFormat pdfStringFormat = new()
                        {
                            LineAlignment = PdfVerticalAlignment.Middle,
                            Alignment = PdfTextAlignment.Left
                        };
                        if (j == 0 || j == 2)
                            grid.Headers[i].Cells[j].Style.CellPadding = new PdfPaddings(30, 1, 1, 1);
                        grid.Headers[i].Cells[j].StringFormat = pdfStringFormat;
                        grid.Headers[i].Cells[j].Style.Font = boldFont;
                    }
                    grid.Headers[0].Cells[0].Value = "Product ID";
                }

                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    grid.Rows[i].Height = 23;
                    for (int j = 0; j < grid.Columns.Count; j++)
                    {
                        PdfStringFormat pdfStringFormat = new()
                        {
                            LineAlignment = PdfVerticalAlignment.Middle,
                            Alignment = PdfTextAlignment.Left
                        };
                        if (j == 0 || j == 2)
                            grid.Rows[i].Cells[j].Style.CellPadding = new PdfPaddings(30, 1, 1, 1);
                        grid.Rows[i].Cells[j].StringFormat = pdfStringFormat;
                        grid.Rows[i].Cells[j].Style.Font = regularFont;
                    }
                }

                grid.ApplyBuiltinStyle(PdfGridBuiltinStyle.ListTable4Accent5);
                grid.BeginCellLayout += Grid_BeginCellLayout;
                PdfGridLayoutResult result = grid.Draw(page, new PointF(0, y + 40));

                y = result.Bounds.Bottom + lineSpace;
                format = new PdfStringFormat { Alignment = PdfTextAlignment.Center };
                RectangleF bounds = new(_quantityCellBounds.X, y, _quantityCellBounds.Width, _quantityCellBounds.Height);
                page.Graphics.DrawString("Grand Total:", boldFont, PdfBrushes.Black, bounds, format);
                bounds = new RectangleF(_totalPriceCellBounds.X, y, _totalPriceCellBounds.Width, _totalPriceCellBounds.Height);
                page.Graphics.DrawString("$" + GetTotalAmount(grid).ToString("F2"), boldFont, PdfBrushes.Black, bounds);

                borderPen.DashStyle = PdfDashStyle.Custom;
                borderPen.DashPattern = new float[] { 3, 3 };
                graphics.DrawLine(borderPen, new PointF(0, pageHeight - 100), new PointF(pageWidth, pageHeight - 100));

                Stream? imageStream = assembly.GetManifestResourceStream(ResourceBasePath + "AdventureWork.png");
                if (imageStream != null)
                {
                    PdfBitmap bitmap = new(imageStream);
                    graphics.DrawImage(bitmap, new RectangleF(10, pageHeight - 90, 80, 80));
                }

                y = pageHeight - 100 + margin;
                size = regularFont.MeasureString("800 Interchange Blvd.");
                x = pageWidth - size.Width - margin;
                graphics.DrawString("800 Interchange Blvd.", regularFont, PdfBrushes.Black, new PointF(x, y));

                y += regularFont.Height + lineSpace;
                size = regularFont.MeasureString("Suite 2501,  Austin, TX 78721");
                x = pageWidth - size.Width - margin;
                graphics.DrawString("Suite 2501,  Austin, TX 78721", regularFont, PdfBrushes.Black, new PointF(x, y));

                y += regularFont.Height + lineSpace;
                size = regularFont.MeasureString("Any Questions? support@adventure-works.com");
                x = pageWidth - size.Width - margin;
                graphics.DrawString("Any Questions? support@adventure-works.com", regularFont, PdfBrushes.Black, new PointF(x, y));

                using MemoryStream ms = new();
                document.Save(ms);
                document.Close(true);
                ms.Position = 0;

                SaveService saveService = new();
                saveService.SaveAndView("Invoice.pdf", "application/pdf", ms);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void Grid_BeginCellLayout(object sender, PdfGridBeginCellLayoutEventArgs args)
        {
            if (sender is PdfGrid grid)
            {
                if (args.CellIndex == grid.Columns.Count - 1)
                {
                    _totalPriceCellBounds = args.Bounds;
                }
                else if (args.CellIndex == grid.Columns.Count - 2)
                {
                    _quantityCellBounds = args.Bounds;
                }
            }
        }

        private static void AddProducts(string productId, string productName, double price, int quantity, double total, PdfGrid grid)
        {
            PdfGridRow row = grid.Rows.Add();
            row.Cells[0].Value = productId;
            row.Cells[1].Value = productName;
            row.Cells[2].Value = price.ToString();
            row.Cells[3].Value = quantity.ToString();
            row.Cells[4].Value = total.ToString();
        }

        private static float GetTotalAmount(PdfGrid grid)
        {
            float total = 0f;
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                string cellValue = (grid.Rows[i].Cells[grid.Columns.Count - 1].Value as string) ?? "0";
                if (float.TryParse(cellValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float result))
                {
                    total += result;
                }
            }
            return total;
        }
        #endregion

        #region 2. Digital Signature
        private void OnDigitalSignatureClicked(object sender, EventArgs e)
        {
            try
            {
                Assembly assembly = typeof(PdfSamplesPage).GetTypeInfo().Assembly;
                Stream? documentStream = assembly.GetManifestResourceStream(ResourceBasePath + "digital_signature_template.pdf");
                if (documentStream == null)
                {
                    DisplayAlert("Error", "digital_signature_template.pdf not found in resources.", "OK");
                    return;
                }

                PdfLoadedDocument loadedDocument = new(documentStream);
                Stream? certificateStream = assembly.GetManifestResourceStream(ResourceBasePath + "certificate.pfx");
                if (certificateStream == null)
                {
                    DisplayAlert("Error", "certificate.pfx not found in resources.", "OK");
                    return;
                }

                PdfLoadedSignatureField? signatureField = loadedDocument.Form.Fields[6] as PdfLoadedSignatureField;
                if (signatureField == null)
                {
                    DisplayAlert("Error", "Signature field not found in template.", "OK");
                    return;
                }

                RectangleF bounds = signatureField.Bounds;
                PdfCertificate pdfCertificate = new(certificateStream, "password123");

                PdfSignature signature = new(loadedDocument, loadedDocument.Pages[0], pdfCertificate, "", signatureField)
                {
                    ContactInfo = "johndoe@owned.us",
                    LocationInfo = "Honolulu, Hawaii",
                    Reason = "I am author of this document."
                };

                signature.Settings.CryptographicStandard = CryptographicStandard.CADES;
                signature.Settings.DigestAlgorithm = DigestAlgorithm.SHA512;

                PdfGraphics graphics = signature.Appearance.Normal.Graphics;
                if (graphics != null)
                {
                    graphics.DrawRectangle(PdfPens.Black, bounds);

                    Stream? imageStream = assembly.GetManifestResourceStream(ResourceBasePath + "signature.png");
                    if (imageStream != null)
                    {
                        PdfBitmap bitmap = new(imageStream, true);
                        graphics.DrawImage(bitmap, new RectangleF(2, 1, 30, 30));
                    }

                    string subject = pdfCertificate.SubjectName;
                    PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 7);
                    RectangleF textRect = new(45, 0, bounds.Width - 45, bounds.Height);
                    PdfStringFormat strFormat = new(PdfTextAlignment.Justify);
                    graphics.DrawString("Digitally signed by " + subject + " \r\nReason: Testing signature \r\nLocation: USA", font, PdfBrushes.Black, textRect, strFormat);
                }

                signatureField.Signature = signature;

                using MemoryStream stream = new();
                loadedDocument.Save(stream);
                loadedDocument.Close(true);

                stream.Position = 0;
                SaveService saveService = new();
                saveService.SaveAndView("DigitalSignature.pdf", "application/pdf", stream);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
        }
        #endregion

        #region 3. Encryption
        private void OnEncryptionClicked(object sender, EventArgs e)
        {
            try
            {
                Assembly assembly = typeof(PdfSamplesPage).GetTypeInfo().Assembly;
                Stream? documentStream = assembly.GetManifestResourceStream(ResourceBasePath + "credit_card_statement.pdf");
                if (documentStream == null)
                {
                    DisplayAlert("Error", "credit_card_statement.pdf not found in resources.", "OK");
                    return;
                }

                PdfLoadedDocument document = new(documentStream);
                PdfSecurity security = document.Security;

                security.KeySize = PdfEncryptionKeySize.Key256Bit;
                security.Algorithm = PdfEncryptionAlgorithm.AES;
                security.EncryptionOptions = PdfEncryptionOptions.EncryptAllContents;
                security.OwnerPassword = "syncfusion";
                security.UserPassword = "password@123";
                security.Permissions = PdfPermissionsFlags.Print | PdfPermissionsFlags.FullQualityPrint;

                using MemoryStream ms = new();
                document.Save(ms);
                document.Close(true);

                ms.Position = 0;
                SaveService saveService = new();
                saveService.SaveAndView("Encryption.pdf", "application/pdf", ms);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
        }
        #endregion

        #region 4. Image to PDF
        private void OnImageToPdfClicked(object sender, EventArgs e)
        {
            try
            {
                Assembly assembly = typeof(PdfSamplesPage).GetTypeInfo().Assembly;
                List<Stream?> imageStreams = new();
                for (int i = 1; i <= 6; i++)
                {
                    Stream? jpgImageStream = assembly.GetManifestResourceStream(ResourceBasePath + "pdf_succinctly_page" + i + ".jpg");
                    if (jpgImageStream != null)
                    {
                        imageStreams.Add(jpgImageStream);
                    }
                }

                if (imageStreams.Count == 0)
                {
                    DisplayAlert("Error", "Sample images not found in resources.", "OK");
                    return;
                }

                ImageToPdfConverter imageToPdfConverter = new()
                {
                    ImagePosition = PdfImagePosition.FitToPageAndMaintainAspectRatio
                };

                PdfDocument document = imageToPdfConverter.Convert(imageStreams);

                using MemoryStream stream = new();
                document.Save(stream);
                document.Close(true);

                stream.Position = 0;
                SaveService saveService = new();
                saveService.SaveAndView("ImageToPDF.pdf", "application/pdf", stream);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
        }
        #endregion

        #region 5. Watermark
        private void OnWatermarkClicked(object sender, EventArgs e)
        {
            try
            {
                Assembly assembly = typeof(PdfSamplesPage).GetTypeInfo().Assembly;
                Stream? documentStream = assembly.GetManifestResourceStream(ResourceBasePath + "Invoice.pdf");
                if (documentStream == null)
                {
                    DisplayAlert("Error", "Invoice.pdf not found in resources.", "OK");
                    return;
                }

                PdfLoadedDocument document = new(documentStream);
                if (document.Pages.Count == 0 || document.Pages[0] is not PdfLoadedPage page)
                {
                    DisplayAlert("Error", "Page not found in PDF.", "OK");
                    return;
                }

                SizeF pageSize = page.Size;

                PdfWatermarkAnnotation watermarkAnnotation = new(new RectangleF(0, 0, pageSize.Width, pageSize.Height))
                {
                    Opacity = 0.25F,
                    AnnotationFlags = PdfAnnotationFlags.Print
                };

                PdfGraphics graphics = watermarkAnnotation.Appearance.Normal.Graphics;
                string watermarkText = "Confidential";
                PdfFont watermarkFont = new PdfStandardFont(PdfFontFamily.Helvetica, 40);
                SizeF textSize = watermarkFont.MeasureString(watermarkText);

                float x = pageSize.Width / 2 - textSize.Width / 2;
                float y = pageSize.Height / 2;
                graphics.Save();
                graphics.TranslateTransform(x, y);
                graphics.RotateTransform(-45);
                graphics.DrawString(watermarkText, watermarkFont, PdfBrushes.Red, PointF.Empty);
                graphics.Restore();

                page.Annotations.Add(watermarkAnnotation);

                using MemoryStream stream = new();
                document.Save(stream);
                document.Close(true);

                stream.Position = 0;
                SaveService saveService = new();
                saveService.SaveAndView("Watermark.pdf", "application/pdf", stream);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
        }
        #endregion

        #region 6. Redaction
        private void OnRedactionClicked(object sender, EventArgs e)
        {
            try
            {
                Assembly assembly = typeof(PdfSamplesPage).GetTypeInfo().Assembly;
                Stream? documentStream = assembly.GetManifestResourceStream(ResourceBasePath + "RedactionTemplate.pdf");
                if (documentStream == null)
                {
                    DisplayAlert("Error", "RedactionTemplate.pdf not found in resources.", "OK");
                    return;
                }

                PdfLoadedDocument loadedDocument = new(documentStream);
                if (loadedDocument.Pages.Count == 0 || loadedDocument.Pages[0] is not PdfLoadedPage lpage)
                {
                    DisplayAlert("Error", "Page not found in PDF.", "OK");
                    return;
                }

                // Create PDF redactions for the page to redact text and images
                PdfRedaction textRedaction1 = new(new RectangleF(477f, 154f, 62.709f, 16.802f), Color.Black);
                PdfRedaction textRedaction2 = new(new RectangleF(70f, 240f, 65.709f, 16.802f), Color.Black);
                PdfRedaction imageRedaction = new(new RectangleF(52.14447f, 712.1465f, 126.10835f, 81.45297f), Color.Black);

                lpage.AddRedaction(textRedaction1);
                lpage.AddRedaction(textRedaction2);
                lpage.AddRedaction(imageRedaction);

                loadedDocument.Redact();

                using MemoryStream stream = new();
                loadedDocument.Save(stream);
                loadedDocument.Close(true);

                stream.Position = 0;
                SaveService saveService = new();
                saveService.SaveAndView("Redaction.pdf", "application/pdf", stream);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
        }
        #endregion
    }
}
