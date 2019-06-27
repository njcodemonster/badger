using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using badger_view.Helpers;
using badger_view.Models;
using System.Dynamic;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace badger_view.Controllers
{
    public class StyleFileData
    {
        public IFormFile StyleImage { get; set; }
        public string product_id { get; set; }
    }
    public class StylesController : Controller
    {
        
        private readonly IConfiguration _config;
        private BadgerApiHelper _BadgerApiHelper;
        //private IItemServiceHelper _itemService
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        private String UploadPath = "";
        public StylesController(IConfiguration config)
        {
            _config = config;
            UploadPath = _config.GetValue<string>("UploadPath:path");

        }
        private void SetBadgerHelper()
        {
            if (_BadgerApiHelper == null)
            {
                _BadgerApiHelper = new BadgerApiHelper(_config);
            }
        }

        [HttpPost("/styles/newdoc")]
        public async Task<String> CreateNewStyleDoc(StyleFileData StyleFileData)
        {
            try
            {
                string product_id = StyleFileData.product_id;
                SetBadgerHelper();
                

                string Fill_path = StyleFileData.StyleImage.FileName;
                Fill_path = UploadPath + Fill_path;
                using (var stream = new FileStream(Fill_path, FileMode.Create))
                {
                    await StyleFileData.StyleImage.CopyToAsync(stream);
                }
                JObject product_doc = new JObject();
                
                product_doc.Add("product_vendor_image", StyleFileData.StyleImage.FileName);
                
                product_id = await _BadgerApiHelper.GenericPutAsyncString<String>(product_doc.ToString(Formatting.None), "/product/updatespecific/"+ product_id); 


                return Fill_path;
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
        [HttpPost("/styles/create")]
        public  async Task<String> CreateNewStyle([FromBody]   JObject json)
        {
            SetBadgerHelper();

            try
            {
                JObject product = new JObject();
            List<JObject> style_attr = new List<JObject>();

            string product_name = json.Value<string>("product_name");
                
            string style_qty = json.Value<string>("style_qty");
            string sku = json.Value<string>("style_sku");
            String sku_family = sku.Split('-')[0];


            product.Add("product_name", product_name);
            product.Add("vendor_color_name", json.Value<string>("vendor_color_name"));
            product.Add("product_cost", json.Value<string>("product_cost"));
            product.Add("product_retail", json.Value<string>("product_retail"));
            product.Add("product_url_handle", product_name.Replace(' ','-').ToLower() );
            product.Add("product_type_id", json.Value<string>("product_type_id"));

            product.Add("product_description", "");
            product.Add("sku_family", sku_family);
            product.Add("size_and_fit_id",0);
            product.Add("wash_type_id", 0);
            product.Add("product_discount", 20);
            product.Add("published_status", 0);
            product.Add("is_on_site_status", 0);


            product.Add("created_by", 2);
            product.Add("created_at", _common.GetTimeStemp());
            String product_id = await _BadgerApiHelper.GenericPostAsyncString<String>(product.ToString(Formatting.None), "/product/create");

            JObject product_attr = new JObject();
            Int16 attribute_id = 3;
            if (json.Value<string>("style_size") == "Extra") { attribute_id = 3; }
            if (json.Value<string>("style_size") == "Small") { attribute_id = 4; }
            if (json.Value<string>("style_size") == "Medium") { attribute_id = 5; }
            if (json.Value<string>("style_size") == "Large") { attribute_id = 6; }


            product_attr.Add("attribute_id", attribute_id);
            product_attr.Add("product_id", Int32.Parse( product_id));
            product_attr.Add("value", json.Value<string>("style_size"));

            product_attr.Add("created_by", 2);
            product_attr.Add("created_at", _common.GetTimeStemp());
            String attr_value_id = await _BadgerApiHelper.GenericPostAsyncString<String>(product_attr.ToString(Formatting.None), "/attributevalues/create");


            JObject product_attr_value = new JObject();
            product_attr_value.Add("product_id", Int32.Parse(product_id));
            product_attr_value.Add("attribute_id", attribute_id);
            product_attr_value.Add("value_id", attr_value_id);

            product_attr_value.Add("created_by", 2);
            // product_attr_value.Add("created_at", _common.GetTimeStemp()); need to create in DB
            String product_attribute_value_id = await _BadgerApiHelper.GenericPostAsyncString<String>(product_attr_value.ToString(Formatting.None), "/product/createAttributesValues");

            JObject product_attribute_obj = new JObject();
            product_attribute_obj.Add("product_id", Int32.Parse(product_id));
            product_attribute_obj.Add("attribute_id", attribute_id);
            product_attribute_obj.Add("sku", sku_family);

            product_attribute_obj.Add("created_by", 2);
            //product_attribute_obj.Add("created_at", _common.GetTimeStemp()); need to create in DB
            String product_attribute_id = await _BadgerApiHelper.GenericPostAsyncString<String>(product_attribute_obj.ToString(Formatting.None), "/product/createProductAttribute");

            JObject Sku_obj = new JObject();
            Sku_obj.Add("sku", sku);
            Sku_obj.Add("vendor_id", attribute_id);
            Sku_obj.Add("product_id", Int32.Parse(product_id));
            String sku_id = await _BadgerApiHelper.GenericPostAsyncString<String>(Sku_obj.ToString(Formatting.None), "/product/createSku");


            JObject lineitem_obj = new JObject();
            lineitem_obj.Add("po_id", 1);
            lineitem_obj.Add("vendor_id", 1);
            lineitem_obj.Add("sku", sku);
            lineitem_obj.Add("product_id", Int32.Parse(product_id));
            lineitem_obj.Add("line_item_cost", json.Value<string>("product_cost"));
            lineitem_obj.Add("line_item_retail", json.Value<string>("product_retail"));
            lineitem_obj.Add("line_item_type_id",1);
            lineitem_obj.Add("line_item_ordered_quantity", style_qty);
            String line_item_id = await _BadgerApiHelper.GenericPostAsyncString<String>(lineitem_obj.ToString(Formatting.None), "/product/createLineitems");




                JObject items = new JObject();
            items.Add("barcode", 0);
            items.Add("slot_number", 0);
            items.Add("bag_code", 0);
            items.Add("item_status_id", 1);
            items.Add("ra_status", 0);
            items.Add("sku", sku);
            items.Add("sku_id", sku_id);
            items.Add("sku_family", sku_family);
            items.Add("product_id", product_id);
            items.Add("vendor_id", 0);
            items.Add("PO_id", 1);
            items.Add("created_by", 2);
            items.Add("created_at", _common.GetTimeStemp());


            String item_id = await _BadgerApiHelper.GenericPostAsyncString<String>(items.ToString(Formatting.None), "/product/create/items");







            return product_id;
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
    }
}