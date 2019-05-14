using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("product_attributes")]
    public partial class ProductAttributes
    {
        public long product_attribute_id { get; set; }
        public int product_id { get; set; }
        public int attribute_id { get; set; }
        public int? sku { get; set; }
        public double created_at { get; set; }
        public double? updated_at { get; set; }
        public int? updated_by { get; set; }
        public int created_by { get; set; }
    }
}
