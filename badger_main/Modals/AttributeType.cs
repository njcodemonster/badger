using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class AttributeType
    {
        public int AttributeTypeId { get; set; }
        public string AttributeTypeName { get; set; }
        public string AttributeTypeDescription { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }
}
