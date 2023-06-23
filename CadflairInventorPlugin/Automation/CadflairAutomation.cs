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
using Newtonsoft.Json;
using System.IO;
using CadflairInventorLibrary.Helpers;

namespace CadflairInventorPlugin.Automation
{
    [ComVisible(true)]
    public class CadflairAutomation
    {
        public CadflairAutomation() { }

        public void Run(Document doc)
        {
            Trace.TraceInformation($"Run called on {System.IO.Path.GetFileName(doc.FullFileName)}");
        }

        public void RunWithArguments(Document doc, NameValueMap map)
        {
            Trace.TraceInformation($"RunWithArguments called on {System.IO.Path.GetFileName(doc.FullFileName)}");

            try
            {
                // start HeartBeat to ensure process does not get force closed
                using (new HeartBeat())
                {
                    //get parameters from json file
                    string jsonFileName = Convert.ToString(map.Value[map.Name[1]]);
                    string jsonValues = System.IO.File.ReadAllText(jsonFileName);

                    Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonValues);
                    foreach (KeyValuePair<string, string> entry in parameters)
                    {
                        map.Add(entry.Key, entry.Value);
                    }

                    // update parameters
                    ReportProgress("Updating model parameters...");
                    ModelAutomation.UpdateParameters(doc, map);

                    // update and save doc
                    ReportProgress("Saving document...");
                    doc.Update();
                    doc.Save();

                    // export results
                    ReportProgress("Exporting viewables...");

                    string rootFolderPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(doc.FullFileName)); // input files get unzipped to a folder in the root directory, go up two levels to get the root
                    string stpFilename = System.IO.Path.Combine(rootFolderPath, "Result.stp");
                    string viewablesFolderPath = System.IO.Path.Combine(rootFolderPath, "Viewables");
                    ExportHelpers.ExportStp(doc, stpFilename);
                    ExportHelpers.ExportSvf(doc, viewablesFolderPath);
                    ExportHelpers.ExportThumbnail(doc, viewablesFolderPath);

                    // update drawing and export pdf
                    //DrawingAutomation.GenerateDrawing(doc, map);

                    doc.Close(SkipSave: true);

                    ReportProgress("Uploading results...");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError($"RunWithArguments failed - {ex}");
            }
        }


        private void ReportProgress(string message)
        {
            dynamic progress = new
            {
                message,
            };

            string progressJson = JsonConvert.SerializeObject(progress);
            Trace.TraceInformation("!ACESAPI:acesHttpOperation({0},\"\",\"\",{1},null)", "onProgress", progressJson);
        }

    }
}

