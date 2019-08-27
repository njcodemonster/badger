using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace GenericModals.Models
{
    [Table("item_events")]
    public partial class ItemEvents
    {
       

        public int item_event_id { get; set; }
        public int item_id { get; set; }
        public double? barcode { get; set; }
        public int event_type_id { get; set; }
        public long reference_id { get; set; }
        public string event_notes { get; set; }
        public int user_id { get; set; }
        public double created_at { get; set; }

        
    }
}
