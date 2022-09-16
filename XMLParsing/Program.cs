using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

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

            ConvertFormSpecToHTML(formItems, ns, 0);

            Console.ReadLine();
        }

        private static string GetiLogicFormName(string xmlFormSpec)
        {
            XDocument doc = XDocument.Parse(xmlFormSpec);
            return doc.Element("FormSpecification").Element("Name").Value;
        }

        //do I want to handle caption images???


        private static void ConvertFormSpecToHTML(XElement items, XNamespace ns, int indentLevel)
        {
            string indents = "";
            for (int i = 0; i < indentLevel; i++)
            {
                indents += "\t";
            }

            indentLevel++;

            foreach (XElement uiElementSpec in items.Elements())
            {
                string uiElementType = uiElementSpec.Attribute(ns + "type").Value;
                string uiElementName = uiElementSpec.Element("Name").Value;
                string parameterName = uiElementSpec.Element("ParameterName")?.Value;
                string editControlType = uiElementSpec.Element("EditControlType")?.Value;
                string toolTip = uiElementSpec.Element("ToolTip")?.Value;
                string readOnly = uiElementSpec.Element("ReadOnly")?.Value;
                string enablingParameterName = uiElementSpec.Element("EnablingParameterName")?.Value;

                Console.WriteLine(indents + "Element Type: " + uiElementType);
                Console.WriteLine(indents + "Element Name: " + uiElementName);
                Console.WriteLine(indents + "Parameter Name: " + parameterName);
                Console.WriteLine(indents + "Control Type: " + editControlType);
                Console.WriteLine(indents + "Tooltip: " + toolTip);

                switch (uiElementType)
                {
                    case "ControlTabGroupSpec":
                    case "ControlGroupSpec":
                        //recurse through sub items
                        XElement subItems = uiElementSpec.Element("Items");
                        ConvertFormSpecToHTML(subItems, ns, indentLevel);

                        //close tag
                        break;

                    case "NumericParameterControlSpec":
                    case "MultiValueNumericParameterControlSpec":
                    case "BooleanParameterControlSpec":
                    case "TextParameterControlSpec":
                    case "MultiValueTextParameterControlSpec":


                    default:
                        break;
                }
            }

            indentLevel--;
        }


    }
}
