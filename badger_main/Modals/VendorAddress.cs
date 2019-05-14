using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class VendorAddress
    {
        public int VendorAddressId { get; set; }
        public int VendorId { get; set; }
        public string VendorStreet { get; set; }
        public string VendorSuiteNumber { get; set; }
        public string VendorCity { get; set; }
        public long VendorZip { get; set; }
        public string VendorState { get; set; }
        public double CreatedAt { get; set; }
        public double UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
    }
}
