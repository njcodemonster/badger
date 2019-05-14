using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class VendorProducts
    {
        public int ProductId { get; set; }
        public int VendorId { get; set; }
        public string VendorColorCode { get; set; }
        public string VendorColorName { get; set; }
        public string VendorProductCode { get; set; }
        public string VendorProductName { get; set; }
        public double CreatedAt { get; set; }
        public double? UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
