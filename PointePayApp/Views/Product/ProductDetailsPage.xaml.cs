using Microsoft.Phone.Controls;
using PointePayApp.Model;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;

namespace PointePayApp.Views
{
    public partial class ProductDetailsPage : PhoneApplicationPage
    {
        public ProductDetailsPage()
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
                    if (ISOFile.FileExists("viewProductDetails"))//read ProductDetails    
                    {
                        using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("viewProductDetails", FileMode.Open))
                        {
                            //====================================================================================================================
                            // Read Product Details
                            //====================================================================================================================

                            DataContractSerializer serializer = new DataContractSerializer(typeof(ProductViewModel));
                            var ObjProductData = (ProductViewModel)serializer.ReadObject(fileStream);
                            lblCategory.Text = ObjProductData.parentCategoryCode;
                            lblSubCategory.Text = ObjProductData.categoryCode; ;
                            lblItemName.Text = ObjProductData.code;
                            lblDiscountedPrice.Text = "";
                            lblSalePrice.Text = ObjProductData.currentPrice;
                            lblCostPrice.Text = ObjProductData.costPrice;
                            lblUPC.Text = ObjProductData.upc;
                            lblDescription.Text = ObjProductData.description;
                            imgProduct.ImageSource = new BitmapImage(new Uri(ObjProductData.fullImagePath, UriKind.RelativeOrAbsolute)); 
                        }

                        ISOFile.DeleteFile("viewProductDetails");
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
            NavigationService.Navigate(new Uri("/Views/Product/ProductListPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}