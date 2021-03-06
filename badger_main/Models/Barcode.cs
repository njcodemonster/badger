﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericModals.Models
{
    public class Barcode
    {
        public int id { get; set; }
        public string size { get; set; }
        public int barcode_from { get; set; }
        public int barcode_to { get; set; }
        public int updated_by { get; set; }
        public int created_by { get; set; }
        public double updated_at { get; set; }
    }
}
