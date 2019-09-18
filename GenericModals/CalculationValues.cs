using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("calculations")]
    public partial class Calculations
    {

        [Key]
        public string calculation_name { get; set; }
        public string description { get; set; }
        public int? calculation_id { get; set; }
        public int? created_by { get; set; }
        public double? calculation_date { get; set; }
        public double? created_at { get; set; }
        public double? updated_at { get; set; }


    }
}
