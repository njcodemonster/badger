using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class ProductPageDetails
    {
        public int ProductPageDetailId { get; set; }
        public int ProductId { get; set; }
        public int ProductDetailType { get; set; }
        public string ProductDetailValue { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public double CreatedAt { get; set; }
        public double? UpdatedAt { get; set; }
    }
}
