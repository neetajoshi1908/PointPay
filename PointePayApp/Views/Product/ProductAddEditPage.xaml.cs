using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using PointePayApp.Common;
using PointePayApp.Model;
using PointePayApp.Provider;
using PointePayApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;


namespace PointePayApp.Views
{
    public partial class ProductAddEditPage : PhoneApplicationPage
    {
        public List<SubCategoryOfflineViewModel> ListCategoryData { get; set; }
        public static int _employeeId ; // Logged In user's employeeId
        public static int _organizationId ; // Logged In user's organizationId
        public static int _categoryId; // Logged In user's organizationId
        public static int _productId;
        public static int _organizationProductId;
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

                    if (ISOFile.FileExists("viewProductDetails"))//read ProductDetails    
                    {
                        using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewProductDetails", FileMode.Open))
                        {
                            //====================================================================================================================
                            // Read Product Details
                            //====================================================================================================================

                            DataContractSerializer serializer = new DataContractSerializer(typeof(ProductViewModel));
                            var ObjProductData = (ProductViewModel)serializer.ReadObject(fileStream);
                            txtItemName.Text = Convert.ToString(ObjProductData.code);
                            txtItemDescription.Text = Convert.ToString(ObjProductData.description);
                            txtSalePrice.Text = Convert.ToString(ObjProductData.currentPrice);
                            txtCostPrice.Text = Convert.ToString(ObjProductData.costPrice);
                            txtUPC.Text = Convert.ToString(ObjProductData.upc);
                            _productId = Convert.ToInt32(ObjProductData.productId);
                            _categoryId = Convert.ToInt32(ObjProductData.categoryId);
                            _organizationProductId = Convert.ToInt32(ObjProductData.organizationProductId);
                            _mode = ObjProductData.mode;


                            if (_mode == "Add")
                            {
                                btnSave.Content = "Add Product";
                                lblHeader.Text = "ADD PRODUCT";
                            }
                            if (_mode == "Edit")
                            {
                                btnSave.Content = "Save";
                                lblHeader.Text = "EDIT PRODUCT";
                            }
                        }
                    }


                    //====================================================================================================================
                    // Fill Category Dropdown Default
                    //====================================================================================================================

