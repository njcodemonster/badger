using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class Sku
    {
        public int SkuId { get; set; }
        public string Sku1 { get; set; }
        public int VendorId { get; set; }
        public int ProductId { get; set; }
        public int Weight { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public double CreatedAt { get; set; }
        public double UpdatedAt { get; set; }
    }
}
