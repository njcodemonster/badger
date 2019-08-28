using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("attributes")]
    public partial class Attributes
    {
        [Key]
        public int attribute_id { get; set; }
        public string attribute { get; set; }
        public int attribute_type_id { get; set; }
        public string attribute_display_name { get; set; }
        public string data_type { get; set; }
        public double created_at { get; set; }
    }
}
