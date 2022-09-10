using Inventor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadflairInventorAddin
{
    internal static class ConvertiLogicFormSpec
    {


        public static void ConvertiLogicFormButton_OnExecute(NameValueMap Context)
        {
            PrintiLogicFormSpec(Globals.InventorApplication.ActiveDocument);
        }


        private static void PrintiLogicFormSpec(Document doc)
        {
            try
            {
                foreach (AttributeSet set in doc.AttributeSets)
                {
                    if (!set.Name.ToLower().Contains("ilogicinternalui"))
                    {
                        continue;
                    }

                    //get form spec attribute
                    Inventor.Attribute formSpec = set["FormSpec"];

                    //convert byte array to xml string
                    byte[] bytes = (byte[])formSpec.Value;
                    string xmlString = System.Text.Encoding.UTF8.GetString(bytes);

                    //print string to txt file
                    string folderName = @"C:\Users\jpgau\source\repos\jpgaukler\CadflairWebApplication\CadflairInventorAddin\bin\Debug";
                    string fileName = set.Name + " - " + formSpec.Name + ".xml";
                    string fullName = System.IO.Path.Combine(folderName, fileName);

                    StreamWriter writer = System.IO.File.CreateText(fullName);
                    writer.Write(xmlString);
                    writer.Close();

                    Process.Start(fullName);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }


        private static void ConvertFormSpecToHTML(string xmlString)
        {

            //convert all control types
            //handle tooltips
            //handle parameters and pull out max and min values
            //ignore readonly parameters
            //pull out form name to be able to allow user to select which form drives the design
            //handle control groups 
            //handle tab groups

        }


    }
}
