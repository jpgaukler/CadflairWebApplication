using CadflairDataAccess.Models;
using CadflairInventorAddin.Utilities;
using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CadflairInventorAddin.Commands
{
    public partial class UploadToCadflairDialog : Form
    {
        private Document _doc;
        private List<ILogicUiElement> _iLogicForms;

        public UploadToCadflairDialog(Document doc)
        {
            InitializeComponent();

            _doc = doc;
            _iLogicForms = UploadToCadflair.GetILogicFormElements(doc);

            foreach(ILogicUiElement form in _iLogicForms)
            {
                ComboBoxILogicForms.Items.Add(form.Name);
            }

            ComboBoxILogicForms.SelectedIndex = 0;
        }

        private void ComboBoxILogicForms_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataGridViewParameters.Rows.Clear();
            ILogicUiElement form = _iLogicForms.FirstOrDefault(i => i.Name == ComboBoxILogicForms.SelectedItem.ToString());
            PopulateParametersGrid(form);
        }

        private void PopulateParametersGrid(ILogicUiElement element)
        {
            foreach(ILogicUiElement item in element.Items)
            {
                if(item.ParameterName != null)
                {
                    DataGridViewParameters.Rows.Add(item, item.ParameterName, item.ParameterUnits, item.UiElementSpec, item.ParameterMinValue, item.ParameterMaxValue);
                }

                if (item.Items != null)
                {
                    PopulateParametersGrid(item);
                }
            }
        }

        private void ButtonUpload_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(TextBoxBucketKey.Text))
            {
                MessageBox.Show("Please enter a value for BucketKey", "BuckeyKey Not Provided", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(TextBoxObjectName.Text))
            {
                MessageBox.Show("Please enter a value for ObjectName", "ObjectName Not Provided", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string zipFileName = UploadToCadflair.CreateTemporaryZipFile(_doc, true);
            UploadToCadflair.UploadModelToForge(zipFileName, TextBoxBucketKey.Text, $"{TextBoxObjectName.Text}.zip");

            System.IO.File.Delete(zipFileName);

            //Process.Start(zipFileName);
            //MessageBox.Show(zipFileName, "Success");

        }

        private void ButtonSaveAttributes_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in DataGridViewParameters.Rows)
            {
                ILogicUiElement element = (ILogicUiElement)row.Cells[ILogicUIElementColumn.Index].Value;

                if (element.UiElementSpec == "NumericParameterControlSpec" && element.EditControlType == "TextBox")
                {
                    double minValue = Double.Parse(row.Cells[MinValueColumn.Index].Value.ToString());
                    double maxValue = Double.Parse(row.Cells[MaxValueColumn.Index].Value.ToString());

                    Parameter parameter = _doc.GetParameter(element.ParameterName);
                    parameter.AddValidationAttributes(minValue, maxValue);
                }
            }

            ILogicUiElement form = _iLogicForms.FirstOrDefault(i => i.Name == ComboBoxILogicForms.SelectedItem.ToString());
            UploadToCadflair.SaveILogicUiElementToJson(form);
            UploadToCadflair.SaveILogicFormSpecToXml(form.Name);
        }
    }
}
