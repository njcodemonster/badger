using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("vendor_types")]
    public partial class VendorTypes
    {
        [Key]
        public int vendor_type_id { get; set; }
        public string vendor_type_description { get; set; }
    }
}
