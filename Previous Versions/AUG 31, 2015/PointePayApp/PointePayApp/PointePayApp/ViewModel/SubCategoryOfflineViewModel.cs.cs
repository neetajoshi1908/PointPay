using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointePayApp.ViewModel
{
    public class SubCategoryOfflineViewModel
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
}
