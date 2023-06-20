using CadflairInventorAddin.Commands.Upload;
using CadflairInventorAddin.Helpers;
using CadflairInventorLibrary.Helpers;
using Inventor;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;

namespace CadflairInventorAddin.Commands
{
    internal static class ExportSvf
    {
        public static ButtonDefinition ExportSvfButton { get; set; }

        public static void ExportSvfButton_OnExecute(NameValueMap Context)
        {
            try
            {
                Document doc = Globals.InventorApplication.ActiveDocument;

                // export svf files for on demand upload
                //string svfFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(doc.FullFileName), "Svf");
                //DirectoryInfo dir = Directory.CreateDirectory(svfFolder);

                // export the svf files to the temp folder
                //UploadToCadflair.ExportSvf(doc, svfFolder);

                //foreach (FileInfo fileInfo in dir.GetFiles())
                //{
                //    if (fileInfo.Extension.Contains("gz"))
                //    {
                //        Decompress(fileInfo);
                //    }
                //}

                //string thumbnailFilename = System.IO.Path.Combine(svfFolder, "thumbnail.png");
                //doc.Thumbnail.ToImage(200, 200).Save(thumbnailFilename);
                //Process.Start(svfFolder);
                //MessageBox.Show("Export successful!");

                //// create output folder
                //string outputFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(doc.FullFileName), "Svf Output Flat");
                //Directory.CreateDirectory(outputFolder);

                //ExportToSvf(doc, outputFolder);

                //// flatten to single folder
                //FlattenSvfToSingleFolder(outputFolder, outputFolder, null);

                //Process.Start(outputFolder);
                //MessageBox.Show("Export successful!");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ExportSvfButton_OnExecute");
                MessageBox.Show("Export failed!");
            }
        }


        public static void Decompress(FileInfo fileToDecompress)
        {
            string currentFileName = fileToDecompress.FullName;
            string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            using (FileStream decompressedFileStream = System.IO.File.Create(newFileName))
            using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
            {
                decompressionStream.CopyTo(decompressedFileStream);
            }
        }

    }
}

