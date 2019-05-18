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
        public string vendor { get; set; }
        public double order_date { get; set; }
        public int po_status { get; set; }

    }

    public class PurchaseOrdersPagerList
    {
        public IEnumerable<PurchaseOrdersInfo> purchaseOrdersInfo { get; set; }
        public string Count { get; set; }
    }
}
