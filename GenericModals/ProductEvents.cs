﻿using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("product_events")]
    public partial class ProductEvents
    {
        [Key]
        public int product_event_id { get; set; }
        public int product_id { get; set; }
        public int event_type_id { get; set; }
        public long reference_id { get; set; }
        public string event_notes { get; set; }
        public int user_id { get; set; }
        public double created_at { get; set; }
    }
}
