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
    public class NewStyleFileData
    {
        public IFormFile StyleImage { get; set; }
        public string product_id { get; set; }
    }
    public class NewStyleController : Controller
    {
        
        private readonly IConfiguration _config;
        private BadgerApiHelper _BadgerApiHelper;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        private String UploadPath = "";
        public NewStyleController(IConfiguration config)
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

        [HttpPost("/addstyle/newstyle_doc")]
        public async Task<String> CreateNewStyleDoc(NewStyleFileData StyleFileData)
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
        [HttpPost("addstyle/newstyle")]
        public  async Task<String> CreateNewStyle([FromBody]   JObject json)
        {
            SetBadgerHelper();
           
            JObject product = new JObject();
            List<JObject> style_attr = new List<JObject>();

            string product_name = json.Value<string>("product_name");


            String sku_family = json.Value<string>("style_sku").Split('-')[0];


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




            //createAttributesValues
            //createProductAttribute

            // style_attr.Add("abc", "a ");
            // style_attr.Add("style_size", json.Value<string>("style_size"));
            // style_attr.Add("style_vendor_size", json.Value<string>("style_vendor_size"));
            // style_attr.Add("style_sku", json.Value<string>("style_sku"));
            // style_attr.Add("style_qty", json.Value<string>("style_qty"));



            //  JObject vendor_rep = new JObject();
            //  vendor_rep.Add("vendor_id", newVendorID);
            //  vendor_rep.Add("first_name", json.Value<string>("Rep_first_name"));
            //  vendor_rep.Add("full_name", json.Value<string>("Rep_first_name"));
            //  vendor_rep.Add("phone1", json.Value<string>("Rep_phone1"));
            // vendor_rep.Add("phone2", json.Value<string>("Rep_phone2"));
            // vendor_rep.Add("email", json.Value<string>("Rep_email"));
            // vendor_rep.Add("main", 1);
            //  vendor_rep.Add("created_by", 2);
            //  vendor_rep.Add("created_at", _common.GetTimeStemp());
            //  String newVendorRepID = await _BadgerApiHelper.GenericPostAsyncString<String>(vendor_rep.ToString(Formatting.None), "/VendorRep/create");

            return product_id;
           
        }
    }
}