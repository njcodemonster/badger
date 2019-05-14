using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class Attributes
    {
        public int AttributeId { get; set; }
        public string Attribute { get; set; }
        public int AttributeTypeId { get; set; }
        public string AttributeDisplayName { get; set; }
        public string DataType { get; set; }
        public int CreatedAt { get; set; }
    }
}
