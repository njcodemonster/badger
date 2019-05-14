using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class ProductEvents
    {
        public int ProductEventId { get; set; }
        public int ProductId { get; set; }
        public int EventTypeId { get; set; }
        public long ReferenceId { get; set; }
        public string EventNotes { get; set; }
        public int UserId { get; set; }
        public double CreatedAt { get; set; }
    }
}
