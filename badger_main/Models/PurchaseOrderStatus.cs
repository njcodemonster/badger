﻿using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("purchase_order_status")]
    public partial class PurchaseOrderStatus
    { 
        [Key]
        public int po_status_id { get; set; }
        public string po_status_name { get; set; }
        public string po_status_description { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
}
