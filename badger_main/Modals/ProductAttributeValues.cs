using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class ProductAttributeValues
    {
        public long ProductAttributeValueId { get; set; }
        public int ProductId { get; set; }
        public int AttributeId { get; set; }
        public int ValueId { get; set; }
    }
}
