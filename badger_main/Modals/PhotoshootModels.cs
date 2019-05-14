using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class PhotoshootModels
    {
        public int ModelId { get; set; }
        public string ModelName { get; set; }
        public string ModelHeight { get; set; }
        public string ModelEthnicity { get; set; }
        public string ModelHair { get; set; }
        public int ActiveStatus { get; set; }
        public double UpdatedAt { get; set; }
        public int UpdatedBy { get; set; }
        public int CreatedBy { get; set; }
        public double CreatedAt { get; set; }
    }
}
