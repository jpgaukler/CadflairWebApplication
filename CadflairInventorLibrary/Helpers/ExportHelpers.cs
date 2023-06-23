using Inventor;
using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.IO.Compression;

namespace CadflairInventorLibrary.Helpers
{
    public static class ExportHelpers
    {

        /// <summary>
        /// Converts the given folder to a zip folder and deletes the original directory (and all child files).
        /// </summary>
        /// <param name="outputFolderPath"></param>
        /// <returns>The file name of the resulting zip folder.</returns>
        public static string ConvertToZip(string outputFolderPath)
        {
            DirectoryInfo dir = new DirectoryInfo(outputFolderPath);

            // zip up the temp folder
            string zipFileName = $"{outputFolderPath}.zip";
            ZipFile.CreateFromDirectory(outputFolderPath, zipFileName);

            // delete the copied files
            dir.Delete(true);

            // return the path of the zip file
            return zipFileName;
        }

        /// <summary>
        /// Save a file to Svf file format for use with the online Forge Viewer. Files are flattened to a single folder level after export.
        /// <br/><br/>
        /// See blog post here: <see href="https://aps.autodesk.com/blog/speed-viewable-generation-when-using-design-automation-inventor">Speed up viewable generation when using Design Automation for Inventor</see>
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="outputFolderPath"></param>
        public static void ExportSvf(Document doc, string outputFolderPath)
        {
            try
            {
                Trace.TraceInformation("Exporting svf for: " + System.IO.Path.GetFileName(doc.FullFileName));

                Directory.CreateDirectory(outputFolderPath);

                dynamic inventorApp = doc.Parent;
                TranslatorAddIn oAddin = (TranslatorAddIn)inventorApp.ApplicationAddIns.ItemById["{C200B99B-B7DD-4114-A5E9-6557AB5ED8EC}"];

                TranslationContext oContext = inventorApp.TransientObjects.CreateTranslationContext();
                oContext.Type = IOMechanismEnum.kFileBrowseIOMechanism;

                DataMedium oData = inventorApp.TransientObjects.CreateDataMedium();
                oData.FileName = System.IO.Path.Combine(outputFolderPath, "result.collaboration");

                NameValueMap oOptions = inventorApp.TransientObjects.CreateNameValueMap();

                // Setup SVF options
                if (oAddin.get_HasSaveCopyAsOptions(doc, oContext, oOptions))
                {
                    oOptions.set_Value("EnableExpressTranslation", false);
                    oOptions.set_Value("SVFFileOutputDir", outputFolderPath);
                    oOptions.set_Value("ExportFileProperties", true);
                    oOptions.set_Value("ObfuscateLabels", false);
                }

                oAddin.SaveCopyAs(doc, oContext, oOptions, oData);

                // move "bubble.json" file up one level to the root output folder
                var bubbleFileOriginal = System.IO.Path.Combine(outputFolderPath, "output", "bubble.json");
                var bubbleFileNew = System.IO.Path.Combine(outputFolderPath, "bubble.json");
                System.IO.File.Move(bubbleFileOriginal, bubbleFileNew);

                // delete "result.collaboration" file
                System.IO.File.Delete(oData.FileName);

                // flatten to single folder
                FlattenSvfToSingleFolder(outputFolderPath, outputFolderPath, null);

                Trace.WriteLine($"Svf exported successfully: {outputFolderPath}");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Could not export svf - {ex}");
            }
        }

