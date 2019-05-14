using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class UserEvents
    {
        public int UserEventId { get; set; }
        public int UserEventType { get; set; }
        public int ReferenceId { get; set; }
        public string EventDescription { get; set; }
        public int CreatedAt { get; set; }
    }
}
