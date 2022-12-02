using CadflairDataAccess.Models;
using CadflairInventorAddin.Utilities;
using Inventor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CadflairInventorAddin.Commands
{
    internal static class UploadToCadflair
    {
        private static UploadToCadflairControl _uploadControl;
        public static DockableWindow UploadWindow { get; set; }

        public static void UploadToCadflairButton_OnExecute(NameValueMap Context)
        {
            try
            {
                if (_uploadControl != null) return;

                _uploadControl = new UploadToCadflairControl(Globals.InventorApplication.ActiveDocument);
                UploadWindow.AddChild(_uploadControl.Handle);
                UploadWindow.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static void UploadWindow_OnHide(DockableWindow DockableWindow, EventTimingEnum BeforeOrAfter, NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            HandlingCode = HandlingCodeEnum.kEventNotHandled;

            if (DockableWindow.InternalName != UploadWindow.InternalName) return;
            if (BeforeOrAfter == EventTimingEnum.kAfter) return;

            UploadWindow.Clear();

            if(UploadWindow != null)
            {
                _uploadControl.Dispose();
                _uploadControl = null;
            }

            HandlingCode = HandlingCodeEnum.kEventHandled;
        }

        public static void UploadWindow_OnHelp(DockableWindow DockableWindow, NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            HandlingCode = HandlingCodeEnum.kEventNotHandled;

            if (DockableWindow.InternalName != UploadWindow.InternalName) return;

            // ADD A LINK TO CADFLAIR HELP PAGE HERE
            Process.Start("http://www.cadflair.com/");

            HandlingCode = HandlingCodeEnum.kEventHandled;
        }

        public static List<ILogicUiElement> GetILogicFormElements(Document doc)
        {
            List<ILogicUiElement> iLogicForms = new List<ILogicUiElement>();

            foreach (AttributeSet set in doc.AttributeSets)
            {
                //ignore all attribute sets that are not iLogicUi
                if (!set.Name.ToLower().Contains("ilogicinternalui")) continue;

                //get form spec attribute
                Inventor.Attribute formSpec = set["FormSpec"];

                //convert byte array to xml string
                byte[] bytes = (byte[])formSpec.Value;
                string xmlString = Encoding.UTF8.GetString(bytes);

                //convert xml string to xdoc for parsing
                XDocument xDoc = XDocument.Parse(xmlString);

                //ignore the browser control spec
                string formName = xDoc.Element("FormSpecification").Element("Name").Value;
                if (formName.Contains("iLogicBrowserUiFormSpecification")) continue;

                //consruct ILogicUIElement object
                ILogicUiElement formElement = new ILogicUiElement()
                {
                    Name = xDoc.Element("FormSpecification").Element("Name").Value,
                    Items = RecurseILogicElements(doc, xDoc.Element("FormSpecification").Element("Items"), xDoc.Root.GetNamespaceOfPrefix("xsi"))
                    //Guid = formSpecElement.Element("Guid").Value,
                };

                //add to list
                iLogicForms.Add(formElement);
            }

            return iLogicForms;

        }

        private static List<ILogicUiElement> RecurseILogicElements(Document doc, XElement xItems, XNamespace ns)
        {

            //how should I handle readonly parameters?
            //should I handle rows?
            //should I handle picture folders?
            //should I handle empty space?
            //should I handle iProperties or iLogic rules?

            if (xItems == null) return null;

            List<ILogicUiElement> iLogicUiElementList = new List<ILogicUiElement>();
            ILogicUiElement tabContainerElement = null;

            foreach (XElement xItem in xItems.Elements())
            {
                Parameter parameter = doc.GetParameter(xItem.Element("ParameterName")?.Value);

                ILogicUiElement iLogicUiElement = new ILogicUiElement()
                {
                    //Guid = item.Element("Guid").Value,
                    UiElementSpec = xItem.Attribute(ns + "type").Value,
                    Name = xItem.Element("Name").Value,
                    EditControlType = xItem.Element("EditControlType")?.Value,
                    ReadOnly = Convert.ToBoolean(xItem.Element("ReadOnly")?.Value),
                    ToolTip = xItem.Element("ToolTip")?.Value,
                    EnablingParameterName = xItem.Element("EnablingParameterName")?.Value,
                    TrackBarMinValue = xItem.Element("EditControlType")?.Value == "TrackBar" ? double.Parse(xItem.Element("TrackBarProperties")?.Element("MinimumValue").Value) : (double?)null,
                    TrackBarMaxValue = xItem.Element("EditControlType")?.Value == "TrackBar" ? double.Parse(xItem.Element("TrackBarProperties")?.Element("MaximumValue").Value) : (double?)null,
                    TrackBarIncrement = xItem.Element("EditControlType")?.Value == "TrackBar" ? double.Parse(xItem.Element("TrackBarProperties")?.Element("ValueIncrement").Value) : (double?)null,
                    ParameterName = parameter?.Name,
                    ParameterUnits = parameter?.get_Units(),
                    ParameterExpression = parameter?.Expression?.Replace("\"", ""),
                    ParameterExpressionList = parameter?.ExpressionList?.ToStringArray(),
                    ParameterMinValue = parameter?.GetMinAttributeValue(),
                    ParameterMaxValue = parameter?.GetMaxAttributeValue(),
                    //Base64Image = item.Element("Image")?.Element("BitmapByteArray")?.Value,
                    //Base64CaptionImage = item.Element("CaptionImage")?.Element("BitmapByteArray")?.Value,
                    Items = RecurseILogicElements(doc, xItem.Element("Items"), ns)
                };

                if (iLogicUiElement.UiElementSpec == "ControlTabGroupSpec")
                {
                    if (tabContainerElement == null)
                    {
                        tabContainerElement = new ILogicUiElement()
                        {
                            UiElementSpec = "ControlTabContainerSpec",
                            Items = new List<ILogicUiElement>()
                        };

                        iLogicUiElementList.Add(tabContainerElement);
                    }

                    tabContainerElement.Items.Add(iLogicUiElement);
                }
                else
                {
                    iLogicUiElementList.Add(iLogicUiElement);
                }
            }

            return iLogicUiElementList;
        }



        public static string SaveILogicUiElementToJson(ILogicUiElement element)
        {
            string jsonString = element.ToJson();

            string folderName = @"C:\Users\jpgau\source\repos\jpgaukler\CadflairWebApplication\Inventor Files";

            if (!Directory.Exists(folderName))
            {
                folderName = @"C:\Users\Admin\source\repos\CadflairWebApplication\Inventor Files";
            }

            string jsonFileName = System.IO.Path.Combine(folderName, element.Name + ".json");
            StreamWriter jsonFile = System.IO.File.CreateText(jsonFileName);

            jsonFile.Write(jsonString);
            jsonFile.Close();
            Clipboard.SetText(jsonString);

            return jsonString;

            //Process.Start(folderName);
        }

        public static void SaveILogicFormSpecToXml(string formName)
        {
            foreach (AttributeSet set in Globals.InventorApplication.ActiveDocument.AttributeSets)
            {
                //ignore all attribute sets that are not iLogicUi
                if (!set.Name.ToLower().Contains("ilogicinternalui")) continue;

                //get form spec attribute
                Inventor.Attribute formSpec = set["FormSpec"];

                //convert byte array to xml string
                byte[] bytes = (byte[])formSpec.Value;
                string xmlString = Encoding.UTF8.GetString(bytes);

                //convert xml string to xdoc for parsing
                XDocument xDoc = XDocument.Parse(xmlString);
                if (xDoc.Element("FormSpecification").Element("Name").Value != formName) continue;

                //print results string to txt files
                string folderName = @"C:\Users\jpgau\source\repos\jpgaukler\CadflairWebApplication\Inventor Files";

                if (!Directory.Exists(folderName))
                {
                    folderName = @"C:\Users\Admin\source\repos\CadflairWebApplication\Inventor Files";
                }

                string xmlFileName = System.IO.Path.Combine(folderName, formName + ".xml");

                StreamWriter xmlFile = System.IO.File.CreateText(xmlFileName);

                xmlFile.Write(xmlString);
                xmlFile.Close();

                return;
                //Process.Start(folderName);
            }
        }


        /// <summary>
        /// Create a zip folder in the temp directory for the Inventor document that is provided. 
        /// Resulting zip file will include all related child documents.
        /// Method will fail if the document has not yet been saved.
        /// <br></br>
        /// <br></br>
        /// includeDrawings - If a drawing file with the same name (.idw only) exists for any of the files given, it will be copied with the file.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="includeDrawings"></param>
        /// <returns>The full file name of the resulting zip file.</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static string CreateTemporaryZipFile(Document doc, bool includeDrawings)
        {
            if (doc.FileSaveCounter == 0) throw new FileNotFoundException();

            //save the doc
            doc.Save();

            //create the temp folder
            string tempFolderName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
            DirectoryInfo dir = Directory.CreateDirectory(tempFolderName);

            //copy the doc to the temp folder
            CopyInventorFile(doc, tempFolderName, true);

            //copy all the references to the temp folder
            foreach (Document refDoc in doc.AllReferencedDocuments)
            {
                CopyInventorFile(refDoc, tempFolderName, true);
            }

            //zip up the temp folder
            string zipFileName = $"{tempFolderName}.zip";
            System.IO.Compression.ZipFile.CreateFromDirectory(tempFolderName, zipFileName);

            //delete the copied files
            dir.Delete(true);

            //return the path of the zip file
            return zipFileName;
        }

        /// <summary>
        /// Copies an Inventor model to the specified directory. 
        /// <br></br>
        /// <br></br>
        /// includeDrawing - If a drawing file with the same name exists (.idw only), it will be copied with the file.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="destinationFolderName"></param>
        /// <param name="includeDrawing"></param>
        private static void CopyInventorFile(Document doc, string destinationFolderName, bool includeDrawing)
        {
            string filePath = System.IO.Path.GetDirectoryName(doc.FullFileName);
            string fileName = System.IO.Path.GetFileName(doc.FullFileName);
            string idwFileName = System.IO.Path.GetFileNameWithoutExtension(fileName) + ".idw";
            string fullIdwFileName = System.IO.Path.Combine(filePath, idwFileName);

            //copy the doc to the temp folder
            System.IO.File.Copy(doc.FullFileName, System.IO.Path.Combine(destinationFolderName, fileName));

            //copy drawing if it exists
            if (includeDrawing && System.IO.File.Exists(fullIdwFileName))
            {
                System.IO.File.Copy(fullIdwFileName, System.IO.Path.Combine(destinationFolderName, idwFileName));
            }
        }

        public static async Task UploadModelToForge(string displayName, string parameterJson, string zipFileName)
        {
            try
            {
                HttpClient client = new HttpClient();
                MultipartFormDataContent content = new MultipartFormDataContent();

                //add bucket key content to the request
                StringContent displayNameContent = new StringContent(displayName);
                displayNameContent.Headers.Add("Content-Disposition", "form-data; name=\"DisplayName\"");
                content.Add(displayNameContent, "DisplayName");

                //add bucket key content to the request
                StringContent parameterJsonContent = new StringContent(parameterJson);
                parameterJsonContent.Headers.Add("Content-Disposition", "form-data; name=\"ParameterJson\"");
                content.Add(parameterJsonContent, "ParameterJson");

                //add file data to the form as a stream content
                //FileStream stream = System.IO.File.Open(fullFileName, FileMode.Open);

                //StreamContent streamContent = new StreamContent(stream);
                //streamContent.Headers.Add("Content-Type", "application/octet-stream");
                //streamContent.Headers.Add("Content-Disposition", $"form-data; name=\"ZipFile\"; filename=\"{System.IO.Path.GetFileName(zipFileName)}\"");
                //content.Add(streamContent, "ZipFile", System.IO.Path.GetFileName(zipFileName));

                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"https://localhost:7269/api/forge/product/upload"),
                    Content = content
                };

                string reqTxt = await request.Content.ReadAsStringAsync(); //this will fail if the memory stream is closed before this line is called

                HttpResponseMessage response = await client.SendAsync(request);

                //print to txt file
                string fileName = @"C:\Users\Admin\source\repos\CadflairWebApplication\CadflairInventorAddin\bin\Debug\request.txt";
                //string fileName = @"C:\Users\jpgau\source\repos\jpgaukler\CadflairWebApplication\CadflairInventorAddin\bin\Debug\request.txt";
                StreamWriter txt = System.IO.File.CreateText(fileName);
                string responseMessage = await response.Content.ReadAsStringAsync();

                txt.WriteLine("Request content:");
                txt.Write(reqTxt);
                txt.WriteLine();

                txt.WriteLine("Response content:");
                txt.Write(responseMessage);
                txt.Close();
                Process.Start(fileName);

                //clean up
                txt.Dispose();
                //stream.Dispose();
                displayNameContent.Dispose();
                parameterJsonContent.Dispose();
                //streamContent.Dispose();
                content.Dispose();
                response.Dispose();
                request.Dispose();
                client.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}


//public static string[] GetILogicFormNames(Document doc)
//{
//    List<string> formNames = new List<string>();

//    foreach (AttributeSet set in doc.AttributeSets)
//    {
//        //ignore all attribute sets that are not iLogicUi
//        if (!set.Name.ToLower().Contains("ilogicinternalui")) continue;

//        //get form spec attribute
//        Inventor.Attribute formSpec = set["FormSpec"];

//        //convert byte array to xml string
//        byte[] bytes = (byte[])formSpec.Value;
//        string xmlString = Encoding.UTF8.GetString(bytes);

//        //ignore the browser control spec
//        string formName = GetiLogicFormName(xmlString);
//        if (formName.Contains("iLogicBrowserUiFormSpecification")) continue;

//        formNames.Add(formName);
//    }

//    return formNames.ToArray();
//}

//public static string GetiLogicFormName(string xmlFormSpec)
//{
//    XDocument doc = XDocument.Parse(xmlFormSpec);
//    return doc.Element("FormSpecification").Element("Name").Value;
//}


//private static string GetMaxExpression(this Parameter parameter)
//{
//    if (parameter == null) return null;
//    return parameter?.Tolerance?.Lower.ToString();
//}

//private static string SerializeiLogicFormSpecToJson(Document doc, string xmlString)
//{
//    //convert xml string to xdoc for parsing
//    XDocument xDoc = XDocument.Parse(xmlString);
//    XNamespace ns = xDoc.Root.GetNamespaceOfPrefix("xsi");
//    XElement formSpecElement = xDoc.Element("FormSpecification");

//    ILogicUiElement element = new ILogicUiElement()
//    {
//        Name = formSpecElement.Element("Name").Value,
//        //Guid = formSpecElement.Element("Guid").Value,
//        Items = RecurseILogicElements(doc, formSpecElement.Element("Items"), ns)
//    };

//    JsonSerializerSettings settings = new JsonSerializerSettings()
//    {
//        NullValueHandling = NullValueHandling.Ignore,
//        DefaultValueHandling = DefaultValueHandling.Ignore //ignore empty strings and arrays
//    };

//    return JsonConvert.SerializeObject(element, settings); ;
//}


