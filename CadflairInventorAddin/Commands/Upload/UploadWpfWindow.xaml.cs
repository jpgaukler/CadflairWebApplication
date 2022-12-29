﻿using CadflairDataAccess.Models;
using CadflairInventorAddin.Helpers;
using Inventor;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

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

            _doc = doc;

            // set colors of window
            SolidColorBrush backgroundBrush = new SolidColorBrush(Globals.InventorApplication.ThemeManager.GetComponentThemeColor("BrowserPane_BackgroundColor").ToSystemMediaColor());
            SolidColorBrush foregroundBrush = new SolidColorBrush(Globals.InventorApplication.ThemeManager.GetComponentThemeColor("BrowserPane_TextColor").ToSystemMediaColor());
            SolidColorBrush appFrameBrush = new SolidColorBrush(Globals.InventorApplication.ThemeManager.GetComponentThemeColor("ApplicationFrame_BackgroundColor").ToSystemMediaColor());

            this.Background = backgroundBrush;
            this.Foreground = foregroundBrush;
            //DataGridViewParameters.BackgroundColor = appFrameBrush;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _iLogicForms = UploadToCadflair.GetILogicFormElements(_doc);

            foreach (ILogicFormElement form in _iLogicForms)
            {
                ILogicFormsComboBox.Items.Add(form.Name);
            }

            ILogicFormsComboBox.SelectedIndex = 0;

            await LoadProductFoldersRecursive(null, ProductFolderTreeView.Items);
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

        private async Task LoadProductFoldersRecursive(int? parentId, ItemCollection treeViewItems)
        {
            List<ProductFolder> folders = await UploadToCadflair.GetProductFoldersBySubscriptionIdAndParentId(1, parentId);

            foreach (ProductFolder folder in folders)
            {
                TreeViewItem treeViewItem = new TreeViewItem
                {
                    DataContext = folder,
                    Header = folder.DisplayName,
                };

                treeViewItems.Add(treeViewItem);
                await LoadProductFoldersRecursive(folder.Id, treeViewItem.Items);
            }
        }

        private async void CreateProductFolderButton_Click(object sender, RoutedEventArgs e)
        {
            int? parentId = null;
            ItemCollection treeViewItems;

            if (ProductFolderTreeView.SelectedItem == null)
            {
                treeViewItems = ProductFolderTreeView.Items;
            }
            else
            {
                TreeViewItem selectedItem = (TreeViewItem)ProductFolderTreeView.SelectedItem;
                treeViewItems = selectedItem.Items;
                parentId = ((ProductFolder)selectedItem.DataContext).Id; 
            }

            ProductFolder folder = await UploadToCadflair.CreateProductFolder(1, 1, "New folder", parentId);

            TreeViewItem treeViewItem = new TreeViewItem
            {
                DataContext = folder,
                Header = folder.DisplayName,
            };

            treeViewItems.Add(treeViewItem);
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
                MessageBox.Show("Please enter a Display Name", "Display Name Not Provided", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ProductFolderTreeView.SelectedItem == null)
            {
                MessageBox.Show("Please select a destination folder.", "No Folder Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            TreeViewItem selectedItem = (TreeViewItem)ProductFolderTreeView.SelectedItem;
            int productFolderId = ((ProductFolder)selectedItem.DataContext).Id; 

            // Upload to Cadflair
            string uploadResult = await UploadToCadflair.UploadProductToCadflair(userId: 1,
                                                                                 subscriptionId: 1,
                                                                                 productFolderId: productFolderId,
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
