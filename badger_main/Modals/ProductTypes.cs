using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("product_types")]
    public partial class ProductTypes
    {
        public int product_type_id { get; set; }
        public string product_type { get; set; }
    }
}
