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
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (Validation() == true)
            {
                // show Loader 
                myIndeterminateProbar.Visibility = Visibility.Visible;

                //====================================================================================================================
                // Login Check
                //====================================================================================================================

                // Parameters
                LoginRequest obj = new LoginRequest();
                obj.email = txtUserName.Text.Trim(); // "djhs16";
                obj.password = txtPassWord.Password.Trim(); //"qaz";
                String data = "email=" + obj.email + "&password=" + obj.password;

                //Initialize WebClient
                WebClient webClient = new WebClient();
                //Set Header
                webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
                webClient.UploadStringAsync(new Uri(Utilities.GetURL("auth/signIn/")), "POST", data);
                //Assign Event Handler
                webClient.UploadStringCompleted += wc_UploadStringCompleted;
            }
        }

        void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                //Parse JSON result 
                var rootObject = JsonConvert.DeserializeObject<RootObject_Login>(e.Result);
                //Console.WriteLine(rootObject.response.data.userName);
                if (rootObject.success == 1)
                {
                    LoginViewModel obj = new LoginViewModel();
                    obj.employeeId = rootObject.response.data.employeeId;
                    obj.organizationId = rootObject.response.data.organizationId;
                    obj.country = rootObject.response.data.country;
                    obj.firstName = rootObject.response.data.firstName;
                    obj.lastName = rootObject.response.data.lastName;
                    obj.organizationName = rootObject.response.data.organizationName;

                    // Write user details
                    var Settings = IsolatedStorageSettings.ApplicationSettings;
                    Settings["islogin"] = "Yes";//write iso 

                    if (ISOFile.FileExists("CurrentLoginUserDetails"))
                    {
                        ISOFile.DeleteFile("CurrentLoginUserDetails");
                    }
                    using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("CurrentLoginUserDetails", FileMode.Create))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(LoginViewModel));
                        serializer.WriteObject(fileStream, obj);
                    }

                    // show Loader 
                    myIndeterminateProbar.Visibility = Visibility.Collapsed;
                    // Redirect to home page
                    NavigationService.Navigate(new Uri("/Views/Home/HomePage.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show(rootObject.response.message.ToString());
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

        private void txtUserName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtUserName.Background = new SolidColorBrush(Colors.Transparent);
            txtUserName.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void btnLogin_GotFocus(object sender, RoutedEventArgs e)
        {
            btnLogin.Background = new SolidColorBrush(Colors.Gray);
        }

        private bool Validation()
        {
            bool isValid = true;
            //UserName Validation  
            if (String.IsNullOrWhiteSpace(txtUserName.Text.Trim()))
            {
                MessageBox.Show("Please enter your UserName!");
                isValid = false;
            }
            //Password length Validation  
            else if (String.IsNullOrWhiteSpace(txtPassWord.Password.Trim()))
            {
                MessageBox.Show("Please enter your PassWord!");
                isValid = false;
            }
            return isValid;
        }

        private void PasswordLostFocus(object sender, RoutedEventArgs e)
        {
            CheckPasswordWatermark();
        }

        public void CheckPasswordWatermark()
        {
            var passwordEmpty = string.IsNullOrEmpty(txtPassWord.Password);
            PasswordWatermark.Opacity = passwordEmpty ? 100 : 0;
            txtPassWord.Opacity = passwordEmpty ? 0 : 100;
        }

        private void PasswordGotFocus(object sender, RoutedEventArgs e)
        {
            PasswordWatermark.Opacity = 0;
            txtPassWord.Opacity = 100;

            txtPassWord.Background = new SolidColorBrush(Colors.Transparent);
            txtPassWord.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }
    }
}