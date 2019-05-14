using System;
using System.Collections.Generic;

namespace itemService.Models
{
    public partial class ItemEvents
    {
        public int ItemEventId { get; set; }
        public int ItemId { get; set; }
        public double? Barcode { get; set; }
        public int EventTypeId { get; set; }
        public long ReferenceId { get; set; }
        public string EventNotes { get; set; }
        public int UserId { get; set; }
        public double CreatedAt { get; set; }

        public virtual event_type EventType { get; set; }
    }
}
