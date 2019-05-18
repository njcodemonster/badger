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

namespace badger_view.Controllers
{
    internal class localDTO
    {

    }
    public class VendorController : Controller
    {
        
        private readonly IConfiguration _config;
        private BadgerApiHelper _BadgerApiHelper;
        private CommonHelper _CommonHelper;
        public VendorController(IConfiguration config)
        {
            _config = config;

        }
        private void SetBadgerHelper()
        {
            if (_BadgerApiHelper == null)
            {
                _BadgerApiHelper = new BadgerApiHelper(_config);
            }
        }
        private void SetCommonHelper()
        {
            if (_CommonHelper == null)
            {
                _CommonHelper = new CommonHelper(_config);
            }
        }
        public async Task<IActionResult> Index()
        {
            SetBadgerHelper();
            VendorPagerList vendorPagerList = await _BadgerApiHelper.GenericGetAsync<VendorPagerList>("/vendor/listpageview/20");
            dynamic VendorPageModal = new ExpandoObject();
            VendorPageModal.VendorCount = vendorPagerList.Count; 
            VendorPageModal.VendorLists = vendorPagerList.vendorInfo;
            
            return View("Index",VendorPageModal);
        }
        [HttpPost("vendor/newvendor")]
        public  async Task<String> CreateNewVendor([FromBody]   JObject json)
        {
            SetBadgerHelper();
            SetCommonHelper();
            JObject vendor = new JObject();
            JObject vendor_adress = new JObject();
            List<JObject> vendor_reps = new List<JObject>();
            vendor.Add("vendor_name", json.Value<string>("vendor_name"));
            vendor.Add("vendor_name", json.Value<string>("corp_name"));
            vendor.Add("vendor_name", json.Value<string>("statement_name"));
            vendor.Add("vendor_name", json.Value<string>("vendor_code"));
            vendor.Add("our_customer_number", json.Value<string>("our_customer_number"));
            vendor.Add("created_by", 2);
            vendor.Add("active_status", 1);
            vendor.Add("created_at", _CommonHelper.GetTimeStemp());




            String newVendorID = await _BadgerApiHelper.GenericPostAsyncString<String>(json.ToString(Formatting.None), "/vendor/create");
            return newVendorID;
           
        }
    }
}