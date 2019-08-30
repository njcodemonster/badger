using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("product_wash_types")]
    public partial class ProductWashTypes
    {
        [Key]
        public int wash_type_id { get; set; }
        public string wash_type { get; set; }
    }
}
