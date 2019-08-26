using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("purchase_order_line_items")]
    public partial class PurchaseOrderLineItems
    {
        [Key]
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
        public double created_at { get; set; }
        public double updated_at { get; set; }
        [Write(false)]
        public string vendor_size { get; set; }
        [Write(false)]
        public int attribute_id { get; set; }
        
    }
}
