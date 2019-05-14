using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class ProductAttributes
    {
        public long ProductAttributeId { get; set; }
        public int ProductId { get; set; }
        public int AttributeId { get; set; }
        public int? Sku { get; set; }
        public double CreatedAt { get; set; }
        public double? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public int CreatedBy { get; set; }
    }
}
