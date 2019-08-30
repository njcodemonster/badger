
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
  
    public partial class ProductPageDetails
    {
      
        public int product_page_detail_id { get; set; }
        public int product_id { get; set; }
        public int product_detail_type { get; set; }
        public string product_detail_value { get; set; }
        public int created_by { get; set; }
        public int? updated_by { get; set; }
        public double created_at { get; set; }
        public double? updated_at { get; set; }
    }
}
