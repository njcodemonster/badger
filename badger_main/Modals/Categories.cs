using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class Categories
    {
        public int CategoryId { get; set; }
        public int CategoryType { get; set; }
        public string CategoryName { get; set; }
        public int CategoryParentId { get; set; }
    }
}
