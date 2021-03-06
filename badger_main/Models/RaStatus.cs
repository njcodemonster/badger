﻿using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("ra_status")]
    public partial class RaStatus
    {
        [Key]
        public int ra_status_id { get; set; }
        public string ra_status_name { get; set; }
        public string ra_status_description { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
        
    }
}
