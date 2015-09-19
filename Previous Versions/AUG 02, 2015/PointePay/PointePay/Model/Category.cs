using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointePay.Model
{
    class Category
    {
    }

    public class CategoryViewModel
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
        public string mode { get; set; }
    }

    public class CategoryRequest
    {
        public int organizationId { get; set; }
        public int set { get; set; }
        public int count { get; set; }
        public int categoryId { get; set; }
        public string categoryCode { get; set; }
        public string categoryDescription { get; set; }
        public int parentCategoryId { get; set; }
    }

    #region Json Serialization Category List
    public class data_Category
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
    }
    public class response_Category
    {
        public List<data_Category> data { get; set; }
        public string message { get; set; }
    }
    public class RootObject_Category
    {
        public response_Category response { get; set; }
        public int success { get; set; }
        public int time { get; set; }
    }
    #endregion

    #region Json Serialization Category Add Edit
    public class data_CategoryAddEdit
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
    }
    public class response_CategoryAddEdit
    {
        public data_CategoryAddEdit data { get; set; }
        public string message { get; set; }
    }
    public class RootObject_CategoryAddEdit
    {
        public response_CategoryAddEdit response { get; set; }
        public int success { get; set; }
        public int time { get; set; }
    }
    #endregion

}
