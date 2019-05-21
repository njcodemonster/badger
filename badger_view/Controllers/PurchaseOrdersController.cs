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

            string NewDateFormat = "";
            string NumDays = "";
            var TotalList = purchaseOrdersPagerList.purchaseOrdersInfo;
            List<PurchaseOrdersInfo> newPurchaseOrderInfoList = new List<PurchaseOrdersInfo>();

            foreach (PurchaseOrdersInfo poList in TotalList)
            {

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

            return View("Index", PurchaseOrdersPageModal);
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
