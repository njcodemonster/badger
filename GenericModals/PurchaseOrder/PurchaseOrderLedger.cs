using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericModals.PurchaseOrder
{
    public class PurchaseOrderLedger
    {
        public int transaction_id { get; set; }
        public int po_id { get; set; }
        public string description { get; set; }
        public decimal credit { get; set; }
        public decimal debit { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
}
