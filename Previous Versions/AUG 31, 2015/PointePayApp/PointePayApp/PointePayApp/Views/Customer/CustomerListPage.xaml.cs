using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using PointePayApp.Model;
using PointePayApp.Common;
using System.IO;
using Newtonsoft.Json;
using PointePayApp.ViewModel;
using PointePayApp.Provider;

namespace PointePayApp.Views.Customer
{
    public partial class CustomerListPage : PhoneApplicationPage
    {
        public List<CustomerOfflineViewModel> ListCustomerData { get; set; }
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        public static int _employeeId; // Logged In user's employeeId
        public static int _organizationId; // Logged In user's organizationId
        public static int _customerId;
        public static string _redirectMode;
        int serchclick = 1;

        public CustomerListPage()
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
                            this.txtHeaderOrgName.Text = ObjUserData.organizationName;
                            this.txtHeaderFullName.Text = ObjUserData.firstName + " " + ObjUserData.lastName;
                            _employeeId = Convert.ToInt32(ObjUserData.employeeId);
                            _organizationId = Convert.ToInt32(ObjUserData.organizationId);
                            lstMenuItems.ItemsSource = Utilities.GetMenuItems(MenuCode.CustMgnt);

                            if (Utilities.CheckInternetConnection())
                            {
                                // ----------------------------------------------------------------------
                                // "Network Status: Connected."

                                //====================================================================================================================
                                // Employee module Data Synchronization
                                //====================================================================================================================

                                // Show Loader 
                                myIndeterminateProbar.Visibility = Visibility.Visible;

                                CustomerDataProvider _CustomerDataProvider = new CustomerDataProvider();
                                var result = _CustomerDataProvider.GetsyncedCustomerOfflineList("False");
                                if (result != null)
                                {
                                    if (result.Count > 0)
                                    {
                                        try
                                        {
                                            foreach (var itm in result)
                                            {
                                                CustomerRequest obj = new CustomerRequest();
                                                obj.employeeId = _employeeId;
                                                obj.organizationId = _organizationId;
                                                obj.customerId = Convert.ToInt32(itm.customerId);
                                                _customerId = Convert.ToInt32(itm.customerId);
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
                                                webClient.UploadStringCompleted += wc_UploadSycnedOfflineCompleted;

                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show("Something wrong happened.");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // ----------------------------------------------------------------------
                                //  "Network Status: Not Connected."

                                //====================================================================================================================
                                // Fill Customer List From Offline DB
                                //====================================================================================================================

                                CustomerDataProvider _CustomerDataProvider = new CustomerDataProvider();
                                ListCustomerData = new List<CustomerOfflineViewModel>();
                                foreach (var itm in _CustomerDataProvider.GetAllCustomerOfflineList())
                                {
                                    var Source = "/Assets/Employee/account-circle.png";
                                    if (!string.IsNullOrEmpty(itm.imageName))
                                    {
                                        Source = Utilities.GetMarketplaceURL() + uploadImagePath.CUSTOMER + itm.imageName;
                                    }

                                    ListCustomerData.Add(new CustomerOfflineViewModel { displayFullName = itm.firstName + " " + itm.lastName, displayContact = itm.phone, email = itm.email, stateName = itm.stateName, areaName = itm.areaName, cityName = itm.cityName, addressLine1 = itm.addressLine1, city = itm.city, state = itm.state, area = itm.area, firstName = itm.firstName, lastName = itm.lastName, employeeId = itm.employeeId, customerId = itm.customerId, fullImagePath = Source });
                                };
                                this.lstCustomerItems.ItemsSource = ListCustomerData;

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
                var rootObject = JsonConvert.DeserializeObject<dynamic>(e.Result);
                if (rootObject.success == 1)
                {
                    ////CustomerDataProvider _CustomerDataProvider = new CustomerDataProvider();
                    ////var result = _CustomerDataProvider.UpdatesyncedStatusCustomerOffline(Convert.ToInt32(_customerId));
                    ////if (result == true)
                    ////{
                    ////    //MessageBox.Show("successfully registerd employee.");
                    ////}
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

                // Show Loader 
                myIndeterminateProbar.Visibility = Visibility.Visible;

                //====================================================================================================================
                // Clear offline Customer table
                //====================================================================================================================

                CustomerDataProvider _CustomerDataProvider = new CustomerDataProvider();
                var result = _CustomerDataProvider.DeleteAllCustomerOffline();
                if (result == true)
                {
                    // Success
                }

                //====================================================================================================================
                // Fill Customer List
                //====================================================================================================================
                // Parameters
                CustomerRequest obj = new CustomerRequest();
                obj.organizationId = _organizationId;
                obj.set = 1;
                obj.count = Utilities.GetListCount();
                String data = "organizationId=" + obj.organizationId + "&set=" + obj.set + "&count=" + obj.count;

                //Initialize WebClient
                WebClient webClient = new WebClient();
                //Set Header
                webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
                webClient.UploadStringAsync(new Uri(Utilities.GetURL("manageCustomer/customerList")), "POST", data);
                //Assign Event Handler
                webClient.UploadStringCompleted += wc_UploadLoadCustomerCompleted;
            }
        }

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
                        obj.employeeId=_employeeId;
                        obj.organizationId=_organizationId;
                        obj.customerId=itm.customerId;
                        obj.addressLine1=itm.addressLine1;
                        obj.address_Line2=itm.address_Line2;
                        obj.firstName=itm.firstName;
                        obj.lastName=itm.lastName;
                        obj.city=itm.city;
                        obj.state=itm.state;
                        obj.area=itm.area;
                        obj.email = itm.email;
                        obj.phone=itm.phone;
                        obj.stateName=itm.stateName;
                        obj.cityName=itm.cityName;
                        obj.areaName = itm.areaName;
                        obj.imageName = itm.imageName;

                        _CustomerDataProvider = new CustomerDataProvider();
                        var result = _CustomerDataProvider.AddCustomerOffline(obj, "True");
                        if (result == true)
                        {
                            //MessageBox.Show("successfully registerd Customer.");
                        }
                    }

                    //====================================================================================================================
                    // Fill Customer List From Offline DB
                    //====================================================================================================================

                    _CustomerDataProvider = new CustomerDataProvider();
                    ListCustomerData = new List<CustomerOfflineViewModel>();
                    foreach (var itm in _CustomerDataProvider.GetAllCustomerOfflineList())
                    {
                        var Source = "/Assets/Employee/account-circle.png";
                        if (!string.IsNullOrEmpty(itm.imageName))
                        {
                            Source = Utilities.GetMarketplaceURL() + uploadImagePath.CUSTOMER + itm.imageName;
                        }

                        ListCustomerData.Add(new CustomerOfflineViewModel { displayFullName = itm.firstName + " " + itm.lastName, displayContact = itm.phone, email = itm.email, stateName = itm.stateName, areaName = itm.areaName, cityName = itm.cityName, addressLine1 = itm.addressLine1, city = itm.city, state = itm.state, area = itm.area, firstName = itm.firstName, lastName = itm.lastName, employeeId = itm.employeeId, customerId = itm.customerId,fullImagePath=Source });
                    };
                    this.lstCustomerItems.ItemsSource = ListCustomerData;


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

        private void ImgEditBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var _CustomerDataContext = (CustomerOfflineViewModel)(sender as Image).DataContext;

            popupEditCUSTOMER.VerticalOffset = 0;
            popupEditCUSTOMER.IsOpen = true;
            _redirectMode = "Default";
            btnEdit.Click += (s, args) =>
            {
                //====================================================================================================================
                // PopUp Edit
                //====================================================================================================================

                popupEditCUSTOMER.IsOpen = false;

                _redirectMode = "Edit";
                _CustomerDataContext.mode = "Edit";

                if (ISOFile.FileExists("viewCustomerDetails"))
                {
                    ISOFile.DeleteFile("viewCustomerDetails");
                }
                using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewCustomerDetails", FileMode.Create))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(CustomerOfflineViewModel));
                    serializer.WriteObject(fileStream, _CustomerDataContext);

                    NavigationService.Navigate(new Uri("/Views/Customer/CustomerAddEdit.xaml", UriKind.RelativeOrAbsolute));
                }
            };

            btncloseEditPopup.Tap += (s, args) =>
            {
                //====================================================================================================================
                // PopUp History Button
                //====================================================================================================================

                popupEditCUSTOMER.IsOpen = false;
            };

            btncloseEditPopup.Tap += (s, args) =>
            {
                //====================================================================================================================
                // PopUp cancel Button
                //====================================================================================================================

                popupEditCUSTOMER.IsOpen = false;
            };
        }

        private void lstCustomerItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //====================================================================================================================
            // View Employee Details
            //====================================================================================================================

            if (_redirectMode == "Add")
            {
                _redirectMode = string.Empty;
            }
            else if (_redirectMode == "Edit")
            {
                _redirectMode = string.Empty;
            }
            else if (_redirectMode == "Default")
            {
                _redirectMode = string.Empty;
            }
            else
            {
                var ListItem = ((CustomerOfflineViewModel)(sender as LongListSelector).SelectedItem);

                if (ISOFile.FileExists("viewCustomerDetails"))
                {
                    ISOFile.DeleteFile("viewCustomerDetails");
                }
                using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewCustomerDetails", FileMode.Create))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(CustomerOfflineViewModel));
                    serializer.WriteObject(fileStream, ListItem);

                    NavigationService.Navigate(new Uri("/Views/Customer/CustomerDetails.xaml", UriKind.RelativeOrAbsolute));
                }
            }
        } 

        private void ImgAddCustomer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //====================================================================================================================
            // Add new Customer
            //====================================================================================================================

            CustomerOfflineViewModel _CustomerDataContext = new CustomerOfflineViewModel();
            // Set page mode for Edit record of employee
            _redirectMode = "Add";
            _CustomerDataContext.mode = "Add";
            if (ISOFile.FileExists("viewCustomerDetails"))
            {
                ISOFile.DeleteFile("viewCustomerDetails");
            }
            using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewCustomerDetails", FileMode.Create))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(CustomerOfflineViewModel));
                serializer.WriteObject(fileStream, _CustomerDataContext);

                NavigationService.Navigate(new Uri("/Views/Customer/CustomerAddEdit.xaml", UriKind.RelativeOrAbsolute));
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