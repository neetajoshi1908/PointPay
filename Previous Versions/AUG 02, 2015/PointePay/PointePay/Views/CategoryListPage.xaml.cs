using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using PointePay.Common;
using PointePay.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PointePay.Views
{
    public partial class CategoryListPage : PhoneApplicationPage
    {
        public List<CategoryViewModel> ListCategoryData { get; set; }
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        public static int _employeeId; // Logged In user's employeeId
        public static int _organizationId; // Logged In user's organizationId
        public static string _redirectMode;
        int serchclick = 1;

        public CategoryListPage()
        {
            InitializeComponent();

            IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
            if (IsolatedStorageSettings.ApplicationSettings.Contains("islogin"))
            {
                if (!(Convert.ToString(IsolatedStorageSettings.ApplicationSettings["islogin"]).ToLower() == "yes"))
                {
                    NavigationService.Navigate(new Uri("/Views/LoginPage.xaml", UriKind.RelativeOrAbsolute));
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
                            lstMenuItems.ItemsSource = Utilities.GetMenuItems(MenuCode.CatMngt);

                            // Show Loader 
                            myIndeterminateProbar.Visibility = Visibility.Visible;

                            //====================================================================================================================
                            // Fill Category List
                            //====================================================================================================================
                            // Parameters
                            CategoryRequest obj = new CategoryRequest();
                            obj.organizationId = _organizationId;
                            obj.set = 1;
                            obj.count = 5;
                            String data = "organizationId=" + obj.organizationId + "&set=" + obj.set + "&count=" + obj.count;

                            //Initialize WebClient
                            WebClient webClient = new WebClient();
                            //Set Header
                            webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                            webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                            webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
                            webClient.UploadStringAsync(new Uri(Utilities.GetURL("category/categoryListing/")), "POST", data);
                            //Assign Event Handler0
                            webClient.UploadStringCompleted += wc_UploadStringCompleted;
                        }
                    }
                }
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/LoginPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Contains("no Category found"))
                {
                    ListCategoryData = new List<CategoryViewModel>();
                    this.lstCateoryItems.ItemsSource = ListCategoryData;
                }
                else
                {
                    //Parse JSON result 
                    var rootObject = JsonConvert.DeserializeObject<RootObject_Category>(e.Result);
                    if (rootObject.success == 1)
                    {
                        ListCategoryData = new List<CategoryViewModel>();
                        foreach (var itm in rootObject.response.data)
                        {
                            ListCategoryData.Add(new CategoryViewModel { categoryId = itm.categoryId, organizationId = itm.organizationId, categoryCode = itm.categoryCode, categoryDescription = itm.categoryDescription, imageName = itm.imageName, imagePath = itm.imagePath, active = itm.active, parentCategoryId = itm.parentCategoryId, createDt = itm.createDt, lastModifiedDt = itm.lastModifiedDt, lastModifiedBy = itm.lastModifiedBy });
                        };
                        this.lstCateoryItems.ItemsSource = ListCategoryData;

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

            }
            finally
            {
                // hide Loader 
                myIndeterminateProbar.Visibility = Visibility.Collapsed;
            }
        }//wc_DownloadStringCompleted

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
            var Result = MessageBox.Show("Are you sure you want to signout from this page?", "", MessageBoxButton.OKCancel);
            if (Result == MessageBoxResult.OK)
            {
                var Settings = IsolatedStorageSettings.ApplicationSettings;
                Settings.Remove("CurrentLoginUserDetails");
                Settings.Remove("islogin");
                Settings.Remove("viewEmployeeDetails");
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Views/LoginPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }
        # endregion

        private void lstCateoryItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //====================================================================================================================
            // View Category Details
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
                var ListItem = ((CategoryViewModel)(sender as LongListSelector).SelectedItem);

                if (ISOFile.FileExists("viewCategoryDetails"))
                {
                    ISOFile.DeleteFile("viewCategoryDetails");
                }
                using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewCategoryDetails", FileMode.Create))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(CategoryViewModel));
                    serializer.WriteObject(fileStream, ListItem);

                    NavigationService.Navigate(new Uri("/Views/CategoryAddEditPage.xaml", UriKind.RelativeOrAbsolute));
                }
            }
        }

        private void ImgEdit_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var _CategoryDataContext = (CategoryViewModel)(sender as Image).DataContext;

            popup.VerticalOffset = 0;
            popup.IsOpen = true;
            _redirectMode = "Default";
            btnEdit.Click += (s, args) =>
            {
                //====================================================================================================================
                // PopUp Edit button
                //====================================================================================================================

                popup.IsOpen = false;
                // Set page mode for Edit record of Category 
                _redirectMode = "Edit";
                _CategoryDataContext.mode = "Edit";

                if (ISOFile.FileExists("viewCategoryDetails"))
                {
                    ISOFile.DeleteFile("viewCategoryDetails");
                }
                using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewCategoryDetails", FileMode.Create))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(CategoryViewModel));
                    serializer.WriteObject(fileStream, _CategoryDataContext);

                    NavigationService.Navigate(new Uri("/Views/CategoryAddEditPage.xaml", UriKind.RelativeOrAbsolute));
                }

            };

            btnclose.Tap += (s, args) =>
            {
                //====================================================================================================================
                // PopUp cancel Button
                //====================================================================================================================

                popup.IsOpen = false;
            };
        }

        private void btnSubCat_Click(object sender, RoutedEventArgs e)
        {
            // Show Loader 
            myIndeterminateProbar.Visibility = Visibility.Visible;
            // Go to Subcategory tab
            NavigationService.Navigate(new Uri("/Views/SubCategoryListPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ImgAddCateory_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //====================================================================================================================
            // Add new Cateory
            //====================================================================================================================

            CategoryViewModel _CategoryDataContext = new CategoryViewModel();
            // Set page mode for Add record of category
            _redirectMode = "Add";
            _CategoryDataContext.mode = "Add";
            if (ISOFile.FileExists("viewCategoryDetails"))
            {
                ISOFile.DeleteFile("viewCategoryDetails");
            }
            using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewCategoryDetails", FileMode.Create))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(CategoryViewModel));
                serializer.WriteObject(fileStream, _CategoryDataContext);

                NavigationService.Navigate(new Uri("/Views/CategoryAddEditPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }
    }
}