using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class PurchaseOrderLedger
    {
        public int TransactionId { get; set; }
        public int PoId { get; set; }
        public string Description { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public double CreatedAt { get; set; }
        public double UpdatedAt { get; set; }
    }
}
