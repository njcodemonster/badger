using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("product_images")]
    public partial class Productimages
    {
        [Key]
        public int product_image_id { get; set; }
        public int product_id { get; set; }
        public string product_image_title { get; set; }
        public string product_image_url { get; set; }
        public int isprimary { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
        public int created_by { get; set; }
    }
}
