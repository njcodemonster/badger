using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace badgerApi.Models
{
    public partial class Productpairwith
    {
        public string pairwith_id { get; set; }
        public string pairwith_product_id { get; set; }
    }
    public partial class Productcolorwith
    {
        public string colorwith_id { get; set; }
        public string colorwith_product_id { get; set; }
    }
    public partial class ProductProperties
    {
        public int attribute_id { get; set; }
        public string sku { get; set; }
        public Int64 product_id { get; set; }
        public int attribute_type_id { get; set; }
        public string attribute { get; set; }
        public string attribute_display_name { get; set; }
    }
    public partial class ProductImages
    {
        public int product_image_id { get; set; }
        public String product_Image_url { get; set; }
        public int product_image_type_id { get; set; }
    }
    public partial class ProductDetails
    {
        
        public Int64 product_page_detail_id { get; set; }
        public Int64 product_id { get; set; }
        public Int64 product_detail_type { get; set; }
        public Int64 product_detail_value { get; set; }
        public Int64 created_by { get; set; }
        public Int64 updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }

    }
    public partial class ProductDetailsPageData
    {
        public Product Product { get; set; }
        public IEnumerable<Sku> skus { get; set; }
        public IEnumerable<String> product_Images { get; set; }
        public String Product_Notes { get; set; }
        public IEnumerable<ProductDetails> ProductDetails { get; set; }
        public IEnumerable<Productpairwith> productpairwiths { get; set; }
        public IEnumerable<Productcolorwith> productcolorwiths { get; set; }
        public IEnumerable<ProductProperties> productProperties { get; set; }
    }
}
