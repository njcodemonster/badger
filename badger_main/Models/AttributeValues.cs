using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("attribute_values")]
    public partial class AttributeValues
    {
        [Key]
        public int value_id { get; set; }
        public int? attribute_id { get; set; }
        public int? Product_id { get; set; }
        public string value { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
}
