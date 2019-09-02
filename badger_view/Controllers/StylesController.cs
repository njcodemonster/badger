using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using badger_view.Helpers;
using GenericModals.Models;
using System.Dynamic;
using System.Web;
using Microsoft.Extensions.Logging;
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
        private ILoginHelper _ILoginHelper;
        private readonly IConfiguration _config;
        private BadgerApiHelper _BadgerApiHelper;
        ILoggerFactory _loggerFactory;

        //private IItemServiceHelper _itemService
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        private String UploadPath = "";
        public StylesController(IConfiguration config, ILoginHelper LoginHelper, ILoggerFactory loggerFactory)
        {
            _config = config;
            _ILoginHelper = LoginHelper;
            UploadPath = _config.GetValue<string>("UploadPath:path");
            _loggerFactory = loggerFactory;


        }
        private void SetBadgerHelper()
        {
            if (_BadgerApiHelper == null)
            {
                _BadgerApiHelper = new BadgerApiHelper(_config);
            }
        }


        /*
        Developer: ubaid
        Date:5-7-19
        Action:get HTML Form (Style vendor image) from addstyle JS and pass the data to API /product/updatespecific/{product_id}
        URL: /styles/newdoc
        Input: HTML form image and new product_id
        output: file path (upload folder)
        */
        [HttpPost("/styles/newdoc")]
        public async Task<String> CreateNewStyleDoc(StyleFileData StyleFileData)
        {
            try
            {
                string Fill_path = "";
                string product_id = StyleFileData.product_id;
                SetBadgerHelper();

                if (StyleFileData != null && StyleFileData.StyleImage != null)
                {
                    Fill_path = StyleFileData.StyleImage.FileName;
                    Fill_path = UploadPath + Fill_path;
                    using (var stream = new FileStream(Fill_path, FileMode.Create))
                    {
                        await StyleFileData.StyleImage.CopyToAsync(stream);
                    }
                    JObject product_doc = new JObject();

                    product_doc.Add("product_vendor_image", StyleFileData.StyleImage.FileName);

                    product_id = await _BadgerApiHelper.GenericPutAsyncString<String>(product_doc.ToString(Formatting.None), "/product/updatespecific/" + product_id);


                }

                return Fill_path;
            }
            catch (Exception ex)
            {
                return "0";
            }
        }

        /*
        Developer: ubaid
        Date:5-7-19
        Action:get HTML Form (New Styles Data) from addstyle JS and pass the data to multiple API functions 
        /product/create , attribute crete, product attreibute create, product attribute values create , SKU create
        URL: /styles/create
        Input: HTML form with the data of new product
        output: New product id
        */

        [HttpPost("/styles/create")]
        public async Task<String> CreateNewStyle([FromBody]   JObject json)
        {
            SetBadgerHelper();
            int user_id = Int32.Parse(await _ILoginHelper.GetLoginUserId());

            String product_id = "";

            JObject product = new JObject();
            JObject vendorProduct = new JObject();

            JObject allData = JObject.Parse(json.ToString());

            JArray vendor_style_sku_data = new JArray();
            JArray product_subtype_ids = new JArray();
            if (allData["vendor_style_sku"] != null)
            {
                vendor_style_sku_data = (JArray)allData["vendor_style_sku"];
            }

            if (allData["product_subtype_ids"] != null)
            {
                product_subtype_ids = (JArray)allData["product_subtype_ids"];
            }



            Int32 product_id_current = json.Value<Int32>("product_id");
            Int32 po_id = json.Value<Int32>("po_id");
            Int32 vendor_id = json.Value<Int32>("vendor_id");
            string product_name = json.Value<string>("product_name");
            bool IsLineItemExists = json.Value<bool>("IsLineItemExists");

            string first_sku_family = "";
            string first_sku = string.Empty;
            if (vendor_style_sku_data.Count > 0)
            {
                string first_style_vendor_size = vendor_style_sku_data[0].Value<string>("style_vendor_size");
                string first_style_size = vendor_style_sku_data[0].Value<string>("style_size");
                string first_style_qty = vendor_style_sku_data[0].Value<string>("style_qty");
                first_sku = vendor_style_sku_data[0].Value<string>("style_sku");

                if (first_sku != null && first_sku.Count() > 0)
                {
                    first_sku_family = first_sku.Split('-')[0];
                }
            }

            string vendor_color_name = json.Value<string>("vendor_color_name");
            string product_cost = json.Value<string>("product_cost");
            string product_name_no = json.Value<string>("product_name_no");
            string product_color_code = json.Value<string>("product_color_code");

            string product_retail = json.Value<string>("product_retail");
            string product_url_handle = product_name.Replace(' ', '-').ToLower();
            string product_type_id = json.Value<string>("product_type_id");



            //default values FIXED hardcoded

            int size_and_fit_id = 0;
            int wash_type_id = 0;
            int product_discount = 20;
            int published_status = 0;
            int is_on_site_status = 0;
            String attr_value_id_color = "15"; // blank entry for color
            int color_attribute_id = 1; // color attribute id


            // item insert default values
            string item_status_id = "1"; // Not Recieved

            int ra_status = 0;
            int barcode = 0;
            int slot_number = 0;
            int bag_code = 0;



            product.Add("vendor_id", vendor_id);
            product.Add("product_name", product_name);
            product.Add("vendor_color_name", vendor_color_name);
            product.Add("product_cost", product_cost);
            product.Add("product_retail", product_retail);
            product.Add("product_url_handle", product_url_handle);
            product.Add("product_type_id", product_type_id);

            product.Add("product_description", "");
            product.Add("sku_family", first_sku_family);
            product.Add("size_and_fit_id", size_and_fit_id);
            product.Add("wash_type_id", wash_type_id);
            product.Add("product_discount", product_discount);
            product.Add("published_status", published_status);
            product.Add("is_on_site_status", is_on_site_status);

            product.Add("created_by", user_id);
            product.Add("created_at", _common.GetTimeStemp());


            vendorProduct.Add("vendor_id", vendor_id);
            vendorProduct.Add("vendor_color_code", product_color_code);
            vendorProduct.Add("vendor_color_name", vendor_color_name);
            vendorProduct.Add("vendor_product_code", product_name_no);
            vendorProduct.Add("vendor_product_name", product_name);
            vendorProduct.Add("created_by", user_id);
            vendorProduct.Add("created_at", _common.GetTimeStemp());


            if (product_id_current > 0)
            {
                product_id = product_id_current.ToString(); //update on selected product id

                // add new product in product table
                var a = await _BadgerApiHelper.GenericPutAsyncString<String>(product.ToString(Formatting.None), "/product/updatespecific/" + product_id);

                if (!string.IsNullOrEmpty(product_id)) ;
                {   //update vendor product
                    vendorProduct.Add("product_id", Convert.ToInt64(product_id));
                    var result = await _BadgerApiHelper.GenericPutAsyncString<String>(vendorProduct.ToString(Formatting.None), "/vendor/vendorproductUpdatespecific");
                }

            }
            else
            {
                try
                {
                    // add new product in product table
                    product_id = await _BadgerApiHelper.GenericPostAsyncString<String>(product.ToString(Formatting.None), "/product/create");

                    if (!string.IsNullOrEmpty(product_id))
                    {
                        vendorProduct.Add("product_id", Convert.ToInt64(product_id));
                        // add new vendor product in vendor product table
                        var temp_product_id = await _BadgerApiHelper.GenericPostAsyncString<String>(vendorProduct.ToString(Formatting.None), "/vendor/createvendorproduct");

                    }

                }
                catch (Exception ex)
                {
                    var logger = _loggerFactory.CreateLogger("internal_error_log");
                    logger.LogInformation("Problem happened in making new Product, no product id genrated ");

                }
            }

            if (Int32.Parse(product_id) > 1)
            {
                JObject productPhotoshoot = new JObject();
                productPhotoshoot.Add("photoshoot_id", 0);
                productPhotoshoot.Add("product_id", product_id);
                productPhotoshoot.Add("product_shoot_status_id", 0);
                productPhotoshoot.Add("updated_by", 0);
                productPhotoshoot.Add("updated_at", 0);
                productPhotoshoot.Add("created_by", user_id);
                productPhotoshoot.Add("created_at", _common.GetTimeStemp());
                await _BadgerApiHelper.GenericPostAsyncString<String>(productPhotoshoot.ToString(Formatting.None), "/photoshoots/addNewPhotoshootProduct");


                JObject product_attr_value_color = new JObject();
                product_attr_value_color.Add("product_id", Int32.Parse(product_id));
                product_attr_value_color.Add("attribute_id", color_attribute_id);
                product_attr_value_color.Add("value_id", attr_value_id_color);

                product_attr_value_color.Add("created_by", user_id);
                String product_attribute_value_id_color = await _BadgerApiHelper.GenericPostAsyncString<String>(product_attr_value_color.ToString(Formatting.None), "/product/createAttributesValues");


                JObject product_attribute_obj_color = new JObject();
                product_attribute_obj_color.Add("product_id", Int32.Parse(product_id));
                product_attribute_obj_color.Add("attribute_id", color_attribute_id);
                product_attribute_obj_color.Add("sku", "");
                product_attribute_obj_color.Add("value_id", attr_value_id_color);

                product_attribute_obj_color.Add("created_by", user_id);
                //product_attribute_obj.Add("created_at", _common.GetTimeStemp()); need to create in DB
                String product_attribute_id_color = await _BadgerApiHelper.GenericPostAsyncString<String>(product_attribute_obj_color.ToString(Formatting.None), "/product/createProductAttribute");



                JObject used_in_obj = new JObject();
                used_in_obj.Add("product_id", product_id);
                used_in_obj.Add("po_id", po_id);
                used_in_obj.Add("created_at", _common.GetTimeStemp());
                String used_in_id = await _BadgerApiHelper.GenericPostAsyncString<String>(used_in_obj.ToString(Formatting.None), "/product/createUsedIn");
            }

            for (int i = 0; i < product_subtype_ids.Count(); i++)
            {
                string category_id = product_subtype_ids[i].Value<string>("category_id");
                string action = product_subtype_ids[i].Value<string>("action");

                JObject productCategories = new JObject();
                productCategories.Add("product_id", product_id);
                productCategories.Add("category_id", category_id);
                productCategories.Add("action", action);
                productCategories.Add("created_by", user_id);
                productCategories.Add("created_at", _common.GetTimeStemp());

                var temp_product_category_id = await _BadgerApiHelper.GenericPostAsyncString<String>(productCategories.ToString(Formatting.None), "/product/UpdateProductCategory");

            }

            //Checking If line items need to be deleted and is it a possibility to delete line items , if not - stop everything and return
            if (IsLineItemExists)
            {
                var OldSkuList = vendor_style_sku_data.Where(x => x.Value<bool>("IsNewSku") == false && x.Value<int>("style_qty") < x.Value<int>("original_qty")).ToList();
                for (int i = 0; i < OldSkuList.Count; i++)
                {
                    string sku = OldSkuList[i].Value<string>("style_sku");
                    int style_qty = vendor_style_sku_data[i].Value<int>("style_qty");
                    var itemCount = await _BadgerApiHelper.GenericGetsAsync("/product/getitems/" + po_id + "/" + sku);

                    if (int.Parse(itemCount) < style_qty)
                    {
                        // -3 for indicating error in javascript.
                        return "-3";
                    }


                }
            }


            for (int i = 0; i < vendor_style_sku_data.Count; i++)
            {

                string style_vendor_size = vendor_style_sku_data[i].Value<string>("style_vendor_size");
                string style_size = vendor_style_sku_data[i].Value<string>("style_size");
                string style_qty = vendor_style_sku_data[i].Value<string>("style_qty");
                string sku = vendor_style_sku_data[i].Value<string>("style_sku");
                bool IsNewSku = vendor_style_sku_data[i].Value<bool>("IsNewSku");
                int original_qty = vendor_style_sku_data[i].Value<int>("original_qty");

                string sku_family = "";
                if (first_sku == null)
                {
                    break;
                }
                if (first_sku.Count() > 0)
                {
                    sku_family = sku.Split('-')[0];
                }



                /// sizes attribute add from here
                JObject product_attr = new JObject();
                Int16 attribute_id = Int16.Parse(style_size);


                product_attr.Add("attribute_id", attribute_id);
                product_attr.Add("product_id", Int32.Parse(product_id));
                product_attr.Add("value", style_vendor_size);

                product_attr.Add("created_by", user_id);
                product_attr.Add("created_at", _common.GetTimeStemp());
                String attr_value_id = await _BadgerApiHelper.GenericPostAsyncString<String>(product_attr.ToString(Formatting.None), "/attributevalues/create");


                JObject product_attr_value = new JObject();
                product_attr_value.Add("product_id", Int32.Parse(product_id));
                product_attr_value.Add("attribute_id", attribute_id);
                product_attr_value.Add("value_id", attr_value_id);

                product_attr_value.Add("created_by", user_id);
                // product_attr_value.Add("created_at", _common.GetTimeStemp()); need to create in DB
                String product_attribute_value_id = await _BadgerApiHelper.GenericPostAsyncString<String>(product_attr_value.ToString(Formatting.None), "/product/createAttributesValues");

                JObject product_attribute_obj = new JObject();
                product_attribute_obj.Add("product_id", Int32.Parse(product_id));
                product_attribute_obj.Add("attribute_id", attribute_id);
                product_attribute_obj.Add("sku", sku);
                product_attribute_obj.Add("value_id", attr_value_id);
                product_attribute_obj.Add("created_by", user_id);
                //product_attribute_obj.Add("created_at", _common.GetTimeStemp()); need to create in DB
                String product_attribute_id = await _BadgerApiHelper.GenericPostAsyncString<String>(product_attribute_obj.ToString(Formatting.None), "/product/createProductAttribute");

                //// size attribute ends here



                JObject Sku_obj = new JObject();
                Sku_obj.Add("sku", sku);
                Sku_obj.Add("vendor_id", vendor_id);
                Sku_obj.Add("product_id", Int32.Parse(product_id));
                String sku_id = await _BadgerApiHelper.GenericPostAsyncString<String>(Sku_obj.ToString(Formatting.None), "/sku/create");

                JObject lineitem_obj = new JObject();
                lineitem_obj.Add("po_id", po_id);
                lineitem_obj.Add("vendor_id", vendor_id);
                lineitem_obj.Add("sku", sku);
                lineitem_obj.Add("product_id", Int32.Parse(product_id));
                lineitem_obj.Add("line_item_cost", product_cost);
                lineitem_obj.Add("line_item_retail", product_retail);
                lineitem_obj.Add("line_item_type_id", product_type_id);
                lineitem_obj.Add("line_item_ordered_quantity", style_qty);

                JObject items = new JObject();
                items.Add("barcode", barcode); // 0
                items.Add("slot_number", slot_number); // 0
                items.Add("bag_code", bag_code); // 0
                items.Add("item_status_id", item_status_id); // 1
                items.Add("ra_status", ra_status); // 0
                items.Add("sku", sku);
                items.Add("sku_id", sku_id);
                items.Add("sku_family", sku_family);
                items.Add("product_id", product_id);
                items.Add("vendor_id", vendor_id);
                items.Add("PO_id", po_id);
                items.Add("created_by", user_id);
                items.Add("original_qty", original_qty);
                items.Add("created_at", _common.GetTimeStemp());


                if (!IsLineItemExists || IsNewSku)
                {
                    String item_id = await _BadgerApiHelper.GenericPostAsyncString<String>(items.ToString(Formatting.None), "/product/createitems/" + style_qty);
                    String line_item_id = await _BadgerApiHelper.GenericPostAsyncString<String>(lineitem_obj.ToString(Formatting.None), "/product/createLineitems");
                }
                else
                {
                    int _style_qty = int.Parse(style_qty);
                    if (original_qty != _style_qty)
                    {


                        //Adding new items in existing Line items
                        String item_id = await _BadgerApiHelper.GenericPostAsyncString<String>(items.ToString(Formatting.None), "/product/updateitems/" + style_qty);
                        String line_item_id = await _BadgerApiHelper.GenericPostAsyncString<String>(lineitem_obj.ToString(Formatting.None), "/product/updateLineitems");

                    }


                }

            }
            return product_id;

        }

        /*
        Developer: Rizvan ali
        Date:5-7-19
        Action:get HTML Form (New Styles Data) from addstyle JS and delete the complete the data of given product if it is 
        not in used in other PO if Exist then just delete from item 
        Input: HTML form with the data of product
        output: status of deletion
        */
        [HttpGet("/styles/deleteFromPO/{product_id}")]
        public async Task<bool> DeleteStyle(int product_id)
        {
            SetBadgerHelper();
            var response = await _BadgerApiHelper.GenericGetAsync<bool>("/product/delete/" + product_id.ToString());
            return response;
        }
    }
}