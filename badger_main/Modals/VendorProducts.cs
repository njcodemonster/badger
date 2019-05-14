using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("vendor_products")]
    public partial class VendorProducts
    {
        public int product_id { get; set; }
        public int vendor_id { get; set; }
        public string vendor_color_code { get; set; }
        public string vendor_color_name { get; set; }
        public string vendor_product_code { get; set; }
        public string vendor_product_name { get; set; }
        public double created_at { get; set; }
        public double? updated_at { get; set; }
        public int created_by { get; set; }
        public int? updated_by { get; set; }
    }
}
