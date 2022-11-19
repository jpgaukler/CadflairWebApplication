using CadflairDataAccess.Models;
using Inventor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CadflairInventorAddin
{
    internal class UploadToCadflair
    {
        public static void UploadToCadflairButton_OnExecute(NameValueMap Context)
        {
            try
            {
                UploadToCadflairDialog commandDialog = new UploadToCadflairDialog(Globals.InventorApplication.ActiveDocument);
                commandDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static ILogicUiElement[] GetILogicFormElements(Document doc)
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

            return iLogicForms.ToArray();

        }

        private static ILogicUiElement[] RecurseILogicElements(Document doc, XElement itemsElement, XNamespace ns)
        {

            //how should I handle readonly parameters?
            //should I handle rows?
            //should I handle picture folders?
            //should I handle empty space?
            //should I handle iProperties or iLogic rules?

            if (itemsElement == null) return null;

            List<ILogicUiElement> elementList = new List<ILogicUiElement>();

            foreach (XElement item in itemsElement.Elements())
            {
                Parameter parameter = doc.GetParameter(item.Element("ParameterName")?.Value);

                ILogicUiElement element = new ILogicUiElement()
                {
                    UiElementSpec = item.Attribute(ns + "type").Value,
                    Name = item.Element("Name").Value,
                    //Guid = item.Element("Guid").Value,
                    ToolTip = item.Element("ToolTip")?.Value,
                    EnablingParameterName = item.Element("EnablingParameterName")?.Value,
                    ParameterName = parameter?.Name,
                    ParameterUnits = parameter?.get_Units(),
                    ParameterExpression = parameter?.Expression?.Replace("\"", ""),
                    ParameterExpressionList = parameter?.ExpressionList?.ToStringArray(),
                    //ParameterMinValue = parameter?.Tolerance.Lower,
                    //ParameterMaxValue = doc.GetParameter(item.Element("ParameterName")?.Value).Tolerance.Upper.ToString(),
                    EditControlType = item.Element("EditControlType")?.Value,
                    ReadOnly = Convert.ToBoolean(item.Element("ReadOnly")?.Value),
                    //Base64Image = item.Element("Image")?.Element("BitmapByteArray")?.Value,
                    //Base64CaptionImage = item.Element("CaptionImage")?.Element("BitmapByteArray")?.Value,
                    Items = RecurseILogicElements(doc, item.Element("Items"), ns)
                };

                elementList.Add(element);
            }

            return elementList.ToArray();
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

        public static async void UploadModelToForge(string fullFileName, string bucketKey, string objectName)
        {
            try
            {
                HttpClient client = new HttpClient();
                MultipartFormDataContent content = new MultipartFormDataContent();

                //add bucket key content to the request
                StringContent bucketKeyContent = new StringContent(bucketKey);
                bucketKeyContent.Headers.Add("Content-Disposition", "form-data; name=\"bucketKey\"");
                content.Add(bucketKeyContent, "bucketKey");

                //add bucket key content to the request
                StringContent objectNameContent = new StringContent(objectName);
                objectNameContent.Headers.Add("Content-Disposition", "form-data; name=\"objectName\"");
                content.Add(objectNameContent, "objectName");

                //add file data to the form as a stream content
                //byte[] bytes = System.IO.File.ReadAllBytes(fullFileName);
                //MemoryStream stream = new MemoryStream(bytes);
                FileStream stream = System.IO.File.Open(fullFileName, FileMode.Open);

                StreamContent streamContent = new StreamContent(stream);
                streamContent.Headers.Add("Content-Type", "application/octet-stream");
                streamContent.Headers.Add("Content-Disposition", $"form-data; name=\"file\"; filename=\"{System.IO.Path.GetFileName(fullFileName)}\"");
                content.Add(streamContent, "file", System.IO.Path.GetFileName(fullFileName));

                HttpRequestMessage request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"http://www.cadflair.com/api/forge/oss/objects/upload"),
                    Content = content
                };

                //string reqTxt = await request.Content.ReadAsStringAsync(); //this will fail if the memory stream is closed before this line is called

                HttpResponseMessage response = await client.SendAsync(request);

                //print to txt file
                string fileName = @"C:\Users\Admin\source\repos\CadflairWebApplication\CadflairInventorAddin\bin\Debug\request.txt";
                //string fileName = @"C:\Users\jpgau\source\repos\jpgaukler\CadflairWebApplication\CadflairInventorAddin\bin\Debug\request.txt";
                StreamWriter txt = System.IO.File.CreateText(fileName);
                string responseMessage = await response.Content.ReadAsStringAsync();

                //txt.WriteLine("Request content:");
                //txt.Write(reqTxt);
                //txt.WriteLine();

                txt.WriteLine("Response content:");
                txt.Write(responseMessage);
                txt.Close();
                Process.Start(fileName);

                //clean up
                txt.Dispose();
                stream.Dispose();
                bucketKeyContent.Dispose();
                objectNameContent.Dispose();
                streamContent.Dispose();
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


