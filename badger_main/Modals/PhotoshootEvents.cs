using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class PhotoshootEvents
    {
        public int PhotoshootEventId { get; set; }
        public int PhotoshootId { get; set; }
        public int EventTypeId { get; set; }
        public long ReferenceId { get; set; }
        public string EventNotes { get; set; }
        public int UserId { get; set; }
        public double CreatedAt { get; set; }
    }
}
