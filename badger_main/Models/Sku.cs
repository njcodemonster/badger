using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("sku")]
    public partial class Sku
    {
        [Key]
        public int sku_id { get; set; }
        public string sku { get; set; }
        public int vendor_id { get; set; }
        public int product_id { get; set; }
        public int weight { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
}
