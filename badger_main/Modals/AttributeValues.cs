using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class AttributeValues
    {
        public int ValueId { get; set; }
        public int? AttributeId { get; set; }
        public string Value { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public double CreatedAt { get; set; }
        public double UpdatedAt { get; set; }
    }
}
