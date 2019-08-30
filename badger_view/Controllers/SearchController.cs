using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using badger_view.Helpers;
using GenericModals.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CommonHelper;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;

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
        [HttpPost("search/autosuggest")]
        public async Task<String> AutoSuggest([FromBody] JObject json)
        {
            SetBadgerHelper();

            dynamic multipleObject = new ExpandoObject();
            string[] xobject = new string[] { };
            string search = json.Value<string>("search");
            int searchLength = search.Length;

            Boolean checkPattern = false;

            /*********   Barcode  *********************/
            if (searchLength == 8)
            {
                checkPattern = Regex.IsMatch(search, "^([0-9]{8})+$");
                if (checkPattern == true)
                {
                    multipleObject.barcodeList = await _BadgerApiHelper.GenericGetAsync<List<object>>("/purchaseorders/getbarcode/" + search);
                }
                else
                {
                    multipleObject.barcodeList = xobject;
                }
            }
            else
            {
                multipleObject.barcodeList = xobject;
            }
           
            /*********   SKU  *********************/
            if (searchLength >= 5 && searchLength <= 8)
            {
                checkPattern = Regex.IsMatch(search, "(^([a-zA-Z]{2}[0-9]{3})|([a-zA-Z]{2}[0-9]{3}-)||([A-Za-z]{2}[0-9]{3}-[0-9]{1}))+$");
                if (checkPattern == true)
                {
                    multipleObject.skuList = await _BadgerApiHelper.GenericGetAsync<List<object>>("/sku/getsku/"+search);
                    
                }
                else
                {
                    multipleObject.skuList = xobject;
                }
            }
            else
            {
                multipleObject.skuList = xobject;
                
            }

            /********* Style Number *********************/
            if (searchLength > 3 && searchLength <= 8)
            {
                multipleObject.styleNumberList = await _BadgerApiHelper.GenericGetAsync<List<object>>("/vendor/getstylenumber/" + search);    
            }
            else
            {
                multipleObject.styleNumberList = xobject;
            }

            /********* Vendor & Product *********************/
            checkPattern = Regex.IsMatch(search, "^[a-zA-Z0-9_ ]+$");
            if (checkPattern)
            {
                multipleObject.vendorList = await _BadgerApiHelper.GenericGetAsync<List<object>>("/vendor/getvendor/"+search);
                multipleObject.productList = await _BadgerApiHelper.GenericGetAsync<List<object>>("/product/getproduct/"+search);
                multipleObject.purchaseOrdersList = await _BadgerApiHelper.GenericGetAsync<List<object>>("/purchaseorders/getpolist/" + search);
            }
            else
            {
                multipleObject.vendorList = xobject;
                multipleObject.productList = xobject;
                multipleObject.purchaseOrdersList = xobject;
            }

            return JsonConvert.SerializeObject(multipleObject);

        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(string search)
        {
            ViewBag.SearchKey = search;

            SetBadgerHelper();
            return View();
           /* PurchaseOrdersPagerList purchaseOrdersPagerList = await _BadgerApiHelper.GenericGetAsync<PurchaseOrdersPagerList>("/purchaseorders/searchbypoandinvoice/" + search);

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

            return View("Index", PurchaseOrdersPageModal);*/
        }
    }
}