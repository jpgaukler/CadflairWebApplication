using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using File = System.IO.File;
using Path = System.IO.Path;

namespace ParameterParsing
{
    internal static class ConvertiLogicFormSpec
    {


        public static void ConvertiLogicXaml()
        {
            try
            {
                string filename = @"C:\Users\jpgau\source\repos\jpgaukler\CadflairWebApplication\Inventor Files\Form 1.xml";
                string xmlString = System.IO.File.ReadAllText(filename);

                ////convert from spec to html
                //string htmlString = ConvertiLogicFormSpecToHtml(Globals.InventorApplication.ActiveDocument, xmlString);

                //convert from spec to json
                string jsonString = ConvertiLogicFormSpecToJson(xmlString);

                //print results string to txt files
                string folderName = @"C:\Users\jpgau\source\repos\jpgaukler\CadflairWebApplication\Inventor Files";
                string jsonFileName = System.IO.Path.Combine(folderName,"Form 1.json");

                StreamWriter jsonFile = System.IO.File.CreateText(jsonFileName);
                jsonFile.Write(jsonString);
                jsonFile.Close();

                Process.Start(folderName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Converts xml string for an iLogic form spec to an json string.
        /// </summary>
        /// <param name="xmlString">XML string containing iLogic form spec data.</param>
        /// <returns></returns>
        private static string ConvertiLogicFormSpecToJson(string xmlString)
        {
            //convert xml string to xdoc for parsing
            XDocument xDoc = XDocument.Parse(xmlString);
            XNamespace ns = xDoc.Root.GetNamespaceOfPrefix("xsi");
            XElement formSpecElement = xDoc.Element("FormSpecification");
            XElement nameElement = formSpecElement.Element("Name");
            XElement guidElement = formSpecElement.Element("Guid");
            XElement itemsElement = formSpecElement.Element("Items");

            JObject jObject = new JObject();
            jObject.AddXElementAsProperty(nameElement);
            jObject.AddXElementAsProperty(guidElement);

            JArray itemsArray = ConvertiLogicFormSpecItemsToJArray(itemsElement, ns);
            jObject.Add(new JProperty(itemsElement.Name.ToString(), itemsArray));

            //consider adding white space removal
            return jObject.ToString();
        }

        private static void AddXElementAsProperty(this JObject jObject, XElement xElement)
        {
            if (xElement == null) return;
            jObject.Add(new JProperty(xElement.Name.ToString(), xElement.Value));
        }
        private static void AddXAttributeAsProperty(this JObject jObject, XAttribute xAttribute)
        {
            if (xAttribute == null) return;
            jObject.Add(new JProperty(xAttribute.Parent.Name.ToString(), xAttribute.Value));
        }

        private static JArray ConvertiLogicFormSpecItemsToJArray(XElement itemsElement, XNamespace ns)
        {

            //how should I handle readonly parameters?
            //should I handle rows?
            //should I handle picture folders?
            //should I handle empty space?
            //should I handle iProperties or iLogic rules?

            JArray itemsArray = new JArray();

            foreach (XElement item in itemsElement.Elements())
            {
                XAttribute uiElementSpecAttrubute = item.Attribute(ns + "type");
                XElement nameElement = item.Element("Name");
                XElement guidElement = item.Element("Guid");
                XElement toolTipElement = item.Element("ToolTip");
                XElement enablingParameterNameElement = item.Element("EnablingParameterName");
                XElement parameterNameElement = item.Element("ParameterName");
                XElement editControlTypeElement = item.Element("EditControlType");
                XElement readOnlyElement = item.Element("ReadOnly");
                XElement subItemsElement = item.Element("Items");
                XElement imgElement = item.Element("Image");
                XElement imgBitByteArrayElement = item.Element("Image")?.Element("BitmapByteArray");
                XElement captionImgElement = item.Element("CaptionImage");
                XElement captionImgBitByteArrayElement = item.Element("CaptionImage")?.Element("BitmapByteArray");

                //open div for element type
                JObject jObject = new JObject();
                jObject.AddXAttributeAsProperty(uiElementSpecAttrubute);
                jObject.AddXElementAsProperty(nameElement);
                jObject.AddXElementAsProperty(guidElement);
                jObject.AddXElementAsProperty(toolTipElement);
                jObject.AddXElementAsProperty(enablingParameterNameElement);
                jObject.AddXElementAsProperty(parameterNameElement);
                jObject.AddXElementAsProperty(editControlTypeElement);
                jObject.AddXElementAsProperty(readOnlyElement);

                if (imgElement != null)
                {
                    JObject imgObject = new JObject();
                    imgObject.AddXElementAsProperty(imgBitByteArrayElement);
                    jObject.Add(new JProperty(imgElement.Name.ToString(), imgObject));
                }

                if (subItemsElement != null)
                {
                    JArray subitemsArray = ConvertiLogicFormSpecItemsToJArray(subItemsElement, ns);
                    jObject.Add(new JProperty(itemsElement.Name.ToString(), subitemsArray));
                }

                if (captionImgElement != null)
                {
                    JObject captionImgObject = new JObject();
                    captionImgObject.AddXElementAsProperty(captionImgBitByteArrayElement);
                    jObject.Add(new JProperty(captionImgElement.Name.ToString(), captionImgBitByteArrayElement));
                }
                
                itemsArray.Add(jObject);



                //switch (uiElementSpec)
                //{

                //    case "NumericParameterControlSpec":

                //        //add input for parameter
                //        RenderInputForInventorParameter(doc, writer, "number", parameterName, name);

                //        break;

                //    case "TextParameterControlSpec":

                //        //add input for parameter
                //        RenderInputForInventorParameter(doc, writer, "text", parameterName, name);

                //        break;

                //    case "MultiValueNumericParameterControlSpec":
                //    case "MultiValueTextParameterControlSpec":

                //        switch (editControlType)
                //        {
                //            case "RadioGroup":
                //                //add radio group input
                //                RenderInputForInventorParameter(doc, writer, "radio", parameterName, name);
                //                break;

                //            case "ComboBox":
                //                //add combo box input
                //                RenderInputForInventorParameter(doc, writer, "combobox", parameterName, name);
                //                break;

                //            case "ListBox":
                //                //add combo box input
                //                RenderInputForInventorParameter(doc, writer, "listbox", parameterName, name);
                //                break;
                //        }

                //        break;

                //    case "BooleanParameterControlSpec":

                //        switch (editControlType)
                //        {
                //            case "CheckBox":
                //                //add radio group input
                //                RenderInputForInventorParameter(doc, writer, "checkbox", parameterName, name);
                //                break;

                //            case "TrueOrFalse":
                //                //add combo box input
                //                RenderInputForInventorParameter(doc, writer, "boolean", parameterName, name);
                //                break;
                //        }

                //        break;


                //    case "PictureControlSpec":

                //        //add picturebox
                //        RenderBase64Image(writer, pictureBoxImgData, name);

                //        break;

                //    case "TextPropertyControlSpec": //this is an iProperty
                //    case "iLogicRuleControlSpec": //this is an iLogic Rule
                //    default:
                //        break;

                //}
            }

            return itemsArray;
        }


        ///// <summary>
        ///// Adds a label tag to the HtmlTextWriter that corresponds to an Inventor parameter.
        ///// </summary>
        ///// <param name="writer">HtmlTextWriter to add the label to.</param>
        ///// <param name="forParameter">Id of the element that the label will apply to.</param>
        ///// <param name="labelText">Value of the label's text.</param>
        //private static void RenderLabelForInventorParameter(HtmlTextWriter writer, string forParameter, string labelText)
        //{
        //    writer.AddAttribute("for", forParameter);
        //    writer.RenderBeginTag("label");
        //    writer.Write(labelText);
        //    writer.RenderEndTag();
        //}

        ///// <summary>
        ///// Adds an image tag with a bmp image from a base64 string. This can be used to render images that are included in the iLogic form.
        ///// </summary>
        ///// <param name="writer"></param>
        ///// <param name="base64String"></param>
        ///// <param name="altText"></param>
        //private static void RenderBase64Image(HtmlTextWriter writer, string base64String, string altText)
        //{
        //    if (!string.IsNullOrWhiteSpace(base64String))
        //    {
        //        writer.AddAttribute("src", $"data:image/bmp;base64,{base64String}");
        //        writer.AddAttribute("alt", altText);
        //        writer.RenderBeginTag("img");
        //        writer.RenderEndTag();
        //    }
        //}

        ///// <summary>
        ///// Add an input tag to the HtmlTextWriter for an Inventor parameter.
        ///// </summary>
        ///// <param name="doc">Inventor document object.</param>
        ///// <param name="writer">HtmlTextWriter to add the input tag to.</param>
        ///// <param name="inputType">Type of input tag to render.</param>
        ///// <param name="parameterName">Name of the Inventor model parameter.</param>
        ///// <param name="parameterLabel">Text value of label for parametet input.</param>
        //private static void RenderInputForInventorParameter(Document doc, HtmlTextWriter writer, string inputType, string parameterName = null, string parameterLabel = null)
        //{
        //    //get parameter for add parameter data
        //    Parameter param = doc.GetParameter(parameterName);

        //    //prefix for html id attributes 
        //    string prefix = "InventorParam-";

        //    switch (inputType)
        //    {
        //        case "text":

        //            //add label
        //            RenderLabelForInventorParameter(writer, prefix + parameterName, parameterLabel);

        //            //add text box input
        //            writer.AddAttribute("id", prefix + parameterName);
        //            writer.AddAttribute("type", "text");
        //            writer.AddAttribute("placeholder", $"Enter {parameterLabel.ToLower()}");
        //            writer.RenderBeginTag("input");
        //            writer.RenderEndTag();

        //            break;

        //        case "number":

        //            //add label
        //            RenderLabelForInventorParameter(writer, prefix + parameterName, parameterLabel);

        //            //add text box input
        //            writer.AddAttribute("id", prefix + parameterName);
        //            writer.AddAttribute("type", "number");
        //            writer.AddAttribute("pattern", "[0-9]*");
        //            writer.AddAttribute("placeholder", $"Enter {parameterLabel.ToLower()} ({param.get_Units()})");

        //            //NEED TO VERIFY THAT THE CORRECT UNITS ARE USED
        //            writer.AddAttribute("data-max-value", param.Tolerance.Upper.ToString());
        //            writer.AddAttribute("data-min-value", param.Tolerance.Lower.ToString());
        //            writer.RenderBeginTag("input");
        //            writer.RenderEndTag();

        //            //add label for unit type
        //            writer.AddAttribute("class", "units-label");
        //            writer.RenderBeginTag("span");
        //            writer.Write(param.get_Units());
        //            writer.RenderEndTag();

        //            break;

        //        case "range":

        //            //use range to make a slider input: https://www.w3schools.com/tags/att_input_type_range.asp

        //            break;

        //        case "checkbox":

        //            //add label
        //            RenderLabelForInventorParameter(writer, prefix + parameterName, parameterLabel);

        //            //add checkbox input
        //            writer.AddAttribute("id", prefix + parameterName);
        //            writer.AddAttribute("type", "checkbox");
        //            writer.RenderBeginTag("input");
        //            writer.RenderEndTag();

        //            break;

        //        case "boolean":

        //            //add label
        //            RenderLabelForInventorParameter(writer, prefix + parameterName, parameterLabel);

        //            //open select tag for combo box 
        //            writer.AddAttribute("id", prefix + parameterName);
        //            writer.AddAttribute("class", "combobox-options");
        //            writer.RenderBeginTag("select");

        //            //add true/false combobox
        //            foreach (string val in new string[] { "True", "False" })
        //            {
        //                //add input for option
        //                writer.AddAttribute("value", val);
        //                writer.RenderBeginTag("option");
        //                writer.Write(val);
        //                writer.RenderEndTag();
        //            }

        //            //close options select tag
        //            writer.RenderEndTag();

        //            break;


        //        case "radio":

        //            //add label
        //            RenderLabelForInventorParameter(writer, prefix + parameterName, parameterLabel);

        //            //open div for radio options
        //            writer.AddAttribute("class", "radio-options");
        //            writer.RenderBeginTag("div");

        //            //add input options
        //            for (int i = 1; i <= param.ExpressionList.Count; i++)
        //            {
        //                string expression = param.ExpressionList[i].Replace("\"", "");

        //                //add label for option
        //                RenderLabelForInventorParameter(writer, prefix + parameterName + expression, expression);

        //                //add input for option
        //                writer.AddAttribute("id", prefix + parameterName + expression);
        //                writer.AddAttribute("type", "radio");
        //                writer.AddAttribute("name", prefix + parameterName);
        //                writer.AddAttribute("value", expression);
        //                writer.RenderBeginTag("input");
        //                writer.RenderEndTag();
        //            }

        //            //close options div
        //            writer.RenderEndTag();

        //            break;

        //        case "combobox":

        //            //add label
        //            RenderLabelForInventorParameter(writer, prefix + parameterName, parameterLabel);

        //            //open select tag for radio options
        //            writer.AddAttribute("id", prefix + parameterName);
        //            writer.AddAttribute("class", "combobox-options");
        //            writer.RenderBeginTag("select");

        //            //add input options
        //            for (int i = 1; i <= param.ExpressionList.Count; i++)
        //            {
        //                string expression = param.ExpressionList[i].Replace("\"", "");

        //                //add input for option
        //                writer.AddAttribute("value", expression);
        //                writer.RenderBeginTag("option");
        //                writer.Write(expression);
        //                writer.RenderEndTag();
        //            }

        //            //close options select tag
        //            writer.RenderEndTag();

        //            break;

        //        case "listbox":

        //            //add label
        //            RenderLabelForInventorParameter(writer, prefix + parameterName, parameterLabel);

        //            //open select tag for radio options
        //            writer.AddAttribute("id", prefix + parameterName);
        //            writer.AddAttribute("class", "listbox-options");
        //            writer.AddAttribute("size", param.Expression.Count().ToString());
        //            writer.RenderBeginTag("select");

        //            //add input options
        //            for (int i = 1; i <= param.ExpressionList.Count; i++)
        //            {
        //                string expression = param.ExpressionList[i].Replace("\"", "");

        //                //add input for option
        //                writer.AddAttribute("value", expression);
        //                writer.RenderBeginTag("option");
        //                writer.Write(expression);
        //                writer.RenderEndTag();
        //            }

        //            //close options select tag
        //            writer.RenderEndTag();

        //            break;

        //        case "splitter":


        //            break;

        //        case "picturebox":

        //            break;

        //    }

        //}

    }
}
