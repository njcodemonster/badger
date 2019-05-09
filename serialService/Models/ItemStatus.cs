using System;
using System.Collections.Generic;

namespace itemService.Models
{
    public partial class ItemStatus
    {
        public int ItemStatusId { get; set; }
        public string Description { get; set; }
        public double CreatedAt { get; set; }
        public double UpdatedAt { get; set; }
    }
}
