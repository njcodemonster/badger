using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class PurchaseOrderDiscounts
    {
        public int PoDiscountId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string DiscountNote { get; set; }
        public int CompletedStatus { get; set; }
        public double CreatedAt { get; set; }
        public double? UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
