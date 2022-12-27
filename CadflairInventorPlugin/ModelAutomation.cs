using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadflairInventorPlugin
{
    static class ModelAutomation
    {
        public static void UpdateParameters(Document doc, NameValueMap map)
        {
            Globals.ReportProgress("Updating model parameters");
            foreach (Parameter p in ((dynamic)doc).ComponentDefinition.Parameters)
            {
                if (map.HasKey(p.Name))
                {
                    try
                    {
                        Trace.WriteLine("Setting " + p.Name + " = " + (string)map.Value[p.Name]);

                        if (p.get_Units() == "Text")
                        {
                            p.Value = (string)map.Value[p.Name];

                        }
                        else
                        {
                            p.Expression = (string)map.Value[p.Name];
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError($"Failed to set parameter {p.Name} - {ex}");
                    }
                }
            }
        }

        public static void UpdateIProperties(Document doc, NameValueMap map)
        {
            ////update iProperties
            //if (map.HasKey("partNumber"))
            //{
            //    Trace.WriteLine("Updating part number...");
            //    doc.PropertySets["Design Tracking Properties"]["Part Number"].Value = map.Value["partNumber"];
            //}
        }

        private static void ExportStp(Document doc, string filename)
        {
            try
            {
                Globals.ReportProgress("Exporting stp file");

                // Get the STEP translator Add-In.
                TranslatorAddIn oSTPAddin = (TranslatorAddIn)Globals.InventorApplication.ApplicationAddIns.ItemById["{90AF7F40-0C01-11D5-8E83-0010B541CD80}"];
                TranslationContext oContext = Globals.InventorApplication.TransientObjects.CreateTranslationContext();
                oContext.Type = IOMechanismEnum.kFileBrowseIOMechanism;

                // Options for drawings...
                NameValueMap oOptions = Globals.InventorApplication.TransientObjects.CreateNameValueMap();
                //oOptions.Value["ApplicationProtocolType"] = 3; //automotive design

                // Create a DataMedium object
                DataMedium oDataMedium = Globals.InventorApplication.TransientObjects.CreateDataMedium();

                // Set the destination file name
                //string stpName = System.IO.Path.GetFileNameWithoutExtension(doc.FullFileName) + ".stp";
                string stpName = filename + ".stp";
                oDataMedium.FileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(doc.FullFileName), stpName);

                // Publish document.
                oSTPAddin.SaveCopyAs(doc, oContext, oOptions, oDataMedium);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Falied to export stp - " + ex.ToString());
            }
        }
    }
}
