using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointePay.Model
{
    class SubCategory
    {
    }

    public class SubCategoryViewModel
    {
        public string categoryId { get; set; }
        public string organizationId { get; set; }
        public string categoryCode { get; set; }
        public string categoryDescription { get; set; }
        public string imageName { get; set; }
        public string imagePath { get; set; }
        public string active { get; set; }
        public string parentCategoryId { get; set; }
        public string createDt { get; set; }
        public string lastModifiedDt { get; set; }
        public string lastModifiedBy { get; set; }
        public string parentCategory { get; set; }
        public string mode { get; set; }
    }

    public class SubCategoryRequest
    {
        public int organizationId { get; set; }
        public int set { get; set; }
        public int count { get; set; }
        public int categoryId { get; set; }
        public string categoryCode { get; set; }
        public string categoryDescription { get; set; }
        public int parentCategoryId { get; set; }
    }

    #region Json Serialization
    public class data_SubCategory
    {
        public string categoryId { get; set; }
        public string organizationId { get; set; }
        public string categoryCode { get; set; }
        public string categoryDescription { get; set; }
        public string imageName { get; set; }
        public string imagePath { get; set; }
        public string active { get; set; }
        public string parentCategoryId { get; set; }
        public string createDt { get; set; }
        public string lastModifiedDt { get; set; }
        public string lastModifiedBy { get; set; }
        public string parentCategory { get; set; }
    }
    public class response_SubCategory
    {
        public List<data_SubCategory> data { get; set; }
        public string message { get; set; }
    }
    public class RootObject_SubCategory
    {
        public response_SubCategory response { get; set; }
        public int success { get; set; }
        public int time { get; set; }
    }
    #endregion

    #region Json Serialization AddEdit
    public class data_SubCategoryAddEdit
    {
        public string categoryId { get; set; }
        public string organizationId { get; set; }
        public string categoryCode { get; set; }
        public string categoryDescription { get; set; }
        public string imageName { get; set; }
        public string imagePath { get; set; }
        public string active { get; set; }
        public string parentCategoryId { get; set; }
        public string createDt { get; set; }
        public string lastModifiedDt { get; set; }
        public string lastModifiedBy { get; set; }
        public string parentCategory { get; set; }
    }
    public class response_SubCategoryAddEdit
    {
        public data_SubCategoryAddEdit data { get; set; }
        public string message { get; set; }
    }
    public class RootObject_SubCategoryAddEdit
    {
        public response_SubCategoryAddEdit response { get; set; }
        public int success { get; set; }
        public int time { get; set; }
    }
    #endregion
}
