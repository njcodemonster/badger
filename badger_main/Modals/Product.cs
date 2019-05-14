using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public int ProductTypeId { get; set; }
        public int VendorId { get; set; }
        public byte? ProductAvailability { get; set; }
        public double PublishedAt { get; set; }
        public string ProductVendorImage { get; set; }
        public string ProductName { get; set; }
        public string ProductUrlHandle { get; set; }
        public string ProductDescription { get; set; }
        public string VendorColorName { get; set; }
        public string SkuFamily { get; set; }
        public int SizeAndFitId { get; set; }
        public int WashTypeId { get; set; }
        public decimal ProductDiscount { get; set; }
        public decimal ProductCost { get; set; }
        public decimal ProductRetail { get; set; }
        public int PublishedStatus { get; set; }
        public int IsOnSiteStatus { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public double UpdatedAt { get; set; }
        public double CreatedAt { get; set; }
    }
}
