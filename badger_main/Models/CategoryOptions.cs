using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("category_options")]
    public partial class CategoryOptions
    {
        [Key]
        public int category_option_id { get; set; }
        public int category_id { get; set; }
        public int attribute_id { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
}
