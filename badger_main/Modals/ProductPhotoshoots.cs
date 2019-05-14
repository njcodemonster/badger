using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class ProductPhotoshoots
    {
        public int PhotoshootId { get; set; }
        public int ProductId { get; set; }
        public int ProductShootStatusId { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public double CreatedAt { get; set; }
        public double UpdatedAt { get; set; }
    }
}
