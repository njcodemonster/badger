using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("product_photoshoots")]
    public partial class ProductPhotoshoots
    {
        [Key]
        public int photoshoot_id { get; set; }
        public int product_id { get; set; }
        public int product_shoot_status_id { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
        public List<ProductForPhotoshoot> products { get; set; }
    }

    public partial class ProductPhotoshootStatusUpdate
    {
        public int product_shoot_status_id { get; set; }
        public int updated_by { get; set; }
        public double updated_at { get; set; }

    }

    public class ProductForPhotoshoot
    {
        public int product_id { get; set; }
        public int vendor_id { get; set; }
        public string product_name { get; set; }
    }

    public class BarcodeUpdate
    {
        public int item_id { get; set; }
        public int barcode { get; set; }
    }

    public class PhotoshootWithItems
    {
        public List<ProductForPhotoshoot> products { get; set; }
        public List<BarcodeUpdate> items { get; set; }
    }
}
