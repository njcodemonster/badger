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
    public class vendorFileData
    {
        public IFormFile vendorDocument { get; set; }
        public string Vendor_id { get; set; }
    }
    public class VendorController : Controller
    {
        
        private readonly IConfiguration _config;
        private BadgerApiHelper _BadgerApiHelper;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        private String UploadPath = "";
        public VendorController(IConfiguration config)
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
       
        public async Task<IActionResult> Index()
        {
            SetBadgerHelper();
            VendorPagerList vendorPagerList = await _BadgerApiHelper.GenericGetAsync<VendorPagerList>("/vendor/listpageview/200");
            dynamic VendorPageModal = new ExpandoObject();
            VendorPageModal.VendorCount = vendorPagerList.Count; 
            VendorPageModal.VendorLists = vendorPagerList.vendorInfo;
           // VenderAdressandRep venderAdressandRep = await _BadgerApiHelper.GenericGetAsync<VenderAdressandRep>("/Vendor/detailsaddressandrep/103");
          
            //VendorPageModal.Reps = venderAdressandRep.Reps;
            return View("Index",VendorPageModal);
        }
        [HttpGet("vendor/details/{id}")]
        public async Task<VenderAdressandRep> GetDetails(Int32 id)
        {
            SetBadgerHelper();
            VenderAdressandRep venderAdressandRep = await _BadgerApiHelper.GenericGetAsync<VenderAdressandRep>("/vendor/detailsaddressandrep/"+id.ToString());
            return venderAdressandRep;
        }
        [HttpPost("vendor/newvendor_doc")]
        public async Task<String> CreateNewVendorDoc(vendorFileData test)
        {
            try
            {
                string Fill_path = test.vendorDocument.FileName;
                Fill_path = UploadPath + Fill_path;
                using (var stream = new FileStream(Fill_path, FileMode.Create))
                {
                    await test.vendorDocument.CopyToAsync(stream);
                }
                return Fill_path;
            }
            catch(Exception ex)
            {
                return "0";
            }
        }
        [HttpPost("vendor/newvendor")]
        public  async Task<String> CreateNewVendor([FromBody]   JObject json)
        {
            SetBadgerHelper();
           
            JObject vendor = new JObject();
            JObject vendor_adress = new JObject();
            List<JObject> vendor_reps = new List<JObject>();
            vendor.Add("vendor_name", json.Value<string>("vendor_name"));
            vendor.Add("corp_name", json.Value<string>("corp_name"));
            vendor.Add("statement_name", json.Value<string>("statement_name"));
            vendor.Add("vendor_code", json.Value<string>("vendor_code"));
            vendor.Add("our_customer_number", json.Value<string>("our_customer_number"));
            vendor.Add("created_by", 2);
            vendor.Add("active_status", 1);
            vendor.Add("created_at", _common.GetTimeStemp());
            String newVendorID = await _BadgerApiHelper.GenericPostAsyncString<String>(vendor.ToString(Formatting.None), "/vendor/create");
            vendor_adress.Add("vendor_id",newVendorID);
            vendor_adress.Add("vendor_street", json.Value<string>("vendor_street"));
            vendor_adress.Add("vendor_suite_number", json.Value<string>("vendor_suite_number"));
            vendor_adress.Add("vendor_city", json.Value<string>("vendor_city"));
            vendor_adress.Add("vendor_zip", json.Value<string>("vendor_zip"));
            vendor_adress.Add("vendor_state", json.Value<string>("vendor_state"));
            vendor_adress.Add("created_by", 2);
            vendor_adress.Add("created_at", _common.GetTimeStemp());
            String newVendorAdressID = await _BadgerApiHelper.GenericPostAsyncString<String>(vendor_adress.ToString(Formatting.None), "/VendorAddress/create");
            JObject vendor_rep = new JObject();
            vendor_rep.Add("vendor_id", newVendorID);
            vendor_rep.Add("first_name", json.Value<string>("Rep_first_name"));
            vendor_rep.Add("full_name", json.Value<string>("Rep_first_name"));
            vendor_rep.Add("phone1", json.Value<string>("Rep_phone1"));
            vendor_rep.Add("phone2", json.Value<string>("Rep_phone2"));
            vendor_rep.Add("email", json.Value<string>("Rep_email"));
            vendor_rep.Add("main", 1);
            vendor_rep.Add("created_by", 2);
            vendor_rep.Add("created_at", _common.GetTimeStemp());
            String newVendorRepID = await _BadgerApiHelper.GenericPostAsyncString<String>(vendor_rep.ToString(Formatting.None), "/VendorRep/create");

            return newVendorID;
           
        }
    }
}