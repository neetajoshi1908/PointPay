using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using PointePayApp.Common;
using PointePayApp.Model;
using PointePayApp.Provider;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PointePayApp.Views
{
    public partial class EmployeeListPage : PhoneApplicationPage
    {
        public List<EmployeeViwModel> ListEmployeeData { get; set; }
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        public static int _employeeId; // Logged In user's employeeId
        public static int _organizationId; // Logged In user's organizationId
        public static int _staffEmployeeId; // Id of employee
        public static string _redirectMode;
        int serchclick = 1;

        public EmployeeListPage()
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
                            lstMenuItems.ItemsSource = Utilities.GetMenuItems(MenuCode.EmpMgnt);

                            if (Utilities.CheckInternetConnection())
                            {
                                // ----------------------------------------------------------------------
                                // "Network Status: Connected."

                                //====================================================================================================================
                                // Employee module Data Synchronization
                                //====================================================================================================================

                                // Show Loader 
                                myIndeterminateProbar.Visibility = Visibility.Visible;

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

                                                string[] dataarry = new string[] { obj.empRoleArray };
                                                // Serialise the data we are sending in to JSON
                                                string serialisedDataempRoleArray = JsonConvert.SerializeObject(dataarry);

                                                String data = string.Empty;

                                                //Initialize WebClient
                                                WebClient webClient = new WebClient();

                                                //Set Header
                                                webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                                                webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                                                webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";

                                                data = "designation=" + obj.designation + "&staffEmployeeId=" + _staffEmployeeId + "&employeeId=" + obj.employeeId + "&organizationId=" + obj.organizationId + "&firstName=" + obj.firstName + "&lastName=" + obj.lastName + "&email=" + obj.email + "&businessPhoneCode=" + obj.businessPhoneCode + "&businessPhone=" + obj.businessPhone + "&state=" + obj.state + "&area=" + obj.area + "&city=" + obj.city + "&addressLine1=" + obj.addressLine1 + "&salary=" + obj.salary + "&userName=" + obj.userName + "&role=" + serialisedDataempRoleArray;
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
                            }
                            else
                            {
                                // ----------------------------------------------------------------------
                                //  "Network Status: Not Connected."

                                EmployeeDataProvider _EmployeeDataProvider = new Provider.EmployeeDataProvider();

                                ListEmployeeData = new List<EmployeeViwModel>();
                                foreach (var itm in _EmployeeDataProvider.GetAllEmployeeOfflineList())
                                {
                                    string _blockUnblockIconSrc = "/Assets/Employee/bullet_red.png";
                                    if (itm.active == "0")
                                    {
                                        // Inactive - already block
                                        _blockUnblockIconSrc = "/Assets/Employee/bullet_green.png";
                                    }
                                    ListEmployeeData.Add(new EmployeeViwModel { displayFullName = itm.firstName + " " + itm.lastName, displayContact = itm.businessPhoneCode + " " + itm.businessPhone, email = itm.email, salary = itm.salary, stateName = itm.stateName, areaName = itm.areaName, cityName = itm.cityName, addressLine1 = itm.addressLine1, active = itm.active, city = itm.city, state = itm.state, area = itm.area, empRoleList = itm.empRoleList, firstName = itm.firstName, lastName = itm.lastName, employeeId = itm.employeeId, staffEmployeeId = itm.staffEmployeeId, businessPhone = itm.businessPhone, userName = itm.userName, blockUnblockIconSrc = _blockUnblockIconSrc });
                                };
                                this.lstEmployeeItems.ItemsSource = ListEmployeeData;

                                // hide Loader 
                                myIndeterminateProbar.Visibility = Visibility.Collapsed;
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
                    //EmployeeDataProvider _EmployeeDataProvider = new Provider.EmployeeDataProvider();
                    //foreach (var itm in rootObject.response.data)
                    //{
                    //    var result = _EmployeeDataProvider.UpdatesyncedStatusEmployeeOffline(Convert.ToInt32(_staffEmployeeId));
                    //    if (result == true)
                    //    {
                    //        //MessageBox.Show("successfully registerd employee.");
                    //    }
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

                // Show Loader 
                myIndeterminateProbar.Visibility = Visibility.Visible;

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
                obj.count = Utilities.GetListCount();
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
            }
        }

        void wc_UploadLoadEmployeeCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            EmployeeDataProvider _EmployeeDataProvider = new Provider.EmployeeDataProvider();

            try
            {
                if (e.Result.Contains("no employee found"))
                {
                    MessageBox.Show("No employee found");
                }
                else
                {
                    //Parse JSON result 
                    var rootObject = JsonConvert.DeserializeObject<RootObject_Employee>(e.Result);
                    if (rootObject.success == 1)
                    {
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

                            _EmployeeDataProvider = new Provider.EmployeeDataProvider();
                            var result = _EmployeeDataProvider.AddEmployeeOffline(obj, "True");
                            if (result == true)
                            {
                                //MessageBox.Show("successfully registerd employee.");
                            }
                        }

                        //====================================================================================================================
                        // Fill Employee List From Offline DB
                        //====================================================================================================================

                        _EmployeeDataProvider = new Provider.EmployeeDataProvider();
                        ListEmployeeData = new List<EmployeeViwModel>();
                        foreach (var itm in _EmployeeDataProvider.GetAllEmployeeOfflineList())
                        {
                            string _blockUnblockIconSrc = "/Assets/Employee/bullet_red.png";
                            if (itm.active == "0")
                            {
                                // Inactive - already block
                                _blockUnblockIconSrc = "/Assets/Employee/bullet_green.png";
                            }

                            var Source = "/Assets/Employee/account-circle.png";
                            if (!string.IsNullOrEmpty(itm.imageName))
                            {
                                Source = Utilities.GetMarketplaceURL() + uploadImagePath.EMPLOYEE + itm.imageName;
                            }

                            ListEmployeeData.Add(new EmployeeViwModel { displayFullName = itm.firstName + " " + itm.lastName, displayContact = itm.businessPhoneCode + " " + itm.businessPhone, email = itm.email, salary = itm.salary, stateName = itm.stateName, areaName = itm.areaName, cityName = itm.cityName, addressLine1 = itm.addressLine1, active = itm.active, city = itm.city, state = itm.state, area = itm.area, empRoleList = itm.role, firstName = itm.firstName, lastName = itm.lastName, employeeId = itm.employeeId, staffEmployeeId = itm.staffEmployeeId, businessPhone = itm.businessPhone, userName = itm.userName, blockUnblockIconSrc = _blockUnblockIconSrc, designation = itm.designation, fullImagePath = Source });
                        };
                        this.lstEmployeeItems.ItemsSource = ListEmployeeData;

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

        private void ImgEditBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var _EmployeeDataContext = (EmployeeViwModel)(sender as Image).DataContext;

            Popup popup = new Popup();
            popup.VerticalOffset = 80;
            PopUpUserControl control = new PopUpUserControl();
            control.HorizontalAlignment = HorizontalAlignment.Center;
            if (_EmployeeDataContext.active == "1")
            {
                control.btnblock.Content = "Block";
            }
            else
            {
                control.btnblock.Content = "Unblock";
            }

            popup.Child = control;
            popup.IsOpen = true;
            _redirectMode = "Default";
            control.btnEdit.Click += (s, args) =>
            {
                //====================================================================================================================
                // PopUp Edit button
                //====================================================================================================================

                popup.IsOpen = false;
                // Set page mode for Edit record of employee
                _redirectMode = "Edit";
                _EmployeeDataContext.mode = "Edit";

                if (ISOFile.FileExists("viewEmployeeDetails"))
                {
                    ISOFile.DeleteFile("viewEmployeeDetails");
                }
                using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewEmployeeDetails", FileMode.Create))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(EmployeeViwModel));
                    serializer.WriteObject(fileStream, _EmployeeDataContext);

                    NavigationService.Navigate(new Uri("/Views/Employee/EmployeeAddEdit.xaml", UriKind.RelativeOrAbsolute));
                }

            };

            control.btnblock.Click += (s, args) =>
            {
                //====================================================================================================================
                // PopUp block Unblock Button
                //====================================================================================================================

                int active = 1;
                if (_EmployeeDataContext.active == "1")
                {
                    active = 0;
                }

                // Parameters
                EmployeeRequest obj = new EmployeeRequest();
                obj.organizationId = _organizationId;
                obj.employeeId = _employeeId;
                obj.staffEmployeeId = Convert.ToInt32(_EmployeeDataContext.staffEmployeeId);
                obj.blockStatus = active;
                String data = "organizationId=" + obj.organizationId + "&employeeId=" + obj.employeeId + "&staffEmployeeId=" + obj.staffEmployeeId + "&blockStatus=" + obj.blockStatus;

                //Initialize WebClient
                WebClient webClient = new WebClient();
                //Set Header
                webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
                webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";
                webClient.UploadStringAsync(new Uri(Utilities.GetURL("manageEmployee/blockUnblockEmployee/")), "POST", data);
                //Assign Event Handler
                webClient.UploadStringCompleted += wc_UploadBlockCompleted;

                popup.IsOpen = false;
            };

            control.btnclose.Tap += (s, args) =>
            {
                //====================================================================================================================
                // PopUp cancel Button
                //====================================================================================================================

                popup.IsOpen = false;
            };
        }

        void wc_UploadBlockCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                //Parse JSON result 
                var rootObject = JsonConvert.DeserializeObject<RootObject_Employee>(e.Result);
                if (rootObject.success == 1)
                {
                    MessageBox.Show(rootObject.response.message.ToString());
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
        }//wc_UploadBlockCompleted

        private void lstEmployeeItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                var ListItem = ((EmployeeViwModel)(sender as LongListSelector).SelectedItem);

                if (ISOFile.FileExists("viewEmployeeDetails"))
                {
                    ISOFile.DeleteFile("viewEmployeeDetails");
                }
                using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewEmployeeDetails", FileMode.Create))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(EmployeeViwModel));
                    serializer.WriteObject(fileStream, ListItem);

                    NavigationService.Navigate(new Uri("/Views/Employee/EmployeeDetailsPage.xaml", UriKind.RelativeOrAbsolute));
                }
            }
        }

        private void ImgAddEmployee_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //====================================================================================================================
            // Add new employee
            //====================================================================================================================

            EmployeeViwModel _EmployeeDataContext = new EmployeeViwModel();
            // Set page mode for Edit record of employee
            _redirectMode = "Add";
            _EmployeeDataContext.mode = "Add";
            if (ISOFile.FileExists("viewEmployeeDetails"))
            {
                ISOFile.DeleteFile("viewEmployeeDetails");
            }
            using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewEmployeeDetails", FileMode.Create))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(EmployeeViwModel));
                serializer.WriteObject(fileStream, _EmployeeDataContext);

                NavigationService.Navigate(new Uri("/Views/Employee/EmployeeAddEdit.xaml", UriKind.RelativeOrAbsolute));
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