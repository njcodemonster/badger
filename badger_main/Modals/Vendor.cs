using System;
using System.Collections.Generic;

namespace badgerApi.Models
{
    public partial class Vendor
    {
        public int VendorId { get; set; }
        public int VendorType { get; set; }
        public string VendorName { get; set; }
        public string VendorDescription { get; set; }
        public string CorpName { get; set; }
        public string StatementName { get; set; }
        public string VendorCode { get; set; }
        public string OurCustomerNumber { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int ActiveStatus { get; set; }
        public double VendorSince { get; set; }
        public double CreatedAt { get; set; }
        public double UpdatedAt { get; set; }
    }
}
