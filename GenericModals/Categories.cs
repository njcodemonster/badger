using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GenericModals.Models
{
    [Table("categories")]
    public partial class Categories
    {
        [Key]
        public int category_id { get; set; }
        public int category_type { get; set; }
        public string category_name { get; set; }
        public int category_parent_id { get; set; }
    }
}
