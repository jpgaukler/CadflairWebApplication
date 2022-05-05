/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using Autodesk.Forge.DesignAutomation.Inventor.Utils.Helpers;
using Newtonsoft.Json;

namespace SiloAutomationPlugin
{
    [ComVisible(true)]
    public class SiloAutomation
    {

        public SiloAutomation(InventorServer inventorApp)
        {
            Globals.InventorApplication = inventorApp;
        }

        public void Run(Document doc)
        {
            Trace.TraceInformation("Run called with {0}", doc.DisplayName);
        }

        public void RunWithArguments(Document doc, NameValueMap map)
        {
            try
            {

                //start HeartBeat to ensure process does not get force closed
                using (new HeartBeat())
                {
                    //get parameters from json file
                    Trace.WriteLine("Converting json to NameValueMap");
                    string jsonFileName = Convert.ToString(map.Value[map.Name[1]]);
                    string jsonValues = System.IO.File.ReadAllText(jsonFileName);

                    Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonValues);
                    foreach (KeyValuePair<string, string> entry in parameters)
                    {
                        map.Add(entry.Key, entry.Value);
                    }

                    //update model
                    GenerateModel((PartDocument)doc, map);

                    //update drawing
                    //GenerateDrawing((PartDocument)doc);


                    doc.Close(SkipSave: true);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("RunWithArguments failed - " + ex.ToString());
            }
        }

        private void GenerateModel(PartDocument doc, NameValueMap map)
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
            Parameters parameters = doc.ComponentDefinition.Parameters;
            foreach (Parameter p in doc.ComponentDefinition.Parameters)
            {
                if (p.IsKey)
                {
                    p.Expression = map.Value[p.Name];
                }
            }


            //update iProperties
            Trace.WriteLine("Updating document iProperties...");
            doc.PropertySets["Design Tracking Properties"]["Part Number"].Value = map.Value["partNumber"];

            doc.Update();
            doc.Save();
            Trace.WriteLine("Document saved:" + doc.FullFileName);
        }

        private void GenerateDrawing(PartDocument doc)
        {
            Trace.TraceInformation("Generating drawing: " + System.IO.Path.GetFileName(doc.FullFileName));

            //open drawing doc
            string drawingName = System.IO.Path.GetFileNameWithoutExtension(doc.FullFileName) + ".idw";
            string fullDrawingName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(doc.FullFileName), drawingName);
            DrawingDocument dwgdoc = (DrawingDocument)Globals.InventorApplication.Documents.Open(fullDrawingName, OpenVisible: false);

            //refresh drawing
            DrawingAutomation.RefreshAutomatedDrawingObjects(dwgdoc);

            //save pdf
            DrawingAutomation.ExportPDF(dwgdoc);

            //save and close
            dwgdoc.Close(SkipSave: false);
            Trace.WriteLine("Document saved:" + drawingName);
        }

    }
}

            //parameters["coneAngle"].Value = map.Value["coneAngle"];
            //parameters["dischargeHeight"].Value = map.Value["dischargeHeight"];
            //parameters["innerDiam"].Value = map.Value["innerDiam"];
            //parameters["ladderAngle"].Value = map.Value["ladderAngle"];
            //parameters["ladderOffsetHeight"].Value = map.Value["ladderOffsetHeight"];
            //parameters["ladderOvershoot"].Value = map.Value["ladderOvershoot"];
            //parameters["ladderRadialOffset"].Value = map.Value["ladderRadialOffset"];
            //parameters["ladderWidth"].Value = map.Value["ladderWidth"];
            //parameters["legMountingOffset"].Value = map.Value["legMountingOffset"];
            //parameters["legTubeDiam"].Value = map.Value["legTubeDiam"];
            //parameters["outletDiam"].Value = map.Value["outletDiam"];
            //parameters["outletStubHeight"].Value = map.Value["outletStubHeight"];
            //parameters["platformDiam"].Value = map.Value["platformDiam"];
            //parameters["postDiam"].Value = map.Value["postDiam"];
            //parameters["railingCount"].Value = map.Value["railingCount"];
            //parameters["ribCount"].Value = map.Value["ribCount"];
            //parameters["ribDiam"].Value = map.Value["ribDiam"];
            //parameters["rungOffsetHeight"].Value = map.Value["rungOffsetHeight"];
            //parameters["rungSpacing"].Value = map.Value["rungSpacing"];
            //parameters["siloHeight"].Value = map.Value["siloHeight"];
            //parameters["topslope"].Value = map.Value["topslope"];


//#region Logging utilities

//     /// <summary>
//     /// Log message with 'trace' log level.
//     /// </summary>
//     private static void LogTrace(string format, params object[] args)
//     {
//         Trace.TraceInformation(format, args);
//     }

//     /// <summary>
//     /// Log message with 'trace' log level.
//     /// </summary>
//     private static void LogTrace(string message)
//     {
//         Trace.TraceInformation(message);
//     }

//     /// <summary>
//     /// Log message with 'error' log level.
//     /// </summary>
//     private static void LogError(string format, params object[] args)
//     {
//         Trace.TraceError(format, args);
//     }

//     /// <summary>
//     /// Log message with 'error' log level.
//     /// </summary>
//     private static void LogError(string message)
//     {
//         Trace.TraceError(message);
//     }

//     #endregion
