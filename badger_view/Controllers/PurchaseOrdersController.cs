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
using CommonHelper;

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
          

            PurchaseOrdersPagerList purchaseOrdersPagerList = await _BadgerApiHelper.GenericGetAsync<PurchaseOrdersPagerList>("/purchaseorders/listpageview/20/true");

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
                                            total_styles = poList.total_styles,
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
        public async Task<String> CreateNewPurchaseOrder([FromBody] JObject json)
        {
            SetBadgerHelper();

            JObject purchaseOrder = new JObject();

            string daterange = json.Value<string>("vendor_po_delievery_range");
            
            string[] dateRangeList = daterange.Split(" - ");

            string startDate = dateRangeList[0].ToString();
            string endDate = dateRangeList[1].ToString();

            string orderDate = json.Value<string>("order_date");

            /*

            string[] startDateList = startDate.Split("/");

            

            string[] endDateList = endDate.Split("/");

            

            string[] orderDateList = orderDate.Split("/");

            DateTime startDateTime = new DateTime(Int32.Parse(startDateList[2]), Int32.Parse(startDateList[0]), Int32.Parse(startDateList[1]), 0, 0, 0, DateTimeKind.Utc);
            DateTime endDateTime = new DateTime(Int32.Parse(endDateList[2]), Int32.Parse(endDateList[0]), Int32.Parse(endDateList[1]), 0, 0, 0, DateTimeKind.Utc);
            DateTime orderDateTime = new DateTime(Int32.Parse(orderDateList[2]), Int32.Parse(orderDateList[0]), Int32.Parse(orderDateList[1]), 0, 0, 0, DateTimeKind.Utc);

            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            double delivery_window_start = (double)(startDateTime - sTime).TotalSeconds;
              double delivery_window_end = (double)(endDateTime - sTime).TotalSeconds;

            double order_date = (double)(orderDateTime - sTime).TotalSeconds;*/

            purchaseOrder.Add("vendor_po_number", json.Value<string>("vendor_po_number"));
            purchaseOrder.Add("vendor_invoice_number", json.Value<string>("vendor_invoice_number"));
            purchaseOrder.Add("vendor_order_number", json.Value<string>("vendor_order_number"));
            purchaseOrder.Add("vendor_id", json.Value<string>("vendor_id"));
            purchaseOrder.Add("total_styles", json.Value<string>("total_styles"));
            purchaseOrder.Add("total_quantity", json.Value<string>("total_quantity"));
            purchaseOrder.Add("subtotal", json.Value<string>("subtotal"));
            purchaseOrder.Add("shipping", json.Value<string>("shipping"));
            purchaseOrder.Add("delivery_window_start", _common.DateConvertToTimeStamp(startDate));
            purchaseOrder.Add("delivery_window_end", _common.DateConvertToTimeStamp(endDate));
            purchaseOrder.Add("po_status", json.Value<string>("po_status"));
            purchaseOrder.Add("deleted", 0);
            purchaseOrder.Add("created_by", 2);
            purchaseOrder.Add("order_date", _common.DateConvertToTimeStamp(orderDate));
            purchaseOrder.Add("created_at", _common.GetTimeStemp());

            if (json.Value<string>("note") != "") {
                purchaseOrder.Add("note", json.Value<string>("note"));
            }
            

            String newPurchaseOrderID = await _BadgerApiHelper.GenericPostAsyncString<String>(purchaseOrder.ToString(Formatting.None), "/purchaseorders/create");
            return newPurchaseOrderID;
        }



        [HttpPost("purchaseorders/updatepurchaseorder/{id}")]
        public async Task<String> UpdatePurchaseOrder(int id, [FromBody] JObject json)
        {
            SetBadgerHelper();
            String newPurchaseOrderID = await _BadgerApiHelper.GenericPutAsyncString<String>(json.ToString(Formatting.None), "/purchaseorders/update/"+id);
            return newPurchaseOrderID;
        }

        public async Task<Object> PurchaseOrderLineItemDetails(int PO_id, int limit)
        {
            SetBadgerHelper();
            dynamic LineItemsDetails = await _BadgerApiHelper.GenericGetAsync<Object>("/PurchaseOrderManagement/GetLineItemDetails/"+PO_id.ToString()+"/"+limit.ToString());
            
            return LineItemsDetails;
        }
        public async Task<IActionResult> PurchaseOrdersManagement()
        {
            SetBadgerHelper();
           
            dynamic PageModal = new ExpandoObject();
            PurchaseOrdersPagerList purchaseOrdersPagerList = await _BadgerApiHelper.GenericGetAsync<PurchaseOrdersPagerList>("/purchaseorders/listpageview/20/false");
            PageModal.POList = purchaseOrdersPagerList.purchaseOrdersInfo;
            PageModal.FirstPOInfor = await PurchaseOrderLineItemDetails(601,0);
            

            return View("PurchaseOrdersManagement",PageModal);
        }
        public IActionResult EditAttr()
        {
            return View();
        }
    }
}
