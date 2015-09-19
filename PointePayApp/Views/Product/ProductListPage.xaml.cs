using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PointePayApp.Common;
using PointePayApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace PointePayApp.Views
{
    public partial class ProductListPage : PhoneApplicationPage
    {
        public List<ProductViewModel> ListProductData { get; set; }
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        public static int _employeeId; // Logged In user's employeeId
        public static int _organizationId; // Logged In user's organizationId
        public static string _redirectMode;
        int serchclick = 1;

        public ProductListPage()
        {
            InitializeComponent();

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
                            this.txtHeaderOrgName.Text = ObjUserData.organizationName;
                            this.txtHeaderFullName.Text = ObjUserData.firstName + " " + ObjUserData.lastName;
                            _employeeId = Convert.ToInt32(ObjUserData.employeeId);
                            _organizationId = Convert.ToInt32(ObjUserData.organizationId);
                            lstMenuItems.ItemsSource = Utilities.GetMenuItems(MenuCode.ProdMgnt);

                            // Show Loader 
                            myIndeterminateProbar.Visibility = Visibility.Visible;

                            //====================================================================================================================
                            // Fill organization Product List
                            //====================================================================================================================
                            // Parameters
                            ProductRequest obj = new ProductRequest();
                            obj.organizationId = _organizationId; 
                            obj.set = 1;
                            obj.count = 50;
                            obj.isInventory = 0; //0 for product list, 1 for inventory list	

                            String data = "organizationId=" + obj.organizationId + "&set=" + obj.set + "&count=" + obj.count + "&count=" + obj.count;

                            //Initialize WebClient
                            WebClient webClient = new WebClient();
                            //Set Header
                            webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                            webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                            webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
                            webClient.UploadStringAsync(new Uri(Utilities.GetURL("manageProduct/organizationProductList/")), "POST", data);
                            //Assign Event Handler0
                            webClient.UploadStringCompleted += wc_UploadOrganizationProductCompleted;
                        }
                    }
                }
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/Login/LoginPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        void wc_UploadOrganizationProductCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                    //Parse JSON result 
                    var rootObject = JsonConvert.DeserializeObject<RootObject_Product>(e.Result);
                    if (rootObject.success == 1)
                    {
                        ListProductData = new List<ProductViewModel>();
                        Productdiscount obj = new Productdiscount();
                        foreach (var itm in rootObject.response.data)
                        {
                            if (!string.IsNullOrEmpty(itm.discount))
                            {
                                var str = itm.discount.Replace("[{", "").Replace("}]", "");
                                string[] words = str.Split(',');
                                foreach (var parentitm in words)
                                {
                                    string[] childitm = parentitm.Split(':');

                                    if (childitm[0].ToString().Trim() == "discount") 
                                    {
                                        obj.discount = Convert.ToString(childitm[1]);
                                    }
                                    if (childitm[0].ToString().Trim() == "discountId") 
                                    {
                                        obj.discountId = Convert.ToString(childitm[1]);
                                    }
                                    if (childitm[0].ToString().Trim() == "title")
                                    {
                                        obj.title = Convert.ToString(childitm[1]);
                                    }
                                }
                            }

                            var Source = "/Assets/Product/archive.png";
                            if (!string.IsNullOrEmpty(itm.imageName))
                            {
                                Source = Utilities.GetMarketplaceURL() + uploadImagePath.PRODUCT + itm.imageName;
                            }
                            var categoryCode = "General";
                            if (!string.IsNullOrEmpty(itm.categoryCode))
                            {
                                categoryCode = itm.categoryCode;
                            }
                            ListProductData.Add(new ProductViewModel { productId = itm.productId, code = itm.code, description = itm.description, currentPrice = itm.currentPrice, categoryId = itm.categoryId, organizationId = itm.organizationId, categoryCode = categoryCode, parentCategoryId = itm.parentCategoryId, parentCategoryCode = itm.parentCategoryCode, categoryDescription = itm.categoryDescription, active = itm.active, createDt = itm.createDt, lastModifiedDt = itm.lastModifiedDt, lastModifiedBy = itm.lastModifiedBy, Productdiscount = obj, fullImagePath = Source, upc = itm.upc, costPrice = itm.costPrice, organizationProductId = itm.organizationProductId });
                        };

                        this.lstProductItems.ItemsSource = ListProductData;

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
        }//wc_DownloadStringCompleted

        private void lstProductItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //====================================================================================================================
            // View Product Details
            //====================================================================================================================

            if (_redirectMode == "Add")
            {
                _redirectMode = string.Empty;
            }
            else if (_redirectMode == "Edit")
            {
                _redirectMode = string.Empty;
            }
            else if (_redirectMode == "Default")
            {
                _redirectMode = string.Empty;
            }
            else
            {
                var ListItem = ((ProductViewModel)(sender as LongListSelector).SelectedItem);

                if (ISOFile.FileExists("viewProductDetails"))
                {
                    ISOFile.DeleteFile("viewProductDetails");
                }
                using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewProductDetails", FileMode.Create))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(ProductViewModel));
                    serializer.WriteObject(fileStream, ListItem);

                    NavigationService.Navigate(new Uri("/Views/Product/ProductDetailsPage.xaml", UriKind.RelativeOrAbsolute));
                }
            }
        }

        // Add Product
        private void ImgAddProduct_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            popupAddProduct.VerticalOffset = 0;
            popupAddProduct.IsOpen = true;
            grdGrayBackground.Visibility = Visibility.Visible;

            btnFromMasterList.Click += (s, args) =>
            {
                //====================================================================================================================
                // PopUp Master List
                //====================================================================================================================

                if (Utilities.CheckInternetConnection())
                {
                    // ----------------------------------------------------------------------
                    // "Network Status: Connected."

                    popupAddProduct.IsOpen = false;
                    grdGrayBackground.Visibility = Visibility.Collapsed;
                    NavigationService.Navigate(new Uri("/Views/Product/ProductAddMasterList.xaml", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    // ----------------------------------------------------------------------
                    //  "Network Status: Not Connected."

                    MessageBox.Show("You can not create a new product in offline mode.");
                }

            };

            btnAddManually.Click += (s, args) =>
            {
                //====================================================================================================================
                // PopUp Add Manually
                //====================================================================================================================

                popupAddProduct.IsOpen = false;
                grdGrayBackground.Visibility = Visibility.Collapsed;
                


                if (Utilities.CheckInternetConnection())
                {
                    // ----------------------------------------------------------------------
                    // "Network Status: Connected."

                    //====================================================================================================================
                    // Add new Cateory
                    //====================================================================================================================

                    ProductViewModel _ProductDataContext = new ProductViewModel();
                    // Set page mode for Add record of category
                    _redirectMode = "Add";
                    _ProductDataContext.mode = "Add";

                    if (ISOFile.FileExists("viewProductDetails"))
                    {
                        ISOFile.DeleteFile("viewProductDetails");
                    }
                    using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewProductDetails", FileMode.Create))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(ProductViewModel));
                        serializer.WriteObject(fileStream, _ProductDataContext);

                        NavigationService.Navigate(new Uri("/Views/Product/ProductAddEditPage.xaml", UriKind.RelativeOrAbsolute));
                    }
                }
                else
                {
                    // ----------------------------------------------------------------------
                    //  "Network Status: Not Connected."

                    MessageBox.Show("You can not create a new product in offline mode.");
                }

            };

            btnclose.Tap += (s, args) =>
            {
                //====================================================================================================================
                // PopUp cancel Button
                //====================================================================================================================

                popupAddProduct.IsOpen = false;
                grdGrayBackground.Visibility = Visibility.Collapsed;
            };
        }

        # region MenuItems

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (serchclick == 1)
            {
                Search_Panel.Visibility = Visibility.Visible;
                SlideTransition slideTransition = new SlideTransition();
                slideTransition.Mode = SlideTransitionMode.SlideRightFadeIn;
                ITransition transition = slideTransition.GetTransition(Search_Panel);
                transition.Completed += delegate
                {
                    transition.Stop();
                };
                transition.Begin();
                serchclick++;
            }
            else
            {
                SlideTransition slideTransition = new SlideTransition();
                slideTransition.Mode = SlideTransitionMode.SlideLeftFadeOut;
                ITransition transition = slideTransition.GetTransition(Search_Panel);
                transition.Completed += delegate
                {
                    transition.Stop();
                    Search_Panel.Visibility = Visibility.Collapsed;
                };
                transition.Begin();

                serchclick = 1;
            }
        }

        private void lstMenuItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var myItem = ((MenuItems)(sender as LongListSelector).SelectedItem);
            NavigationService.Navigate(new Uri(myItem.redirecturl, UriKind.RelativeOrAbsolute));
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            //var Result = MessageBox.Show("Are you sure you want to signout from this page?", "", MessageBoxButton.OKCancel);
            //if (Result == MessageBoxResult.OK)
            //{
            if (Utilities.CheckSignOut())
            {
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Views/Login/LoginPage.xaml", UriKind.RelativeOrAbsolute));
            }
            //}
        }

        # endregion

        // Product List Vertical dots
        private void imgEdit_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var _ProductDataContext = (ProductViewModel)(sender as Image).DataContext;

            popupEditProduct.VerticalOffset = 0;
            popupEditProduct.IsOpen = true;
            grdGrayBackground.Visibility = Visibility.Visible;
            _redirectMode = "Default";
            btnEdit.Click += (s, args) =>
            {
                //====================================================================================================================
                // PopUp Master List
                //====================================================================================================================

                popupEditProduct.IsOpen = false;
                grdGrayBackground.Visibility = Visibility.Collapsed;

                _redirectMode = "Edit";
                _ProductDataContext.mode = "Edit";

                if (ISOFile.FileExists("viewProductDetails"))
                {
                    ISOFile.DeleteFile("viewProductDetails");
                }
                using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewProductDetails", FileMode.Create))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(ProductViewModel));
                    serializer.WriteObject(fileStream, _ProductDataContext);

                    NavigationService.Navigate(new Uri("/Views/Product/ProductAddEditPage.xaml", UriKind.RelativeOrAbsolute));
                }
            };

            btncloseEditPopup.Tap += (s, args) =>
            {
                //====================================================================================================================
                // PopUp cancel Button
                //====================================================================================================================

                popupEditProduct.IsOpen = false;
                grdGrayBackground.Visibility = Visibility.Collapsed;
            };
        }
    }
}