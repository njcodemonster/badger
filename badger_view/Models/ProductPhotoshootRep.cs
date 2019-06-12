using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using badger_view.Models;
using badgerApi.Models;

namespace badger_view.Models
{
    public class ProductPhotoshootRep
    {
        public int product_shoot_status_id  { get; set; }
        public int product_id { get; set; }
        public string product_vendor_image { get; set; }
        public string sku_family { get; set; }
        public string vendor_name { get; set; }
    }
    public class ProductPhotoshootPagerList
    {
        public IEnumerable<ProductPhotoshootRep> photoshootsInfo { get; set; }
    }

    public class ProductPhotoshootInProgressRep
    {
        public string photoshoot_name  { get; set; }
        public int photoshoot_id  { get; set; }
        
    }
    public class ProductPhotoshootInProgressPagerList
    {
        public IEnumerable<ProductPhotoshootInProgressRep> photoshootsInprogress { get; set; }
    }
}
