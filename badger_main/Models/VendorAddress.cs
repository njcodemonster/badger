﻿using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("vendor_address")]
    public partial class VendorAddress
    {
        [Key]
        public int vendor_address_id { get; set; }
        public int vendor_id { get; set; }
        public string vendor_street { get; set; }
        public string vendor_suite_number { get; set; }
        public string vendor_city { get; set; }
        public int vendor_zip { get; set; }
        public string vendor_state { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
    }
}
