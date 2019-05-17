using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class PurchaseOrders
    {     
        public int po_id { get; set; }
        public string vendor_po_number { get; set; }
        public string vendor_invoice_number { get; set; }
        public int vendor_order_number { get; set; }
        public int vendor_id { get; set; }
        public int defected { get; set; }
        public int good_condition { get; set; }
        public decimal total_quantity { get; set; }
        public decimal subtotal { get; set; }
        public decimal shipping { get; set; }
        public double? delivery_window_start { get; set; }
        public double? delivery_window_end { get; set; }
        public int po_status { get; set; }
        public decimal po_discount_id { get; set; }
        public int deleted { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double order_date { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
}
