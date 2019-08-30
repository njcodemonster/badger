using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericModals.PurchaseOrder
{
    public class PurchaseOrdersDiscount
    {
        public int po_discount_id { get; set; }
        public int po_id { get; set; }
        public decimal discount_percentage { get; set; }
        public string discount_note { get; set; }
        public int completed_status { get; set; }
        public double created_at { get; set; }
        public double? updated_at { get; set; }
        public int created_by { get; set; }
        public int? updated_by { get; set; }
    }
}
