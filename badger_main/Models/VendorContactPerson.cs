using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    [Table("vendor_contact_person")]
    public partial class VendorContactPerson
    {
        [Key]
        public int contact_id { get; set; }
        public int vendor_id { get; set; }
        public string first_name { get; set; }
        public string full_name { get; set; }
        public byte main { get; set; }
        public string phone1 { get; set; }
        public string phone2 { get; set; }
        public string email { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public double created_at { get; set; }
        public double updated_at { get; set; }
    }
}
