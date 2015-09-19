using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointePayApp.Model
{
    public class SubCategoryOffline
    {
        [SQLite.PrimaryKey]

        public string categoryId { get; set; }
        public string organizationId { get; set; }
        public string categoryCode { get; set; }
        public string categoryDescription { get; set; }
        public string parentCategoryId { get; set; }
        public string parentCategory { get; set; }
        public string imageName { get; set; }
        public string active { get; set; }
        public string synced { get; set; }
    }
}
