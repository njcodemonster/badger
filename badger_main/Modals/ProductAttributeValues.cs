﻿using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    [Table("product_attribute_values")]
    public partial class ProductAttributeValues
    {
        public long product_attribute_value_id { get; set; }
        public int product_id { get; set; }
        public int attribute_id { get; set; }
        public int value_id { get; set; }
    }
}
