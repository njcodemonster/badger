﻿using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("product_used_in")]
    public partial class ProductUsedIn
    {
        [Key]
        public string product_used_in_id { get; set; }
        public string product_id { get; set; }
        public int po_id { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
}
