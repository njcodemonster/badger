using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace badger_view.Models
{
    public partial class Productpairwith
    {
        public string pairing_product_id { get; set; }
        public string paired_product_id { get; set; }
    }
    public partial class AllColors
    {
        public Int32 value_id { get; set; }
        public string value { get; set; }
    }
    public partial class AllTags
    {
        public Int32 attribute_id { get; set; }
        public string attribute { get; set; }
        public string attribute_display_name { get; set; }
        public string sub_heading { get; set; }
    }
    public partial class Productcolorwith
    {
        public string product_id { get; set; }
        public string same_color_product_id { get; set; }
    }
    public partial class ProductProperties
    {
        public int attribute_id { get; set; }
        public string sku { get; set; }
        public Int64 product_id { get; set; }
        public int attribute_type_id { get; set; }
        public string attribute { get; set; }
        public string attribute_display_name { get; set; }
        public string value { get; set; }
    }
    public partial class ProductImages
    {
        public int product_image_id { get; set; }
        public string product_id { get; set; }
        public String product_image_title { get; set; }
        public String product_image_url { get; set; }
        public int isprimary { get; set; }
    }
    public partial class ProductDetails
    {

        public Int64 product_page_detail_id { get; set; }
        public Int64 product_id { get; set; }
        public Int64 product_detail_type { get; set; }
        public string product_detail_value { get; set; }
        public Int64 created_by { get; set; }
        public Int64 updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }

    }
    public partial class ProductDetailsPageData
    {
        public Product Product { get; set; }
        public IEnumerable<Sku> skus { get; set; }
        public IEnumerable<ProductImages> product_Images { get; set; }
        public String Product_Notes { get; set; }
        public Int32 shootstatus { get; set; }
        public IEnumerable<AllColors> AllColors { get; set; }
        public IEnumerable<AllTags> AllTags { get; set; }
        public IEnumerable<ProductDetails> ProductDetails { get; set; }
        public IEnumerable<Productpairwith> productpairwiths { get; set; }
        public IEnumerable<Productcolorwith> productcolorwiths { get; set; }
        public IEnumerable<ProductProperties> productProperties { get; set; }
    }
}
