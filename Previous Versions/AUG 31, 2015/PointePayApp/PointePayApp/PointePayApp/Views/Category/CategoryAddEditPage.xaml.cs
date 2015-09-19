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
using PointePayApp.Model;
using System.IO;
using PointePayApp.Common;
using Newtonsoft.Json;
using PointePayApp.ViewModel;
using PointePayApp.Provider;

namespace PointePayApp.Views
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

                    if (ISOFile.FileExists("viewCategoryDetails"))//read current user login details    
                    {
                        using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewCategoryDetails", FileMode.Open))
                        {
                            DataContractSerializer serializer = new DataContractSerializer(typeof(CategoryOfflineViewModel));
                            var ObjCategoryData = (CategoryOfflineViewModel)serializer.ReadObject(fileStream);
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
                NavigationService.Navigate(new Uri("/Views/Login/LoginPage.xaml", UriKind.RelativeOrAbsolute));
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

            if (Utilities.CheckInternetConnection())
            {
                // ----------------------------------------------------------------------
                // "Network Status: Connected."

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
            else
            {
                // ----------------------------------------------------------------------
                //  "Network Status: Not Connected."

                MessageBoxResult objMessageBox = MessageBox.Show("Right now you are in offline mode. data save and will be sent to the server the next time you are online.", "Network Status !", MessageBoxButton.OKCancel);
                if (objMessageBox == MessageBoxResult.OK)
                {
                    try
                    {
                        if (Validation() == true)
                        {
                            if (_mode == "Add")
                            {
                                MessageBox.Show("You can not create a new category in offline mode.");
                            }

                            if (_mode == "Edit")
                            {
                                CategoryDataProvider _CategoryDataProvider = new CategoryDataProvider();
                                var result = _CategoryDataProvider.UpdateCategoryOffline(obj, "False");
                                if (result == true)
                                {
                                    MessageBox.Show("successfully Updated.");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Something wrong happened.");
                    }
                }
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
            NavigationService.Navigate(new Uri("/Views/Category/CategoryListPage.xaml", UriKind.Relative));
        }
    }
}