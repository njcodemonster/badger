using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericModals.Models
{
    public class ProductPhotoshootRep
    {
        public int product_shoot_status_id  { get; set; }
        public int product_id { get; set; }
        public string product_vendor_image { get; set; }
        public string sku_family { get; set; }
        public string vendor_name { get; set; }
        public string product_name { get; set; }
        public string photoshoot_id { get; set; }
        public string username { get; set; }
        public string color { get; set; }
        public string po_id { get; set; }
        public string po_status { get; set; }
        public int vendor_id { get; set; }

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
    public class ProductPhotoshootSendToEditorPagerList
    {
        public IEnumerable<ProductPhotoshootRep> photoshootSendToEditor { get; set; }
    }

    public class ProductPhotoshootSummaryRep
    {
        public int photoshoot_id { get; set; }
        public int styles { get; set; }
        public double scheduled_date { get; set; }
        public int model_id { get; set; }
        public int product_shoot_status_id{ get; set; }
    }
    public class ProductPhotoshootSummaryPagerList
    {
        public IEnumerable<ProductPhotoshootSummaryRep> productPhotoshootSummary { get; set; }
    }

}
