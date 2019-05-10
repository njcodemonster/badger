using System;
using System.Collections.Generic;

namespace itemService.Models
{
    public partial class EventTypes
    {
        public int EventTypeId { get; set; }
        public string EventTypeName { get; set; }
        public string EventTypeDescription { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedAt2 { get; set; }
    }
}
