using System;
using System.Collections.Generic;

namespace itemService_entity.Models
{
    public partial class EventTypes
    {
        public EventTypes()
        {
            ItemEvents = new HashSet<ItemEvents>();
        }

        public int EventTypeId { get; set; }
        public string EventTypeName { get; set; }
        public string EventTypeDescription { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<ItemEvents> ItemEvents { get; set; }
    }
}
