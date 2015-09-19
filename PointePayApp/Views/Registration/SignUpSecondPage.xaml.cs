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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace PointePayApp.Views
{
    public partial class SignUpSecondPage : PhoneApplicationPage
    {
        public List<data_State> ListPickerStateData { get; set; }
        public List<data_Area> ListPickerAreaData { get; set; }
        public List<data_City> ListPickerCityData { get; set; }
        public static int _countryId=154;
        public static string _businessPhoneCode = "+234";
        public SignUpSecondPage()
        {
            InitializeComponent();

            //Show Loader
            myIndeterminateProbar.Visibility = Visibility.Visible;

            //====================================================================================================================
            // Registration state dropdown
            //====================================================================================================================

            // Parameters
            StateRequest obj = new StateRequest();
            obj.country_id = _countryId;
            String data = "country_id=" + obj.country_id;

            //Initialize WebClient
            WebClient webClient = new WebClient();
            //Set Header
            webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
            webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
            webClient.UploadStringAsync(new Uri(Utilities.GetURL("location/getState/")), "POST", data);
            //Assign Event Handler
            webClient.UploadStringCompleted += wc_UploadStateCompleted;
        }

        void wc_UploadStateCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                //Parse JSON result 
                var rootObject = JsonConvert.DeserializeObject<RootObject_State>(e.Result);
                if (rootObject.success == 1)
                {
                    ListPickerStateData = new List<data_State>();
                    ListPickerStateData.Add(new data_State { countryId = "0", stateId = "0", stateName = "-- Select State --" });
                    foreach (var itm in rootObject.response.data)
                    {
                        ListPickerStateData.Add(new data_State { countryId = itm.countryId, stateId = itm.stateId, stateName = itm.stateName });
                    };
                    this.listPickerState.ItemsSource = ListPickerStateData;
                    this.listPickerState.SelectedIndex = 0;

                    // Default Area binding
                    ListPickerAreaData = new List<data_Area>();
                    ListPickerAreaData.Add(new data_Area { stateId = "0", areaId = "0", area = "-- Select Area --" });
                    this.listPickerArea.ItemsSource = ListPickerAreaData;
                    this.listPickerArea.SelectedIndex = 0;
                    
                    // Default city binding
                    ListPickerCityData = new List<data_City>();
                    ListPickerCityData.Add(new data_City { zipId = "0", countryId = "0", stateId = "0", city = "-- Select City --" });
                    this.listPickerCity.ItemsSource = ListPickerCityData;
                    this.listPickerCity.SelectedIndex = 0;

                    //Hide Loader
                    myIndeterminateProbar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //Hide Loader
                    myIndeterminateProbar.Visibility = Visibility.Collapsed;
                    MessageBox.Show(rootObject.response.message.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something wrong happened.");  
            }
            finally
            {
                //Hide Loader
                myIndeterminateProbar.Visibility = Visibility.Collapsed;
            }
        }//wc_DownloadStringCompleted

        private void listPickerState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Show loader 
            myIndeterminateProbar.Visibility = Visibility.Visible;

            ListPickerItem selectedItem = this.listPickerState.ItemContainerGenerator.ContainerFromItem(this.listPickerState.SelectedItem) as ListPickerItem;
            if (selectedItem != null)
            {  
                data_State Selecteddata = selectedItem.DataContext as data_State;
                if (Selecteddata.stateId == "0")
                {
                    // Default Area binding
                    ListPickerAreaData = new List<data_Area>();
                    ListPickerAreaData.Add(new data_Area { stateId = "0", areaId = "0", area = "-- Select Area --" });
                    this.listPickerArea.ItemsSource = ListPickerAreaData;
                    this.listPickerArea.SelectedIndex = 0;

                    // Default city binding
                    ListPickerCityData = new List<data_City>();
                    ListPickerCityData.Add(new data_City { zipId = "0", countryId = "0", stateId = "0", city = "-- Select City --" });
                    this.listPickerCity.ItemsSource = ListPickerCityData;
                    this.listPickerCity.SelectedIndex = 0;

                    //Hide Loader
                    myIndeterminateProbar.Visibility = Visibility.Collapsed;
                }
                else 
                {

                    //====================================================================================================================
                    // Registration Area dropdown
                    //====================================================================================================================

                    // Parameters
                    AreaRequest obj = new AreaRequest();
                    obj.country_id = Convert.ToInt32(Selecteddata.countryId);
                    obj.state_id = Convert.ToInt32(Selecteddata.stateId);
                    String data = "country_id=" + obj.country_id + "&state_id=" + obj.state_id;

                    //Initialize WebClient
                    WebClient webClient = new WebClient();
                    //Set Header
                    webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                    webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                    webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
                    webClient.UploadStringAsync(new Uri(Utilities.GetURL("location/getArea/")), "POST", data);
                    //Assign Event Handler
                    webClient.UploadStringCompleted += wc_UploadAresCompleted;
                }
            }
        }

        void wc_UploadAresCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                //Parse JSON result 
                var rootObject = JsonConvert.DeserializeObject<RootObject_Area>(e.Result);
                if (rootObject.success == 1)
                {
                    ListPickerAreaData = new List<data_Area>();
                    ListPickerAreaData.Add(new data_Area { stateId = "0", areaId = "0", area = "-- Select Area --" });
                    foreach (var itm in rootObject.response.data)
                    {
                        ListPickerAreaData.Add(new data_Area { stateId = itm.stateId, areaId = itm.areaId, area = itm.area });
                    };
                    this.listPickerArea.ItemsSource = ListPickerAreaData;
                    this.listPickerArea.SelectedIndex = 0;

                    //Hide Loader
                    myIndeterminateProbar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //Hide Loader
                    myIndeterminateProbar.Visibility = Visibility.Collapsed;
                    MessageBox.Show(rootObject.response.message.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something wrong happened.");  
            }
            finally
            {
                //Hide Loader
                myIndeterminateProbar.Visibility = Visibility.Collapsed;
            }
        }

        private void listPickerArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            myIndeterminateProbar.Visibility = Visibility.Visible;

            ListPickerItem selectedItem = this.listPickerArea.ItemContainerGenerator.ContainerFromItem(this.listPickerArea.SelectedItem) as ListPickerItem;
            if (selectedItem != null)
            {
                data_Area Selecteddata = selectedItem.DataContext as data_Area;
                if (Selecteddata.areaId == "0")
                {
                    ListPickerCityData = new List<data_City>();
                    ListPickerCityData.Add(new data_City { zipId = "0", countryId = "0", stateId = "0", city = "-- Select City --" });
                    this.listPickerCity.ItemsSource = ListPickerCityData;
                    this.listPickerCity.SelectedIndex = 0;

                    //Hide Loader
                    myIndeterminateProbar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //====================================================================================================================
                    // Registration City dropdown
                    //====================================================================================================================

                    // Parameters
                    CityRequest obj = new CityRequest();
                    obj.country_id = Convert.ToInt32(Selecteddata.countryId);
                    obj.state_id = Convert.ToInt32(Selecteddata.stateId);
                    obj.area_id = Convert.ToInt32(Selecteddata.areaId);
                    String data = "country_id=" + obj.country_id + "&state_id=" + obj.state_id + "&area_id=" + obj.area_id;

                    //Initialize WebClient
                    WebClient webClient = new WebClient();
                    //Set Header
                    webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                    webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                    webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
                    webClient.UploadStringAsync(new Uri(Utilities.GetURL("location/getCity/")), "POST", data);
                    //Assign Event Handler
                    webClient.UploadStringCompleted += wc_UploadCityCompleted;
                }
            }
        }

        void wc_UploadCityCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                //Parse JSON result 
                var rootObject = JsonConvert.DeserializeObject<RootObject_City>(e.Result);
                if (rootObject.success == 1)
                {
                    ListPickerCityData = new List<data_City>();
                    ListPickerCityData.Add(new data_City { zipId = "0", countryId = "0", stateId = "0", city = "-- Select City --" });
                    foreach (var itm in rootObject.response.data)
                    {
                        ListPickerCityData.Add(new data_City { zipId = itm.zipId, countryId = itm.countryId, stateId=itm.stateId, city = itm.city });
                    };
                    this.listPickerCity.ItemsSource = ListPickerCityData;
                    this.listPickerCity.SelectedIndex = 0;

                    //Hide Loader
                    myIndeterminateProbar.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //Hide Loader
                    myIndeterminateProbar.Visibility = Visibility.Collapsed;
                    MessageBox.Show(rootObject.response.message.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something wrong happened.");  
            }
            finally
            {
                //Hide Loader
                myIndeterminateProbar.Visibility = Visibility.Collapsed;
            }
        }

        private void txtStreet_GotFocus(object sender, RoutedEventArgs e)
        {
            txtStreet.Background = new SolidColorBrush(Colors.Transparent);
            txtStreet.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txtConfirmPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            txtConfirmPassword.Background = new SolidColorBrush(Colors.Transparent);
            txtConfirmPassword.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }
        
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication(); 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ISOFile.FileExists("SignUpFirstPageDetails"))//read current SignUp First Page Details    
            {
                using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("SignUpFirstPageDetails", FileMode.Open))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(RegistrationRequest));
                    var ObjSignUpFirstPageData = (RegistrationRequest)serializer.ReadObject(fileStream);

                    //====================================================================================================================
                    // Read and Set first registration form details 
                    //====================================================================================================================

                    // Parameters
                    RegistrationRequest obj = new RegistrationRequest();
                    //First Screen
                    obj.firstName = ObjSignUpFirstPageData.firstName;
                    obj.lastName = ObjSignUpFirstPageData.lastName;
                    obj.email = ObjSignUpFirstPageData.email;
                    obj.userName = ObjSignUpFirstPageData.userName;
                    obj.organizationName = ObjSignUpFirstPageData.organizationName;
                    obj.businessPhone = ObjSignUpFirstPageData.businessPhone;
                    
                    // clear session info
                    var Settings = IsolatedStorageSettings.ApplicationSettings;
                    Settings.Remove("SignUpFirstPageDetails");

                    //Second Screen
                    ListPickerItem selectedItemState = this.listPickerState.ItemContainerGenerator.ContainerFromItem(this.listPickerState.SelectedItem) as ListPickerItem;
                    data_State SelecteddataState = selectedItemState.DataContext as data_State;
                    obj.state = Convert.ToInt32(SelecteddataState.stateId);
                    
                    ListPickerItem selectedItemArea = this.listPickerArea.ItemContainerGenerator.ContainerFromItem(this.listPickerArea.SelectedItem) as ListPickerItem;
                    data_Area SelecteddataArea = selectedItemArea.DataContext as data_Area;
                    obj.area = Convert.ToInt32(SelecteddataArea.areaId);

                    ListPickerItem selectedItemCity = this.listPickerCity.ItemContainerGenerator.ContainerFromItem(this.listPickerCity.SelectedItem) as ListPickerItem;
                    data_City SelecteddatCity = selectedItemCity.DataContext as data_City;
                    obj.city = Convert.ToInt32(SelecteddatCity.zipId); // here zipId is cityID	

                    obj.addressLine1 = txtStreet.Text.Trim();
                    obj.password = txtPassword.Password.Trim();
                    obj.isPointeMart = (chkRequest.IsChecked == true) ? 1 : 0;
                    obj.isPointePay = 1; // Always 1 since app itself is PointePay		
                    obj.country = _countryId; // id of country (nigeria only)	
                    obj.businessPhoneCode = _businessPhoneCode; //must add + before code e.g. +234	

                    String data = "firstName=" + obj.firstName + "&lastName=" + obj.lastName + "&email=" + obj.email + "&userName=" + obj.userName + "&organizationName=" + obj.organizationName + "&businessPhone=" + obj.businessPhone + "&state=" + obj.state + "&area=" + obj.area + "&city=" + obj.city + "&addressLine1=" + obj.addressLine1 + "&password=" + obj.password + "&isPointeMart=" + obj.isPointeMart + "&isPointePay=" + obj.isPointePay + "&country=" + obj.country + "&businessPhoneCode=" + obj.businessPhoneCode;

                    if (Validation() == true)
                    {
                        // Show Loader 
                        myIndeterminateProbar.Visibility = Visibility.Visible; 

                        //====================================================================================================================
                        // Submit Registration Form
                        //====================================================================================================================

                        //Initialize WebClient
                        WebClient webClient = new WebClient();
                        //Set Header
                        webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                        webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                        webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
                        webClient.UploadStringAsync(new Uri(Utilities.GetURL("auth/signUp/")), "POST", data);
                        //Assign Event Handler
                        webClient.UploadStringCompleted += wc_UploadStringCompleted;
                    }
                }
            }
        }

        void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                //Parse JSON result 
                //var rootObject = JsonConvert.DeserializeObject<RootObject_Registration>(e.Result);
                var rootObject = JsonConvert.DeserializeObject<dynamic>(e.Result);
                if (rootObject.success == 1)
                {
                    LoginViewModel obj = new LoginViewModel();
                    obj.employeeId = rootObject.response.data.employeeId;
                    obj.organizationId = rootObject.response.data.organizationId;
                    obj.country = rootObject.response.data.country;
                    obj.firstName = Convert.ToString(rootObject.response.data.firstName);
                    obj.lastName = Convert.ToString(rootObject.response.data.lastName);
                    obj.organizationName = Convert.ToString(rootObject.response.data.organizationName);
                    obj.businessPhoneCode = Convert.ToString(rootObject.response.data.businessPhoneCode);
                    obj.businessPhone = Convert.ToString(rootObject.response.data.businessPhone);

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

                    // hide Loader 
                    myIndeterminateProbar.Visibility = Visibility.Collapsed;
                    // Redirect to home page
                    NavigationService.Navigate(new Uri("/Views/Registration/SignUpThirdPage.xaml", UriKind.Relative));
                }
                if (rootObject.success == 0)
                {
                    string  msgstr=rootObject.response.message.ToString();
                    string[] words = msgstr.Split(':');
                    string msg =Convert.ToString(words[1].Replace("}",""));
                    msg = Regex.Replace(msg, @"[\""]", "", RegexOptions.None); 
                    MessageBox.Show(msg);
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

            ListPickerItem selectedItemState = this.listPickerState.ItemContainerGenerator.ContainerFromItem(this.listPickerState.SelectedItem) as ListPickerItem;
            data_State SelecteddataState = selectedItemState.DataContext as data_State;

            ListPickerItem selectedItemArea = this.listPickerArea.ItemContainerGenerator.ContainerFromItem(this.listPickerArea.SelectedItem) as ListPickerItem;
            data_Area SelecteddataArea = selectedItemArea.DataContext as data_Area;

            ListPickerItem selectedItemCity = this.listPickerCity.ItemContainerGenerator.ContainerFromItem(this.listPickerCity.SelectedItem) as ListPickerItem;
            data_City SelecteddatCity = selectedItemCity.DataContext as data_City;

            //stateId Validation  
            if (SelecteddataState.stateId=="0")
            {
                MessageBox.Show("Select State");
                isValid = false;
            }
            //areaId Validation  
            else if (SelecteddataArea.areaId == "0")
            {
                MessageBox.Show("Select Area");
                isValid = false;
            }
            //zipId Validation  
            else if (SelecteddatCity.zipId == "0")
            {
                MessageBox.Show("Select City");
                isValid = false;
            }
            //Street Validation  
            else if (String.IsNullOrWhiteSpace(txtStreet.Text.Trim()))
            {
                MessageBox.Show("Enter Street");
                isValid = false;
            }
            //Password   Validation  
            else if (String.IsNullOrWhiteSpace(txtPassword.Password.Trim()))
            {
                MessageBox.Show("Enter Password");
                isValid = false;
            }
            //Password   Validation  
            else if (String.IsNullOrWhiteSpace(txtConfirmPassword.Password.Trim()))
            {
                MessageBox.Show("Enter Confirm Password!");
                isValid = false;
            }
            else if (txtPassword.Password.Trim() != txtConfirmPassword.Password.Trim())
            {
                MessageBox.Show("Password do not match");
                isValid = false;
            }
            else if (chkRequest.IsChecked == false)
            {
                MessageBox.Show("Please select pointemart request");
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
            var passwordEmpty = string.IsNullOrEmpty(txtPassword.Password);
            PasswordWatermark.Opacity = passwordEmpty ? 100 : 0;
            txtPassword.Opacity = passwordEmpty ? 0 : 100;
        }
        private void PasswordGotFocus(object sender, RoutedEventArgs e)
        {
            PasswordWatermark.Opacity = 0;
            txtPassword.Opacity = 100;

            txtPassword.Background = new SolidColorBrush(Colors.Transparent);
            txtPassword.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void ConfirmPasswordLostFocus(object sender, RoutedEventArgs e)
        {
            ConfirmCheckPasswordWatermark();
        }
        public void ConfirmCheckPasswordWatermark()
        {
            var passwordEmpty = string.IsNullOrEmpty(txtConfirmPassword.Password);
            ConfirmPasswordWatermark.Opacity = passwordEmpty ? 100 : 0;
            txtConfirmPassword.Opacity = passwordEmpty ? 0 : 100;
        }
        private void ConfirmPasswordGotFocus(object sender, RoutedEventArgs e)
        {
            ConfirmPasswordWatermark.Opacity = 0;
            txtConfirmPassword.Opacity = 100;

            txtConfirmPassword.Background = new SolidColorBrush(Colors.Transparent);
            txtConfirmPassword.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }
    }
}