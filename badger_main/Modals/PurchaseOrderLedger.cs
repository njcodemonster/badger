using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("purchase_order_ledger")]
    public partial class PurchaseOrderLedger
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
