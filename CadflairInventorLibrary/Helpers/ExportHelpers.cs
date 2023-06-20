using Inventor;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace CadflairInventorLibrary.Helpers
{
    public static class ExportHelpers
    {

        /// <summary>
        /// Generates Svf files for the given document in a temporary folder. The svf files will be converted to a flattened file structure and saved to a zip folder
        /// for upload to Cadflair. These files can then be uploaded to Forge OSS to be used by the model viewer.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="outputFolderPath"></param>
        /// <returns>The file name of the resulting zip folder.</returns>
        public static string ExportSvfAsZip(Document doc, string outputFolderPath)
        {
            DirectoryInfo dir = Directory.CreateDirectory(outputFolderPath);

            // export the svf files to the temp folder
            ExportSvf(doc, outputFolderPath);

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

                // include the thumbnail as png (THIS IS INTENDED TO BE A TEMPORARY WORKAROUND)
                string thumbnailFilename = System.IO.Path.Combine(outputFolderPath, "thumbnail.png");
                doc.Thumbnail.ToImage(200, 200).Save(thumbnailFilename);

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

