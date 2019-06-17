using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("purchase_order_tracking")]
    public partial class PurchaseOrdersTracking
    {
        [Key]
        public int po_tracking_id { get; set; }
        public int po_id { get; set; }
        public int tracking_number { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
}
