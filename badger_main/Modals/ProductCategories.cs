using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class ProductCategories
    {
        public int ProductCategoryId { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public double CreatedAt { get; set; }
        public double? UpdatedAt { get; set; }
    }
}
