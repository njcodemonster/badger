using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("purchase_order_line_items")]
    public partial class PurchaseOrderLineItems
    {
        public int line_item_id { get; set; }
        public int po_id { get; set; }
        public int vendor_id { get; set; }
        public string sku { get; set; }
        public int product_id { get; set; }
        public decimal line_item_cost { get; set; }
        public decimal line_item_retail { get; set; }
        public int line_item_type { get; set; }
        public int line_item_ordered_quantity { get; set; }
        public int line_item_accepted_quantity { get; set; }
        public int line_item_rejected_quantity { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public int created_at { get; set; }
        public int updated_at { get; set; }
    }
}
