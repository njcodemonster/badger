using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class ProductPageDetailTypes
    {
        public int ProductDetailTypeId { get; set; }
        public string ProductDetailName { get; set; }
        public string ProductDetailDescription { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public double? CreatedAt { get; set; }
        public double UpdatedAt { get; set; }
    }
}
