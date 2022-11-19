using CadflairDataAccess.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace ParameterParsing
{
    internal static class ConvertiLogicFormSpec
    {


        public static void ConvertiLogicXaml()
        {
            try
            {
                //string filename = @"C:\Users\jpgau\source\repos\jpgaukler\CadflairWebApplication\Inventor Files\Form 1.xml";
                string filename = @"C:\Users\Admin\source\repos\CadflairWebApplication\Inventor Files\Form 1.xml";
                string xmlString = System.IO.File.ReadAllText(filename);

                ////convert from spec to html
                //string htmlString = ConvertiLogicFormSpecToHtml(Globals.InventorApplication.ActiveDocument, xmlString);

                //convert from spec to json
                //string jsonString = ConvertiLogicFormSpecToJson(xmlString);
                string jsonString = SerializeiLogicFormSpecToJson(xmlString);

                //print results string to txt files
                //string folderName = @"C:\Users\jpgau\source\repos\jpgaukler\CadflairWebApplication\Inventor Files";
                string folderName = @"C:\Users\Admin\source\repos\CadflairWebApplication\Inventor Files";
                string jsonFileName = System.IO.Path.Combine(folderName,"Form 1.json");

                StreamWriter jsonFile = System.IO.File.CreateText(jsonFileName);
                jsonFile.Write(jsonString);
                jsonFile.Close();

                Clipboard.SetText("hello");
                //Process.Start(folderName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static string SerializeiLogicFormSpecToJson(string xmlString)
        {
            //convert xml string to xdoc for parsing
            XDocument xDoc = XDocument.Parse(xmlString);
            XNamespace ns = xDoc.Root.GetNamespaceOfPrefix("xsi");
            XElement formSpecElement = xDoc.Element("FormSpecification");

            ILogicUiElement element = new ILogicUiElement()
            {
                Name = formSpecElement.Element("Name").Value,
                Guid = formSpecElement.Element("Guid").Value,
                Items = RecurseILogicElements(formSpecElement.Element("Items"), ns)
            };

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(element, settings);;
        }


        private static ILogicUiElement[] RecurseILogicElements(XElement itemsElement, XNamespace ns)
        {

            //how should I handle readonly parameters?
            //should I handle rows?
            //should I handle picture folders?
            //should I handle empty space?
            //should I handle iProperties or iLogic rules?

            if(itemsElement == null) return null;

            List<ILogicUiElement> elementList = new List<ILogicUiElement>();

            foreach (XElement item in itemsElement.Elements())
            {
                ILogicUiElement element = new ILogicUiElement()
                {
                    UiElementSpec = item.Attribute(ns + "type").Value,
                    Name = item.Element("Name").Value,
                    Guid = item.Element("Guid").Value,
                    ToolTip = item.Element("ToolTip")?.Value,
                    EnablingParameterName = item.Element("EnablingParameterName")?.Value,
                    ParameterName = item.Element("ParameterName")?.Value,
                    //ParameterValue = ,
                    //ParameterMinValue = ,
                    //ParameterMaxValue = ,
                    EditControlType = item.Element("EditControlType")?.Value,
                    ReadOnly = item.Element("ReadOnly")?.Value,
                    Base64Image = item.Element("Image")?.Element("BitmapByteArray")?.Value,
                    Base64CaptionImage = item.Element("CaptionImage")?.Element("BitmapByteArray")?.Value,
                    Items = RecurseILogicElements(item.Element("Items"), ns)
                };

                elementList.Add(element);
            }

            return elementList.ToArray();
        }


        ///// <summary>
        ///// Converts xml string for an iLogic form spec to an json string.
        ///// </summary>
        ///// <param name="xmlString">XML string containing iLogic form spec data.</param>
        ///// <returns></returns>
        //private static string ConvertiLogicFormSpecToJson(string xmlString)
        //{
        //    //convert xml string to xdoc for parsing
        //    XDocument xDoc = XDocument.Parse(xmlString);
        //    XNamespace ns = xDoc.Root.GetNamespaceOfPrefix("xsi");
        //    XElement formSpecElement = xDoc.Element("FormSpecification");
        //    XElement nameElement = formSpecElement.Element("Name");
        //    XElement guidElement = formSpecElement.Element("Guid");
        //    XElement itemsElement = formSpecElement.Element("Items");

        //    JObject jObject = new JObject();
        //    jObject.AddXElementAsProperty(nameElement);
        //    jObject.AddXElementAsProperty(guidElement);

        //    JArray itemsArray = ConvertiLogicFormSpecItemsToJArray(itemsElement, ns);
        //    jObject.Add(new JProperty(itemsElement.Name.ToString(), itemsArray));

        //    //consider adding white space removal
        //    return jObject.ToString();
        //}



        //private static void AddXElementAsProperty(this JObject jObject, XElement xElement)
        //{
        //    if (xElement == null) return;
        //    jObject.Add(new JProperty(xElement.Name.ToString(), xElement.Value));
        //}

        //private static void AddXAttributeAsProperty(this JObject jObject, XAttribute xAttribute)
        //{
        //    if (xAttribute == null) return;
        //    jObject.Add(new JProperty(xAttribute.Parent.Name.ToString(), xAttribute.Value));
        //}

        //private static JArray ConvertiLogicFormSpecItemsToJArray(XElement itemsElement, XNamespace ns)
        //{

        //    //how should I handle readonly parameters?
        //    //should I handle rows?
        //    //should I handle picture folders?
        //    //should I handle empty space?
        //    //should I handle iProperties or iLogic rules?

        //    JArray itemsArray = new JArray();

        //    foreach (XElement item in itemsElement.Elements())
        //    {
        //        XAttribute uiElementSpecAttrubute = item.Attribute(ns + "type");
        //        XElement nameElement = item.Element("Name");
        //        XElement guidElement = item.Element("Guid");
        //        XElement toolTipElement = item.Element("ToolTip");
        //        XElement enablingParameterNameElement = item.Element("EnablingParameterName");
        //        XElement parameterNameElement = item.Element("ParameterName");
        //        XElement editControlTypeElement = item.Element("EditControlType");
        //        XElement readOnlyElement = item.Element("ReadOnly");
        //        XElement subItemsElement = item.Element("Items");
        //        XElement imgElement = item.Element("Image");
        //        XElement imgBitByteArrayElement = item.Element("Image")?.Element("BitmapByteArray");
        //        XElement captionImgElement = item.Element("CaptionImage");
        //        XElement captionImgBitByteArrayElement = item.Element("CaptionImage")?.Element("BitmapByteArray");

        //        //open div for element type
        //        JObject jObject = new JObject();
        //        jObject.AddXAttributeAsProperty(uiElementSpecAttrubute);
        //        jObject.AddXElementAsProperty(nameElement);
        //        jObject.AddXElementAsProperty(guidElement);
        //        jObject.AddXElementAsProperty(toolTipElement);
        //        jObject.AddXElementAsProperty(enablingParameterNameElement);
        //        jObject.AddXElementAsProperty(parameterNameElement);
        //        jObject.AddXElementAsProperty(editControlTypeElement);
        //        jObject.AddXElementAsProperty(readOnlyElement);

        //        if (imgElement != null)
        //        {
        //            JObject imgObject = new JObject();
        //            imgObject.AddXElementAsProperty(imgBitByteArrayElement);
        //            jObject.Add(new JProperty(imgElement.Name.ToString(), imgObject));
        //        }

        //        if (subItemsElement != null)
        //        {
        //            JArray subitemsArray = ConvertiLogicFormSpecItemsToJArray(subItemsElement, ns);
        //            jObject.Add(new JProperty(itemsElement.Name.ToString(), subitemsArray));
        //        }

        //        if (captionImgElement != null)
        //        {
        //            JObject captionImgObject = new JObject();
        //            captionImgObject.AddXElementAsProperty(captionImgBitByteArrayElement);
        //            jObject.Add(new JProperty(captionImgElement.Name.ToString(), captionImgBitByteArrayElement));
        //        }
                
        //        itemsArray.Add(jObject);



        //        //switch (uiElementSpec)
        //        //{

        //        //    case "NumericParameterControlSpec":

        //        //        //add input for parameter
        //        //        RenderInputForInventorParameter(doc, writer, "number", parameterName, name);

        //        //        break;

        //        //    case "TextParameterControlSpec":

        //        //        //add input for parameter
        //        //        RenderInputForInventorParameter(doc, writer, "text", parameterName, name);

        //        //        break;

        //        //    case "MultiValueNumericParameterControlSpec":
        //        //    case "MultiValueTextParameterControlSpec":

        //        //        switch (editControlType)
        //        //        {
        //        //            case "RadioGroup":
        //        //                //add radio group input
        //        //                RenderInputForInventorParameter(doc, writer, "radio", parameterName, name);
        //        //                break;

        //        //            case "ComboBox":
        //        //                //add combo box input
        //        //                RenderInputForInventorParameter(doc, writer, "combobox", parameterName, name);
        //        //                break;

        //        //            case "ListBox":
        //        //                //add combo box input
        //        //                RenderInputForInventorParameter(doc, writer, "listbox", parameterName, name);
        //        //                break;
        //        //        }

        //        //        break;

        //        //    case "BooleanParameterControlSpec":

        //        //        switch (editControlType)
        //        //        {
        //        //            case "CheckBox":
        //        //                //add radio group input
        //        //                RenderInputForInventorParameter(doc, writer, "checkbox", parameterName, name);
        //        //                break;

        //        //            case "TrueOrFalse":
        //        //                //add combo box input
        //        //                RenderInputForInventorParameter(doc, writer, "boolean", parameterName, name);
        //        //                break;
        //        //        }

        //        //        break;


        //        //    case "PictureControlSpec":

        //        //        //add picturebox
        //        //        RenderBase64Image(writer, pictureBoxImgData, name);

        //        //        break;

        //        //    case "TextPropertyControlSpec": //this is an iProperty
        //        //    case "iLogicRuleControlSpec": //this is an iLogic Rule
        //        //    default:
        //        //        break;

        //        //}
        //    }

        //    return itemsArray;
        //}

    }
}
