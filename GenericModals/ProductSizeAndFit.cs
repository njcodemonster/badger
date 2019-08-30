using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("product_size_and_fit")]
    public partial class ProductSizeAndFit
    {
        [Key]
        public int size_and_fit_id { get; set; }
        public string size_and_fit_name { get; set; }
        public double created_at { get; set; }
    }
}
