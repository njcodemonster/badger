using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class RaStatus
    {
        public int RaStatusId { get; set; }
        public string RaStatusName { get; set; }
        public string RaStatusDescription { get; set; }
        public int CreatedAt { get; set; }
        public int UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }
}
