﻿using Dapper.Contrib.Extensions;
using System;

namespace GenericModals.Models
{
    [Table("event_types")]
    public partial class EventTypes
    {

        [Key]
        public int event_type_id { get; set; }
        public string event_type_name { get; set; }
        public string event_type_description { get; set; }
        public DateTime? created_at { get; set; }

        
    }
}
