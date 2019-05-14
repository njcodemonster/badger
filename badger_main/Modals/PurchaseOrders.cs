using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("purchase_orders")]
    public partial class PurchaseOrders
    {
        public int PoId { get; set; }
        public string VendorPoNumber { get; set; }
        public string VendorInvoiceNumber { get; set; }
        public int VendorOrderNumber { get; set; }
        public int VendorId { get; set; }
        public int Defected { get; set; }
        public int GoodCondition { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public double? DeliveryWindowStart { get; set; }
        public double? DeliveryWindowEnd { get; set; }
        public int PoStatus { get; set; }
        public decimal PoDiscountId { get; set; }
        public int Deleted { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public double OrderDate { get; set; }
        public double CreatedAt { get; set; }
        public double UpdatedAt { get; set; }
    }
}
