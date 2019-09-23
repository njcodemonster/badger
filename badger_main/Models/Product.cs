using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("product")]
    public partial class Product
    {
        [Key]
        public int product_id { get; set; }
        public int product_type_id { get; set; }
        public int vendor_id { get; set; }
        public byte? product_availability { get; set; }
        public double published_at { get; set; }
        public string product_vendor_image { get; set; }
        public string product_name { get; set; }
        public string product_url_handle { get; set; }
        public string product_description { get; set; }
        public string vendor_color_name { get; set; }
        public string sku_family { get; set; }
        public int size_and_fit_id { get; set; }
        public int wash_type_id { get; set; }
        public decimal product_discount { get; set; }
        public decimal product_cost { get; set; }
        public decimal product_retail { get; set; }
        public int published_status { get; set; }
        public int is_on_site_status { get; set; }
        public bool is_ready { get; set; } = false;
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double updated_at { get; set; }
        public double created_at { get; set; }
        [Write(false)]
        public string vendor_color_code { get; set; }
        [Write(false)]
        public string vendor_product_code { get; set; }
        [Write(false)]
        public string productCategories { get; set; }
        [Write(false)]
        public string skulist { get; set; }
        
    }


   
       
}
