using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointePayApp.Model
{
    class Employee
    { }
    public class EmployeeViwModel
    {
        public string mode { get; set; }
        public string displayFullName { get; set; }
        public string displayContact { get; set; }
        public string roleId { get; set; }
        public string employeeId { get; set; }
        public string code { get; set; }
        public string userName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public string cell { get; set; }
        public string businessPhoneCode { get; set; }
        public string businessPhone { get; set; }
        public string email { get; set; }
        public string salary { get; set; }
        public string stateName { get; set; }
        public string cityName { get; set; }
        public string areaName { get; set; }
        public string countryName { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string area { get; set; }
        public string country { get; set; }
        public string addressLine1 { get; set; }
        public string address_Line2 { get; set; }
        public string zip { get; set; }
        public string designation { get; set; }
        public string imagePath { get; set; }
        public string imageName { get; set; }
        public string empRoleList { get; set; }
        public string active { get; set; }
        public string blockUnblockIconSrc { get; set; }
        public string organizationId { get; set; }
        public string staffEmployeeId { get; set; }
        public string fullImagePath { get; set; }

    }
    public class EmployeeRequest
    {
        public int organizationId { get; set; }
        public int set { get; set; }
        public int count { get; set; }

        public string stateName { get; set; }
        public string cityName { get; set; }
        public string areaName { get; set; }
        public string imageName { get; set; }

        public int employeeId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string businessPhoneCode { get; set; }
        public string businessPhone { get; set; }
        public string email { get; set; }
        public int city { get; set; }
        public int state { get; set; }
        public int area { get; set; }
        public string addressLine1 { get; set; }
        public string salary { get; set; }
        public string empRoleArray { get; set; }
        public string userName { get; set; }
        public int staffEmployeeId { get; set; }
        public int blockStatus { get; set; }
        public byte[] file { get; set; }
        public string active { get; set; }
        public string designation { get; set; }

    }

    #region Json Serialization Employee List
    public class data_Employee
    {
        public string roleId { get; set; }
        public string employeeId { get; set; }
        public string code { get; set; }
        public string userName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone { get; set; }
        public string cell { get; set; }
        public string businessPhoneCode { get; set; }
        public string businessPhone { get; set; }
        public string email { get; set; }
        public string salary { get; set; }
        public string stateName { get; set; }
        public string cityName { get; set; }
        public string areaName { get; set; }
        public string countryName { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string area { get; set; }
        public string country { get; set; }
        public string addressLine1 { get; set; }
        public string address_Line2 { get; set; }
        public string zip { get; set; }
        public string designation { get; set; }
        public string imagePath { get; set; }
        public string imageName { get; set; }
        public string empRoleList { get; set; }
        public string active { get; set; }
        public string organizationId { get; set; }

    }
    public class response_Employee
    {
        public List<data_Employee> data { get; set; }
        public string message { get; set; }
    }
    public class RootObject_Employee
    {
        public response_Employee response { get; set; }
        public int success { get; set; }
        public int time { get; set; }
    }
    #endregion

    public class EmployeeRoleRequest
    {
        // No parameter required
    }
    public class EmployeeRoleViewModel
        {
        public string roleId { get; set; }
        public string code { get; set; }
        public string Description { get; set; }
        public string parentRoleId { get; set; }
        public string createDt { get; set; }
        public string lastModifiedDt { get; set; }
        public string lastModifiedBy { get; set; }
        public bool chkstate { get; set; }
    }

    #region Json Serialization Employee Role
    public class data_EmployeeRole
    {
        public string roleId { get; set; }
        public string code { get; set; }
        public string Description { get; set; }
        public string parentRoleId { get; set; }
        public string createDt { get; set; }
        public string lastModifiedDt { get; set; }
        public string lastModifiedBy { get; set; }
    }
    public class response_EmployeeRole
    {
        public List<data_EmployeeRole> data { get; set; }
        public string message { get; set; }
    }
    public class RootObject_EmployeeRole
    {
        public response_EmployeeRole response { get; set; }
        public int success { get; set; }
        public int time { get; set; }
    }
    #endregion
}
