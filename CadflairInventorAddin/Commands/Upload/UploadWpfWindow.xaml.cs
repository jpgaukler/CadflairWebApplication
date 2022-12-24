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
    public partial class UploadWpfWindow : Window
    {
        private Document _doc;
        private List<ILogicFormElement> _iLogicForms;


        public UploadWpfWindow(Inventor.Document doc)
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
            foreach (ILogicFormElement form in _iLogicForms) ILogicFormsComboBox.Items.Add(form.Name);
            ILogicFormsComboBox.SelectedIndex = 0;

        }

        private void ILogicFormsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ParametersDataGrid.Items.Clear();
            ILogicFormElement iLogicForm = _iLogicForms.FirstOrDefault(i => i.Name == ILogicFormsComboBox.SelectedItem.ToString());
            PopulateParametersGrid(iLogicForm);
        }

        private void PopulateParametersGrid(ILogicFormElement iLogicForm)
        {
            foreach (ILogicFormElement item in iLogicForm.Items)
            {
                if (item.ParameterName != null) ParametersDataGrid.Items.Add(item);
                if (item.Items != null) PopulateParametersGrid(item);
            }
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {

            if (_doc.FileSaveCounter == 0)
            {
                MessageBox.Show("Please save the file before uploading.", "File Not Saved", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if (string.IsNullOrWhiteSpace(DisplayNameTextBox.Text))
            {
                MessageBox.Show("Please enter a value for Display Name", "Display Name Not Provided", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Save limits for numeric text box parameters as attributes
            //foreach (DataGridViewRow row in DataGridViewParameters.Rows)
            //{
            //    ILogicUiElement element = (ILogicUiElement)row.Cells[ILogicUIElementColumn.Index].Value;

            //    if (element.UiElementSpec == "NumericParameterControlSpec" && element.EditControlType == "TextBox")
            //    {
            //        double minValue = Double.Parse(row.Cells[MinValueColumn.Index].Value.ToString());
            //        double maxValue = Double.Parse(row.Cells[MaxValueColumn.Index].Value.ToString());

            //        Parameter parameter = _doc.GetParameter(element.ParameterName);
            //        parameter.AddValidationAttributes(minValue, maxValue);
            //    }
            //}

            // Get parameters in form of json
            ILogicFormElement iLogicFormSpec = _iLogicForms.FirstOrDefault(i => i.Name == ILogicFormsComboBox.SelectedItem.ToString());

            //UploadToCadflair.SaveILogicUiElementToJson(iLogicFormSpec);
            //UploadToCadflair.SaveILogicFormSpecToXml(iLogicFormSpec.Name);

            //save model to zipfile
            string zipFileName = UploadToCadflair.CreateTemporaryZipFile(_doc, true);


            // NEED TO ADD CHECK FOR DUPLICATE DISPLAY NAME AND OTHER DATA VALIDATION!!!!

            // Upload to Cadflair
            string uploadResult = await UploadToCadflair.UploadProductToCadflair(userId: 1,
                                                                                 subscriptionId: 1,
                                                                                 productFolderId: 1,
                                                                                 displayName: DisplayNameTextBox.Text,
                                                                                 rootFileName: System.IO.Path.GetFileName(_doc.FullFileName),
                                                                                 iLogicFormJson: iLogicFormSpec.GetFormJson(),
                                                                                 argumentJson: iLogicFormSpec.GetArgumentJson(),
                                                                                 isPublic: true,
                                                                                 isConfigurable: true,
                                                                                 zipFileName: zipFileName);

            // clean up
            System.IO.File.Delete(zipFileName);

            ConnectionRichTextBox.AppendText(uploadResult);
        }

    }
}
