using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("product_photoshoots")]
    public partial class ProductPhotoshoots
    {
        [Key]
        public int photoshoot_id { get; set; }
        public int product_id { get; set; }
        public int product_shoot_status_id { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }

    public partial class ProductPhotoshootStatusUpdate
    {
        public int product_shoot_status_id { get; set; }
        public int updated_by { get; set; }
        public int updated_at { get; set; }
        
    }
}
