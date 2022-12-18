using CadflairDataAccess.Models;
using CadflairInventorAddin.Helpers;
using Inventor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CadflairInventorAddin.Commands.Upload
{
    /// <summary>
    /// Interaction logic for UploadWpfControl.xaml
    /// </summary>
    public partial class UploadWpfControl : UserControl
    {
        private Document _doc;
        private List<ILogicUiElement> _iLogicForms;

        public UploadWpfControl(Inventor.Document doc)
        {
            InitializeComponent();

            // set colors of window
            SolidColorBrush backgroundBrush = new SolidColorBrush(Globals.InventorApplication.ThemeManager.GetComponentThemeColor("BrowserPane_BackgroundColor").ToSystemMediaColor());
            SolidColorBrush foregroundBrush = new SolidColorBrush(Globals.InventorApplication.ThemeManager.GetComponentThemeColor("BrowserPane_TextColor").ToSystemMediaColor());
            SolidColorBrush appFrameBrush = new SolidColorBrush(Globals.InventorApplication.ThemeManager.GetComponentThemeColor("ApplicationFrame_BackgroundColor").ToSystemMediaColor());

            this.Background = backgroundBrush;
            this.Foreground = foregroundBrush;
            //DataGridViewParameters.BackgroundColor = appFrameBrush;

            _doc = doc;
            _iLogicForms = UploadToCadflair.GetILogicFormElements(doc);
            foreach (ILogicUiElement form in _iLogicForms) ILogicFormsComboBox.Items.Add(form.Name);
            ILogicFormsComboBox.SelectedIndex = 0;
        }

        private void ILogicFormsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ParametersDataGrid.Items.Clear();
            ILogicUiElement iLogicForm = _iLogicForms.FirstOrDefault(i => i.Name == ILogicFormsComboBox.SelectedItem.ToString());
            PopulateParametersGrid(iLogicForm);
        }

        private void PopulateParametersGrid(ILogicUiElement iLogicForm)
        {
            foreach (ILogicUiElement item in iLogicForm.Items)
            {
                if (item.ParameterName != null) ParametersDataGrid.Items.Add(item);
                if (item.Items != null) PopulateParametersGrid(item);
            }
        }

        //private async void ButtonUpload_Click(object sender, EventArgs e)
        //{
        //    if (_doc.FileSaveCounter == 0)
        //    {
        //        MessageBox.Show("Please save the file before uploading.", "File Not Saved", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        //        return;
        //    }

        //    if (string.IsNullOrWhiteSpace(TextBoxDisplayName.Text))
        //    {
        //        MessageBox.Show("Please enter a value for Display Name", "Display Name Not Provided", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    // Save limits for numeric text box parameters as attributes
        //    foreach(DataGridViewRow row in DataGridViewParameters.Rows)
        //    {
        //        ILogicUiElement element = (ILogicUiElement)row.Cells[ILogicUIElementColumn.Index].Value;

        //        if (element.UiElementSpec == "NumericParameterControlSpec" && element.EditControlType == "TextBox")
        //        {
        //            double minValue = Double.Parse(row.Cells[MinValueColumn.Index].Value.ToString());
        //            double maxValue = Double.Parse(row.Cells[MaxValueColumn.Index].Value.ToString());

        //            Parameter parameter = _doc.GetParameter(element.ParameterName);
        //            parameter.AddValidationAttributes(minValue, maxValue);
        //        }
        //    }

        //    // Get parameters in form of json
        //    ILogicUiElement iLogicFormSpec = _iLogicForms.FirstOrDefault(i => i.Name == ComboBoxILogicForms.SelectedItem.ToString());

        //    //UploadToCadflair.SaveILogicUiElementToJson(iLogicFormSpec);
        //    //UploadToCadflair.SaveILogicFormSpecToXml(iLogicFormSpec.Name);

        //    // Create new product model
        //    Product newProduct = new Product()
        //    {
        //        CreatedById = Convert.ToInt32(TextBoxUserId.Text),
        //        ProductFamilyId = Convert.ToInt32(TextBoxProductFamilyId.Text),
        //        DisplayName = TextBoxDisplayName.Text,
        //        ParameterJson = iLogicFormSpec.ToJson(),
        //        IsPublic = CheckBoxIsPublic.Checked,
        //        IsConfigurable = CheckBoxIsConfigurable.Checked,
        //    };

        //    // save model to zipfile
        //    string zipFileName = UploadToCadflair.CreateTemporaryZipFile(_doc, true);



        //    // NEED TO UPDATE THIS METHOD TO REFLECT CHANGES TO THE PRODUCTCONTROLLER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        //    // Upload to Cadflair
        //    bool uploadSuccessful = await UploadToCadflair.UploadModelToForge(newProduct, zipFileName);

        //    // clean up
        //    System.IO.File.Delete(zipFileName);

        //    MessageBox.Show(uploadSuccessful.ToString(), "Success?", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //}

    }
}
