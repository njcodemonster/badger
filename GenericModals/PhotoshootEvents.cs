using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("photoshoot_events")]
    public partial class PhotoshootEvents
    {
        [Key]
        public int photoshoot_event_id { get; set; }
        public int photoshoot_id { get; set; }
        public int event_type_id { get; set; }
        public long reference_id { get; set; }
        public string event_notes { get; set; }
        public int user_id { get; set; }
        public double created_at { get; set; }
    }
}
