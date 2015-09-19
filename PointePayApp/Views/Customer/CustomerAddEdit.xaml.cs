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
using PointePayApp.Model;
using PointePayApp.Common;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.IO;
using PointePayApp.ViewModel;
using Newtonsoft.Json;
using PointePayApp.Provider;

namespace PointePayApp.Views.Customer
{
    public partial class CustomerAddEdit : PhoneApplicationPage
    {
        public List<data_State> ListPickerStateData { get; set; }
        public List<data_Area> ListPickerAreaData { get; set; }
        public List<data_City> ListPickerCityData { get; set; }

        public static int _employeeId; // Logged In user's employeeId
        public static int _organizationId; // Logged In user's organizationId
        public static int _customerId; // Id of customer
        public static int _countryId;
        public static int _stateId;
        public static int _areaId;
        public static int _cityId;
        public static string _businessPhoneCode = "+234";
        public static string _mode;

        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        public CustomerAddEdit()
        {
            InitializeComponent();

            //====================================================================================================================
            // Fetch Login and Customer details
            //====================================================================================================================

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
                            _employeeId = Convert.ToInt32(ObjUserData.employeeId);
                            _organizationId = Convert.ToInt32(ObjUserData.organizationId);
                            _countryId = Convert.ToInt32(ObjUserData.country);
                        }
                    }

