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
using System.Windows.Media.Imaging;

namespace PointePayApp.Views.SubCategory
{
    public partial class SubCategoryDetailsPage : PhoneApplicationPage
    {
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        SubCategoryOfflineViewModel ObjSubCategoryData;

        public SubCategoryDetailsPage()
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
                    if (ISOFile.FileExists("viewSubCategoryDetails"))//read current user login details    
                    {
                        using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewSubCategoryDetails", FileMode.Open))
                        {
                            //====================================================================================================================
                            // Read Category Details
                            //====================================================================================================================
                            ObjSubCategoryData = new SubCategoryOfflineViewModel();
                            DataContractSerializer serializer = new DataContractSerializer(typeof(SubCategoryOfflineViewModel));
                            ObjSubCategoryData = (SubCategoryOfflineViewModel)serializer.ReadObject(fileStream);
                            lblSubCategory.Text = ObjSubCategoryData.categoryCode;
                            lblDescription.Text = ObjSubCategoryData.categoryDescription;
                            lblCategory.Text = ObjSubCategoryData.parentCategory;
                            if (!string.IsNullOrEmpty(ObjSubCategoryData.fullImagePath))
                            {
                                imgSubCategory.ImageSource = new BitmapImage(new Uri(ObjSubCategoryData.fullImagePath, UriKind.RelativeOrAbsolute));
                            }
                            else
                            {
                                imgSubCategory.ImageSource = new BitmapImage(new Uri("/Assets/category/icon_sub_categories.png", UriKind.RelativeOrAbsolute));
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

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SubCategory/SubCategoryListPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            ObjSubCategoryData.mode = "Edit";

            if (ISOFile.FileExists("viewSubCategoryDetails"))
            {
                ISOFile.DeleteFile("viewSubCategoryDetails");
            }
            using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewSubCategoryDetails", FileMode.Create))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(SubCategoryOfflineViewModel));
                serializer.WriteObject(fileStream, ObjSubCategoryData);

                NavigationService.Navigate(new Uri("/Views/SubCategory/SubCategoryAddEditPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }
    }
}