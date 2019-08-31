using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("items")]
    public partial class Items
    {
        [Key]
        public int item_id { get; set; }
        public decimal barcode { get; set; }
        public int? slot_number { get; set; }
        public int? bag_code { get; set; }
        public int item_status_id { get; set; }
        public int ra_status { get; set; }
        public string sku { get; set; }
        public short sku_id { get; set; }
        public int product_id { get; set; }
        public int vendor_id { get; set; }
        public string sku_family { get; set; }
        public int? PO_id { get; set; }
        public int? published { get; set; }
        public string small_sku { get; set; }
        public string product_name { get; set; }
        public string size { get; set; }
        public int? published_by { get; set; }
        public int has_doc { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }

    }
}
