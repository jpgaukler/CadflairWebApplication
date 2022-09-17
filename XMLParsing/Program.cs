using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Web.UI;

namespace XMLParsing
{
    internal class Program
    {

        private static void Main(string[] args)
        {
            string filename = @"C:\Users\Admin\source\repos\CadflairWebApplication\Inventor Files\iLogicInternalUi_Testing- FormSpec.xml";

            string xmlString = File.ReadAllText(filename);
            XDocument doc = XDocument.Parse(xmlString);

            XNamespace ns = doc.Root.GetNamespaceOfPrefix("xsi");
            XElement formSpec = doc.Element("FormSpecification");
            XElement formItems = formSpec.Element("Items");


            StringWriter stringWriter = new StringWriter();

            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                ConvertFormSpecToHTML(writer, formItems, ns);
            }

            Console.WriteLine(stringWriter.ToString());

            Console.ReadLine();
        }

        private static string GetiLogicFormName(string xmlFormSpec)
        {
            XDocument doc = XDocument.Parse(xmlFormSpec);
            return doc.Element("FormSpecification").Element("Name").Value;
        }

        //do I want to handle caption images???


        private static void ConvertFormSpecToHTML(HtmlTextWriter writer, XElement items, XNamespace ns)
        {
            foreach (XElement uiElementSpec in items.Elements())
            {
                string uiElementType = uiElementSpec.Attribute(ns + "type").Value;
                string uiElementName = uiElementSpec.Element("Name").Value;
                string parameterName = uiElementSpec.Element("ParameterName")?.Value;
                string editControlType = uiElementSpec.Element("EditControlType")?.Value;
                string toolTip = uiElementSpec.Element("ToolTip")?.Value;
                string readOnly = uiElementSpec.Element("ReadOnly")?.Value;
                string enablingParameterName = uiElementSpec.Element("EnablingParameterName")?.Value;
                string idPrefix = "InventorParam-";


                switch (uiElementType)
                {

                    case "ControlTabGroupSpec":
                        //recurse through sub items
                        ConvertFormSpecToHTML(writer, uiElementSpec.Element("Items"), ns);
                        break;

                    case "ControlGroupSpec":
                        //open group div
                        writer.AddAttribute("class", uiElementType);
                        writer.RenderBeginTag("div");

                        //add header for parameter group
                        writer.AddAttribute("class", "header");
                        writer.RenderBeginTag("div");

                        //add h1 for group name
                        writer.RenderBeginTag("span");
                        writer.Write(uiElementName);
                        writer.RenderEndTag();
                        writer.WriteLine();

                        //add dropdown indicator
                        writer.AddAttribute("class", "dropdown-indicator");
                        writer.RenderBeginTag("span");
                        writer.RenderEndTag();

                        //end header tag
                        writer.RenderEndTag();
                        writer.WriteLine();

                        //open div to contain group items
                        writer.AddAttribute("class", "items");
                        writer.RenderBeginTag("div");

                        //recurse through sub items
                        ConvertFormSpecToHTML(writer, uiElementSpec.Element("Items"), ns);

                        //close group div and parent div
                        writer.RenderEndTag();
                        writer.RenderEndTag();
                        writer.WriteLine();

                        break;

                    case "NumericParameterControlSpec":

                        //NEED TO GET THE PARAMETER MAX AND MIN VALUES 


                        //open div with uiElementType
                        writer.AddAttribute("class", uiElementType);
                        writer.RenderBeginTag("div");

                        //add label for parameter
                        writer.AddAttribute("for", idPrefix + parameterName);
                        writer.RenderBeginTag("label");
                        writer.Write(uiElementName);
                        writer.RenderEndTag();
                        writer.WriteLine();

                        //add input for parameter
                        writer.AddAttribute("class", editControlType);
                        writer.AddAttribute("id", idPrefix + parameterName);
                        writer.AddAttribute("type", "number");
                        writer.AddAttribute("pattern", "[0-9]*");
                        writer.AddAttribute("placeholder", "Enter " + uiElementName);
                        writer.RenderBeginTag("input");
                        writer.RenderEndTag();
                        writer.WriteLine();

                        //add label for unit type
                        writer.AddAttribute("class", "units-label");
                        writer.RenderBeginTag("span");
                        writer.Write("in");
                        writer.RenderEndTag();


                        //end parent div
                        writer.RenderEndTag();
                        writer.WriteLine();
                        break;

                    case "TextParameterControlSpec":

                        //open div with uiElementType
                        writer.AddAttribute("class", uiElementType);
                        writer.RenderBeginTag("div");

                        //add label for parameter
                        writer.AddAttribute("for", idPrefix + parameterName);
                        writer.RenderBeginTag("label");
                        writer.Write(uiElementName);
                        writer.RenderEndTag();
                        writer.WriteLine();

                        //add input for parameter
                        writer.AddAttribute("class", editControlType);
                        writer.AddAttribute("id",  idPrefix + parameterName);
                        writer.AddAttribute("type", "text");
                        writer.AddAttribute("placeholder", "Enter " + uiElementName);
                        writer.RenderBeginTag("input");
                        writer.RenderEndTag();

                        //end parent div
                        writer.RenderEndTag();
                        writer.WriteLine();
                        break;

                    case "MultiValueNumericParameterControlSpec":

                        //handle radio groups, listboxes, and comboboxes

                    case "MultiValueTextParameterControlSpec":

                    //handle radio groups, listboxes, and comboboxes

                    //use size attribute on html select element to make a listbox https://www.javatpoint.com/html-list-box

                    case "BooleanParameterControlSpec":

                        //need to handle true/false, or check box





                       
                    case "TextPropertyControlSpec": //this is an iProperty
                    case "iLogicRuleControlSpec": //this is an iLogic Rule
                    default:
                        break;
                }

            }
        }


    }
}
