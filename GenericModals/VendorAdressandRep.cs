using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericModals.Models;
namespace GenericModals.Models
{
    public class VenderAdressandRep
    {
        public Vendor vendor { get; set; }
        public IEnumerable<VendorAddress> Addresses { get; set; }
        public IEnumerable<VendorContactPerson> Reps { get; set; }
    }
}
