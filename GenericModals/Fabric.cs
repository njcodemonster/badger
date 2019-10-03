using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace GenericModals.Models
{
    
    public  class Fabric
    {
        public string name { get; set; }
        public decimal percentage { get; set; }
        public int value_id { get; set; }
        public string attribute_id { get; set; }
        public int product_id { get; set; }
        public bool toDelete { get; set; }

    }
}
