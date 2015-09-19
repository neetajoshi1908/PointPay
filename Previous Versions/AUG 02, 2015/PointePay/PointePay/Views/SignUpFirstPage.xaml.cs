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

namespace PointePay.Views
{
    public partial class SignUpFirstPage : PhoneApplicationPage
    {
        public static string _isUsernameValid;

        public SignUpFirstPage()
        {
            InitializeComponent();
            _isUsernameValid = string.Empty;
        }

        private void txtFirstName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtFirstName.Background = new SolidColorBrush(Colors.Transparent);
            txtFirstName.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txtLastName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtLastName.Background = new SolidColorBrush(Colors.Transparent);
            txtLastName.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txtEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            txtEmail.Background = new SolidColorBrush(Colors.Transparent);
            txtEmail.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txtUserName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtUserName.Background = new SolidColorBrush(Colors.Transparent);
            txtUserName.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txtBusinessName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBusinessName.Background = new SolidColorBrush(Colors.Transparent);
            txtBusinessName.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txtBusinessPhone_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBusinessPhone.Background = new SolidColorBrush(Colors.Transparent);
            txtBusinessPhone.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }
        
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication(); 
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (Validation() == true)
            {
                // show Loader 
                myIndeterminateProbar.Visibility = Visibility.Visible;

                // Parameters
                RegistrationRequest obj = new RegistrationRequest();
                obj.firstName = txtFirstName.Text.Trim();
                obj.lastName = txtLastName.Text.Trim();
                obj.email = txtEmail.Text.Trim();
                obj.userName = txtUserName.Text.Trim();
                obj.organizationName = txtBusinessName.Text.Trim();
                obj.businessPhone = Convert.ToInt64(txtBusinessPhone.Text.Trim()); 
 
                // Write user details
                if (ISOFile.FileExists("SignUpFirstPageDetails"))
                {
                    ISOFile.DeleteFile("SignUpFirstPageDetails");
                }
                using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("SignUpFirstPageDetails", FileMode.Create))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(RegistrationRequest));
                    serializer.WriteObject(fileStream, obj);

                    // show Loader 
                    myIndeterminateProbar.Visibility = Visibility.Visible;
                    // Redirect to home page
                    NavigationService.Navigate(new Uri("/Views/SignUpSecondPage.xaml", UriKind.Relative));
                }
            }
        }

        private bool Validation()
        {
            bool isValid = true;
            //FirstNames Validation  
            if (String.IsNullOrWhiteSpace(txtFirstName.Text.Trim()))
            {
                MessageBox.Show("Enter First name");
                isValid = false;
            }
            //Last name   Validation  
            else if (String.IsNullOrWhiteSpace(txtLastName.Text.Trim()))
            {
                MessageBox.Show("Enter Last name");
                isValid = false;
            }
            //User name   Validation  
            else if (String.IsNullOrWhiteSpace(txtUserName.Text.Trim()))
            {
                MessageBox.Show("Enter Username");
                isValid = false;
            }
            //User name   Validation  
            else if (String.IsNullOrWhiteSpace(txtBusinessName.Text.Trim()))
            {
                MessageBox.Show("Enter business name");
                isValid = false;
            }
            //User name   Validation  
            else if (String.IsNullOrWhiteSpace(txtUserName.Text.Trim()))
            {
                MessageBox.Show("Enter valid phone number");
                isValid = false;
            }
            else if (_isUsernameValid != "Yes")
            {
                MessageBox.Show("Enter valid username");
                isValid = false;
            }
            return isValid;
        }

        private void txtUserName_LostFocus(object sender, RoutedEventArgs e)
        {
            //====================================================================================================================
            // check UserName Availability
            //====================================================================================================================

            if (!string.IsNullOrEmpty(txtUserName.Text.Trim()))
            {
                if (txtUserName.Text.Length < 3)
                {
                    MessageBox.Show("Enter valid username (min 3)");

                    Uri uri = new Uri("", UriKind.Relative);
                    BitmapImage imgSource = new BitmapImage(uri);
                    txtUserName.ActionIcon = imgSource;
                    _isUsernameValid = "No";
                }
                else
                {
                    // Show Loader 
                    myIndeterminateProbar.Visibility = Visibility.Visible;

                    String data = "userName=" + txtUserName.Text.Trim();
                    //Initialize WebClient
                    WebClient webClient = new WebClient();
                    //Set Header
                    webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                    webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                    webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
                    webClient.UploadStringAsync(new Uri("http://54.173.246.245/marketplace/api/auth/checkUserName/"), "POST", data);
                    //Assign Event Handler
                    webClient.UploadStringCompleted += wc_UploadcheckUserNameCompleted;
                }
            }
            else 
            {
                Uri uri = new Uri("", UriKind.Relative);
                BitmapImage imgSource = new BitmapImage(uri);
                txtUserName.ActionIcon = imgSource;
            }
        }

        void wc_UploadcheckUserNameCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Contains("The User Name field must contain a unique value."))
                {
                    Uri uri = new Uri("/Assets/Employee/User-red.png", UriKind.Relative);
                    BitmapImage imgSource = new BitmapImage(uri);
                    txtUserName.ActionIcon = imgSource;

                    MessageBox.Show("Username already exists.");
                    _isUsernameValid = "No";
                }
                else
                {
                    //Parse JSON result 
                    var rootObject = JsonConvert.DeserializeObject<RootObject_checkUserName>(e.Result);
                    if (rootObject.success == 0)
                    {
                        MessageBox.Show("Username already exists.");
                        _isUsernameValid = "No";
                    }
                    else if (rootObject.success == 1)
                    {
                        Uri uri = new Uri("/Assets/Employee/User-green.png", UriKind.Relative);
                        BitmapImage imgSource = new BitmapImage(uri);
                        txtUserName.ActionIcon = imgSource;

                        MessageBox.Show("Username available.");
                        _isUsernameValid = "Yes";
                    }
                    else
                    {
                        MessageBox.Show(rootObject.response.message.ToString());
                    }
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
    }
}