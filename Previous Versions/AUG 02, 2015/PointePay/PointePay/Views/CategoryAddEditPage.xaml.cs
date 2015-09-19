using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using PointePay.Model;
using System.IO;
using PointePay.Common;
using Newtonsoft.Json;

namespace PointePay.Views
{
    public partial class CategoryAddEditPage : PhoneApplicationPage
    {
        public static int _organizationId; // Logged In user's organizationId
        public static int _categoryId;
        public static string _mode;

        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        public CategoryAddEditPage()
        {
            InitializeComponent();

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
                            _organizationId = Convert.ToInt32(ObjUserData.organizationId);
                        }
                    }

                    if (ISOFile.FileExists("viewCategoryDetails"))//read current user login details    
                    {
                        using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewCategoryDetails", FileMode.Open))
                        {
                            DataContractSerializer serializer = new DataContractSerializer(typeof(CategoryViewModel));
                            var ObjCategoryData = (CategoryViewModel)serializer.ReadObject(fileStream);
                            txtcategoryName.Text = Convert.ToString(ObjCategoryData.categoryCode);
                            txtcategoryDescription.Text = Convert.ToString(ObjCategoryData.categoryDescription);
                            _categoryId =Convert.ToInt32(ObjCategoryData.categoryId);
                            _mode = ObjCategoryData.mode;
                        }
                    }
                }
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/LoginPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        private void txtcategoryName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtcategoryName.Background = new SolidColorBrush(Colors.Transparent);
            txtcategoryName.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txtcategoryDescription_GotFocus(object sender, RoutedEventArgs e)
        {
            txtcategoryDescription.Background = new SolidColorBrush(Colors.Transparent);
            txtcategoryDescription.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Show Loader 
            myIndeterminateProbar.Visibility = Visibility.Visible;

            // Parameters
            CategoryRequest obj = new CategoryRequest();
            obj.organizationId = _organizationId; // Logged In organizationId  
            obj.categoryCode = txtcategoryName.Text;
            obj.categoryDescription = txtcategoryDescription.Text;
            obj.parentCategoryId = 0; // 0 for category and category id for subcategory

            String data = string.Empty;

            if (Validation() == true)
            {
                //Initialize WebClient
                WebClient webClient = new WebClient();

                //Set Header
                webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";

                if (_mode == "Add")
                {
                    data = "organizationId=" + obj.organizationId + "&categoryCode=" + obj.categoryCode + "&categoryDescription=" + obj.categoryDescription + "&parentCategoryId=" + obj.parentCategoryId ;
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
                var rootObject = JsonConvert.DeserializeObject<RootObject_CategoryAddEdit>(e.Result);
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

            //categoryName Validation  
            if (String.IsNullOrWhiteSpace(txtcategoryName.Text.Trim()))
            {
                MessageBox.Show("Enter name!");
                isValid = false;
            }

            return isValid;
        }

        private void ImgBack_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/CategoryListPage.xaml", UriKind.Relative));
        }
    }
}