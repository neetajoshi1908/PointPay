using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointePayApp.Model
{
    class Product
    {
    }
    public class ProductRequest
    {
        public int organizationId { get; set; }
        public int set { get; set; }
        public int count { get; set; }
        public int isInventory { get; set; }

        //public int categoryId { get; set; }
        //public int brandId { get; set; }

        public int productId { get; set; }
        public int employeeId { get; set; }
        public string productCodeOveride { get; set; }
        public string productDescription { get; set; }
        public int categoryId { get; set; }
        public string currentPrice { get; set; }
        public string costPrice { get; set; }
        public string upc { get; set; }
        //files

    }

    #region  Organization Product List
    public class ProductViewModel
    {
        public string mode { get; set; }

        public string productId { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string upc { get; set; }
        public string imageName { get; set; }
        public string imagePath { get; set; }
        public string active { get; set; }
        public string unitOfMeasure { get; set; }
        public string unit { get; set; }
        public string slug { get; set; }
        public string verificationResultId { get; set; }
        public string dimensions { get; set; }
        public string weight { get; set; }
        public string sizes { get; set; }
        public string shippingWeight { get; set; }
        public string verifiedBy { get; set; }
        public string productTypeId { get; set; }
        public string brandId { get; set; }
        public string requestReason { get; set; }
        public string createBy { get; set; }
        public string createDt { get; set; }
        public string lastModifiedBy { get; set; }
        public string lastModifiedDt { get; set; }
        public string categoryId { get; set; }
        public string organizationId { get; set; }
        public string categoryCode { get; set; }
        public string categoryDescription { get; set; }
        public string parentCategoryId { get; set; }
        public string parentCategoryCode { get; set; }
        public string organizationProductId { get; set; }
        public string productCodeOveride { get; set; }
        public string productDescription { get; set; }
        public string currentSerial { get; set; }
        public string minQty { get; set; }
        public string currentQty { get; set; }
        public string currentPrice { get; set; }
        public string availableDt { get; set; }
        public string availableSize { get; set; }
        public string lastVerifiedDt { get; set; }
        public string giftWrapping { get; set; }
        public string packagingCost { get; set; }
        public string associationType { get; set; }
        public string costPrice { get; set; }
        public string discount { get; set; }
        public string fullImagePath { get; set; }
        public Productdiscount Productdiscount { get; set; }
    }

    public class Productdiscount
    {
        public string discount { get; set; }
        public string discountId { get; set; }
        public string title { get; set; }
    }

    public class discount
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    #region Json Serialization Organization Product List
    public class response_discount
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class data_Product
    {
        public string productId { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string upc { get; set; }
        public string imageName { get; set; }
        public string imagePath { get; set; }
        public string active { get; set; }
        public string unitOfMeasure { get; set; }
        public string unit { get; set; }
        public string slug { get; set; }
        public string verificationResultId { get; set; }
        public string dimensions { get; set; }
        public string weight { get; set; }
        public string sizes { get; set; }
        public string shippingWeight { get; set; }
        public string verifiedBy { get; set; }
        public string productTypeId { get; set; }
        public string brandId { get; set; }
        public string requestReason { get; set; }
        public string createBy { get; set; }
        public string createDt { get; set; }
        public string lastModifiedBy { get; set; }
        public string lastModifiedDt { get; set; }
        public string categoryId { get; set; }
        public string organizationId { get; set; }
        public string categoryCode { get; set; }
        public string categoryDescription { get; set; }
        public string parentCategoryId { get; set; }
        public string parentCategoryCode { get; set; }
        public string organizationProductId { get; set; }
        public string productCodeOveride { get; set; }
        public string productDescriptio { get; set; }
        public string currentSerial { get; set; }
        public string minQty { get; set; }
        public string currentQty { get; set; }
        public string currentPrice { get; set; }
        public string availableDt { get; set; }
        public string availableSize { get; set; }
        public string lastVerifiedDt { get; set; }
        public string giftWrapping { get; set; }
        public string packagingCost { get; set; }
        public string associationType { get; set; }
        public string costPrice { get; set; }

        public string discount { get; set; }
    }
    public class response_Product
    {
        public List<data_Product> data { get; set; }
        public string message { get; set; }
    }
    public class RootObject_Product
    {
        public response_Product response { get; set; }
        public int success { get; set; }
        public int time { get; set; }
    }
    #endregion
    #endregion


    #region Master Product List

    public class MasterProductViewModel
    {
        public string productId { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string upc { get; set; }
        public string imageName { get; set; }
        public string imagePath { get; set; }
        public string active { get; set; }
        public string unitOfMeasure { get; set; }
        public string unit { get; set; }
        public string slug { get; set; }
        public string verificationResultId { get; set; }
        public string dimensions { get; set; }
        public string weight { get; set; }
        public string sizes { get; set; }
        public string shippingWeight { get; set; }
        public string verifiedBy { get; set; }
        public string productTypeId { get; set; }
        public string brandId { get; set; }
        public string requestReason { get; set; }
        public string createBy { get; set; }
        public string createDt { get; set; }
        public string lastModifiedBy { get; set; }
        public string lastModifiedDt { get; set; }
        public string brandName { get; set; }
        public string categoryCode { get; set; }
        public string productImageId { get; set; }
        public string displayOrder { get; set; }
        public string fullImagePath { get; set; }
    }

    #region Json Serialization Master Product List
    public class data_MasterProduct
    {
        public string productId { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string upc { get; set; }
        public string imageName { get; set; }
        public string imagePath { get; set; }
        public string active { get; set; }
        public string unitOfMeasure { get; set; }
        public string unit { get; set; }
        public string slug { get; set; }
        public string verificationResultId { get; set; }
        public string dimensions { get; set; }
        public string weight { get; set; }
        public string sizes { get; set; }
        public string shippingWeight { get; set; }
        public string verifiedBy { get; set; }
        public string productTypeId { get; set; }
        public string brandId { get; set; }
        public string requestReason { get; set; }
        public string createBy { get; set; }
        public string createDt { get; set; }
        public string lastModifiedBy { get; set; }
        public string lastModifiedDt { get; set; }
        public string brandName { get; set; }
        public string categoryCode { get; set; }
        public string productImageId { get; set; }
        public string displayOrder { get; set; }
    }
    public class response_MasterProduct
    {
        public List<data_MasterProduct> data { get; set; }
        public string message { get; set; }
    }
    public class RootObject_MasterProduct
    {
        public response_MasterProduct response { get; set; }
        public int success { get; set; }
        public int time { get; set; }
    }
    #endregion

    #endregion
}
