using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils.Helpers;
using System;
using System.Diagnostics;

namespace CadflairInventorPlugin.Automation
{
    static class ModelAutomation
    {
        public static void UpdateParameters(Document doc, NameValueMap map)
        {
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
    }
}
