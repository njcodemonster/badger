using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("attribute_type")]
    public partial class AttributeType
    {
        public int attribute_type_id { get; set; }
        public string attribute_type_name { get; set; }
        public string attribute_type_description { get; set; }
        public int created_at { get; set; }
        public int updated_at { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
    }
}
