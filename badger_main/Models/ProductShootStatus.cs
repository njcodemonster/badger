﻿using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("product_shoot_status")] 
    public partial class ProductShootStatus
    {
        [Key]
        public int product_shoot_status_id { get; set; }
        public int product_shoot_status_name { get; set; }
        public int product_shoot_description { get; set; }
    }
}
