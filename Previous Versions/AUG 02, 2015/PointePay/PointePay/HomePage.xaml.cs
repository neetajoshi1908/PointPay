using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace PointePay
{
    public partial class HomePage : PhoneApplicationPage
    {
        public HomePage()
        {
            InitializeComponent();

            //InitializeComponent();
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/Assets/AppBarIcon/appbar.add.png", UriKind.Relative));
            button1.Text = "Search";
            ApplicationBar.Buttons.Add(button1);

            string a = "Test Icons " + " (22)";
            ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem(a);
            ApplicationBarMenuItem menuItem2 = new ApplicationBarMenuItem(a);
            
            ApplicationBar.MenuItems.Add(menuItem1);
            menuItem1.Click += new EventHandler(menuItem1_Click);
             
            ApplicationBar.MenuItems.Add(menuItem2);
            menuItem1.Click += new EventHandler(menuItem2_Click);

            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).IsEnabled = false;
        }

        private void menuItem1_Click(object sender, EventArgs e)
        { 

        }
        private void menuItem2_Click(object sender, EventArgs e)
        {

        }


    //    private void OnApplicationBarStateChanged(object sender,
    //ApplicationBarStateChangedEventArgs e)
    //    {
    //        var appBar = sender as ApplicationBar;
    //        if (appBar == null) return;

    //        appBar.Opacity = e.IsMenuVisible ? 1 : .65;
    //    }
    }
}