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
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PointePay.Model;
using PointePay.Common;
using System.Text.RegularExpressions;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;

namespace PointePay
{
    public partial class Login : PhoneApplicationPage
    {
        public Login()
        {
            InitializeComponent();
        }
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication(); 
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (Validation()==true)
            {
                // show Loader 
                myIndeterminateProbar.Visibility = Visibility.Visible;

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

                webClient.UploadStringAsync(new Uri("http://54.173.246.245/marketplace/api/auth/signIn/"), "POST", data);

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
                    NavigationService.Navigate(new Uri("/Views/HomePage.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show(rootObject.response.message.ToString());
                }
            }
            catch (Exception ex)
            {
                WebException we = (WebException)e.Error;
                HttpWebResponse response = (System.Net.HttpWebResponse)we.Response;
                if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized)
                { MessageBox.Show("Invalid Username and Password."); }
                else
                { MessageBox.Show("Invalid Username and Password."); }
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

