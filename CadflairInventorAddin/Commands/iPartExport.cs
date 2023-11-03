using CadflairInventorAddin.Helpers;
using CadflairInventorLibrary.Helpers;
using Inventor;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Path = System.IO.Path;
using ProgressBar = Inventor.ProgressBar;

namespace CadflairInventorAddin.Commands.iParts
{
    internal static class iPartExport
    {
        public static ButtonDefinition ExportiPartsPdfsButton { get; set; }
        public static ButtonDefinition ExportiPartsStpsButton { get; set; }

        public static void ExportiPartStpsButton_OnExecute(NameValueMap Context)
        {
            try
            {
                PartDocument doc = (PartDocument)Globals.InventorApplication.ActiveDocument;
                iPartFactory factory = doc.ComponentDefinition.iPartFactory;

                string folderName = Path.Combine(Path.GetDirectoryName(doc.FullFileName), "iPart Stp Export");

                if (Directory.Exists(folderName))
                {
                    if(MessageBox.Show("An existing export folder has been found, would you like to delete it to continue?", "Delete Existing Folder?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        Directory.Delete(folderName, true);
                }

                Directory.CreateDirectory(folderName);

                string logFileName = Path.Combine(folderName, "Log.txt");
                StreamWriter logFile = System.IO.File.CreateText(logFileName);

                ProgressBar progressBar = Globals.InventorApplication.CreateProgressBar(false, factory.TableRows.Count, "Export Progress");
                progressBar.Message = "Exporting stp files...";

                foreach(iPartTableRow row in factory.TableRows)
                {
                    try
                    {
                        // set active row
                        factory.DefaultRow = row;

                        // export stp
                        string filename = Path.Combine(folderName, $"{row.MemberName}.stp");
                        ExportHelpers.ExportStp((Document)doc, filename);

                        logFile.WriteLine($"INFO: Successfully exported stp file for member: {row.MemberName}");
                    }
                    catch (Exception ex)
                    {
                        logFile.WriteLine($"ERROR: Failed to stp file for member: {row.MemberName}");
                        logFile.WriteLine(ex.ToString());
                    }

                    progressBar.UpdateProgress();
                }

                progressBar.Close();
                logFile.Close();
                Process.Start(folderName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ExportiPartStpsButton_OnExecute");
                MessageBox.Show(ex.ToString(), "Export failed!");
            }
        }

        public static void ExportiPartPdfsButton_OnExecute(NameValueMap Context)
        {
            try
            {
                DrawingDocument drawingDoc = (DrawingDocument)Globals.InventorApplication.ActiveDocument;
                PartDocument partDoc = (PartDocument)drawingDoc.ReferencedDocuments[1];
                iPartFactory factory = partDoc.ComponentDefinition.iPartFactory;

                string folderName = Path.Combine(Path.GetDirectoryName(drawingDoc.FullFileName), "iPart Pdf Export");

                if (Directory.Exists(folderName))
                {
                    if(MessageBox.Show("An existing export folder has been found, would you like to delete it to continue?", "Delete Existing Folder?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        Directory.Delete(folderName, true);
                }

                Directory.CreateDirectory(folderName);

                string logFileName = Path.Combine(folderName, "Log.txt");
                StreamWriter logFile = System.IO.File.CreateText(logFileName);

                ProgressBar progressBar = Globals.InventorApplication.CreateProgressBar(false, factory.TableRows.Count, "Export Progress");
                progressBar.Message = "Exporting pdf files...";

                foreach(iPartTableRow row in factory.TableRows)
                {
                    try
                    {
                        // set active row
                        factory.DefaultRow = row;

                        // update doc and refresh scales and dimensions
                        drawingDoc.Update();
                        DrawingAttributesCommand.RefreshAutomatedDrawingObjects(drawingDoc);

                        // export pdf
                        string filename = Path.Combine(folderName, $"{row.MemberName}.pdf");
                        ExportHelpers.ExportPdf(drawingDoc, filename);

                        logFile.WriteLine($"INFO: Successfully exported pdf file for member: {row.MemberName}");
                    }
                    catch (Exception ex)
                    {
                        logFile.WriteLine($"ERROR: Failed to pdf file for member: {row.MemberName}");
                        logFile.WriteLine(ex.ToString());
                    }

                    progressBar.UpdateProgress();
                }

                progressBar.Close();
                logFile.Close();
                Process.Start(folderName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ExportiPartPdfsButton_OnExecute");
                MessageBox.Show(ex.ToString(), "Export failed!");
            }
        }
    }
}

