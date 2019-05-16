using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using badger_view.Helpers;
using badger_view.Models;
using System.Dynamic;

namespace badger_view.Controllers
{
    public class VendorController : Controller
    {
        private BadgerApiHelper _BadgerApiHelper;
        public IActionResult Index()
        {

            ViewData["tottalVendors"] = "5";
            VendorCountAndList vendorCountAndList =  _BadgerApiHelper.GenericGetAsync<VendorCountAndList>("vendor/countandList");
            dynamic VendorPageModal = new ExpandoObject();
            VendorPageModal.VendorCount = vendorCountAndList.Count; ;
            VendorPageModal.VendorLists = vendorCountAndList.vendors;
            
            return View("Index",VendorPageModal);
        }
    }
}