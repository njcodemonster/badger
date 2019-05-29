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
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
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

       

        public async Task<IActionResult> Index()
        {
            SetBadgerHelper();
          

            PurchaseOrdersPagerList purchaseOrdersPagerList = await _BadgerApiHelper.GenericGetAsync<PurchaseOrdersPagerList>("/purchaseorders/listpageview/20");

            List<Vendor> getVendorsNameAndId = await _BadgerApiHelper.GenericGetAsync<List<Vendor>>("/vendor/getvendorsnameandid");

            string DeliveryStartEnd = "";

            string NewDateFormat = "";
            string NumDays = "";

            var TotalList = purchaseOrdersPagerList.purchaseOrdersInfo;

            List<PurchaseOrdersInfo> newPurchaseOrderInfoList = new List<PurchaseOrdersInfo>();

            foreach (PurchaseOrdersInfo poList in TotalList)
            {

                DeliveryStartEnd = _common.MultiDatePickerFormat(poList.delivery_window_start, poList.delivery_window_end);
                
                NewDateFormat = _common.ConvertToDate(poList.order_date);
                     NumDays = _common.NumberOfDays(poList.updated_at);

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
        public async Task<String> GetDetails(Int32 id)
        {
            SetBadgerHelper();
            Object poDetails = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseorders/list/" + id.ToString());
            return poDetails.ToString();
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



        [HttpPost("purchaseorders/updatepurchaseorder/{id}")]
        public async Task<String> UpdatePurchaseOrder(int id, [FromBody] JObject json)
        {
            SetBadgerHelper();
            String newPurchaseOrderID = await _BadgerApiHelper.GenericPutAsyncString<String>(json.ToString(Formatting.None), "/purchaseorders/update/"+id);
            return newPurchaseOrderID;
        }


        public IActionResult POMgmt()
        {
            return View();
        }
        public IActionResult EditAttr()
        {
            return View();
        }
        public IActionResult POReporting()
        {
            return View();
        }
    }
}
