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
        public VendorController(IConfiguration config)
        {
            _config = config;

        }
        private BadgerApiHelper _BadgerApiHelper ;
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
            String newVendorID = await _BadgerApiHelper.GenericPostAsyncString<String>(json.ToString(Formatting.None), "/vendor/create");
            return newVendorID;
           
        }
    }
}