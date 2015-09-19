using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointePayApp.Model
{
    public class EmployeeOffline
    {
        [SQLite.PrimaryKey]
        public string staffEmployeeId { get; set; }
        public string employeeId { get; set; }
        public string organizationId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string businessPhoneCode { get; set; }
        public string businessPhone { get; set; }
        public string designation { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string area { get; set; }
        public string addressLine1 { get; set; }
        public string salary { get; set; }
        public string userName { get; set; }
        public string role { get; set; }
        //public string file { get; set; }
        public string synced { get; set; }
        public string active { get; set; }
        public string stateName { get; set; }
        public string cityName { get; set; }
        public string areaName { get; set; }
        public string imageName { get; set; }
    }
}
