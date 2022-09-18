using Inventor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace CadflairInventorAddin
{
    internal static class ConvertiLogicFormSpec
    {


        public static void ConvertiLogicFormButton_OnExecute(NameValueMap Context)
        {

            try
            {
                foreach (AttributeSet set in Globals.InventorApplication.ActiveDocument.AttributeSets)
                {
                    //ignore all attribute sets that are not iLogicUi
                    if (!set.Name.ToLower().Contains("ilogicinternalui")) continue;

                    //get form spec attribute
                    Inventor.Attribute formSpec = set["FormSpec"];

                    //convert byte array to xml string
                    byte[] bytes = (byte[])formSpec.Value;
                    string xmlString = System.Text.Encoding.UTF8.GetString(bytes);

                    //ignore the browser control spec
                    string formName = GetiLogicFormName(xmlString);
                    if (formName.Contains("iLogicBrowserUiFormSpecification")) continue;

                    //convert from spec to html
                    string htmlString = ConvertiLogicFormSpecToHtml(Globals.InventorApplication.ActiveDocument, xmlString);

                    //print results string to txt files
                    string folderName = @"C:\Users\jpgau\source\repos\jpgaukler\CadflairWebApplication\Inventor Files";

                    string xmlFileName = System.IO.Path.Combine(folderName, formName + ".xml");
                    string htmlFileName = System.IO.Path.Combine(folderName, formName + ".html");

                    StreamWriter xmlFile = System.IO.File.CreateText(xmlFileName);
                    StreamWriter htmlFile = System.IO.File.CreateText(htmlFileName);

                    htmlFile.Write(htmlString);
                    xmlFile.Write(xmlString);

                    htmlFile.Close();
                    xmlFile.Close();

                    Process.Start(folderName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }


        /// <summary>
        /// Converts xml string for an iLogic form spec to an html string.
        /// </summary>
        /// <param name="doc">Inventor document containing the iLogic form.</param>
        /// <param name="xmlString">XML string containing iLogic form spec data.</param>
        /// <returns></returns>
        private static string ConvertiLogicFormSpecToHtml(Document doc, string xmlString)
        {
            //convert xml string to xdoc for parsing
            XDocument xDoc = XDocument.Parse(xmlString);
            XNamespace ns = xDoc.Root.GetNamespaceOfPrefix("xsi");

            //construct html string from form spec xml
            StringWriter stringWriter = new StringWriter();

            using (HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter))
            {
                ConvertiLogicFormSpecItemsToHTML(doc, htmlWriter, xDoc.Element("FormSpecification").Element("Items"), ns);
            }

            //need to fine tune white space removal
            return stringWriter.ToString().RemoveLineBreaks();
        }


        /// <summary>
        /// Returns the name of an iLogic form given the form spec xml string.
        /// </summary>
        /// <param name="xmlFormSpec"></param>
        /// <returns></returns>
        private static string GetiLogicFormName(string xmlFormSpec)
        {
            XDocument doc = XDocument.Parse(xmlFormSpec);
            return doc.Element("FormSpecification").Element("Name").Value;
        }


        private static void ConvertiLogicFormSpecItemsToHTML(Document doc, HtmlTextWriter writer, XElement items, XNamespace ns)
        {
            //how should I handle readonly parameters?
            //should I handle rows?
            //should I handle picture folders?
            //should I handle empty space?
            //should I handle iProperties or iLogic rules?


            foreach (XElement uiElementSpec in items.Elements())
            {
                string uiElementType = uiElementSpec.Attribute(ns + "type").Value;
                string uiElementName = uiElementSpec.Element("Name").Value;
                string parameterName = uiElementSpec.Element("ParameterName")?.Value;
                string editControlType = uiElementSpec.Element("EditControlType")?.Value;
                string toolTip = uiElementSpec.Element("ToolTip")?.Value;
                string readOnly = uiElementSpec.Element("ReadOnly")?.Value;
                string enablingParameterName = uiElementSpec.Element("EnablingParameterName")?.Value;
                string pictureBoxImgData = uiElementSpec.Element("Image")?.Element("BitmapByteArray")?.Value;
                string captionImgData = uiElementSpec.Element("CaptionImage")?.Element("BitmapByteArray")?.Value;

                //open div for element type
                writer.AddAttribute("class", uiElementType);
                writer.RenderBeginTag("div");

                switch (uiElementType)
                {
                    case "ControlTabGroupSpec":

                        //add header for tab group
                        writer.RenderBeginTag("h1");

                        //add caption image if it exists
                        RenderBase64Image(writer, captionImgData, $"{uiElementName}-image");

                        //add span for tab name
                        writer.RenderBeginTag("span");
                        writer.Write(uiElementName);
                        writer.RenderEndTag();

                        //end header tag
                        writer.RenderEndTag();

                        //open div to contain items
                        writer.AddAttribute("class", "items");
                        writer.RenderBeginTag("div");

                        //recurse through sub items
                        ConvertiLogicFormSpecItemsToHTML(doc, writer, uiElementSpec.Element("Items"), ns);

                        //close items div
                        writer.RenderEndTag();

                        break;


                    case "ControlGroupSpec":

                        //add header for parameter group
                        writer.RenderBeginTag("h1");

                        //add caption image if it exists
                        RenderBase64Image(writer, captionImgData, $"{uiElementName}-image");

                        //add span for group name
                        writer.RenderBeginTag("span");
                        writer.Write(uiElementName);
                        writer.RenderEndTag();

                        //add dropdown indicator
                        writer.AddAttribute("class", "dropdown-indicator");
                        writer.RenderBeginTag("span");
                        writer.RenderEndTag();

                        //end header tag
                        writer.RenderEndTag();

                        //open div to contain group items
                        writer.AddAttribute("class", "items");
                        writer.RenderBeginTag("div");

                        //recurse through sub items
                        ConvertiLogicFormSpecItemsToHTML(doc, writer, uiElementSpec.Element("Items"), ns);

                        //close group div 
                        writer.RenderEndTag();

                        break;

                    case "NumericParameterControlSpec":

                        //add input for parameter
                        RenderInputForInventorParameter(doc, writer, "number", parameterName, uiElementName);

                        break;

                    case "TextParameterControlSpec":

                        //add input for parameter
                        RenderInputForInventorParameter(doc, writer, "text", parameterName, uiElementName);

                        break;

                    case "MultiValueNumericParameterControlSpec":
                    case "MultiValueTextParameterControlSpec":

                        switch (editControlType)
                        {
                            case "RadioGroup":
                                //add radio group input
                                RenderInputForInventorParameter(doc, writer, "radio", parameterName, uiElementName);
                                break;

                            case "ComboBox":
                                //add combo box input
                                RenderInputForInventorParameter(doc, writer, "combobox", parameterName, uiElementName);
                                break;

                            case "ListBox":
                                //add combo box input
                                RenderInputForInventorParameter(doc, writer, "listbox", parameterName, uiElementName);
                                break;
                        }

                        break;

                    case "BooleanParameterControlSpec":

                        switch (editControlType)
                        {
                            case "CheckBox":
                                //add radio group input
                                RenderInputForInventorParameter(doc, writer, "checkbox", parameterName, uiElementName);
                                break;

                            case "TrueOrFalse":
                                //add combo box input
                                RenderInputForInventorParameter(doc, writer, "boolean", parameterName, uiElementName);
                                break;
                        }

                        break;

                    case "LabelSpec":

                        //add a label
                        writer.RenderBeginTag("h1");
                        writer.Write(uiElementName);
                        writer.RenderEndTag();

                        break;

                    case "SplitterSpec":

                        //add a horizontal rule
                        writer.RenderBeginTag("hr");
                        writer.RenderEndTag();

                        break;

                    case "PictureControlSpec":

                        //add picturebox
                        RenderBase64Image(writer, pictureBoxImgData, uiElementName);

                        break;

                    case "TextPropertyControlSpec": //this is an iProperty
                    case "iLogicRuleControlSpec": //this is an iLogic Rule


                    default:
                        break;

                }

                //end parent div
                writer.RenderEndTag();
            }
        }

        /// <summary>
        /// Adds a label tag to the HtmlTextWriter that corresponds to an Inventor parameter.
        /// </summary>
        /// <param name="writer">HtmlTextWriter to add the label to.</param>
        /// <param name="forParameter">Id of the element that the label will apply to.</param>
        /// <param name="labelText">Value of the label's text.</param>
        private static void RenderLabelForInventorParameter(HtmlTextWriter writer, string forParameter, string labelText)
        {
            writer.AddAttribute("for", forParameter);
            writer.RenderBeginTag("label");
            writer.Write(labelText);
            writer.RenderEndTag();
        }

        /// <summary>
        /// Adds an image tag with a bmp image from a base64 string. This can be used to render images that are included in the iLogic form.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="base64String"></param>
        /// <param name="altText"></param>
        private static void RenderBase64Image(HtmlTextWriter writer, string base64String, string altText)
        {
            if (!string.IsNullOrWhiteSpace(base64String))
            {
                writer.AddAttribute("src", $"data:image/bmp;base64,{base64String}");
                writer.AddAttribute("alt", altText);
                writer.RenderBeginTag("img");
                writer.RenderEndTag();
            }
        }

        /// <summary>
        /// Add an input tag to the HtmlTextWriter for an Inventor parameter.
        /// </summary>
        /// <param name="doc">Inventor document object.</param>
        /// <param name="writer">HtmlTextWriter to add the input tag to.</param>
        /// <param name="inputType">Type of input tag to render.</param>
        /// <param name="parameterName">Name of the Inventor model parameter.</param>
        /// <param name="parameterLabel">Text value of label for parametet input.</param>
        private static void RenderInputForInventorParameter(Document doc, HtmlTextWriter writer, string inputType, string parameterName = null, string parameterLabel = null)
        {
            //get parameter for add parameter data
            Parameter param = doc.GetParameter(parameterName);

            //prefix for html id attributes 
            string prefix = "InventorParam-";

            switch (inputType)
            {
                case "text":

                    //add label
                    RenderLabelForInventorParameter(writer, prefix + parameterName, parameterLabel);

                    //add text box input
                    writer.AddAttribute("id", prefix + parameterName);
                    writer.AddAttribute("type", "text");
                    writer.AddAttribute("placeholder", $"Enter {parameterLabel.ToLower()}");
                    writer.RenderBeginTag("input");
                    writer.RenderEndTag();

                    break;

                case "number":

                    //add label
                    RenderLabelForInventorParameter(writer, prefix + parameterName, parameterLabel);

                    //add text box input
                    writer.AddAttribute("id", prefix + parameterName);
                    writer.AddAttribute("type", "number");
                    writer.AddAttribute("pattern", "[0-9]*");
                    writer.AddAttribute("placeholder", $"Enter {parameterLabel.ToLower()} ({param.get_Units()})");

                    //NEED TO VERIFY THAT THE CORRECT UNITS ARE USED
                    writer.AddAttribute("data-max-value", param.Tolerance.Upper.ToString());
                    writer.AddAttribute("data-min-value", param.Tolerance.Lower.ToString());
                    writer.RenderBeginTag("input");
                    writer.RenderEndTag();

                    //add label for unit type
                    writer.AddAttribute("class", "units-label");
                    writer.RenderBeginTag("span");
                    writer.Write(param.get_Units());
                    writer.RenderEndTag();

                    break;

                case "range":

                    //use range to make a slider input: https://www.w3schools.com/tags/att_input_type_range.asp

                    break;

                case "checkbox":

                    //add label
                    RenderLabelForInventorParameter(writer, prefix + parameterName, parameterLabel);

                    //add checkbox input
                    writer.AddAttribute("id", prefix + parameterName);
                    writer.AddAttribute("type", "checkbox");
                    writer.RenderBeginTag("input");
                    writer.RenderEndTag();

                    break;

                case "boolean":

                    //add label
                    RenderLabelForInventorParameter(writer, prefix + parameterName, parameterLabel);

                    //open select tag for combo box 
                    writer.AddAttribute("id", prefix + parameterName);
                    writer.AddAttribute("class", "combobox-options");
                    writer.RenderBeginTag("select");

                    //add true/false combobox
                    foreach (string val in new string[] { "True", "False" })
                    {
                        //add input for option
                        writer.AddAttribute("value", val);
                        writer.RenderBeginTag("option");
                        writer.Write(val);
                        writer.RenderEndTag();
                    }

                    //close options select tag
                    writer.RenderEndTag();

                    break;


                case "radio":

                    //add label
                    RenderLabelForInventorParameter(writer, prefix + parameterName, parameterLabel);

                    //open div for radio options
                    writer.AddAttribute("class", "radio-options");
                    writer.RenderBeginTag("div");

                    //add input options
                    for (int i = 1; i <= param.ExpressionList.Count; i++)
                    {
                        string expression = param.ExpressionList[i].Replace("\"", "");

                        //add label for option
                        RenderLabelForInventorParameter(writer, prefix + parameterName + expression, expression);

                        //add input for option
                        writer.AddAttribute("id", prefix + parameterName + expression);
                        writer.AddAttribute("type", "radio");
                        writer.AddAttribute("name", prefix + parameterName);
                        writer.AddAttribute("value", expression);
                        writer.RenderBeginTag("input");
                        writer.RenderEndTag();
                    }

                    //close options div
                    writer.RenderEndTag();

                    break;

                case "combobox":

                    //add label
                    RenderLabelForInventorParameter(writer, prefix + parameterName, parameterLabel);

                    //open select tag for radio options
                    writer.AddAttribute("id", prefix + parameterName);
                    writer.AddAttribute("class", "combobox-options");
                    writer.RenderBeginTag("select");

                    //add input options
                    for (int i = 1; i <= param.ExpressionList.Count; i++)
                    {
                        string expression = param.ExpressionList[i].Replace("\"", "");

                        //add input for option
                        writer.AddAttribute("value", expression);
                        writer.RenderBeginTag("option");
                        writer.Write(expression);
                        writer.RenderEndTag();
                    }

                    //close options select tag
                    writer.RenderEndTag();

                    break;

                case "listbox":

                    //add label
                    RenderLabelForInventorParameter(writer, prefix + parameterName, parameterLabel);

                    //open select tag for radio options
                    writer.AddAttribute("id", prefix + parameterName);
                    writer.AddAttribute("class", "listbox-options");
                    writer.AddAttribute("size", param.Expression.Count().ToString());
                    writer.RenderBeginTag("select");

                    //add input options
                    for (int i = 1; i <= param.ExpressionList.Count; i++)
                    {
                        string expression = param.ExpressionList[i].Replace("\"", "");

                        //add input for option
                        writer.AddAttribute("value", expression);
                        writer.RenderBeginTag("option");
                        writer.Write(expression);
                        writer.RenderEndTag();
                    }

                    //close options select tag
                    writer.RenderEndTag();

                    break;

                case "splitter":


                    break;

                case "picturebox":

                    break;

            }

        }

    }
}
