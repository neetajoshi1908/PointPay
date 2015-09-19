using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using PointePayApp.Common;
using PointePayApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace PointePayApp.Views
{
    public partial class SignUpThirdPage : PhoneApplicationPage
    {
        public static int _employeeId; // Logged In user's employeeId
        public static int _organizationId; // Logged In user's organizationId

        public SignUpThirdPage()
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
                            txtBusinessPhone.Text="+"+Convert.ToString(ObjUserData.businessPhoneCode.Trim())+" "+Convert.ToString(ObjUserData.businessPhone.Trim());
                            _employeeId = Convert.ToInt32(ObjUserData.employeeId);
                            _organizationId = Convert.ToInt32(ObjUserData.organizationId);

                            String data = "employeeId=" + _employeeId + "&businessPhoneCode=" + Convert.ToString(ObjUserData.businessPhoneCode.Trim()) + "&businessPhone=" + Convert.ToString(ObjUserData.businessPhone.Trim());

                            //====================================================================================================================
                            // Send Verification Code
                            //====================================================================================================================

                            //Initialize WebClient
                            WebClient webClient = new WebClient();
                            //Set Header
                            webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                            webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                            webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
                            webClient.UploadStringAsync(new Uri(Utilities.GetURL("verification/send_verification/")), "POST", data);
                            //Assign Event Handler
                            webClient.UploadStringCompleted += wc_UploadStringCompleted;

                        }
                    }
                }
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/Login/LoginPage.xaml", UriKind.RelativeOrAbsolute));
            } 
        }

        void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                //Parse JSON result 
                var results = JsonConvert.DeserializeObject<dynamic>(e.Result);
                if (results.success == 1)
                {
                    var verificationPref = results.response.data.verificationPref;
                }
                if (results.success == 2)
                {
                    MessageBox.Show(results.response.message.businessPhone.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something wrong happened.");
            }
            finally
            {
 
            }
        }

        private void btnVerify_Click(object sender, RoutedEventArgs e)
        {
            var _verificationPref = verificationPref1.Text.Trim() + verificationPref2.Text.Trim() + verificationPref3.Text.Trim() + verificationPref4.Text.Trim() + verificationPref5.Text.Trim();
            String data = "oraganizationId=" + _organizationId + "&verificationPref=" + Convert.ToString(_verificationPref.Trim());

            //====================================================================================================================
            // Verify the Code
            //====================================================================================================================

            //Initialize WebClient
            WebClient webClient = new WebClient();
            //Set Header
            webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
            webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
            webClient.UploadStringAsync(new Uri(Utilities.GetURL("verification/verifyUser/")), "POST", data);
            //Assign Event Handler
            webClient.UploadStringCompleted += wc_UploadVerifyCompleted;
        }

        void wc_UploadVerifyCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                //Parse JSON result 
                var results = JsonConvert.DeserializeObject<dynamic>(e.Result);
                if (results.success == 1)
                {
                    NavigationService.Navigate(new Uri("/Views/Home/HomePage.xaml", UriKind.RelativeOrAbsolute));
                }
                if (results.success == 0)
                {
                    MessageBox.Show(results.response.message.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something wrong happened.");
            }
            finally
            {

            }
        }

        private void btnResendCode_Click(object sender, RoutedEventArgs e)
        {
            
        } 
    }
}