using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointePayApp.Model
{
    public class Login
    {

    }
    public class LoginViewModel
    {
        public string organizationType { get; set; }
        public string organizationId { get; set; }
        public string organizationName { get; set; }
        public string imageName { get; set; }
        public string employeeId { get; set; }
        public string email { get; set; }
        public string userName { get; set; }
        public string businessPhone { get; set; }
        public string firstName { get; set; }
        public string middle { get; set; }
        public string lastName { get; set; }
        public string businessPhoneCode { get; set; }
        public string resetPasswordCode { get; set; }
        public string active { get; set; }
        public string password { get; set; }
        public string passwordStatus { get; set; }
        public string countryName { get; set; }
        public string stateName { get; set; }
        public string cityName { get; set; }
        public string areaName { get; set; }
        public string areaId { get; set; }
        public string addressLine1 { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string code { get; set; }
        public string requestStatus { get; set; }
        public string isPointepay { get; set; }
        public string isPointemart { get; set; }
    }
    public class LoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }
   
    #region Json Serialization
    public class RootObject_Login
    {
        public response_Login response { get; set; }
        public int success { get; set; }
        public int time { get; set; }
    }
    public class response_Login
    {
        public  data_Login data { get; set; }
        public string message { get; set; }
    }
    public class data_Login
    {
        public string organizationType { get; set; }
        public string organizationId { get; set; }
        public string organizationName { get; set; }
        public string imageName { get; set; }
        public string employeeId { get; set; }
        public string email { get; set; }
        public string userName { get; set; }
        public string businessPhone { get; set; }
        public string firstName { get; set; }
        public string middle { get; set; }
        public string lastName { get; set; }
        public string businessPhoneCode { get; set; }
        public string resetPasswordCode { get; set; }
        public string active { get; set; }
        public string password { get; set; }
        public string passwordStatus { get; set; }
        public string countryName { get; set; }
        public string stateName { get; set; }
        public string cityName { get; set; }
        public string areaName { get; set; }
        public string areaId { get; set; }
        public string addressLine1 { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string code { get; set; }
        public string requestStatus { get; set; }
        public string isPointepay { get; set; } 
       public string isPointemart { get; set; }

    }
    #endregion


}
