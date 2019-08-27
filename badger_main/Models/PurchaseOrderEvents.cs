using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("purchase_order_events")]
    public partial class PurchaseOrderEvents
    {
        [Key]
        public int po_event_id { get; set; }
        public int po_id { get; set; }
        public int event_type_id { get; set; }
        public int reference_id { get; set; }
        public string event_notes { get; set; }
        public int user_id { get; set; }
        public double created_at { get; set; }
    }
}
