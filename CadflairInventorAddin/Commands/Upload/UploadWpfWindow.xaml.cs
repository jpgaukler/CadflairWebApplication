using CadflairDataAccess.Models;
using CadflairInventorAddin.Api;
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
        private User _loggedInUser;
        private List<ProductFolder> productFolders= new List<ProductFolder>()
        {
            new ProductFolder(){ DisplayName = "Products" }
        };

        public UploadWpfWindow(Inventor.Document doc)
        {
            InitializeComponent();

            _doc = doc;

            // set colors of window
            if(Globals.InventorApplication.ThemeManager.ActiveTheme.Name.Contains("Light"))
            {
                (System.Windows.Media.Color)FindResource("ApplicationFrameBackgroundColor") = ColorConverter.ConvertFromString("#d9d9d9");
                Resources["ApplicationFrameBackgroundColor"] = ColorConverter.ConvertFromString("#d9d9d9");
                Resources["BrowserPaneColor"] = ColorConverter.ConvertFromString("#f5f5f5");
                Resources["BrowserPaneAccentColor"] = ColorConverter.ConvertFromString("#eaeaea");
                Resources["BrowserPaneTextColor"] = ColorConverter.ConvertFromString("#666666");
                Resources["InputBackgroundColor"] = ColorConverter.ConvertFromString("#ffffff");
                Resources["ButtonBorderColor"] = ColorConverter.ConvertFromString("#bababa");
                MessageBox.Show("light theme");

            }

            if(Globals.InventorApplication.ThemeManager.ActiveTheme.Name.Contains("Dark"))
            {
                Resources["ApplicationFrameBackgroundColor"] = ColorConverter.ConvertFromString("#222933");
                Resources["BrowserPaneColor"] = ColorConverter.ConvertFromString("#3b4453");
                Resources["BrowserPaneAccentColor"] = ColorConverter.ConvertFromString("#4b5463");
                Resources["BrowserPaneTextColor"] = ColorConverter.ConvertFromString("#f5f5f5");
                Resources["InputBackgroundColor"] = ColorConverter.ConvertFromString("#2c3340");
                Resources["ButtonBorderColor"] = ColorConverter.ConvertFromString("#8691a1");
                MessageBox.Show("dark theme");
            }

            //SolidColorBrush backgroundBrush = new SolidColorBrush(Globals.InventorApplication.ThemeManager.GetComponentThemeColor("BrowserPane_BackgroundColor").ToSystemMediaColor());
            //SolidColorBrush foregroundBrush = new SolidColorBrush(Globals.InventorApplication.ThemeManager.GetComponentThemeColor("BrowserPane_TextColor").ToSystemMediaColor());
            //SolidColorBrush appFrameBrush = new SolidColorBrush(Globals.InventorApplication.ThemeManager.GetComponentThemeColor("ApplicationFrame_BackgroundColor").ToSystemMediaColor());

            //this.Background = backgroundBrush;
            //this.Foreground = foregroundBrush;


            //DataGridViewParameters.BackgroundColor = appFrameBrush;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // get logged in user
            AuthenticationResult auth = await Authentication.GetAuthenticationResult();
            if (auth != null) _loggedInUser = await UserApi.GetUserByObjectIdentifier(auth.UniqueId);

            // load product folders into the UI
            await LoadProductFoldersRecursive(null, ProductFolderTreeView.Items);

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

        private async Task LoadProductFoldersRecursive(int? parentId, ItemCollection treeViewItems)
        {
            if (_loggedInUser == null || _loggedInUser.SubscriptionId == null) return;

            List<ProductFolder> folders = await ProductApi.GetProductFoldersBySubscriptionIdAndParentId((int)_loggedInUser.SubscriptionId, parentId);

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

            if (folder == null) return;

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



            //save model to zipfile
            string zipFileName = UploadToCadflair.CreateTemporaryZipFile(_doc, true);

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
                                                             zipFileName: zipFileName);

            // clean up
            System.IO.File.Delete(zipFileName);

            if(product == null)
            {
                ConnectionRichTextBox.AppendText("Upload failed!");
            }
            else
            {
                ConnectionRichTextBox.AppendText("Upload successful!\n");
                ConnectionRichTextBox.AppendText($"Product Id: {product.Id} \n");
                ConnectionRichTextBox.AppendText($"Display Name: {product.DisplayName} \n");
                ConnectionRichTextBox.AppendText($"Created On: {product.CreatedOn} \n");
            }
        }
    }
}
