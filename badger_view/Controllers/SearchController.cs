using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using badger_view.Helpers;
using badger_view.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CommonHelper;
using Microsoft.AspNetCore.Authorization;
namespace badger_view.Controllers
{
    public class SearchController : Controller
    {
        private ILoginHelper _ILoginHelper;
        private readonly IConfiguration _config;
        private BadgerApiHelper _BadgerApiHelper;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        public SearchController(IConfiguration config, ILoginHelper LoginHelper)
        {
            _ILoginHelper = LoginHelper;
            _config = config;

        }

        private void SetBadgerHelper()
        {
            if (_BadgerApiHelper == null)
            {
                _BadgerApiHelper = new BadgerApiHelper(_config);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(string search)
        {
            ViewBag.SearchKey = search;

            SetBadgerHelper();

            PurchaseOrdersPagerList purchaseOrdersPagerList = await _BadgerApiHelper.GenericGetAsync<PurchaseOrdersPagerList>("/purchaseorders/searchbypoandinvoice/" + search);

            string NewDateFormat = "";

            var TotalList = purchaseOrdersPagerList.purchaseOrdersInfo;

            List<PurchaseOrdersInfo> newPurchaseOrderInfoList = new List<PurchaseOrdersInfo>();

            foreach (PurchaseOrdersInfo poList in TotalList)
            {
                NewDateFormat = _common.ConvertToDate(poList.order_date);


                newPurchaseOrderInfoList.Add(new PurchaseOrdersInfo
                {
                    po_id = poList.po_id,
                    vendor_po_number = poList.vendor_po_number,
                    vendor_invoice_number = poList.vendor_invoice_number,
                    vendor_order_number = poList.vendor_order_number,
                    vendor = poList.vendor,
                    custom_order_date = NewDateFormat,
                });

                NewDateFormat = "";
            }

            dynamic PurchaseOrdersPageModal = new ExpandoObject();
            PurchaseOrdersPageModal.PurchaseOrdersLists = newPurchaseOrderInfoList;

            return View("Index", PurchaseOrdersPageModal);
        }
    }
}