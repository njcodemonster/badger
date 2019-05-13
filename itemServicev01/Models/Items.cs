using System;
using System.Collections.Generic;

namespace itemService.Models
{
    public partial class Items
    {
        public int ItemId { get; set; }
        public decimal Barcode { get; set; }
        public int? SlotNumber { get; set; }
        public int? BagCode { get; set; }
        public int ItemStatusId { get; set; }
        public int RaStatus { get; set; }
        public string Sku { get; set; }
        public short SkuId { get; set; }
        public int ProductId { get; set; }
        public int VendorId { get; set; }
        public string SkuFamily { get; set; }
        public int? Published { get; set; }
        public int? PublishedBy { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public double CreatedAt { get; set; }
        public double UpdatedAt { get; set; }

        public virtual ItemStatus ItemStatus { get; set; }
    }
}
