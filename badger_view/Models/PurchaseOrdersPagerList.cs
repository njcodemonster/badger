using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace badger_view.Models
{
    public class PurchaseOrdersInfo
    {
        public int po_id { get; set; }
        public string vendor_po_number { get; set; }
        public string vendor_invoice_number { get; set; }
        public int vendor_order_number { get; set; }
        public int vendor_id { get; set; }
        public int total_styles { get; set; }
        public double order_date { get; set; }
        public string vendor { get; set; }
        public double delivery_window_start { get; set; }
        public double delivery_window_end { get; set; }
        public int po_status { get; set; }
        public double updated_at { get; set; }
        public string custom_delivery_window_start_end { get; set; }
        public string custom_order_date { get; set; }
        public string num_of_days { get; set; }


    }

    public class PurchaseOrdersPagerList
    {
        public IEnumerable<PurchaseOrdersInfo> purchaseOrdersInfo { get; set; }
        public string Count { get; set; }
    }
}
