using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace badgerApi.Models
{
    public class VendorCountAndList
    {
        public IEnumerable<Vendor> vendors { get; set; }
        public string Count { get; set; }

    }
}
