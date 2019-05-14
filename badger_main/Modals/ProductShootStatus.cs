using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("product_shoot_status")] 
    public partial class ProductShootStatus
    {
        public int product_shoot_status_id { get; set; }
        public int product_shoot_status_name { get; set; }
        public int product_shoot_description { get; set; }
    }
}
