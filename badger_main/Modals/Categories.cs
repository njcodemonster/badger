using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("categories")]
    public partial class Categories
    {
        public int category_id { get; set; }
        public int category_type { get; set; }
        public string category_name { get; set; }
        public int category_parent_id { get; set; }
    }
}
