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
        public static void GenerateModel(Document doc, NameValueMap map)
        {
            Trace.TraceInformation("Generating model: " + System.IO.Path.GetFileName(doc.FullFileName));

            //write parameters to log for debugging purposes
            Trace.WriteLine("Mapped parameters:");
            for (int i = 1; i <= map.Count; i++)
            {
                string name = map.Name[i];
                string value = Convert.ToString(map.Value[name]);
                Trace.WriteLine($"Parameter: {name} = {value}");
            }

            //update parameters
            Trace.WriteLine("Updating document parameters...");
            foreach (Parameter p in ((dynamic)doc).ComponentDefinition.Parameters)
            {
                if (map.HasKey(p.Name))
                {
                    try
                    {
                        Trace.WriteLine("Setting " + p.Name + " = " + (string)map.Value[p.Name]);
                        Trace.WriteLine(p.Name + " units = " + p.get_Units());

                        if(p.get_Units() == "Text")
                        {
                            p.Value = (string)map.Value[p.Name];

                        }
                        else
                        {
                            p.Expression = (string)map.Value[p.Name];
                        }
                    }
                    catch
                    {
                        Trace.WriteLine("Failed to set paraemeter value");
                    }
                }
            }

            //update iProperties
            if (map.HasKey("partNumber"))
            {
                Trace.WriteLine("Updating part number...");
                doc.PropertySets["Design Tracking Properties"]["Part Number"].Value = map.Value["partNumber"];
            }

            //update and save doc
            doc.Update();
            doc.Save();
            Trace.WriteLine("Document saved:" + doc.FullFileName);

            //export stp
            ExportStp(doc, (string)map.Value["outputObjectKey"]);
        }

        private static void ExportStp(Document doc, string filename)
        {
            try
            {
                Trace.TraceInformation("Exporting stp for: " + System.IO.Path.GetFileName(doc.FullFileName));

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

                Trace.WriteLine("Stp exported successfully: " + stpName);

            }
            catch (Exception ex)
            {
                Trace.TraceError("Could not export stp - " + ex.ToString());
            }
        }
    }
}
