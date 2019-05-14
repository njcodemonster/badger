using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("product_page_detail_types")]
    public partial class ProductPageDetailTypes
    {
        public int product_detail_type_id { get; set; }
        public string product_detail_name { get; set; }
        public string product_detail_description { get; set; }
        public int created_by { get; set; }
        public int? updated_by { get; set; }
        public double? created_at { get; set; }
        public double updated_at { get; set; }
    }
}
