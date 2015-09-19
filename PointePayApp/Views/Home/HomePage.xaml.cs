using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using PointePayApp.Common;
using PointePayApp.Model;
using PointePayApp.Provider;
using PointePayApp.ViewModel;
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
    public partial class HomePage : PhoneApplicationPage
    {
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        public List<EmployeeViwModel> ListEmployeeData { get; set; }
        public List<CustomerOfflineViewModel> ListCustomerData { get; set; }
        public static int _employeeId; // Logged In user's employeeId
        public static int _organizationId; // Logged In user's organizationId
        public static int _staffEmployeeId; // Id of employee
        int serchclick = 1;
        public HomePage()
        {
            InitializeComponent();

            // Show Loader 
            myIndeterminateProbar.Visibility = Visibility.Visible;

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
                            this.txtHeaderOrgName.Text = ObjUserData.organizationName;
                            this.txtHeaderFullName.Text = ObjUserData.firstName + " " + ObjUserData.lastName;
                            _employeeId = Convert.ToInt32(ObjUserData.employeeId);
                            _organizationId = Convert.ToInt32(ObjUserData.organizationId);
                            lstMenuItems.ItemsSource = Utilities.GetMenuItems(MenuCode.Home);


                            if (Utilities.CheckInternetConnection())
                            {
                                // ----------------------------------------------------------------------
                                // "Network Status: Connected."

                                //====================================================================================================================
                                // Employee module Data Synchronization
                                //====================================================================================================================

                                EmployeeDataProvider _EmployeeDataProvider = new Provider.EmployeeDataProvider();
                                var result = _EmployeeDataProvider.GetsyncedEmployeeOfflineList("False");
                                if (result != null)
                                {
                                    if (result.Count > 0)
                                    {
                                        try
                                        {
                                            foreach (var itm in result)
                                            {
                                                EmployeeRequest obj = new EmployeeRequest();
                                                obj.employeeId = Convert.ToInt32(itm.employeeId);
                                                obj.organizationId = Convert.ToInt32(itm.organizationId);
                                                _staffEmployeeId = Convert.ToInt32(itm.staffEmployeeId);
                                                obj.firstName = itm.firstName;
                                                obj.lastName = itm.lastName;
                                                obj.email = itm.email;
                                                obj.businessPhoneCode = itm.businessPhoneCode;  // hardcoded now
                                                obj.businessPhone = itm.businessPhone;
                                                obj.addressLine1 = itm.addressLine1;
                                                obj.salary = itm.salary;
                                                obj.userName = itm.userName;
                                                obj.designation = itm.designation;
                                                obj.empRoleArray = itm.role;

                                                String data = string.Empty;

                                                //Initialize WebClient
                                                WebClient webClient = new WebClient();

                                                //Set Header
                                                webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                                                webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                                                webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";

                                                data = "designation=" + "" + "&staffEmployeeId=" + _staffEmployeeId + "&employeeId=" + obj.employeeId + "&organizationId=" + obj.organizationId + "&firstName=" + obj.firstName + "&lastName=" + obj.lastName + "&email=" + obj.email + "&businessPhoneCode=" + obj.businessPhoneCode + "&businessPhone=" + obj.businessPhone + "&state=" + obj.state + "&area=" + obj.area + "&city=" + obj.city + "&addressLine1=" + obj.addressLine1 + "&salary=" + obj.salary + "&userName=" + obj.userName + "&role=" + obj.empRoleArray;
                                                webClient.UploadStringAsync(new Uri(Utilities.GetURL("manageEmployee/updateEmployee")), "POST", data);

                                                //Assign Event Handler
                                                webClient.UploadStringCompleted += wc_UploadSycnedOfflineCompleted;

                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show("Something wrong happened.");
                                        }
                                    }
                                }


                                CustomerDataProvider _CustomerDataProvider = new CustomerDataProvider();
                                var resultCustomer = _CustomerDataProvider.GetsyncedCustomerOfflineList("False");
                                if (resultCustomer != null)
                                {
                                    if (resultCustomer.Count > 0)
                                    {
                                        try
                                        {
                                            foreach (var itm in resultCustomer)
                                            {
                                                CustomerRequest obj = new CustomerRequest();
                                                obj.employeeId = _employeeId;
                                                obj.organizationId = _organizationId;
                                                obj.customerId = Convert.ToInt32(itm.customerId);
                                                //_customerId = Convert.ToInt32(itm.customerId);
                                                obj.addressLine1 = itm.addressLine1;
                                                obj.street = itm.address_Line2;
                                                obj.firstName = itm.firstName;
                                                obj.lastName = itm.lastName;
                                                obj.city = Convert.ToInt32(itm.city);
                                                obj.state = Convert.ToInt32(itm.state);
                                                obj.area = Convert.ToInt32(itm.area);
                                                obj.email = itm.email;
                                                obj.phone = itm.phone;
                                                obj.notes = string.Empty;

                                                String data = string.Empty;

                                                //Initialize WebClient
                                                WebClient webClient = new WebClient();

                                                //Set Header
                                                webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                                                webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                                                webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";

                                                data = "customerId=" + obj.customerId + "&employeeId=" + obj.employeeId + "&organizationId=" + obj.organizationId + "&firstName=" + obj.firstName + "&lastName=" + obj.lastName + "&phone=" + obj.phone + "&state=" + obj.state + "&area=" + obj.area + "&city=" + obj.city + "&addressLine1=" + obj.addressLine1 + "&street=" + obj.street + "&notes=" + obj.notes + "&email=" + obj.email;
                                                webClient.UploadStringAsync(new Uri(Utilities.GetURL("manageCustomer/updateCustomer")), "POST", data);

                                                //Assign Event Handler
                                                webClient.UploadStringCompleted += wc_UploadSycnedCustomerOfflineCompleted;

                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show("Something wrong happened.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/Login/LoginPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        void wc_UploadSycnedOfflineCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                //Parse JSON result 
                var rootObject = JsonConvert.DeserializeObject<RootObject_Employee>(e.Result);
                if (rootObject.success == 1)
                {
                    EmployeeDataProvider _EmployeeDataProvider = new Provider.EmployeeDataProvider();
                    foreach (var itm in rootObject.response.data)
                    {
                        //var result = _EmployeeDataProvider.UpdatesyncedStatusEmployeeOffline(Convert.ToInt32(_staffEmployeeId));
                        //if (result == true)
                        //{
                        //    //MessageBox.Show("successfully registerd employee.");
                        //}
                    }
                }
                else
                {
                    //MessageBox.Show(rootObject.response.message.ToString());
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Something wrong happened.");
            }
            finally
            {
                // hide Loader 
                //myIndeterminateProbar.Visibility = Visibility.Collapsed;
            }
        }//wc_UploadSycnedOfflineCompleted

        void wc_UploadSycnedCustomerOfflineCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                //Parse JSON result 
                var rootObject = JsonConvert.DeserializeObject<dynamic>(e.Result);
                if (rootObject.success == 1)
                {
                    //CustomerDataProvider _CustomerDataProvider = new CustomerDataProvider();
                    //var result = _CustomerDataProvider.UpdatesyncedStatusCustomerOffline(Convert.ToInt32(_customerId));
                    //if (result == true)
                    //{
                    //    //MessageBox.Show("successfully registerd employee.");
                    //}
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
        }//wc_UploadSycnedOfflineCompleted

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Utilities.CheckInternetConnection())
            {
                // ----------------------------------------------------------------------
                // "Network Status: Connected."

                //====================================================================================================================
                // Clear offline Employee table
                //====================================================================================================================

                EmployeeDataProvider _EmployeeDataProvider = new Provider.EmployeeDataProvider();
                var result = _EmployeeDataProvider.DeleteAllEmployeeOffline();
                if (result == true)
                {
                    // Success
                }

                //====================================================================================================================
                // Fill Employee List
                //====================================================================================================================
                // Parameters
                EmployeeRequest obj = new EmployeeRequest();
                obj.organizationId = _organizationId;
                obj.set = 1;
                obj.count = 100;
                String data = "organizationId=" + obj.organizationId + "&set=" + obj.set + "&count=" + obj.count;

                //Initialize WebClient
                WebClient webClient = new WebClient();
                //Set Header
                webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
                webClient.UploadStringAsync(new Uri(Utilities.GetURL("manageEmployee/employeeList/")), "POST", data);
                //Assign Event Handler
                webClient.UploadStringCompleted += wc_UploadLoadEmployeeCompleted;


                //====================================================================================================================
                // Clear offline Customer table
                //====================================================================================================================

                CustomerDataProvider _CustomerDataProvider = new CustomerDataProvider();
                var resultCustomer = _CustomerDataProvider.DeleteAllCustomerOffline();
                if (resultCustomer == true)
                {
                    // Success
                }

                //====================================================================================================================
                // Fill Customer List
                //====================================================================================================================
                // Parameters
                CustomerRequest objCustomer = new CustomerRequest();
                objCustomer.organizationId = _organizationId;
                objCustomer.set = 1;
                objCustomer.count = 100;
                String dataCustomer = "organizationId=" + objCustomer.organizationId + "&set=" + objCustomer.set + "&count=" + objCustomer.count;

                //Initialize WebClient
                WebClient webClientobjCustomer = new WebClient();
                //Set Header
                webClientobjCustomer.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                webClientobjCustomer.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                webClientobjCustomer.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
                webClientobjCustomer.UploadStringAsync(new Uri(Utilities.GetURL("manageCustomer/customerList")), "POST", dataCustomer);
                //Assign Event Handler
                webClientobjCustomer.UploadStringCompleted += wc_UploadLoadCustomerCompleted;
            }
        }

        void wc_UploadLoadEmployeeCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Contains("no employee found"))
                {

                }
                else
                {
                    //Parse JSON result 
                    var rootObject = JsonConvert.DeserializeObject<RootObject_Employee>(e.Result);
                    if (rootObject.success == 1)
                    {
                        ListEmployeeData = new List<EmployeeViwModel>();
                        foreach (var itm in rootObject.response.data)
                        {

                            EmployeeRequest obj = new EmployeeRequest();
                            obj.employeeId = Convert.ToInt32(_employeeId);
                            obj.organizationId = Convert.ToInt32(itm.organizationId);
                            obj.staffEmployeeId = Convert.ToInt32(itm.employeeId);
                            obj.firstName = itm.firstName;
                            obj.lastName = itm.lastName;
                            obj.email = itm.email;
                            obj.businessPhoneCode = itm.businessPhoneCode;
                            obj.businessPhone = itm.businessPhone;
                            obj.addressLine1 = itm.addressLine1;
                            obj.salary = itm.salary;
                            obj.userName = itm.userName;
                            obj.state = Convert.ToInt32(itm.state);
                            obj.area = Convert.ToInt32(itm.area);
                            obj.city = Convert.ToInt32(itm.city); // here zipId is cityID
                            obj.empRoleArray = itm.empRoleList;
                            obj.designation = itm.designation;
                            obj.active = itm.active;
                            obj.areaName = itm.areaName;
                            obj.cityName = itm.cityName;
                            obj.stateName = itm.stateName;
                            obj.imageName = itm.imageName;

                            EmployeeDataProvider _EmployeeDataProvider = new Provider.EmployeeDataProvider();
                            var result = _EmployeeDataProvider.AddEmployeeOffline(obj, "True");
                            if (result == true)
                            {
                                //MessageBox.Show("successfully registerd employee.");
                            }
                        };
                    }
                    else
                    {
                        MessageBox.Show(rootObject.response.message.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("no employee found"))
                {
                }
                else { MessageBox.Show("Something wrong happened."); }
            }
            finally
            {
                // hide Loader 
                myIndeterminateProbar.Visibility = Visibility.Collapsed;
            }
        }//wc_UploadLoadEmployeeCompleted

        void wc_UploadLoadCustomerCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                CustomerDataProvider _CustomerDataProvider = new CustomerDataProvider();
                //Parse JSON result 
                var results = JsonConvert.DeserializeObject<dynamic>(e.Result);
                if (results.success == 1)
                {
                    //Parse JSON result 
                    var rootObject = JsonConvert.DeserializeObject<RootObject_Customer>(e.Result);
                    foreach (var itm in rootObject.response.data)
                    {
                        CustomerOfflineViewModel obj = new CustomerOfflineViewModel();
                        obj.employeeId = _employeeId;
                        obj.organizationId = _organizationId;
                        obj.customerId = itm.customerId;
                        obj.addressLine1 = itm.addressLine1;
                        obj.address_Line2 = itm.address_Line2;
                        obj.firstName = itm.firstName;
                        obj.lastName = itm.lastName;
                        obj.city = itm.city;
                        obj.state = itm.state;
                        obj.area = itm.area;
                        obj.email = itm.email;
                        obj.phone = itm.phone;
                        obj.stateName = itm.stateName;
                        obj.cityName = itm.cityName;
                        obj.areaName = itm.areaName;
                        obj.imageName = itm.imageName;

                        _CustomerDataProvider = new CustomerDataProvider();
                        var result = _CustomerDataProvider.AddCustomerOffline(obj, "True");
                        if (result == true)
                        {
                            //MessageBox.Show("successfully registerd Customer.");
                        }
                    }

                }
                if (results.success == 0)
                {
                    MessageBox.Show(results.response.message.ToString());
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("no user found"))
                {
                }
                else { MessageBox.Show("Something wrong happened."); }
            }
            finally
            {
                // hide Loader 
                myIndeterminateProbar.Visibility = Visibility.Collapsed;
            }
        }

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
            //var Result = MessageBox.Show("Are you sure you want to signout from this page?", "", MessageBoxButton.OKCancel);
            //if (Result == MessageBoxResult.OK)
            //{
                if (Utilities.CheckSignOut())
                {
                    (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Views/Login/LoginPage.xaml", UriKind.RelativeOrAbsolute));
                }
            //}
        }
        # endregion
    }
}