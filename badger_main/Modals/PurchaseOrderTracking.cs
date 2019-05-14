using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class PurchaseOrderTracking
    {
        public int PoTrackingId { get; set; }
        public int PoId { get; set; }
        public int TrackingNumber { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }
    }
}
