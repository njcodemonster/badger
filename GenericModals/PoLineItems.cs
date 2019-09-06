using System;
using System.Collections.Generic;
using System.Text;
using GenericModals.Models;

namespace GenericModals
{
    public class POLineItems
    {
        public int? vendor_id { get; set; }
        public int? po_id { get; set; }
        public string product_id { get; set; }
        public decimal product_cost { get; set; }
        public int? wash_type_id { get; set; }
        public string vendor_color_name { get; set; }
        public string product_name { get; set; }
        public string product_vendor_image { get; set; }
        public int? line_item_id { get; set; }
        public string sku { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal weight { get; set; }
        public int? product_attribute_id { get; set; }
        public List<Items> EndItems { get; set; }

    }
    public class POLineItemsView
    {
        public List<POLineItems> FirstPOInfor { get; set; }
        public object AllItemStatus { get; set; }
        public object AllRaStatus { get; set; }
        public object AllWashTypes { get; set; }
    }
}
