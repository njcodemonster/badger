using System;
using System.Collections.Generic;

namespace GenericModals.Models
{

    public partial class VendorNote
    {
        public int note_id { get; set; }
        public int ref_id { get; set; }
        public int note_type_id { get; set; }
        public string note { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
}
