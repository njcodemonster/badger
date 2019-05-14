using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class PurchaseOrderLineItems
    {
        public int LineItemId { get; set; }
        public int PoId { get; set; }
        public int VendorId { get; set; }
        public string Sku { get; set; }
        public int ProductId { get; set; }
        public decimal LineItemCost { get; set; }
        public decimal LineItemRetail { get; set; }
        public int LineItemType { get; set; }
        public int LineItemOrderedQuantity { get; set; }
        public int LineItemAcceptedQuantity { get; set; }
        public int LineItemRejectedQuantity { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }
    }
}
