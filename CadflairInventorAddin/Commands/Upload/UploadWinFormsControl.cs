using CadflairDataAccess.Models;
using CadflairInventorAddin.Helpers;
using Inventor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadflairInventorAddin.Commands.Upload
{
    public partial class UploadWinFormsControl : UserControl
    {
        private Document _doc;
        private List<ILogicFormElement> _iLogicForms;

        public UploadWinFormsControl(Document doc)
        {
            InitializeComponent();

            BackColor = Globals.InventorApplication.ThemeManager.GetComponentThemeColor("BrowserPane_BackgroundColor").ToSystemDrawingColor();
            ForeColor = Globals.InventorApplication.ThemeManager.GetComponentThemeColor("BrowserPane_TextColor").ToSystemDrawingColor();
            DataGridViewParameters.BackgroundColor = Globals.InventorApplication.ThemeManager.GetComponentThemeColor("ApplicationFrame_BackgroundColor").ToSystemDrawingColor();

            _doc = doc;
            _iLogicForms = UploadToCadflair.GetILogicFormElements(doc);

            foreach(ILogicFormElement form in _iLogicForms)
            {
                ComboBoxILogicForms.Items.Add(form.Name);
            }

            ComboBoxILogicForms.SelectedIndex = 0;
        }

        private void ComboBoxILogicForms_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataGridViewParameters.Rows.Clear();
            ILogicFormElement form = _iLogicForms.FirstOrDefault(i => i.Name == ComboBoxILogicForms.SelectedItem.ToString());
            PopulateParametersGrid(form);
        }

        private void PopulateParametersGrid(ILogicFormElement element)
        {
            foreach(ILogicFormElement item in element.Items)
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

        private async void ButtonUpload_Click(object sender, EventArgs e)
        {
            if (_doc.FileSaveCounter == 0)
            {
                MessageBox.Show("Please save the file before uploading.", "File Not Saved", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (string.IsNullOrWhiteSpace(TextBoxDisplayName.Text))
            {
                MessageBox.Show("Please enter a value for Display Name", "Display Name Not Provided", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Save limits for numeric text box parameters as attributes
            foreach(DataGridViewRow row in DataGridViewParameters.Rows)
            {
                ILogicFormElement element = (ILogicFormElement)row.Cells[ILogicUIElementColumn.Index].Value;

                if (element.UiElementSpec == "NumericParameterControlSpec" && element.EditControlType == "TextBox")
                {
                    double minValue = Double.Parse(row.Cells[MinValueColumn.Index].Value.ToString());
                    double maxValue = Double.Parse(row.Cells[MaxValueColumn.Index].Value.ToString());

                    Parameter parameter = _doc.GetParameter(element.ParameterName);
                    parameter.AddValidationAttributes(minValue, maxValue);
                }
            }

            // Get parameters in form of json
            ILogicFormElement iLogicFormSpec = _iLogicForms.FirstOrDefault(i => i.Name == ComboBoxILogicForms.SelectedItem.ToString());

            //UploadToCadflair.SaveILogicUiElementToJson(iLogicFormSpec);
            //UploadToCadflair.SaveILogicFormSpecToXml(iLogicFormSpec.Name);

            //// Create new product model
            //Product newProduct = new Product()
            //{
            //    CreatedById = Convert.ToInt32(TextBoxUserId.Text),
            //    //ProductFamilyId = Convert.ToInt32(TextBoxProductFamilyId.Text),
            //    DisplayName = TextBoxDisplayName.Text,
            //    ParameterJson = iLogicFormSpec.ToJson(),
            //    IsPublic = CheckBoxIsPublic.Checked,
            //    IsConfigurable = CheckBoxIsConfigurable.Checked,
            //};

            //// save model to zipfile
            //string zipFileName = UploadToCadflair.CreateTemporaryZipFile(_doc, true);



            // NEED TO UPDATE THIS METHOD TO REFLECT CHANGES TO THE PRODUCTCONTROLLER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            //// Upload to Cadflair
            //bool uploadSuccessful = await UploadToCadflair.UploadModelToForge(newProduct, zipFileName);

            //// clean up
            //System.IO.File.Delete(zipFileName);

            //MessageBox.Show(uploadSuccessful.ToString(), "Success?", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
