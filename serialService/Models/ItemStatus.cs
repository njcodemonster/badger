using System;
using System.Collections.Generic;

namespace itemService_entity.Models
{
    public partial class ItemStatus
    {
        public ItemStatus()
        {
            Items = new HashSet<Items>();
        }

        public int ItemStatusId { get; set; }
        public string Description { get; set; }
        public double CreatedAt { get; set; }
        public double UpdatedAt { get; set; }

        public virtual ICollection<Items> Items { get; set; }
    }
}
