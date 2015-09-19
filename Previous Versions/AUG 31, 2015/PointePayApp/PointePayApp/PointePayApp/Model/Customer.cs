using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointePayApp.Model
{
    class Customer
    {
    }

    public class CustomerRequest
    {
        public int organizationId { get; set; }
        public int set { get; set; }
        public int count { get; set; }

        public int customerId { get; set; }
        public int employeeId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public int state { get; set; }
        public int area { get; set; }
        public int city { get; set; }
        public string addressLine1 { get; set; }
        public string street { get; set; }
        public string notes { get; set; }
        //file

    }

    #region Json Serialization Customer List
    public class data_Customer
    {
        public string addressId { get; set; }
        public string addressLine1 { get; set; }
        public string address_Line2 { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string glNbr { get; set; }
        public string phone { get; set; }
        public string secondary_phone { get; set; }
        public string fax { get; set; }
        public string cell { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string timeZone { get; set; }
        public string company { get; set; }
        public string landMark { get; set; }
        public string zone { get; set; }
        public string area { get; set; }
        public string createDt { get; set; }
        public string lastModifiedBy { get; set; }
        public string lastModifiedDt { get; set; }
        public string customerId { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string resetPasswordCode { get; set; }
        public string middle { get; set; }
        public string verified { get; set; }
        public string imageName { get; set; }
        public string imagePath { get; set; }
        public string birthDay { get; set; }
        public string isMarketingUser { get; set; }
        public string countryName { get; set; }
        public string stateName { get; set; }
        public string cityName { get; set; }
        public string areaName { get; set; }

    }
    public class response_Customer
    {
        public List<data_Customer> data { get; set; }
        ////public string message { get; set; }
    }
    public class RootObject_Customer
    {
        public response_Customer response { get; set; }
        public int success { get; set; }
        public int time { get; set; }
    }
    #endregion
}
