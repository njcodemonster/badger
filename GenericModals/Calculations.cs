using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("calculation_values")]
    public partial class CalculationValues
    {

        [Key]
        public int calculation_value_id { get; set; }
        public int calculation_id { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public string reffrence_id { get; set; }
        public double value { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }


    }
}
