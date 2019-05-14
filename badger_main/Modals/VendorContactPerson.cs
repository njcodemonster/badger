using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class VendorContactPerson
    {
        public int ContactId { get; set; }
        public int VendorId { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public byte Main { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Email { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public double CreatedAt { get; set; }
        public double UpdatedAt { get; set; }
    }
}
