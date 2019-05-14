using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class ProductUsedIn
    {
        public int ProductId { get; set; }
        public int PoId { get; set; }
        public double CreatedAt { get; set; }
        public double UpdatedAt { get; set; }
    }
}
