﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GenericModals.Models
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
