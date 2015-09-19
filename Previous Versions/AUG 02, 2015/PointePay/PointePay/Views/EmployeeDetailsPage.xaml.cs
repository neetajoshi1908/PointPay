using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using PointePay.Common;
using PointePay.Model;
using Newtonsoft.Json;
using System.Windows.Controls.Primitives;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.IO;

namespace PointePay.Views
{
    public partial class EmployeeDetailsPage : PhoneApplicationPage
    {
 
        public EmployeeDetailsPage()
        {
            InitializeComponent();

            IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
            if (IsolatedStorageSettings.ApplicationSettings.Contains("islogin"))
            {
                if (!(Convert.ToString(IsolatedStorageSettings.ApplicationSettings["islogin"]).ToLower() == "yes"))
                {
                    NavigationService.Navigate(new Uri("/Views/LoginPage.xaml", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    if (ISOFile.FileExists("viewEmployeeDetails"))//read current user login details    
                    {
                        using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewEmployeeDetails", FileMode.Open))
                        {
                            //====================================================================================================================
                            // Read Employee Details
                            //====================================================================================================================

                            DataContractSerializer serializer = new DataContractSerializer(typeof(EmployeeViwModel));
                            var ObjEmployeData = (EmployeeViwModel)serializer.ReadObject(fileStream);
                            lblName.Text = ObjEmployeData.displayFullName;
                            lblEmail.Text = ObjEmployeData.email;
                            lblPhone.Text = ObjEmployeData.displayContact;
                            lblSalary.Text = ObjEmployeData.salary;
                            lblState.Text = ObjEmployeData.stateName;
                            lblArea.Text = ObjEmployeData.areaName;
                            lblCity.Text = ObjEmployeData.cityName;
                            lblStreet.Text = ObjEmployeData.addressLine1;
                        }
                    }
                }
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/LoginPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/EmployeeListPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}