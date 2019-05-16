using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("size_and_fit_other_text")]
    public partial class SizeAndFitOtherText
    {
        [Key]
        public int product_id { get; set; }
        public string size_and_fit_other_text { get; set; }
        public int created_by { get; set; }
        public int? updated_by { get; set; }
        public double created_at { get; set; }
        public double? updated_at { get; set; }
    }
}
