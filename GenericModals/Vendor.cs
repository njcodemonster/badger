using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
   
    public partial class Vendor
    {
    
        public int vendor_id { get; set; }
        public int vendor_type { get; set; }
        public string vendor_name { get; set; }
        public string vendor_description { get; set; }
        public string corp_name { get; set; }
        public string statement_name { get; set; }
        public string vendor_code { get; set; }
        public string our_customer_number { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public int active_status { get; set; }
        public double vendor_since { get; set; }
        public string logo { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
        public int has_note { get; set; }
    }
}
