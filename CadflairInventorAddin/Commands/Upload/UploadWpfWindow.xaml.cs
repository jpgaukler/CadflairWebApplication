using CadflairDataAccess.Models;
using CadflairInventorAddin.Api;
using CadflairInventorAddin.Helpers;
using CadflairInventorLibrary.Helpers;
using Inventor;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CadflairInventorAddin.Commands.Upload
{
    public partial class UploadWpfWindow : Window
    {
        private Document _doc;
        private User _loggedInUser;

        public UploadWpfWindow(Document doc)
        {
            InitializeComponent();

            // set colors of window
            if(Globals.InventorApplication.ThemeManager.ActiveTheme.Name.Contains("Light"))
            {
                Resources["ApplicationFrameBackgroundColor"] = ColorConverter.ConvertFromString("#d9d9d9");
                Resources["BrowserPaneColor"] = ColorConverter.ConvertFromString("#f5f5f5");
                Resources["BrowserPaneAccentColor"] = ColorConverter.ConvertFromString("#eaeaea");
                Resources["BrowserPaneTextColor"] = ColorConverter.ConvertFromString("#000000");
                Resources["InputBackgroundColor"] = ColorConverter.ConvertFromString("#ffffff");
                Resources["ButtonBorderColor"] = ColorConverter.ConvertFromString("#bababa");
            }

            if(Globals.InventorApplication.ThemeManager.ActiveTheme.Name.Contains("Dark"))
            {
                Resources["ApplicationFrameBackgroundColor"] = (System.Windows.Media.Color)ColorConverter.ConvertFromString("#222933");
                Resources["BrowserPaneColor"] = (System.Windows.Media.Color)ColorConverter.ConvertFromString("#3b4453");
                Resources["BrowserPaneAccentColor"] = (System.Windows.Media.Color)ColorConverter.ConvertFromString("#4b5463");
                Resources["BrowserPaneTextColor"] = (System.Windows.Media.Color)ColorConverter.ConvertFromString("#f5f5f5");
                Resources["InputBackgroundColor"] = (System.Windows.Media.Color)ColorConverter.ConvertFromString("#2c3340");
                Resources["ButtonBorderColor"] = (System.Windows.Media.Color)ColorConverter.ConvertFromString("#8691a1");
            }

            _doc = doc;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // get logged in user
            AuthenticationResult auth = await Authentication.GetAuthenticationResult();
            if (auth != null) _loggedInUser = await UserApi.GetUserByObjectIdentifier(auth.UniqueId);

            // load product folders into the UI
            await LoadProductFolders();

            // load iLogic form data into UI
            ILogicFormsComboBox.ItemsSource = UploadToCadflair.GetILogicFormElements(_doc);
            ILogicFormsComboBox.DisplayMemberPath = nameof(ILogicFormElement.Name);
            ILogicFormsComboBox.SelectedIndex = 0;
        }

        private void ILogicFormsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ILogicFormElement iLogicForm = (ILogicFormElement)ILogicFormsComboBox.SelectedItem;
            ParametersDataGrid.ItemsSource = iLogicForm.GetParameterList();
        }

        private async Task LoadProductFolders()
        {
            if (_loggedInUser == null || _loggedInUser.SubscriptionId == null) return;

            List<ProductFolder> folderList = await ProductApi.GetProductFoldersBySubscriptionId((int)_loggedInUser.SubscriptionId);

            RecurseFolders(ProductFolderTreeView.Items, folderList);

            void RecurseFolders(ItemCollection collection, IEnumerable<ProductFolder> folders)
            {
                foreach (ProductFolder folder in folders)
                {
                    TreeViewItem treeViewItem = new TreeViewItem
                    {
                        DataContext = folder,
                        Header = folder.DisplayName,
                    };

                    collection.Add(treeViewItem);

                    if (folder.ChildFolders.Any())
                        RecurseFolders(treeViewItem.Items, folder.ChildFolders);
                }
            }

        }

        private async void CreateProductFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductFolderTextBox.Text))
            {
                MessageBox.Show("Please enter a folder name.", "Folder Name Not Provided", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_loggedInUser == null)
            {
                MessageBox.Show("A valid user profile could not be found. Please verify that user is signed in to Cadflair.", "User Not Found", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if (_loggedInUser.SubscriptionId == null)
            {
                MessageBox.Show("A valid Cadflair subscription could not be found.", "Subscription Not Found", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

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

            ProductFolder folder = await ProductApi.CreateProductFolder((int)_loggedInUser.SubscriptionId, _loggedInUser.Id, ProductFolderTextBox.Text, parentId);

            if (folder == null) 
                return;

            TreeViewItem treeViewItem = new TreeViewItem
            {
                DataContext = folder,
                Header = folder.DisplayName,
            };

            treeViewItems.Add(treeViewItem);
        }

        private async void CreateProductButton_Click(object sender, RoutedEventArgs e)
        {

            if (_loggedInUser == null)
            {
                MessageBox.Show("A valid user profile could not be found. Please verify that user is signed in to Cadflair.", "User Not Found", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            if (_loggedInUser.SubscriptionId == null)
            {
                MessageBox.Show("A valid Cadflair subscription could not be found.", "Subscription Not Found", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

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

            Product existingProduct = await ProductApi.GetProductBySubscriptionIdAndDisplayName((int)_loggedInUser.SubscriptionId, DisplayNameTextBox.Text);
            if (existingProduct != null)
            {
                if (MessageBox.Show($"There is already a product named '{DisplayNameTextBox.Text}'. Would you like to overwrite it with a new version?", "Existing Product Found", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
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
            ILogicFormElement iLogicForm = (ILogicFormElement)ILogicFormsComboBox.SelectedItem;

            // NEED TO ADD CHECK FOR DUPLICATE DISPLAY NAME AND OTHER DATA VALIDATION!!!!

            TreeViewItem selectedItem = (TreeViewItem)ProductFolderTreeView.SelectedItem;
            int productFolderId = ((ProductFolder)selectedItem.DataContext).Id; 

            // save model to zipfile
            string inventorZipName = UploadToCadflair.ZipInventorFiles(_doc, true);

            // save viewables files to zip folder
            string viewablesFolderName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            ExportHelpers.ExportSvf(_doc, viewablesFolderName);
            ExportHelpers.ExportThumbnail(_doc, viewablesFolderName);
            string viewablesZipName = ExportHelpers.ConvertToZip(viewablesFolderName);

            // save stp file
            string stpFileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid()}.stp");
            ExportHelpers.ExportStp(_doc, stpFileName);

            // Upload to Cadflair
            Product product = await ProductApi.CreateProduct(userId: _loggedInUser.Id,
                                                             subscriptionId: (int)_loggedInUser.SubscriptionId,
                                                             productFolderId: productFolderId,
                                                             displayName: DisplayNameTextBox.Text,
                                                             rootFileName: System.IO.Path.GetFileName(_doc.FullFileName),
                                                             iLogicFormJson: iLogicForm.GetFormJson(),
                                                             argumentJson: iLogicForm.GetArgumentJson(),
                                                             isPublic: (bool)IsPublicCheckBox.IsChecked,
                                                             isConfigurable: (bool)AllowProductConfigurationCheckBox.IsChecked,
                                                             inventorZipName: inventorZipName,
                                                             stpFileName: stpFileName,
                                                             viewablesZipName: viewablesZipName);

            // clean up
            System.IO.File.Delete(inventorZipName);
            System.IO.File.Delete(viewablesZipName);
            System.IO.File.Delete(stpFileName);

            if(product == null)
            {
                MessageBox.Show("Upload failed!", "Upload Failed!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show($"Product Id: {product.Id} \nDisplay Name: {product.DisplayName} \n", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            UploadToCadflair.UploadToCadflair_OnTerminate();
        }
    }
}