                    if (Utilities.CheckInternetConnection())
                    {
                        // ----------------------------------------------------------------------
                        // "Network Status: Connected."

                        // Show Loader 
                        myIndeterminateProbar.Visibility = Visibility.Visible;

                        SubCategoryRequest obj = new SubCategoryRequest();
                        obj.organizationId = _organizationId;
                        obj.set = 1;
                        obj.count = Utilities.GetListCount();
                        String data = "organizationId=" + obj.organizationId + "&set=" + obj.set + "&count=" + obj.count;

                        //Initialize WebClient
                        WebClient webClient = new WebClient();
                        //Set Header
                        webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                        webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                        webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
                        webClient.UploadStringAsync(new Uri(Utilities.GetURL("category/categoryListing/")), "POST", data);
                        //Assign Event Handler0
                        webClient.UploadStringCompleted += wc_UploadCategoryCompleted;
                    }
                }
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/Login/LoginPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        void wc_UploadCategoryCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Contains("no Category found"))
                {
                    ListCategoryData = new List<SubCategoryOfflineViewModel>();
                    this.listPickerCategory.ItemsSource = ListCategoryData;
                }
                else
                {
                    //Parse JSON result 
                    var rootObject = JsonConvert.DeserializeObject<RootObject_Category>(e.Result);
                    if (rootObject.success == 1)
                    {
                        ListCategoryData = new List<SubCategoryOfflineViewModel>();
                        ListCategoryData.Add(new SubCategoryOfflineViewModel { categoryId = "0", categoryCode = "-- Select category --" });
                        foreach (var itm in rootObject.response.data)
                        {
                            if (itm.parentCategoryId == "0")
                            {
                                ListCategoryData.Add(new SubCategoryOfflineViewModel { categoryId = itm.categoryId, categoryCode = itm.categoryCode });
                            }
                        };
                        this.listPickerCategory.ItemsSource = ListCategoryData;

                        if (_mode == "Add")
                        {
                            this.listPickerCategory.SelectedIndex = 0;
                        }
                        if (_mode == "Edit")
                        {
                            var item = ListCategoryData.Where(x => x.categoryId == _categoryId.ToString()).FirstOrDefault();
                            this.listPickerCategory.SelectedItem = item;
                        }

                        // hide Loader 
                        myIndeterminateProbar.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        MessageBox.Show(rootObject.response.message.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something wrong happened.");
            }
            finally
            {
                // hide Loader 
                myIndeterminateProbar.Visibility = Visibility.Collapsed;
            }
        }//wc_DownloadStringCompleted

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Parameters
            ProductRequest obj = new ProductRequest();
            obj.employeeId = _employeeId;  // Logged In User's id
            obj.organizationId = _organizationId; // Logged In organizationId  
            obj.productCodeOveride = txtItemName.Text.Trim();
            obj.productDescription = txtItemDescription.Text.Trim();
            obj.currentPrice=txtSalePrice.Text.Trim();
            obj.costPrice=txtCostPrice.Text.Trim();
            obj.upc=txtUPC.Text.Trim();
            //obj.file = "";

            if (Utilities.CheckInternetConnection())
            {
                // ----------------------------------------------------------------------
                // "Network Status: Connected."
                ListPickerItem selectedItemState = this.listPickerCategory.ItemContainerGenerator.ContainerFromItem(this.listPickerCategory.SelectedItem) as ListPickerItem;
                SubCategoryOfflineViewModel SelecteddataCategory = selectedItemState.DataContext as SubCategoryOfflineViewModel;
                obj.categoryId = Convert.ToInt32(SelecteddataCategory.categoryId);

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

                    if (_mode == "Add")
                    {
                        data = "employeeId=" + obj.employeeId + "&organizationId=" + obj.organizationId + "&productCodeOveride=" + obj.productCodeOveride + "&productDescription=" + obj.productDescription + "&categoryId=" + obj.categoryId + "&currentPrice=" + obj.currentPrice + "&costPrice=" + obj.costPrice + "&upc=" + obj.upc;
                        webClient.UploadStringAsync(new Uri(Utilities.GetURL("manageProduct/addProduct/")), "POST", data);
                    }
                    if (_mode == "Edit")
                    {
                        obj.productId = _productId;

                        data = "productId=" + obj.productId + "&organizationProductId=" + _organizationProductId + "&employeeId=" + obj.employeeId + "&organizationId=" + obj.organizationId + "&productCodeOveride=" + obj.productCodeOveride + "&productDescription=" + obj.productDescription + "&categoryId=" + obj.categoryId + "&currentPrice=" + obj.currentPrice + "&costPrice=" + obj.costPrice + "&upc=" + obj.upc;
                        webClient.UploadStringAsync(new Uri(Utilities.GetURL("manageProduct/updateProduct/")), "POST", data);
                    }

                    //Assign Event Handler
                    webClient.UploadStringCompleted += wc_UploadSaveCompleted;
                }
            }
            else
            {
                // ----------------------------------------------------------------------
                //  "Network Status: Not Connected."
            }
        }

        void wc_UploadSaveCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                //Parse JSON result 
                var rootObject = JsonConvert.DeserializeObject<dynamic>(e.Result);
                if (rootObject.success == 1)
                {
                    MessageBox.Show(rootObject.response.message.ToString());
                    // hide Loader 
                    myIndeterminateProbar.Visibility = Visibility.Collapsed;

                    NavigationService.Navigate(new Uri("/Views/Product/ProductListPage.xaml", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    string msgstr = rootObject.response.message.ToString();
                    string[] words = msgstr.Split(':');
                    string msg = Convert.ToString(words[1].Replace("}", ""));
                    msg = Regex.Replace(msg, @"[\""]", "", RegexOptions.None);
                    MessageBox.Show(msg);
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

            if (Utilities.CheckInternetConnection())
            {
                // ----------------------------------------------------------------------
                // "Network Status: Connected."
                ListPickerItem selectedItemState = this.listPickerCategory.ItemContainerGenerator.ContainerFromItem(this.listPickerCategory.SelectedItem) as ListPickerItem;
                SubCategoryOfflineViewModel SelecteddataCategory = selectedItemState.DataContext as SubCategoryOfflineViewModel;

                if (Convert.ToInt32(SelecteddataCategory.categoryId) <= 0)
                {
                    MessageBox.Show("Select category!");
                    isValid = false;
                }
            }

            return isValid;
        }

        private void ImgBack_Tap(object sender, System.Windows.Input.GestureEventArgs e)
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