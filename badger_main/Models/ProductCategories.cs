using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("product_categories")]
    public partial class ProductCategories
    {
        [Key]
        public int product_category_id { get; set; }
        public int product_id { get; set; }
        public int category_id { get; set; }
        public int created_by { get; set; }
        public int? updated_by { get; set; }
        public double created_at { get; set; }
        public double? updated_at { get; set; }
        [Write(false)]
        public string action { get; set; }
    }
}
