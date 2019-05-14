using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class PurchaseOrderStatus
    {
        public int PoStatusId { get; set; }
        public int PoStatusName { get; set; }
        public int PoStatusDescription { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }
    }
}
