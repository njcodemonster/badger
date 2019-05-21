using System;
using System.Collections.Generic;

namespace badger_view.Models
{
   
    public partial class VendorContactPerson
    {
        public int contact_id { get; set; }
        public int vendor_id { get; set; }
        public string first_name { get; set; }
        public string full_name { get; set; }
        public Boolean main { get; set; }
        public string phone1 { get; set; }
        public string phone2 { get; set; }
        public string email { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
}
