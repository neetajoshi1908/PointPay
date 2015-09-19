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


namespace PointePayApp.Views.Category
{
    public partial class CategoryDetailsPage : PhoneApplicationPage
    {
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        CategoryOfflineViewModel ObjCategoryData;

        public CategoryDetailsPage()
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
                    if (ISOFile.FileExists("viewCategoryDetails"))//read current user login details    
                    {
                        using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewCategoryDetails", FileMode.Open))
                        {
                            //====================================================================================================================
                            // Read Category Details
                            //====================================================================================================================
                            ObjCategoryData = new CategoryOfflineViewModel();
                            DataContractSerializer serializer = new DataContractSerializer(typeof(CategoryOfflineViewModel));
                            ObjCategoryData = (CategoryOfflineViewModel)serializer.ReadObject(fileStream);
                            lblCategory.Text = ObjCategoryData.categoryCode;
                            lblDescription.Text = ObjCategoryData.categoryDescription;
 
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
            NavigationService.Navigate(new Uri("/Views/Category/CategoryListPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ObjCategoryData.mode = "Edit";

            if (ISOFile.FileExists("viewCategoryDetails"))
            {
                ISOFile.DeleteFile("viewCategoryDetails");
            }
            using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewCategoryDetails", FileMode.Create))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(CategoryOfflineViewModel));
                serializer.WriteObject(fileStream, ObjCategoryData);

                NavigationService.Navigate(new Uri("/Views/Category/CategoryAddEditPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }
    }
}