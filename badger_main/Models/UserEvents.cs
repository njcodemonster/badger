﻿using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("user_events")]
    public partial class UserEvents
    {
        [Key]
        public int user_event_id { get; set; }
        public int user_event_type { get; set; }
        public int reference_id { get; set; }
        public string event_description { get; set; }
        public int user_id { get; set; }
        public double created_at { get; set; }
    }
}
