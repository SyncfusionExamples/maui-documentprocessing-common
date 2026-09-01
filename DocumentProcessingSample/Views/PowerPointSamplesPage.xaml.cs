using DocumentProcessingSample.Services;
using Syncfusion.Pdf;
using Syncfusion.Presentation;
using Syncfusion.PresentationRenderer;
using System.Reflection;

namespace DocumentProcessingSample.Views
{
    public partial class PowerPointSamplesPage : ContentPage
    {
        private const string ResourceBasePath = "DocumentProcessingSample.Resources.PPTX.";
        private const string PowerPointMimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";

        public PowerPointSamplesPage()
        {
            InitializeComponent();
        }

        #region 1. PowerPoint to PDF
        private void OnPowerPointToPdfClicked(object sender, EventArgs e)
        {
            try
            {
                Assembly assembly = typeof(PowerPointSamplesPage).GetTypeInfo().Assembly;
                Stream? presentationStream = assembly.GetManifestResourceStream(ResourceBasePath + "PptxtoPDFTemplate.pptx");
                if (presentationStream == null)
                {
                    DisplayAlert("Error", "PptxtoPDFTemplate.pptx not found in resources.", "OK");
                    return;
                }

                IPresentation presentation = Presentation.Open(presentationStream);

                // Convert presentation to PDF.
                PdfDocument pdfDocument = PresentationToPdfConverter.Convert(presentation);

                using MemoryStream stream = new();
                pdfDocument.Save(stream);
                presentation.Close();
                stream.Position = 0;

                SaveService saveService = new();
                saveService.SaveAndView("PowerPoint_to_PDF.pdf", "application/pdf", stream);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
        }
        #endregion

        #region 2. Clone and Merge
        private void OnCloneAndMergeClicked(object sender, EventArgs e)
        {
            try
            {
                Assembly assembly = typeof(PowerPointSamplesPage).GetTypeInfo().Assembly;
                Stream? presentationStream = assembly.GetManifestResourceStream(ResourceBasePath + "CloneandMergeTemplate.pptx");
                if (presentationStream == null)
                {
                    DisplayAlert("Error", "CloneandMergeTemplate.pptx not found in resources.", "OK");
                    return;
                }

                IPresentation sourcePresentation = Presentation.Open(presentationStream);

                // Clone the third section's slides.
                ISlides slides = sourcePresentation.Sections[2].Clone();

                // Create destination presentation.
                IPresentation destinationPresentation = Presentation.Create();
                foreach (ISlide slide in slides)
                {
                    destinationPresentation.Slides.Add(slide);
                }

                using MemoryStream stream = new();
                destinationPresentation.Save(stream);
                sourcePresentation.Close();
                destinationPresentation.Close();
                stream.Position = 0;

                SaveService saveService = new();
                saveService.SaveAndView("Cloned_and_Merged.pptx", PowerPointMimeType, stream);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
        }
        #endregion

        #region 3. Find and Replace
        private async void OnFindAndReplaceClicked(object sender, EventArgs e)
        {
            try
            {
                // Get find text from the user.
                string? findText = await DisplayPromptAsync("Find", "Enter the text to find:", "OK", "Cancel", "Text to find");
                if (string.IsNullOrWhiteSpace(findText))
                {
                    DisplayAlert("Cancelled", "No text was entered to find.", "OK");
                    return;
                }

                // Get replace text from the user.
                string? replaceText = await DisplayPromptAsync("Replace", $"Replace '{findText}' with:", "OK", "Cancel", "Replacement text");
                if (replaceText == null)
                {
                    return;
                }

                Assembly assembly = typeof(PowerPointSamplesPage).GetTypeInfo().Assembly;
                Stream? presentationStream = assembly.GetManifestResourceStream(ResourceBasePath + "FindAndReplaceTemplate.pptx");
                if (presentationStream == null)
                {
                    DisplayAlert("Error", "FindAndReplaceTemplate.pptx not found in resources.", "OK");
                    return;
                }

                IPresentation presentation = Presentation.Open(presentationStream);

                ITextSelection[] textSelections = presentation.FindAll(findText, false, false);
                foreach (ITextSelection textSelection in textSelections)
                {
                    // Gets the found text as a single text part and replaces it.
                    ITextPart textPart = textSelection.GetAsOneTextPart();
                    textPart.Text = replaceText;
                }

                using MemoryStream stream = new();
                presentation.Save(stream);
                presentation.Close();
                stream.Position = 0;

                SaveService saveService = new();
                saveService.SaveAndView("Find_and_Replace_Result.pptx", PowerPointMimeType, stream);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
        }
        #endregion
    }
}
