using System;
using System.Collections.Generic;

namespace badger_view.Models
{
    public partial class Productimages
    {
        
        public int product_image_id { get; set; }
        public int product_id { get; set; }
        public string product_image_title { get; set; }
        public string product_image_url { get; set; }
        public int isprimary { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
}
