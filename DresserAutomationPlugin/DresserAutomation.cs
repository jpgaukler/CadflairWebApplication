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

using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using Inventor;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace DresserAutomationPlugin
{
    [ComVisible(true)]
    public class DresserAutomation
    {
        public DresserAutomation(InventorServer inventorApp)
        {
            Globals.InventorApplication = inventorApp;
        }

        public void Run(Document doc)
        {
            Trace.TraceInformation("Run called with {0}", System.IO.Path.GetFileName(doc.FullFileName));
        }

        public void RunWithArguments(Document doc, NameValueMap map)
        {
            try
            {
                string jsonFileName = Convert.ToString(map.Value[map.Name[1]]);
                //Trace.TraceInformation($"Json file = {jsonFile}");

                string jsonValues = System.IO.File.ReadAllText(jsonFileName);
                Trace.WriteLine($"Converting json to NameValueMap = {jsonValues}");

                Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonValues);
                foreach (KeyValuePair<string, string> entry in parameters)
                {
                    map.Add(entry.Key, entry.Value);
                }

                if ((string)map.Value["action"] == "GenerateModel")
                {
                    using (new HeartBeat())
                    {
                        GenerateModel((AssemblyDocument)doc, map);
                        GenerateDrawing((AssemblyDocument)doc);
                    }
                }
                else if ((string)map.Value["action"] == "GenerateDrawings")
                {
                    Trace.WriteLine("Generating dresser drawing (not implemented)...");
                    //using (new HeartBeat())
                    //{
                    //    GenerateDrawing((AssemblyDocument)doc);
                    //}
                }

            }
            catch (Exception ex)
            {
                Trace.TraceError("RunWithArguments failed - " + ex.ToString());
            }
        }

        private void GenerateModel(AssemblyDocument dresserAssembly, NameValueMap map)
        {
            Trace.TraceInformation("Generating dresser model: " + System.IO.Path.GetFileName(dresserAssembly.FullFileName));            

            for (int i = 1; i <= map.Count; i++)
            {
                string name = map.Name[i];
                string value = Convert.ToString(map.Value[name]);
                Trace.WriteLine($"Parameter: {name} = {value}");
            }

            AssemblyDocument frame = null;
            AssemblyDocument drawer = null;
            PartDocument skeleton = null;
            PartDocument leg = null;
            PartDocument topPanel = null;
            PartDocument drawerFrontPanel = null;

            foreach (Document i in dresserAssembly.AllReferencedDocuments)
            {
                if (i.FullFileName.Contains("SKELETON"))
                {
                    skeleton = (PartDocument)i;
                }
                else if (i.FullFileName.Contains("Drawer Assembly"))
                {
                    drawer = (AssemblyDocument)i;
                }
                else if (i.FullFileName.Contains("Dresser Frame"))
                {
                    frame = (AssemblyDocument)i;
                }
                else if (i.FullFileName.Contains("Leg"))
                {
                    leg = (PartDocument)i;
                }
                else if (i.FullFileName.Contains("Top Panel"))
                {
                    topPanel = (PartDocument)i;
                }
                else if (i.FullFileName.Contains("Drawer Front Panel"))
                {
                    drawerFrontPanel = (PartDocument)i;
                }
            }

            //update part number
            Trace.WriteLine("Updating part number...");            
            string partNumber = map.Value["partNumber"];
            Property partNumberProp = dresserAssembly.PropertySets["Design Tracking Properties"]["Part Number"];
            partNumberProp.Value = partNumber;

            //update parameters
            Trace.WriteLine("Updating document parameters...");

            Parameters frameParams = frame.ComponentDefinition.Parameters;
            frameParams["finishStyle"].Value = map.Value["finishStyle"];

            Parameters drawerParams = drawer.ComponentDefinition.Parameters;
            drawerParams["handleStyle"].Value = map.Value["handleStyle"];
            drawerParams["finishStyle"].Value = map.Value["finishStyle"];

            Parameters legParams = leg.ComponentDefinition.Parameters;
            legParams["edgeStyle"].Value = map.Value["edgeStyle"];

            Parameters topPanelParams = topPanel.ComponentDefinition.Parameters;
            topPanelParams["edgeStyle"].Value = map.Value["edgeStyle"];

            Parameters drawerFrontPanelParams = drawerFrontPanel.ComponentDefinition.Parameters;
            drawerFrontPanelParams["edgeStyle"].Value = map.Value["edgeStyle"];

            Parameters skeletonParams = skeleton.ComponentDefinition.Parameters;

            skeletonParams["dresserDepth"].Value = Convert.ToDouble(map.Value["dresserDepth"]);
            skeletonParams["dresserWidth"].Value = Convert.ToDouble(map.Value["dresserWidth"]);
            skeletonParams["dresserHeight"].Value = Convert.ToDouble(map.Value["dresserHeight"]);
            skeletonParams["drawerRows"].Value = Convert.ToInt32(map.Value["drawerRows"]);
            skeletonParams["drawerColumns"].Value = Convert.ToInt32(map.Value["drawerColumns"]);
            skeletonParams["legDepth"].Value = Convert.ToDouble(map.Value["legDepth"]);
            skeletonParams["legWidth"].Value = Convert.ToDouble(map.Value["legWidth"]);
            skeletonParams["legExposure"].Value = Convert.ToDouble(map.Value["legExposure"]);

            dresserAssembly.Update();

            //set all drawers to associative default representation
            OccurrencePattern drawerPattern = dresserAssembly.ComponentDefinition.OccurrencePatterns["Drawer Pattern"];
            foreach (OccurrencePatternElement element in drawerPattern.OccurrencePatternElements)
            {
                foreach (ComponentOccurrence drawerOccurrence in element.Components)
                {
                    drawerOccurrence.SetDesignViewRepresentation("Default", "", true);
                }
            }

            dresserAssembly.Save();
            Trace.WriteLine("Document saved:" + dresserAssembly.FullFileName);
        }

        private void GenerateDrawing(AssemblyDocument dresserAssembly)
        {
            Trace.TraceInformation("Generating dresser drawings: " + System.IO.Path.GetFileName(dresserAssembly.FullFileName));

            //open drawing doc
            string drawingName = System.IO.Path.GetFileNameWithoutExtension(dresserAssembly.FullFileName) + ".idw";
            string fullDrawingName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(dresserAssembly.FullFileName), drawingName);
            DrawingDocument dwgdoc = (DrawingDocument)Globals.InventorApplication.Documents.Open(fullDrawingName, false);

            //regresh drawing
            DrawingAutomation.RefreshAutomatedDrawingObjects(dwgdoc);  

            //save pdf
            DrawingAutomation.ExportPDF(dwgdoc);            

            //save and close
            dwgdoc.Close(SkipSave: false);
            Trace.WriteLine("Document saved:" + drawingName);
        }      
    }
}


//#region Logging utilities

///// <summary>
///// Log message with 'trace' log level.
///// </summary>
//private static void LogTrace(string format, params object[] args)
//{
//    Trace.TraceInformation(format, args);
//}

///// <summary>
///// Log message with 'trace' log level.
///// </summary>
//private static void LogTrace(string message)
//{
//    Trace.TraceInformation(message);
//}

///// <summary>
///// Log message with 'error' log level.
///// </summary>
//private static void LogError(string format, params object[] args)
//{
//    Trace.TraceError(format, args);
//}

///// <summary>
///// Log message with 'error' log level.
///// </summary>
//private static void LogError(string message)
//{
//    Trace.TraceError(message);
//}

//#endregion


//// Using NameValueMapExtension
//if (map.HasKey("intIndex"))
//{
//    int intValue = map.AsInt("intIndex");
//    Trace.TraceInformation($"Value of intIndex is: {intValue}");
//}

//if (map.HasKey("stringCollectionIndex"))
//{
//    IEnumerable<string> strCollection = map.AsStringCollection("stringCollectionIndex");

//    foreach (string strValue in strCollection)
//    {
//        Trace.TraceInformation($"String value is: {strValue}");
//    }
//}