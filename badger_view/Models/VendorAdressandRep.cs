using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using badger_view.Models;
namespace badger_view.Models
{
    public class VenderAdressandRep
    {
        public Vendor vendor { get; set; }
        public IEnumerable<VendorAddress> Addresses { get; set; }
        public IEnumerable<VendorContactPerson> Reps { get; set; }
    }
}
