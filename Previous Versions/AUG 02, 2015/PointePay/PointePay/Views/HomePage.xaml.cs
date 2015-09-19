using System;
using System.Collections.Generic;
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
    public partial class HomePage : PhoneApplicationPage
    {
        int serchclick = 1;
        public HomePage()
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
                    if (ISOFile.FileExists("CurrentLoginUserDetails"))//read current user login details    
                    {
                        using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("CurrentLoginUserDetails", FileMode.Open))
                        {
                            DataContractSerializer serializer = new DataContractSerializer(typeof(LoginViewModel));
                            var ObjUserData = (LoginViewModel)serializer.ReadObject(fileStream);
                            this.txtHeaderOrgName.Text = ObjUserData.organizationName;
                            this.txtHeaderFullName.Text = ObjUserData.firstName + " " + ObjUserData.lastName;
                            lstMenuItems.ItemsSource = Utilities.GetMenuItems(MenuCode.Home);
                        }
                    }
                }
            }
            else
            {
                NavigationService.Navigate(new Uri("/Views/LoginPage.xaml", UriKind.RelativeOrAbsolute));
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
            var Result = MessageBox.Show("Are you sure you want to signout from this page?", "", MessageBoxButton.OKCancel);
            if (Result == MessageBoxResult.OK)
            {
                var Settings = IsolatedStorageSettings.ApplicationSettings;
                Settings.Remove("CurrentLoginUserDetails");
                Settings.Remove("islogin");
                Settings.Remove("viewEmployeeDetails");
                Settings.Remove("SignUpFirstPageDetails");
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Views/LoginPage.xaml", UriKind.RelativeOrAbsolute));
            }  
        }
        # endregion
 
    }
}