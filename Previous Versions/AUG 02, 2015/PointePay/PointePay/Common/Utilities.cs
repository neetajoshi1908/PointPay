using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointePay.Common
{
    public class Utilities
    {
        public static string GetAuthorization()
        {
            string authInfo = "admin" + ":" + "1234";
            authInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(authInfo));
            authInfo = string.Format("Basic {0}", authInfo);
            return authInfo;
        }

        public static string GetURL(String EndPoint)
        {
            string url = "http://54.173.246.245/marketplace/api/" + EndPoint;
            return url;
        }

        public static List<MenuItems> GetMenuItems(MenuCode MenuCode)
        {
            string _HomeBackground = "Transparent"; string _HomeForeground = "white";
            if (MenuCode == MenuCode.Home) 
            {
                  _HomeBackground = "black";   _HomeForeground = "Green";
            }
            string _SaleBackground = "Transparent"; string _SaleForeground = "white";
            if (MenuCode == MenuCode.Sale)
            {
                _SaleBackground = "black"; _SaleForeground = "Green";
            }
            string _CatMngtBackground = "Transparent"; string _CatMngtForeground = "white";
            if (MenuCode == MenuCode.CatMngt)
            {
                _CatMngtBackground = "black"; _CatMngtForeground = "Green";
            }
            string _ProdMgntBackground = "Transparent"; string _ProdMgntForeground = "white";
            if (MenuCode == MenuCode.ProdMgnt)
            {
                _ProdMgntBackground = "black"; _ProdMgntForeground = "Green";
            }
            string _InvtMgntBackground = "Transparent"; string _InvtMgntForeground = "white";
            if (MenuCode == MenuCode.InvtMgnt)
            {
                _InvtMgntBackground = "black"; _InvtMgntForeground = "Green";
            }
            string _EmpMgntBackground = "Transparent"; string _EmpMgntForeground = "white";
            if (MenuCode == MenuCode.EmpMgnt)
            {
                _EmpMgntBackground = "black"; _EmpMgntForeground = "Green";
            }
            string _CustMgntBackground = "Transparent"; string _CustMgntForeground = "white";
            if (MenuCode == MenuCode.CustMgnt)
            {
                _CustMgntBackground = "black"; _CustMgntForeground = "Green";
            }
            string _DiscMgntBackground = "Transparent"; string _DiscMgntForeground = "white";
            if (MenuCode == MenuCode.DiscMgnt)
            {
                _DiscMgntBackground = "black"; _DiscMgntForeground = "Green";
            }

            List<MenuItems> MenuItemsList = new List<MenuItems>();

            MenuItemsList.Add(new MenuItems { Text = "Dashboard", code = MenuCode.Home.ToString(), redirecturl = "/Views/HomePage.xaml", iconsrc = "/Assets/MenuIcon/Home.png", count = "0", selectedBgColor = _HomeBackground, selectedTextColor = _HomeForeground });
            MenuItemsList.Add(new MenuItems { Text = "Make a Sale", code = MenuCode.Sale.ToString(), redirecturl = "/Views/HomePage.xaml", iconsrc = "/Assets/MenuIcon/Make_sale.png", count = "0", selectedBgColor = _SaleBackground, selectedTextColor = _SaleForeground });
            MenuItemsList.Add(new MenuItems { Text = "Category Management", code = MenuCode.CatMngt.ToString(), redirecturl = "/Views/CategoryListPage.xaml", iconsrc = "/Assets/MenuIcon/CatMgnt.png", count = "0", selectedBgColor = _CatMngtBackground, selectedTextColor = _CatMngtForeground });
            MenuItemsList.Add(new MenuItems { Text = "Product Management", code = MenuCode.ProdMgnt.ToString(), redirecturl = "/Views/HomePage.xaml", iconsrc = "/Assets/MenuIcon/ProductMgnt.png", count = "0", selectedBgColor = _ProdMgntBackground, selectedTextColor = _ProdMgntForeground });
            MenuItemsList.Add(new MenuItems { Text = "Inventory Management", code = MenuCode.InvtMgnt.ToString(), redirecturl = "/Views/HomePage.xaml", iconsrc = "/Assets/MenuIcon/InventoryMgnt.png", count = "0", selectedBgColor = _InvtMgntBackground, selectedTextColor = _InvtMgntForeground });
            MenuItemsList.Add(new MenuItems { Text = "Employee Management", code = MenuCode.EmpMgnt.ToString(), redirecturl = "/Views/EmployeeListPage.xaml", iconsrc = "/Assets/MenuIcon/EmpMgnt.png", count = "0", selectedBgColor = _EmpMgntBackground, selectedTextColor = _EmpMgntForeground });
            MenuItemsList.Add(new MenuItems { Text = "Customer Management", code = MenuCode.CustMgnt.ToString(), redirecturl = "/Views/HomePage.xaml", iconsrc = "/Assets/MenuIcon/CustMgnt.png", count = "0", selectedBgColor = _CustMgntBackground, selectedTextColor = _CustMgntForeground });
            MenuItemsList.Add(new MenuItems { Text = "Discount Management", code = MenuCode.DiscMgnt.ToString(), redirecturl = "/Views/HomePage.xaml", iconsrc = "/Assets/MenuIcon/appbar.warning.circle.png", count = "0", selectedBgColor = _DiscMgntBackground, selectedTextColor = _DiscMgntForeground });

            return MenuItemsList;
        }

    }
    public class MenuItems
    {
        public string Text { get; set; }
        public string code { get; set; }
        public string redirecturl { get; set; }
        public string iconsrc { get; set; }
        public string count { get; set; }
        public string selectedBgColor { get; set; }
        public string selectedTextColor { get; set; }

    }
    public enum MenuCode
    {
        Home,
        Sale,
        CatMngt,
        ProdMgnt,
        InvtMgnt,
        EmpMgnt,
        CustMgnt,
        DiscMgnt
    };
}