        /// <summary>
        /// Flattens the Svf output to a single folder. Renames the files so that the original path separators are in the file name, in a url encoded form. <br/><br/>
        /// <strong>Example:</strong><br/>
        /// folderName\assemblyName.iam => "folderName%2FassemblyName.iam"
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="rootFolder"></param>
        /// <param name="urlEncodedPath"></param>
        private static void FlattenSvfToSingleFolder(string folder, string rootFolder, string urlEncodedPath)
        {
            // move all files to the root folder
            if (folder != rootFolder)
            {
                string[] filePaths = Directory.GetFiles(folder);
                foreach (string filePath in filePaths)
                {
                    string fileName = urlEncodedPath + System.IO.Path.GetFileName(filePath);
                    string filePathNew = System.IO.Path.Combine(rootFolder, fileName);
                    System.IO.File.Move(filePath, filePathNew);
                }
            }

            // recurse through child folders 
            string[] folderPaths = Directory.GetDirectories(folder);
            foreach (string folderPath in folderPaths)
            {
                // encode the folder name to url form (/ => %2F)
                string folderName = System.IO.Path.GetFileName(folderPath);
                urlEncodedPath += folderName + "%2F";

                // move child files
                FlattenSvfToSingleFolder(folderPath, rootFolder, urlEncodedPath);

                // Delete the empty folder after child files are moved
                Directory.Delete(folderPath);
            }
        }

