using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PointePayApp.Common;
using PointePayApp.Model;
using PointePayApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PointePayApp.Views
{
    public partial class ProductAddMasterList : PhoneApplicationPage
    {
        public List<SubCategoryOfflineViewModel> ListCategoryData { get; set; }
        public List<MasterProductViewModel> ListMasterProductData { get; set; }
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        public static int _employeeId ; // Logged In user's employeeId
        public static int _organizationId ; // Logged In user's organizationId
        public static int _productId; // Selected items productId
        public static string _productCodeOveride { get; set; }// Selected items productCodeOveride
        public static string _productDescription { get; set; }// Selected items productDescription
        public static bool _isPopUpOpen;
        public ProductAddMasterList()
        {
            InitializeComponent();

            _isPopUpOpen = false;

            IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
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

                            // Show Loader 
                            myIndeterminateProbar.Visibility = Visibility.Visible;

                            //====================================================================================================================
                            // Fill Master Product List
                            //====================================================================================================================

                            // Parameters
                            ProductRequest obj = new ProductRequest();
                            obj.organizationId = _organizationId; 
                            obj.set = 1;
                            obj.count = Utilities.GetListCount();
                            obj.isInventory = 0; //0 for product list, 1 for inventory list	

                            String data = "organizationId=" + obj.organizationId + "&set=" + obj.set + "&count=" + obj.count + "&count=" + obj.count;

                            //Initialize WebClient
                            WebClient webClient = new WebClient();
                            //Set Header
                            webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                            webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                            webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
                            webClient.UploadStringAsync(new Uri(Utilities.GetURL("manageProduct/masterProductList/")), "POST", data);
                            //Assign Event Handler0
                            webClient.UploadStringCompleted += wc_UploadOrganizationProductCompleted;

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
                        this.listPickerCategory.SelectedIndex = 0;

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

        void wc_UploadOrganizationProductCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                //Parse JSON result 
                var rootObject = JsonConvert.DeserializeObject<RootObject_MasterProduct>(e.Result);
                if (rootObject.success == 1)
                {
                    ListMasterProductData = new List<MasterProductViewModel>();
                    foreach (var itm in rootObject.response.data)
                    {
                        var Source = "/Assets/Product/archive.png";
                        if (!string.IsNullOrEmpty(itm.imageName))
                        {
                            Source = Utilities.GetMarketplaceURL() + uploadImagePath.PRODUCT + itm.imageName;
                        }
                        ListMasterProductData.Add(new MasterProductViewModel { productId = itm.productId, code = itm.code, description = itm.description,  categoryCode = itm.categoryCode, brandName=itm.brandName,active = itm.active, createDt = itm.createDt, lastModifiedDt = itm.lastModifiedDt, lastModifiedBy = itm.lastModifiedBy,fullImagePath=Source });
                    }
                    this.lstMasterProductItems.ItemsSource = ListMasterProductData;

                    // Hide Loader 
                    myIndeterminateProbar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show(rootObject.response.message.ToString());

                    // Hide Loader 
                    myIndeterminateProbar.Visibility = Visibility.Collapsed;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Something wrong happened.");

                // Hide Loader 
                myIndeterminateProbar.Visibility = Visibility.Collapsed;
            }
            finally
            {
                // Hide Loader 
                myIndeterminateProbar.Visibility = Visibility.Collapsed;
            }
        }

        private void lstMasterProductItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var _MasterProductDataContext = ((MasterProductViewModel)(sender as LongListSelector).SelectedItem);
            lblItemName.Text = _MasterProductDataContext.code.ToString();
            _productId = Convert.ToInt32(_MasterProductDataContext.productId);
            _productCodeOveride = _MasterProductDataContext.code.Trim();
            _productDescription = _MasterProductDataContext.description.Trim();

            popupAddMasterProduct.IsOpen = true;
            grdGrayBackground.Visibility = Visibility.Visible;
            _isPopUpOpen = false;
            btnCancelMasterProduct.Click += (s, args) =>
            {
                //====================================================================================================================
                // PopUp cancel Button
                //====================================================================================================================

                popupAddMasterProduct.IsOpen = false;
                grdGrayBackground.Visibility = Visibility.Collapsed;
            };
        }

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

            //CostPrice  Validation  
            if (String.IsNullOrWhiteSpace(txtCostPrice.Text.Trim()))
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

        private void btnSaveMasterProduct_Click(object sender, RoutedEventArgs e)
        {
            //====================================================================================================================
            // PopUp Save Master Product Button
            //====================================================================================================================

            // Parameters
            ProductRequest obj = new ProductRequest();
            obj.productId = _productId;  // Logged In User's id
            obj.employeeId = _employeeId;  // Logged In User's id
            obj.organizationId = _organizationId; // Logged In organizationId  
            obj.productCodeOveride = _productCodeOveride;
            obj.productDescription = _productDescription;
            obj.categoryId = 0; // Need to change
            obj.currentPrice = txtSalePrice.Text.Trim();
            obj.costPrice = txtCostPrice.Text.Trim();
            obj.upc = txtUPC.Text.Trim();
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
                data = "productId=" + obj.productId + "&employeeId=" + obj.employeeId + "&organizationId=" + obj.organizationId + "&productCodeOveride=" + obj.productCodeOveride + "&productDescription=" + obj.productDescription + "&categoryId=" + obj.categoryId + "&currentPrice=" + obj.currentPrice + "&costPrice=" + obj.costPrice + "&upc=" + obj.upc;
                webClient.UploadStringAsync(new Uri(Utilities.GetURL("manageProduct/addRetailerPrdMasterList/")), "POST", data);
                //Assign Event Handler
                webClient.UploadStringCompleted += wc_UploadSaveCompleted;
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
                    MessageBox.Show("product Added successfully");
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
                //MessageBox.Show("Something wrong happened.");
            }
            finally
            {
                // hide Loader 
                myIndeterminateProbar.Visibility = Visibility.Collapsed;
                NavigationService.Navigate(new Uri("/Views/Product/ProductListPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        private void listPickerCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(_isPopUpOpen)
            {
                popupAddMasterProduct.IsOpen = true;
                grdGrayBackground.Visibility = Visibility.Visible;
            }

            _isPopUpOpen = true;
        }//wc_DownloadStringCompleted
    }
}