using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using PointePayApp.Common;
using PointePayApp.Model;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

namespace PointePayApp.Views
{
    public partial class ProductAddEditPage : PhoneApplicationPage
    {
        public static int _employeeId ; // Logged In user's employeeId
        public static int _organizationId ; // Logged In user's organizationId
        public static string _mode;

        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        public ProductAddEditPage()
        {
            InitializeComponent();

            if (IsolatedStorageSettings.ApplicationSettings.Contains("islogin"))
            {
                if (!(Convert.ToString(IsolatedStorageSettings.ApplicationSettings["islogin"]).ToLower() == "yes"))
                {
                    NavigationService.Navigate(new Uri("/Views/Login/LoginPage.xaml", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    if (ISOFile.FileExists("CurrentLoginUserDetails"))//read current user login details    
                    {
                        using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("CurrentLoginUserDetails", FileMode.Open))
                        {
                            DataContractSerializer serializer = new DataContractSerializer(typeof(LoginViewModel));
                            var ObjUserData = (LoginViewModel)serializer.ReadObject(fileStream);
                            _employeeId = Convert.ToInt32(ObjUserData.employeeId);
                            _organizationId = Convert.ToInt32(ObjUserData.organizationId);
                        }
                    }
                }
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/Login/LoginPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Parameters
            ProductRequest obj = new ProductRequest();
            obj.employeeId = _employeeId;  // Logged In User's id
            obj.organizationId = _organizationId; // Logged In organizationId  
            obj.productCodeOveride = txtItemName.Text.Trim();
            obj.productDescription = txtItemDescription.Text.Trim();
            obj.categoryId=0; // Need to change
            obj.currentPrice=txtSalePrice.Text.Trim();
            obj.costPrice=txtCostPrice.Text.Trim();
            obj.upc=txtUPC.Text.Trim();
            //obj.file = "";

            String data = string.Empty;

            if (Validation() == true)
            {
                // Show Loader 
                myIndeterminateProbar.Visibility = Visibility.Visible;

                //====================================================================================================================
                // Submit Details
                //====================================================================================================================

                //Initialize WebClient
                WebClient webClient = new WebClient();
                //Set Header
                webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";

                //if (_mode == "Add")
                //{
                    data = "employeeId=" + obj.employeeId + "&organizationId=" + obj.organizationId + "&productCodeOveride=" + obj.productCodeOveride + "&productDescription=" + obj.productDescription + "&categoryId=" + obj.categoryId + "&currentPrice=" + obj.currentPrice + "&costPrice=" + obj.costPrice + "&upc=" + obj.upc ;
                    webClient.UploadStringAsync(new Uri(Utilities.GetURL("manageProduct/addProduct/")), "POST", data);
                //}
                //if (_mode == "Edit")
                //{
                //    data = "productId=" + obj.employeeId + "&employeeId=" + obj.employeeId + "&organizationId=" + obj.organizationId + "&productCodeOveride=" + obj.productCodeOveride + "&productDescription=" + obj.productDescription + "&categoryId=" + obj.categoryId + "&currentPrice=" + obj.currentPrice + "&costPrice=" + obj.costPrice + "&upc=" + obj.upc;
                //    webClient.UploadStringAsync(new Uri(Utilities.GetURL("manageProduct/updateProduct/")), "POST", data);
                //}

                //Assign Event Handler
                webClient.UploadStringCompleted += wc_UploadSaveCompleted;
            }
        }
        void wc_UploadSaveCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                //Parse JSON result 
                var rootObject = JsonConvert.DeserializeObject<RootObject_Employee>(e.Result);
                if (rootObject.success == 1)
                {
                    MessageBox.Show(rootObject.response.message.ToString());
                    // hide Loader 
                    myIndeterminateProbar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show(rootObject.response.message.ToString());
                    // hide Loader 
                    myIndeterminateProbar.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something wrong happened.");
                // hide Loader 
                myIndeterminateProbar.Visibility = Visibility.Collapsed;
            }
            finally
            {
                // hide Loader 
                myIndeterminateProbar.Visibility = Visibility.Collapsed;
            }
        }//wc_DownloadStringCompleted
        private bool Validation()
        {
            bool isValid = true;

            //ListPickerItem selectedItemState = this.listPickerCategory.ItemContainerGenerator.ContainerFromItem(this.listPickerCategory.SelectedItem) as ListPickerItem;
            //data_State SelecteddataState = selectedItemState.DataContext as data_State;


            //// Validation State
            //else if (SelecteddataState.stateId == "0")
            //{
            //    MessageBox.Show("Select Category!");
            //    isValid = false;
            //}
            // Validation ItemName
            if (String.IsNullOrWhiteSpace(txtItemName.Text.Trim()))
            {
                MessageBox.Show("Enter Item Name!");
                isValid = false;
            }
            //CostPrice  Validation  
            else if (String.IsNullOrWhiteSpace(txtCostPrice.Text.Trim()))
            {
                MessageBox.Show("Enter Cost Price!");
                isValid = false;
            }
            //SalePrice Validation
            else if (String.IsNullOrWhiteSpace(txtSalePrice.Text.Trim()))
            {
                MessageBox.Show("Enter Sale Price!");
                isValid = false;
            }

            return isValid;
        }
        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Product/ProductListPage.xaml", UriKind.RelativeOrAbsolute));
        }

        #region  Set focus on page textboxes
        private void txtItemName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtItemName.Background = new SolidColorBrush(Colors.Transparent);
            txtItemName.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txtItemDescription_GotFocus(object sender, RoutedEventArgs e)
        {
            txtItemDescription.Background = new SolidColorBrush(Colors.Transparent);
            txtItemDescription.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txtUPC_GotFocus(object sender, RoutedEventArgs e)
        {
            txtUPC.Background = new SolidColorBrush(Colors.Transparent);
            txtUPC.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txtCostPrice_GotFocus(object sender, RoutedEventArgs e)
        {
            txtCostPrice.Background = new SolidColorBrush(Colors.Transparent);
            txtCostPrice.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txtSalePrice_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSalePrice.Background = new SolidColorBrush(Colors.Transparent);
            txtSalePrice.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }
        #endregion

    }
}