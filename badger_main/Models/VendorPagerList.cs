using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace badgerApi.Models
{
   
    public class VendorInfo
    {
        public int vendor_id { get; set; }
        public int vendor_type { get; set; }
        public string vendor_name { get; set; }
        public string vendor_code { get; set; }
        public string vendor_description { get; set; }
        public string last_order { get; set; }
        public int order_count { get; set; }
        public string Count { get; set; }

    }
    public class VendorPagerList
    {
        public IEnumerable<VendorInfo> vendorInfo { get; set; }
        public string Count { get; set; }
    }
}