        public static void ExportThumbnail(Document doc, string outputFolderPath, int sizeInPixels = 200, Inventor.Color backgroundColor = null)
        {
            try
            {
                if (!(doc is PartDocument) && !(doc is AssemblyDocument))
                    return;

                Trace.TraceInformation("Exporting thumbnail for: " + System.IO.Path.GetFileName(doc.FullFileName));

                DirectoryInfo dir = Directory.CreateDirectory(outputFolderPath);

                // turn off unwanted objects
                dynamic invDoc = doc;
                bool showWorkFeatures = invDoc.ObjectVisibility.AllWorkFeatures;
                bool showSketches = invDoc.ObjectVisibility.Sketches;
                bool showSketches3D = invDoc.ObjectVisibility.Sketches3D;
                bool showSketchDimensions = invDoc.ObjectVisibility.SketchDimensions;
                bool showAnnotations3D = invDoc.ObjectVisibility.Annotations3D;

                invDoc.ObjectVisibility.AllWorkFeatures = false;
                invDoc.ObjectVisibility.Sketches = false;
                invDoc.ObjectVisibility.Sketches3D = false;
                invDoc.ObjectVisibility.SketchDimensions = false;
                invDoc.ObjectVisibility.Annotations3D = false;

                // setup camera
                dynamic inventorApp = doc.Parent;
                inventorApp.DisplayOptions.Show3DIndicator = false;
                Camera cam = inventorApp.TransientObjects.CreateCamera();
                cam.SceneObject = invDoc.ComponentDefinition;
                cam.ViewOrientationType = ViewOrientationTypeEnum.kIsoTopRightViewOrientation;
                cam.Fit();
                cam.ApplyWithoutTransition();

                // apply background color
                if (backgroundColor == null)
                {
                    backgroundColor = inventorApp.TransientObjects.CreateColor(0xEC, 0xEC, 0xEC, 0.0); // hardcoded. Make as a parameter
                    //backgroundColor = inventorApp.TransientObjects.CreateColor(0, 0, 0); // black, tested this for converting to transparent background but it resulted in jagged edges
                }

                // generate image twice as large, and then downsample it (antialiasing)
                string filePathLarge = System.IO.Path.Combine(outputFolderPath, "thumbnail-large.png");
                cam.SaveAsBitmap(filePathLarge, sizeInPixels * 2, sizeInPixels * 2, backgroundColor, backgroundColor);

                // reset object visibility to inital settings
                invDoc.ObjectVisibility.AllWorkFeatures = showWorkFeatures;
                invDoc.ObjectVisibility.Sketches = showSketches;
                invDoc.ObjectVisibility.Sketches3D = showSketches3D;
                invDoc.ObjectVisibility.Annotations3D = showAnnotations3D;
                invDoc.ObjectVisibility.SketchDimensions = showSketchDimensions;

                // resize image, see reference https://stackoverflow.com/a/24199315
                using (var image = Image.FromFile(filePathLarge))
                using (var destImage = new Bitmap(sizeInPixels, sizeInPixels))
                {

                    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                    using (var graphics = Graphics.FromImage(destImage))
                    {
                        graphics.CompositingMode = CompositingMode.SourceCopy;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        using (var wrapMode = new ImageAttributes())
                        {
                            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                            var destRect = new Rectangle(0, 0, sizeInPixels, sizeInPixels);
                            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                        }
                    }

                    //// replace background pixels with transparent color
                    //System.Drawing.Color colorToReplace = backgroundColor.ToSystemDrawingColor();
                    //System.Drawing.Color transparentColor = System.Drawing.Color.FromArgb(0,0,0,0);
                    //for (int x = 0; x < destImage.Width; x++)
                    //{
                    //    for (int y = 0; y < destImage.Height; y++)
                    //    {
                    //        if (destImage.GetPixel(x, y).Equals(colorToReplace))
                    //            destImage.SetPixel(x, y, transparentColor);
                    //    }
                    //}

                    string filePath = System.IO.Path.Combine(outputFolderPath, "thumbnail.png");
                    destImage.Save(filePath);

                    // clean up
                    System.IO.File.Delete(filePathLarge);

                    Trace.WriteLine($"Thumbnail exported successfully: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Could not export thumbnail - {ex}");
            }
        }

        public static void ExportStp(Document doc, string resultFileName)
        {
            try
            {
                Trace.TraceInformation("Exporting stp for: " + System.IO.Path.GetFileName(doc.FullFileName));

                // Get the STEP translator Add-In.
                dynamic inventorApp = doc.Parent;
                TranslatorAddIn oSTPAddin = (TranslatorAddIn)inventorApp.ApplicationAddIns.ItemById["{90AF7F40-0C01-11D5-8E83-0010B541CD80}"];
                TranslationContext oContext = inventorApp.TransientObjects.CreateTranslationContext();
                oContext.Type = IOMechanismEnum.kFileBrowseIOMechanism;

                // Options for drawings...
                NameValueMap oOptions = inventorApp.TransientObjects.CreateNameValueMap();
                //oOptions.Value["ApplicationProtocolType"] = 3; //automotive design

                // Create a DataMedium object
                DataMedium oDataMedium = inventorApp.TransientObjects.CreateDataMedium();

                // Set the destination file name
                oDataMedium.FileName = resultFileName;

                // Publish document.
                oSTPAddin.SaveCopyAs(doc, oContext, oOptions, oDataMedium);


                Trace.WriteLine($"Stp exported successfully: {oDataMedium.FileName}");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Could not export stp - {ex}");
            }
        }

        public static void ExportPdf(DrawingDocument doc, string resultFileName)
        {
            try
            {
                Trace.TraceInformation("Exporting pdf for: " + System.IO.Path.GetFileName(doc.FullFileName));

                dynamic inventorApp = doc.Parent;
                TranslatorAddIn oPDFAddin = (TranslatorAddIn)inventorApp.ApplicationAddIns.ItemById["{0AC6FD96-2F4D-42CE-8BE0-8AEA580399E4}"];
                TranslationContext oContext = inventorApp.TransientObjects.CreateTranslationContext();
                oContext.Type = IOMechanismEnum.kFileBrowseIOMechanism;

                // Options for drawings...
                NameValueMap oOptions = inventorApp.TransientObjects.CreateNameValueMap();
                oOptions.Value["Vector_Resolution"] = 720;
                oOptions.Value["Sheet_Range"] = PrintRangeEnum.kPrintSheetRange;

                // Create a DataMedium object
                DataMedium oDataMedium = inventorApp.TransientObjects.CreateDataMedium();

                // Set the destination file name
                oDataMedium.FileName = resultFileName;

                // Publish document.
                oPDFAddin.SaveCopyAs(doc, oContext, oOptions, oDataMedium);

                Trace.WriteLine($"Pdf exported successfully: {oDataMedium.FileName}");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Could not export pdf - {ex}");
            }
        }

    }
}

