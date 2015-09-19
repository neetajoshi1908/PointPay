using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using PointePayApp.Common;
using PointePayApp.Model;
using PointePayApp.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

namespace PointePayApp.Views
{
    public partial class SubCategoryAddEditPage : PhoneApplicationPage
    {
        public List<SubCategoryOfflineViewModel> ListCategoryData { get; set; }
        public static int _organizationId; // Logged In user's organizationId
        public static int _categoryId;
        public static int _ParentCategoryID;
        public static string _mode;

        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        public SubCategoryAddEditPage()
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
                            _organizationId = Convert.ToInt32(ObjUserData.organizationId);
                        }
                    }

                    if (ISOFile.FileExists("viewSubCategoryDetails"))//read current user login details    
                    {
                        using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewSubCategoryDetails", FileMode.Open))
                        {
                            DataContractSerializer serializer = new DataContractSerializer(typeof(SubCategoryOfflineViewModel));
                            var ObjCategoryData = (SubCategoryOfflineViewModel)serializer.ReadObject(fileStream);
                            txtsubcategoryName.Text = Convert.ToString(ObjCategoryData.categoryCode);
                            txtsubcategoryDescription.Text = Convert.ToString(ObjCategoryData.categoryDescription);
                            _categoryId = Convert.ToInt32(ObjCategoryData.categoryId);
                            _ParentCategoryID = Convert.ToInt32(ObjCategoryData.parentCategoryId);
                            _mode = ObjCategoryData.mode;
                        }
                    }

                    //====================================================================================================================
                    // Fill Category Dropdown Default
                    //====================================================================================================================
                  
                    // Show Loader 
                    myIndeterminateProbar.Visibility = Visibility.Visible;

                    SubCategoryRequest obj = new SubCategoryRequest();
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
                        ListCategoryData.Add(new SubCategoryOfflineViewModel { categoryId = "0", categoryCode = "-- Select State --" });
                        foreach (var itm in rootObject.response.data)
                        {
                            ListCategoryData.Add(new SubCategoryOfflineViewModel { categoryId = itm.categoryId, categoryCode = itm.categoryCode });
                        };
                        this.listPickerCategory.ItemsSource = ListCategoryData;

                        if (_mode == "Add")
                        {
                            this.listPickerCategory.SelectedIndex = 0;
                        }
                        if (_mode == "Edit")
                        {
                            var item = ListCategoryData.Where(x => x.categoryId == _ParentCategoryID.ToString()).FirstOrDefault();
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

        private void ImgBack_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Category/CategoryListPage.xaml", UriKind.Relative));
        }

        private void txtsubcategoryName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtsubcategoryName.Background = new SolidColorBrush(Colors.Transparent);
            txtsubcategoryName.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txtsubcategoryDescription_GotFocus(object sender, RoutedEventArgs e)
        {
            txtsubcategoryDescription.Background = new SolidColorBrush(Colors.Transparent);
            txtsubcategoryDescription.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            // Parameters
            SubCategoryRequest obj = new SubCategoryRequest();
            obj.organizationId = _organizationId; // Logged In organizationId  
            obj.categoryCode = txtsubcategoryName.Text;
            obj.categoryDescription = txtsubcategoryDescription.Text;

            ListPickerItem selectedItemState = this.listPickerCategory.ItemContainerGenerator.ContainerFromItem(this.listPickerCategory.SelectedItem) as ListPickerItem;
            SubCategoryOfflineViewModel SelecteddataCategory = selectedItemState.DataContext as SubCategoryOfflineViewModel;
            obj.parentCategoryId = Convert.ToInt32(SelecteddataCategory.categoryId);

            String data = string.Empty;

            if (Validation() == true)
            {
                // Show Loader 
                myIndeterminateProbar.Visibility = Visibility.Visible;

                //Initialize WebClient
                WebClient webClient = new WebClient();

                //Set Header
                webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";

                if (_mode == "Add")
                {
                    data = "organizationId=" + obj.organizationId + "&categoryCode=" + obj.categoryCode + "&categoryDescription=" + obj.categoryDescription + "&parentCategoryId=" + obj.parentCategoryId;
                    webClient.UploadStringAsync(new Uri(Utilities.GetURL("category/addCategory/")), "POST", data);
                }
                if (_mode == "Edit")
                {
                    obj.categoryId = _categoryId;

                    data = "organizationId=" + obj.organizationId + "&categoryId=" + obj.categoryId + "&categoryCode=" + obj.categoryCode + "&categoryDescription=" + obj.categoryDescription + "&parentCategoryId=" + obj.parentCategoryId;
                    webClient.UploadStringAsync(new Uri(Utilities.GetURL("category/updateCategory/")), "POST", data);
                }

                //Assign Event Handler
                webClient.UploadStringCompleted += wc_UploadSaveCompleted;
            }
        }

        void wc_UploadSaveCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                //Parse JSON result 
                var rootObject = JsonConvert.DeserializeObject<RootObject_SubCategoryAddEdit>(e.Result);
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

            //subcategoryName Validation  
            if (String.IsNullOrWhiteSpace(txtsubcategoryName.Text.Trim()))
            {
                MessageBox.Show("Enter name!");
                isValid = false;
            }

            ListPickerItem selectedItemState = this.listPickerCategory.ItemContainerGenerator.ContainerFromItem(this.listPickerCategory.SelectedItem) as ListPickerItem;
            SubCategoryOfflineViewModel SelecteddataCategory = selectedItemState.DataContext as SubCategoryOfflineViewModel;

            if (Convert.ToInt32(SelecteddataCategory.categoryId)<=0)
            {
                MessageBox.Show("Select category!");
                isValid = false;
            }

            return isValid;
        }
    }
}