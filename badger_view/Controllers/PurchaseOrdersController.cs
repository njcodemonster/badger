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
    public class PurchaseOrdersController : Controller
    {
        private readonly IConfiguration _config;
        public PurchaseOrdersController(IConfiguration config)
        {
            _config = config;

        }
        private BadgerApiHelper _BadgerApiHelper;
       
        private void SetBadgerHelper()
        {
            if (_BadgerApiHelper == null)
            {
                _BadgerApiHelper = new BadgerApiHelper(_config);
            }
        }

        private CommonHelper _CommonHelper;

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
            SetCommonHelper();

            PurchaseOrdersPagerList purchaseOrdersPagerList = await _BadgerApiHelper.GenericGetAsync<PurchaseOrdersPagerList>("/purchaseorders/listpageview/20");

            List<Vendor> getVendorsNameAndId = await _BadgerApiHelper.GenericGetAsync<List<Vendor>>("/vendor/getvendorsnameandid");

            string DeliveryStartEnd = "";

            string NewDateFormat = "";
            string NumDays = "";

            var TotalList = purchaseOrdersPagerList.purchaseOrdersInfo;

            List<PurchaseOrdersInfo> newPurchaseOrderInfoList = new List<PurchaseOrdersInfo>();

            foreach (PurchaseOrdersInfo poList in TotalList)
            {

                DeliveryStartEnd = _CommonHelper.MultiDatePickerFormat(poList.delivery_window_start, poList.delivery_window_end);
                
                NewDateFormat = _CommonHelper.ConvertToDate(poList.order_date);
                     NumDays = _CommonHelper.NumberOfDays(poList.updated_at);

                newPurchaseOrderInfoList.Add(new PurchaseOrdersInfo {
                                            po_id = poList.po_id,
                                            vendor_po_number = poList.vendor_po_number,
                                            vendor_invoice_number = poList.vendor_invoice_number,
                                            vendor_order_number = poList.vendor_order_number,
                                            vendor_id = poList.vendor_id,
                                            order_date = poList.order_date,
                                            vendor = poList.vendor,
                                            custom_delivery_window_start_end = DeliveryStartEnd,
                                            po_status = poList.po_status,
                                            updated_at = poList.updated_at,
                                            custom_order_date = NewDateFormat,
                                            num_of_days = NumDays
                });

                NewDateFormat = "";
                NumDays = "";
            }

            dynamic PurchaseOrdersPageModal = new ExpandoObject();
            PurchaseOrdersPageModal.PurchaseOrdersCount = purchaseOrdersPagerList.Count;
            PurchaseOrdersPageModal.PurchaseOrdersLists = newPurchaseOrderInfoList;
            PurchaseOrdersPageModal.GetVendorsNameAndId = getVendorsNameAndId;

            return View("Index", PurchaseOrdersPageModal);
        }

        [HttpGet("purchaseorders/details/{id}")]
        public async Task<PurchaseOrdersPagerList> GetDetails(Int32 id)
        {
            SetBadgerHelper();
            dynamic poDetails = await _BadgerApiHelper.GenericGetAsync<object>("/purchaseorders/list/" + id.ToString());
            return poDetails;
        }

        public IActionResult Single()
        {
            return View();
        }

        [HttpPost("purchaseorders/newpurchaseorder")]
        public async Task<String> CreateNewPurchaseOrder([FromBody]   JObject json)
        {
            SetBadgerHelper();
            String newPurchaseOrderID = await _BadgerApiHelper.GenericPostAsyncString<String>(json.ToString(Formatting.None), "/purchaseorders/create");
            return newPurchaseOrderID;

        }
        public IActionResult POMgmt()
 {
            return View();
        }
}
}
