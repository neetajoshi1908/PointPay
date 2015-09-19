using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using PointePayApp.Common;
using PointePayApp.Model;
using Newtonsoft.Json;
using System.Windows.Controls.Primitives;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.IO;
using PointePayApp.ViewModel;

namespace PointePayApp.Views.Customer
{
    public partial class CustomerDetails : PhoneApplicationPage
    {            
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        CustomerOfflineViewModel ObjCustomerData;
        public CustomerDetails()
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
                    if (ISOFile.FileExists("viewCustomerDetails"))//read current user login details    
                    {
                        using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewCustomerDetails", FileMode.Open))
                        {
                            //====================================================================================================================
                            // Read Customer Details
                            //====================================================================================================================
                            ObjCustomerData = new CustomerOfflineViewModel();
                            DataContractSerializer serializer = new DataContractSerializer(typeof(CustomerOfflineViewModel));
                            //var ObjCustomerData = (CustomerOfflineViewModel)serializer.ReadObject(fileStream);
                            ObjCustomerData = (CustomerOfflineViewModel)serializer.ReadObject(fileStream);
                            lblName.Text = ObjCustomerData.displayFullName;
                            lblEmail.Text = ObjCustomerData.email;
                            lblPhone.Text = ObjCustomerData.displayContact;
                            lblState.Text = ObjCustomerData.stateName;
                            lblArea.Text = ObjCustomerData.areaName;
                            lblCity.Text = ObjCustomerData.cityName;
                            lblStreet.Text = ObjCustomerData.addressLine1;
                        }

                    }
                }
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/Login/LoginPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Customer/CustomerListPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ObjCustomerData.mode = "Edit";

            if (ISOFile.FileExists("viewCustomerDetails"))
            {
                ISOFile.DeleteFile("viewCustomerDetails");
            }
            using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewCustomerDetails", FileMode.Create))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(CustomerOfflineViewModel));
                serializer.WriteObject(fileStream, ObjCustomerData);

                NavigationService.Navigate(new Uri("/Views/Customer/CustomerAddEdit.xaml", UriKind.RelativeOrAbsolute));
            }
        }
    }
}