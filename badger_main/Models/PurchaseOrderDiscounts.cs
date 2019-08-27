using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("purchase_order_discounts")]
    public partial class PurchaseOrderDiscounts
    {
        [Key]
        public int po_discount_id { get; set; }
        public int po_id { get; set; }
        public decimal discount_percentage { get; set; }
        public string discount_note { get; set; }
        public int completed_status { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
    }
}
