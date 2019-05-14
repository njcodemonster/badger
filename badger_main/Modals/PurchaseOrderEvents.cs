using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class PurchaseOrderEvents
    {
        public int PoEventId { get; set; }
        public int PoId { get; set; }
        public int EventTypeId { get; set; }
        public int ReferenceId { get; set; }
        public string EventNotes { get; set; }
        public int UserId { get; set; }
        public double CreatedAt { get; set; }
    }
}
