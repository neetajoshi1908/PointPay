using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PointePay.Common;

namespace PointePay
{
    public partial class MainPage : PhoneApplicationPage
    {
        const string apiUrl = @"http://54.173.246.245/marketplace/api/auth/getCountryFromIp";

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            String data = "organizationId=" + "1881" + "&set=1" + "&count=5";

            //Initialize WebClient
            WebClient webClient = new WebClient();

            //Set Header
            webClient.Headers[HttpRequestHeader.Authorization] = Utilities.GetAuthorization();
            webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            webClient.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";

            webClient.UploadStringAsync(new Uri("http://54.173.246.245/marketplace/api/category/subCategoryListing/"), "POST", data);

            //Assign Event Handler
            webClient.UploadStringCompleted += wc_UploadStringCompleted;

        }

        void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show(e.Result);
                //Parse JSON result 
                var rootObject = JsonConvert.DeserializeObject<RootObject>(e.Result);
                foreach (var blog in rootObject.response.data)
                {
                    Console.WriteLine(blog.stateId);
                }
                // 1
                //JObject obj = JObject.Parse(e.Result);
                //JArray jarr = (JArray)obj["data"];
                //foreach (var item in jarr)
                //    Console.WriteLine(item["stateId"]); //Gets the title of each book in the list


                // 2 
                //var root1 = JsonConvert.DeserializeObject<User>(e.Result);
                //this.DataContext = root1;


                //3
                //var result = JObject.Parse(response)["response"]["data"];
                //foreach (var item in result)
                //{

                //    // Code to store the result
                //}


                //4

                //RootObject root = JsonConvert.DeserializeObject<RootObject>(e.Result);
                //JObject obj = root.response.data;
                //foreach (KeyValuePair<string, JToken> pair in root.response.data)
                //{
                //    string key = pair.Key; // here you got 39.
                //    foreach (JObject detail in pair.Value as JArray)
                //    {
                //        string date = detail["test_date"].ToString();
                //        string score = detail["score"].ToString();
                //        string total_marks = detail["total_marks"].ToString();
                //    }
                //}



                //5
                //var rootObject = JsonConvert.DeserializeObject<RootObject>(e.Result);
                //MessageBox.Show(e.Result.ToString());
                //foreach (var res in rootObject.response.data)
                //{
                //   // string date = res["stateId"].ToString(); 
                //    //string rs = res.Key;
                //    //MessageBox.Show(rs.ToString());

                //    // ......
                //} 



            }
            catch (Exception ex)
            {
                WebException we = (WebException)e.Error;
                HttpWebResponse response = (System.Net.HttpWebResponse)we.Response;
                //if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized)
                //    ("Invalid Username and Password.");
                //else
                //ToastMessage.Warning(Global.Global.ErrorMessage);
            }
        }//wc_DownloadStringCompleted

    }
}

public class loginlst
{
    public string email { get; set; }
    public string password { get; set; }
 
}

public class employeelst
{
    public int organizationId { get; set; }
    public int set { get; set; }
    public int count { get; set; }
    public int employeeId { get; set; }
}
public class countrylst
{
    public int country_id { get; set; }
}
public class data1
{
    public string stateId { get; set; }
    public string countryId { get; set; }
    public string stateName { get; set; }

}

public class response1
{
    public List<data1> data { get; set; }
    public bool message { get; set; }
}
public class RootObject
{
    public response1 response { get; set; }
    public int success { get; set; }

    public int time { get; set; }
}