                    if (ISOFile.FileExists("viewCustomerDetails"))//read current user login details    
                    {
                        using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewCustomerDetails", FileMode.Open))
                        {
                            DataContractSerializer serializer = new DataContractSerializer(typeof(CustomerOfflineViewModel));
                            var ObjCustomerData = (CustomerOfflineViewModel)serializer.ReadObject(fileStream);
                            txtFirstName.Text = Convert.ToString(ObjCustomerData.firstName);
                            txtLastName.Text = Convert.ToString(ObjCustomerData.lastName);
                            txtEmail.Text = Convert.ToString(ObjCustomerData.email);
                            txtPhone.Text = Convert.ToString(ObjCustomerData.displayContact);
                            txtStreet.Text = Convert.ToString(ObjCustomerData.addressLine1);
                            _stateId = Convert.ToInt32(ObjCustomerData.state);
                            _areaId = Convert.ToInt32(ObjCustomerData.area);
                            _cityId = Convert.ToInt32(ObjCustomerData.city);
                            _customerId = Convert.ToInt32(ObjCustomerData.customerId);
                            _mode = ObjCustomerData.mode;

                            if (_mode == "Add")
                            {
                                btnSave.Content = "Add Customer";
                                lblHeader.Text = "ADD CUSTOMER";
                            }
                            if (_mode == "Edit")
                            {
                                btnSave.Content = "Save";
                                lblHeader.Text = "EDIT CUSTOMER";
                            }
                        }

                    }
                }
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/Login/LoginPage.xaml", UriKind.RelativeOrAbsolute));
            }


            if (Utilities.CheckInternetConnection())
            {
                // ----------------------------------------------------------------------
                // "Network Status: Connected."

                // Show Loader
                myIndeterminateProbar.Visibility = Visibility.Visible;

                //====================================================================================================================
                // Fill State Dropdown Default
                //====================================================================================================================

                // Parameters
                String data = "country_id=" + _countryId;
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

            if (_mode == "Add")
            {

            }
            if (_mode == "Edit")
            {
                if (Utilities.CheckInternetConnection())
                {
                    // ----------------------------------------------------------------------
                    // "Network Status: Connected."
                    //====================================================================================================================
                    // Fill Area Dropdown On Edit mode
                    //====================================================================================================================

                    //Parameters
                    String dataArea = "country_id=" + _countryId + "&state_id=" + _stateId;
                    //Initialize WebClient
                    WebClient webClientArea = new WebClient();
                    //Set Header
                    webClientArea.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                    webClientArea.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                    webClientArea.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
                    webClientArea.UploadStringAsync(new Uri(Utilities.GetURL("location/getArea/")), "POST", dataArea);
                    //Assign Event Handler
                    webClientArea.UploadStringCompleted += wc_UploadAresCompleted;

                    //====================================================================================================================
                    // Fill city Dropdown On Edit mode
                    //====================================================================================================================

                    // Parameters
                    String dataCity = "country_id=" + _countryId + "&state_id=" + _stateId + "&area_id=" + _areaId;
                    //Initialize WebClient
                    WebClient webClientCity = new WebClient();
                    //Set Header
                    webClientCity.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                    webClientCity.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                    webClientCity.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
                    webClientCity.UploadStringAsync(new Uri(Utilities.GetURL("location/getCity/")), "POST", dataCity);
                    //Assign Event Handler
                    webClientCity.UploadStringCompleted += wc_UploadCityCompleted;
                }
            }
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

                    if (_mode == "Add")
                    {
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
                    }
                    if (_mode == "Edit")
                    {
                        var item = ListPickerStateData.Where(x => x.stateId == _stateId.ToString()).FirstOrDefault();
                        this.listPickerState.SelectedItem = item;
                    }

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
            if (Utilities.CheckInternetConnection())
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
                        // Fill Area Dropdown  
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
                    if (_mode == "Add")
                    {
                        this.listPickerArea.SelectedIndex = 0;
                    }
                    if (_mode == "Edit")
                    {
                        var item = ListPickerAreaData.Where(x => x.areaId == _areaId.ToString()).FirstOrDefault();
                        this.listPickerArea.SelectedItem = item;
                    }

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
            if (Utilities.CheckInternetConnection())
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
                        // Fill City Dropdown  
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
                        ListPickerCityData.Add(new data_City { zipId = itm.zipId, countryId = itm.countryId, stateId = itm.stateId, city = itm.city });
                    };
                    this.listPickerCity.ItemsSource = ListPickerCityData;
                    if (_mode == "Add")
                    {
                        this.listPickerCity.SelectedIndex = 0;
                    }
                    if (_mode == "Edit")
                    {
                        var item = ListPickerCityData.Where(x => x.zipId == _cityId.ToString()).FirstOrDefault();
                        this.listPickerCity.SelectedItem = item;
                    }

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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // ----------------------------------------------------------------------
            // Parameters

            CustomerRequest obj = new CustomerRequest();
            obj.employeeId = _employeeId;  // Logged In User's id
            obj.organizationId = _organizationId; // Logged In organizationId  
            obj.customerId = _customerId;  // Logged In User's id
            obj.firstName = txtFirstName.Text.Trim();
            obj.lastName = txtLastName.Text.Trim();
            obj.phone = txtPhone.Text.Trim();
            obj.email = txtEmail.Text.Trim();
            obj.addressLine1 = txtStreet.Text.Trim();
            obj.street = txtStreet.Text.Trim();
            obj.notes = string.Empty;

            if (Utilities.CheckInternetConnection())
            {
                // ----------------------------------------------------------------------
                // "Network Status: Connected."

                ListPickerItem selectedItemState = this.listPickerState.ItemContainerGenerator.ContainerFromItem(this.listPickerState.SelectedItem) as ListPickerItem;
                data_State SelecteddataState = selectedItemState.DataContext as data_State;
                obj.state = Convert.ToInt32(SelecteddataState.stateId);

                ListPickerItem selectedItemArea = this.listPickerArea.ItemContainerGenerator.ContainerFromItem(this.listPickerArea.SelectedItem) as ListPickerItem;
                data_Area SelecteddataArea = selectedItemArea.DataContext as data_Area;
                obj.area = Convert.ToInt32(SelecteddataArea.areaId);

                ListPickerItem selectedItemCity = this.listPickerCity.ItemContainerGenerator.ContainerFromItem(this.listPickerCity.SelectedItem) as ListPickerItem;
                data_City SelecteddatCity = selectedItemCity.DataContext as data_City;
                obj.city = Convert.ToInt32(SelecteddatCity.zipId); // here zipId is cityID	

                String data = string.Empty;

                if (Validation() == true)
                {
                    // Show Loader 
                    myIndeterminateProbar.Visibility = Visibility.Visible;

                    //====================================================================================================================
                    // Submit Details
                    //====================================================================================================================

                    //Initialize WebClient
                    WebClient webClient = new WebClient();
                    //Set Header
                    webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                    webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                    webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";

                    if (_mode == "Add")
                    {
                        data = "employeeId=" + obj.employeeId + "&organizationId=" + obj.organizationId + "&firstName=" + obj.firstName + "&lastName=" + obj.lastName + "&phone=" + obj.phone + "&state=" + obj.state + "&area=" + obj.area + "&city=" + obj.city + "&addressLine1=" + obj.addressLine1 + "&street=" + obj.street + "&notes=" + obj.notes + "&email=" + obj.email;
                        webClient.UploadStringAsync(new Uri(Utilities.GetURL("manageCustomer/addCustomer")), "POST", data);
                    }
                    if (_mode == "Edit")
                    {
                        data = "customerId=" + obj.customerId + "&employeeId=" + obj.employeeId + "&organizationId=" + obj.organizationId + "&firstName=" + obj.firstName + "&lastName=" + obj.lastName + "&phone=" + obj.phone + "&state=" + obj.state + "&area=" + obj.area + "&city=" + obj.city + "&addressLine1=" + obj.addressLine1 + "&street=" + obj.street + "&notes=" + obj.notes + "&email=" + obj.email;
                        webClient.UploadStringAsync(new Uri(Utilities.GetURL("manageCustomer/updateCustomer")), "POST", data);
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
                                MessageBox.Show("You can not create a new customer in offline mode.");
                                // Redirect to customer list
                                NavigationService.Navigate(new Uri("/Views/Customer/CustomerListPage.xaml", UriKind.Relative));
                            }

                            if (_mode == "Edit")
                            {
                                CustomerDataProvider _CustomerDataProvider = new CustomerDataProvider();
                                var result = _CustomerDataProvider.UpdateCustomerOffline(obj, "False");
                                if (result == true)
                                {
                                    MessageBox.Show("successfully Updated.");
                                    // Redirect to customer list
                                    NavigationService.Navigate(new Uri("/Views/Customer/CustomerListPage.xaml", UriKind.Relative));
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
                var rootObject = JsonConvert.DeserializeObject<dynamic>(e.Result);
                if (rootObject.success == 1)
                {
                    MessageBox.Show(rootObject.response.message.ToString());
                    // hide Loader 
                    myIndeterminateProbar.Visibility = Visibility.Collapsed;
                    // Redirect to customer list
                    NavigationService.Navigate(new Uri("/Views/Customer/CustomerListPage.xaml", UriKind.Relative));
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

            //FirstNames Validation  
            if (String.IsNullOrWhiteSpace(txtFirstName.Text.Trim()))
            {
                MessageBox.Show("Enter First name!");
                isValid = false;
            }
            //Last name   Validation  
            else if (String.IsNullOrWhiteSpace(txtLastName.Text.Trim()))
            {
                MessageBox.Show("Enter Last name!");
                isValid = false;
            }
            //phone number  Validation  
            else if (String.IsNullOrWhiteSpace(txtPhone.Text.Trim()))
            {
                MessageBox.Show("Enter valid phone number!");
                isValid = false;
            }

            if (Utilities.CheckInternetConnection())
            {
                ListPickerItem selectedItemState = this.listPickerState.ItemContainerGenerator.ContainerFromItem(this.listPickerState.SelectedItem) as ListPickerItem;
                data_State SelecteddataState = selectedItemState.DataContext as data_State;


                ListPickerItem selectedItemArea = this.listPickerArea.ItemContainerGenerator.ContainerFromItem(this.listPickerArea.SelectedItem) as ListPickerItem;
                data_Area SelecteddataArea = selectedItemArea.DataContext as data_Area;

                ListPickerItem selectedItemCity = this.listPickerCity.ItemContainerGenerator.ContainerFromItem(this.listPickerCity.SelectedItem) as ListPickerItem;
                data_City SelecteddatCity = selectedItemCity.DataContext as data_City;

                // Validation State
                if (SelecteddataState.stateId == "0")
                {
                    MessageBox.Show("Select state!");
                    isValid = false;
                }
                // Validation area
                else if (SelecteddataArea.areaId == "0")
                {
                    MessageBox.Show("Select area!");
                    isValid = false;
                }
                // Validation city
                else if (SelecteddatCity.zipId == "0")
                {
                    MessageBox.Show("Select city!");
                    isValid = false;
                }
            }

            //Street  Validation  
            else if (String.IsNullOrWhiteSpace(txtStreet.Text.Trim()))
            {
                MessageBox.Show("Enter Street!");
                isValid = false;
            }
            return isValid;
        }

        private void ImgBack_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Customer/CustomerListPage.xaml", UriKind.Relative));
        }

        #region  Set focus on page textboxes
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

        private void txtPhone_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPhone.Background = new SolidColorBrush(Colors.Transparent);
            txtPhone.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txtStreet_GotFocus(object sender, RoutedEventArgs e)
        {
            txtStreet.Background = new SolidColorBrush(Colors.Transparent);
            txtStreet.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        #endregion
    }
}