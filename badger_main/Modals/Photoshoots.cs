using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class Photoshoots
    {
        public int PhotoshootId { get; set; }
        public string PhotoshootName { get; set; }
        public int ModelId { get; set; }
        public double ShootStartDate { get; set; }
        public double ShootEndDate { get; set; }
        public int ActiveStatus { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public double CreatedAt { get; set; }
        public double UpdatedAt { get; set; }
    }
}
