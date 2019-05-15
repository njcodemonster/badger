using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("photoshoots")]
    public partial class Photoshoots
    {
        [Key]
        public int photoshoot_id { get; set; }
        public string photoshoot_name { get; set; }
        public int model_id { get; set; }
        public double shoot_start_date { get; set; }
        public double shoot_end_date { get; set; }
        public int active_status { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
}
