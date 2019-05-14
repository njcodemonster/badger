using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class CategoryOptions
    {
        public int CategoryOptionId { get; set; }
        public int CategoryId { get; set; }
        public int AttributeId { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public double CreatedAt { get; set; }
        public double UpdatedAt { get; set; }
    }
}
