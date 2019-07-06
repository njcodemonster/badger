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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace badger_view.Controllers
{
    public class purchaseOrderFileData
    {
        public List<IFormFile> purchaseOrderDocuments { get; set; }
        public string po_id { get; set; }
    }
    public class PurchaseOrdersController : Controller
    {
        private readonly IConfiguration _config;

        private String UploadPath = "";

        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();

        private ILoginHelper _LoginHelper;

        ILoggerFactory _loggerFactory;

        public PurchaseOrdersController(IConfiguration config, ILoginHelper LoginHelper, ILoggerFactory loggerFactory)
        {
            _LoginHelper = LoginHelper;
            _config = config;
            UploadPath = _config.GetValue<string>("UploadPath:path");
            _loggerFactory = loggerFactory;
        }
        private BadgerApiHelper _BadgerApiHelper;

        private void SetBadgerHelper()
        {
            if (_BadgerApiHelper == null)
            {
                _BadgerApiHelper = new BadgerApiHelper(_config);
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: View ALL Purchase Orders List & Get All Vendors with id and name & Get All Vendor types using badger api helper and common helper   
        URL: /purchaseorders
        Request: Get
        Input: Null
        output: Dynamic object of purchase orders
        */
        [Authorize]
        public async Task<IActionResult> Index()
        {
            SetBadgerHelper();

            PurchaseOrdersPagerList purchaseOrdersPagerList = await _BadgerApiHelper.GenericGetAsync<PurchaseOrdersPagerList>("/purchaseorders/listpageview/0/true");

            List<Vendor> getVendorsNameAndId = await _BadgerApiHelper.GenericGetAsync<List<Vendor>>("/vendor/getvendorsnameandid");

            List<VendorType> getVendorTypes = await _BadgerApiHelper.GenericGetAsync<List<VendorType>>("/vendor/getvendortypes");

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

                newPurchaseOrderInfoList.Add(new PurchaseOrdersInfo
                {
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
            PurchaseOrdersPageModal.GetVendorsTypes = getVendorTypes;

            return View("Index", PurchaseOrdersPageModal);
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: View Single Purchase Orders List & Get Note,Document,Tracking,Ledger and Discount by using badger api helper   
        URL: /purchaseorders/details/id
        Request: Get
        Input: int id
        output: Dynamic object of purchase orders
        */
        [Authorize]
        [HttpGet("purchaseorders/details/{id}")]
        public async Task<string> GetDetails(Int32 id)
        {
            SetBadgerHelper();

            dynamic purchaseOrdersData = new ExpandoObject();

            dynamic purchaseOrder = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseorders/list/" + id.ToString());

            dynamic purchaseOrderNote = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseorders/getnote/" + id.ToString()+"/1");

            dynamic purchaseOrderDocs = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseorders/getdocuments/" + id.ToString() + "/0");

            dynamic purchaseOrderTracking = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseorderstracking/gettracking/" + id.ToString());

            dynamic getLedger = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseordersledger/getledger/" + id.ToString());

            dynamic getDiscount = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseordersdiscounts/getdiscount/" + id.ToString());

            purchaseOrdersData.purchase_order = purchaseOrder;
            purchaseOrdersData.notes = purchaseOrderNote;
            purchaseOrdersData.documents = purchaseOrderDocs;
            purchaseOrdersData.tracking = purchaseOrderTracking;
            purchaseOrdersData.ledger = getLedger;
            purchaseOrdersData.discount = getDiscount;

            return JsonConvert.SerializeObject(purchaseOrdersData);
        }


        [Authorize]
        public async Task<IActionResult> Single()
        {
            SetBadgerHelper();
            List<Vendor> getVendorsNameAndId = await _BadgerApiHelper.GenericGetAsync<List<Vendor>>("/vendor/getvendorsnameandid");

            dynamic vendor = new ExpandoObject();
            vendor.GetVendorsNameAndId = getVendorsNameAndId;

            return View("Single", vendor);
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: create new Purchase Order & Note using badger api helper and login helper
        URL: /purchaseorders/newpurchaseorder
        Request: Post
        Input: FromBody Json jobject
        output: string of purchase orders id
        */
        [Authorize]
        [HttpPost("purchaseorders/newpurchaseorder")]
        public async Task<String> CreateNewPurchaseOrder([FromBody] JObject json)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            JObject purchaseOrder = new JObject();

            string daterange = json.Value<string>("vendor_po_delievery_range");

            string[] dateRangeList = daterange.Split(" - ");

            string startDate = dateRangeList[0].ToString();
            string endDate = dateRangeList[1].ToString();

            string orderDate = json.Value<string>("order_date");

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
            purchaseOrder.Add("created_by", Int32.Parse(loginUserId));
            purchaseOrder.Add("order_date", _common.DateConvertToTimeStamp(orderDate));
            purchaseOrder.Add("created_at", _common.GetTimeStemp());

            String newPurchaseOrderID = await _BadgerApiHelper.GenericPostAsyncString<String>(purchaseOrder.ToString(Formatting.None), "/purchaseorders/create");

            if (newPurchaseOrderID != "0")
            {
                if (json.Value<string>("note") != null)
                {
                    JObject purchaseOrderNote = new JObject();
                    purchaseOrderNote.Add("ref_id", newPurchaseOrderID);
                    purchaseOrderNote.Add("note", json.Value<string>("note"));
                    purchaseOrderNote.Add("created_by", Int32.Parse(loginUserId));

                    await _BadgerApiHelper.GenericPostAsyncString<String>(purchaseOrderNote.ToString(Formatting.None), "/purchaseorders/notecreate");
                }
            }

            return newPurchaseOrderID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: create new Purchase Order document it check file already exist in upload folder by using badger api helper and login helper
        URL: /purchaseorders/purchaseorder_doc
        Request: Post
        Input: FromBody string multiple files
        output: string of purchase orders document
        */
        [Authorize]
        [HttpPost("purchaseorders/purchaseorder_doc")]
        public async Task<String> CreateNewPurchaseOrderDoc(purchaseOrderFileData purchaseorderfile)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            string messageDocuments = "";
            string messageAlreadyDocuments = "";
            try
            {
                List<IFormFile> files = purchaseorderfile.purchaseOrderDocuments;

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        string Fill_path = formFile.FileName;
                        Fill_path = UploadPath + Fill_path;

                        if (System.IO.File.Exists(Fill_path))
                        {
                            messageAlreadyDocuments += "File Already Exists: " + Fill_path + " \r\n";
                        }
                        else
                        {
                            using (var stream = new FileStream(Fill_path, FileMode.Create))
                            {
                                messageDocuments += Fill_path + " \r\n";

                                await formFile.CopyToAsync(stream);

                                JObject purchaseOrderDocuments = new JObject();
                                purchaseOrderDocuments.Add("ref_id", purchaseorderfile.po_id);
                                purchaseOrderDocuments.Add("url", Fill_path);
                                purchaseOrderDocuments.Add("created_by", Int32.Parse(loginUserId));
                                await _BadgerApiHelper.GenericPostAsyncString<String>(purchaseOrderDocuments.ToString(Formatting.None), "/purchaseorders/documentcreate");


                            }
                        }
                    }
                }

                return messageDocuments + " \r\n " + messageAlreadyDocuments;

            }
            catch (Exception ex)
            {
                return "0";
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update Purchase Order by id and create note,tracking if already not exist by using badger api helper and login helper
        URL: /purchaseorders/updatepurchaseorder/id
        Request: Post
        Input: int id, FromBody json object
        output: string of purchase orders id
        */
        [Authorize]
        [HttpPost("purchaseorders/updatepurchaseorder/{id}")]
        public async Task<String> UpdatePurchaseOrder(int id, [FromBody] JObject json)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            JObject purchaseOrder = new JObject();

            string daterange = json.Value<string>("vendor_po_delievery_range");

            string[] dateRangeList = daterange.Split(" - ");

            string startDate = dateRangeList[0].ToString();
            string endDate = dateRangeList[1].ToString();

            string orderDate = json.Value<string>("order_date");

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
            purchaseOrder.Add("updated_by", Int32.Parse(loginUserId));
            purchaseOrder.Add("order_date", _common.DateConvertToTimeStamp(orderDate));
            purchaseOrder.Add("updated_at", _common.GetTimeStemp());

            String newPurchaseOrderID = await _BadgerApiHelper.GenericPutAsyncString<String>(purchaseOrder.ToString(Formatting.None), "/purchaseorders/update/"+id);

            if (newPurchaseOrderID == "Success") {

                if (json.Value<string>("old_note") != "")
                {
                    if (json.Value<string>("old_note") == json.Value<string>("note")) {
                    }
                    else
                    {
                        JObject purchaseOrderNote = new JObject();
                        purchaseOrderNote.Add("ref_id", id);
                        purchaseOrderNote.Add("note", json.Value<string>("note"));
                        purchaseOrderNote.Add("created_by", Int32.Parse(loginUserId));

                        await _BadgerApiHelper.GenericPostAsyncString<String>(purchaseOrderNote.ToString(Formatting.None), "/purchaseorders/notecreate");
                    }                    
                }

                if (json.Value<string>("old_note") == "0" && json.Value<string>("note") != null) {
                    JObject purchaseOrderNote = new JObject();
                    purchaseOrderNote.Add("ref_id", id);
                    purchaseOrderNote.Add("note", json.Value<string>("note"));
                    purchaseOrderNote.Add("created_by", Int32.Parse(loginUserId));

                    await _BadgerApiHelper.GenericPostAsyncString<String>(purchaseOrderNote.ToString(Formatting.None), "/purchaseorders/notecreate");
                }

                JObject allData = JObject.Parse(json.ToString());
                JArray trackings = (JArray)allData["tracking"];
                foreach (var track in trackings)
                {
                    if (track.Value<string>("track") != "")
                    {
                        if (track.Value<string>("id") == null) {
                            JObject PurchaseOrdersTracking = new JObject();
                            PurchaseOrdersTracking.Add("po_id", id);
                            PurchaseOrdersTracking.Add("tracking_number", track.Value<string>("track"));
                            PurchaseOrdersTracking.Add("created_by", Int32.Parse(loginUserId));
                            PurchaseOrdersTracking.Add("created_at", _common.GetTimeStemp());
                            await _BadgerApiHelper.GenericPostAsyncString<String>(PurchaseOrdersTracking.ToString(Formatting.None), "/purchaseorderstracking/create");
                        }
                        else
                        {
                            JObject PurchaseOrdersTracking = new JObject();
                            PurchaseOrdersTracking.Add("po_id", id);
                            PurchaseOrdersTracking.Add("tracking_number", track.Value<string>("track"));
                            PurchaseOrdersTracking.Add("updated_by", Int32.Parse(loginUserId));
                            PurchaseOrdersTracking.Add("updated_at", _common.GetTimeStemp());                                                       
                            await _BadgerApiHelper.GenericPutAsyncString<String>(PurchaseOrdersTracking.ToString(Formatting.None), "/purchaseorderstracking/update/"+ track.Value<string>("id").ToString());
                        }
                        
                    }
                }

            }

            return newPurchaseOrderID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: create new Purchase Order discount and get discount list by id by using badger api helper and login helper
        URL: /purchaseorders/discountcreate
        Request: Post
        Input: FromBody json object
        output: string of purchase orders discount 
        */
        [Authorize]
        [HttpPost("purchaseorders/discountcreate")]
        public async Task<String> DiscountCreate([FromBody] JObject json)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            String newPurchaseDiscountID = "0";

            if (json.Value<string>("po_id") != "")
            {
                JObject PurchaseOrdersDiscount = new JObject();

                PurchaseOrdersDiscount.Add("po_id", json.Value<string>("po_id"));
                PurchaseOrdersDiscount.Add("discount_percentage", json.Value<string>("discount_percentage"));
                PurchaseOrdersDiscount.Add("discount_note", json.Value<string>("discount_note"));
                PurchaseOrdersDiscount.Add("completed_status", json.Value<string>("completed_status"));                
                PurchaseOrdersDiscount.Add("created_by", Int32.Parse(loginUserId));
                PurchaseOrdersDiscount.Add("created_at", _common.GetTimeStemp());

                newPurchaseDiscountID = await _BadgerApiHelper.GenericPostAsyncString<String>(PurchaseOrdersDiscount.ToString(Formatting.None), "/purchaseordersdiscounts/create");
            }

            if (newPurchaseDiscountID != "0")
            {
                dynamic GetDiscount = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseordersdiscounts/list/" + newPurchaseDiscountID.ToString());

                return JsonConvert.SerializeObject(GetDiscount);
            }
            else
            {
                return newPurchaseDiscountID;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update Purchase Order discount and get discount list by id by using badger api helper and login helper
        URL: /purchaseorders/discountupdate/id
        Request: Post
        Input:int id, FromBody json object
        output: string of purchase orders discount 
        */
        [Authorize]
        [HttpPost("purchaseorders/discountupdate/{id}")]
        public async Task<String> DiscountUpdate(int id,[FromBody] JObject json)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            String updatePurchaseDiscountID = "0";

                JObject PurchaseOrdersDiscount = new JObject();

                PurchaseOrdersDiscount.Add("po_id", json.Value<string>("po_id"));
                PurchaseOrdersDiscount.Add("discount_percentage", json.Value<string>("discount_percentage"));
                PurchaseOrdersDiscount.Add("discount_note", json.Value<string>("discount_note"));
                PurchaseOrdersDiscount.Add("completed_status", json.Value<string>("completed_status"));
                PurchaseOrdersDiscount.Add("updated_by", Int32.Parse(loginUserId));
                PurchaseOrdersDiscount.Add("updated_at", _common.GetTimeStemp());

            updatePurchaseDiscountID = await _BadgerApiHelper.GenericPutAsyncString<String>(PurchaseOrdersDiscount.ToString(Formatting.None), "/purchaseordersdiscounts/update/"+id.ToString());
            

            if (updatePurchaseDiscountID == "Success")
            {
                dynamic GetDiscount = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseordersdiscounts/list/" + id.ToString());

                return JsonConvert.SerializeObject(GetDiscount);
            }
            else
            {
                return updatePurchaseDiscountID;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: create new Purchase Order ledger and get ledger list by id by using badger api helper and login helper
        URL: /purchaseorders/ledgercreate
        Request: Post
        Input:FromBody json object
        output: string of purchase orders ledger
        */
        [Authorize]
        [HttpPost("purchaseorders/ledgercreate")]
        public async Task<String> LedgerCreate([FromBody] JObject json)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            String newPurchaseLedgerID = "0";

            if (json.Value<string>("ledger_adjustment") != "")
            {
                JObject PurchaseOrdersLedger = new JObject();

                PurchaseOrdersLedger.Add("po_id", json.Value<string>("po_id"));
                PurchaseOrdersLedger.Add("description", json.Value<string>("ledger_note"));
                PurchaseOrdersLedger.Add(json.Value<string>("ledger_adjustment"), json.Value<string>("ledger_amount"));
                PurchaseOrdersLedger.Add("created_by", Int32.Parse(loginUserId));
                PurchaseOrdersLedger.Add("created_at", _common.GetTimeStemp());

                newPurchaseLedgerID = await _BadgerApiHelper.GenericPostAsyncString<String>(PurchaseOrdersLedger.ToString(Formatting.None), "/purchaseordersledger/create");
            }

            if (newPurchaseLedgerID != "0") {

                dynamic GetLedger = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseordersledger/list/" + newPurchaseLedgerID.ToString());

                return JsonConvert.SerializeObject(GetLedger);
            }
            else
            {
                return newPurchaseLedgerID;
            }  
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update Purchase Order ledger and get ledger list by id by using badger api helper and login helper
        URL: /purchaseorders/ledgerupdate/id
        Request: Post
        Input:int id, FromBody json object
        output: string of purchase orders ledger
        */
        [Authorize]
        [HttpPost("purchaseorders/ledgerupdate/{id}")]
        public async Task<String> LedgerUpdate(int id, [FromBody] JObject json)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            String updatePurchaseLedgerID = "0";

                JObject PurchaseOrdersLedger = new JObject();

                PurchaseOrdersLedger.Add("po_id", json.Value<string>("po_id"));
                PurchaseOrdersLedger.Add("description", json.Value<string>("ledger_note"));
                PurchaseOrdersLedger.Add(json.Value<string>("ledger_adjustment"), json.Value<string>("ledger_amount"));
                PurchaseOrdersLedger.Add("created_by", Int32.Parse(loginUserId));
                PurchaseOrdersLedger.Add("created_at", _common.GetTimeStemp());

               updatePurchaseLedgerID = await _BadgerApiHelper.GenericPutAsyncString<String>(PurchaseOrdersLedger.ToString(Formatting.None), "/purchaseordersledger/update/"+id.ToString());

            if (updatePurchaseLedgerID == "Success")
            {
                dynamic GetLedger = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseordersledger/list/" + id.ToString());

                return JsonConvert.SerializeObject(GetLedger);
            }
            else
            {
                return updatePurchaseLedgerID;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Purchase Order line item detail by id by using badger api helper
        URL: /purchaseorders/PurchaseOrderLineItemDetails(poid,limit)
        Request: Get
        Input:int poid, int limit
        output: dynamic object of purchase orders line item
        */
        public async Task<Object> PurchaseOrderLineItemDetails(int PO_id, int limit)
        {
            SetBadgerHelper();

            dynamic LineItemsDetails = await _BadgerApiHelper.GenericGetAsync<Object>("/PurchaseOrderManagement/GetLineItemDetails/" + PO_id.ToString() + "/" + limit.ToString());

            return LineItemsDetails;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Purchase Order List & Get first purchase order by id and Get purchase order line item by id & 
        List all status by purchase order for itemm by using badger api helper
        URL: /purchaseorders/PurchaseOrdersManagement
        Request: Get
        Input: Null
        output: dynamic object of purchase orders management list
        */
        public async Task<IActionResult> PurchaseOrdersManagement()
        {
            SetBadgerHelper();

            dynamic PageModal = new ExpandoObject();
            PurchaseOrdersPagerList purchaseOrdersPagerList = await _BadgerApiHelper.GenericGetAsync<PurchaseOrdersPagerList>("/purchaseorders/listpageview/20/false");
            PageModal.POList = purchaseOrdersPagerList.purchaseOrdersInfo;
            int purchase_order_id = PageModal.POList[0].po_id;
            PageModal.FirstPOInfor = await PurchaseOrderLineItemDetails(purchase_order_id, 0);
            PageModal.AllItemStatus =  await _BadgerApiHelper.GenericGetAsync<Object>("/PurchaseOrderManagement/ListAllItemStatus");

            return View("PurchaseOrdersManagement", PageModal);
        }
        
        public async Task<IActionResult> InventoryReporting()
        {
            ViewData["loginUserFirstName"] = await _LoginHelper.GetLoginUserFirstName();
            return View();
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Create Purchase Order notes by using badger api helper and login helper 
        URL: /purchaseorders/notecreate
        Request: Post
        Input: FromBody json object
        output: string of purchase orders note id
        */
        [Authorize]
        [HttpPost("purchaseorders/notecreate")]
        public async Task<String> NoteCreate([FromBody] JObject json)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            String newPurchaseLedgerID = "0";

            if (json.Value<string>("po_notes") != null)
            {
                JObject purchaseOrderNote = new JObject();
                purchaseOrderNote.Add("ref_id", json.Value<string>("po_id"));
                purchaseOrderNote.Add("note", json.Value<string>("po_notes"));
                purchaseOrderNote.Add("created_by", Int32.Parse(loginUserId));

                newPurchaseLedgerID = await _BadgerApiHelper.GenericPostAsyncString<String>(purchaseOrderNote.ToString(Formatting.None), "/purchaseorders/notecreate");
            }

            return newPurchaseLedgerID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Create Purchase Order item note create by using badger api helper and login helper 
        URL: /purchaseorders/itemnotecreate
        Request: Post
        Input: FromBody json object
        output: string of purchase orders item note id
        */
        [Authorize]
        [HttpPost("purchaseorders/itemnotecreate")]
        public async Task<String> ItemNoteCreate([FromBody] JObject json)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            String newItemID = "0";

            if (json.Value<string>("item_note") != null)
            {
                JObject ItemNotes = new JObject();
                ItemNotes.Add("ref_id", json.Value<string>("item_id"));
                ItemNotes.Add("note", json.Value<string>("item_note"));
                ItemNotes.Add("created_by", Int32.Parse(loginUserId));

                newItemID = await _BadgerApiHelper.GenericPostAsyncString<String>(ItemNotes.ToString(Formatting.None), "/purchaseordermanagement/notecreate");
            }

            return newItemID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get Purchase Order note by id by using badger api helper
        URL: /purchaseorders/getnote/id
        Request: Get
        Input: int id
        output: string of purchase orders note 
        */
        [Authorize]
        [HttpGet("purchaseorders/getnote/{id}")]
        public async Task<String> GetNote(int id)
        {
            SetBadgerHelper();

            dynamic purchaseOrderNote = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseorders/getnote/" + id.ToString() + "/1");

            return JsonConvert.SerializeObject(purchaseOrderNote);
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get Purchase Order item notes by multiple ids by using badger api helper 
        URL: /purchaseorders/getitemnotes/id
        Request: Get
        Input: int id
        output: string of purchase orders item notes
        */
        [Authorize]
        [HttpGet("purchaseorders/getitemnotes/{ids}")]
        public async Task<String> GetItemNotes(string ids)
        {
            SetBadgerHelper();
                     
            dynamic purchaseOrderNote = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseordermanagement/getitemnotes/" + ids.ToString());

            return JsonConvert.SerializeObject(purchaseOrderNote);
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get Purchase Order document by id by using badger api helper  
        URL: /purchaseorders/getitemnotes/id
        Request: Get
        Input: int id
        output: string of purchase orders document
        */
        [Authorize]
        [HttpGet("purchaseorders/getdocument/{id}")]
        public async Task<String> GetDocument(int id)
        {
            SetBadgerHelper();

            dynamic purchaseOrdersData = new ExpandoObject();

            dynamic purchaseOrderDocs = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseorders/getdocuments/" + id.ToString() + "/0");
            purchaseOrdersData.documents = purchaseOrderDocs;

            return JsonConvert.SerializeObject(purchaseOrdersData);
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get Purchase Order item document by id by using badger api helper  
        URL: /purchaseorders/getitemdocument/id
        Request: Get
        Input: int id
        output: string of purchase orders document
        */
        [Authorize]
        [HttpGet("purchaseorders/getitemdocument/{id}")]
        public async Task<String> GetItemDocument(int id)
        {
            SetBadgerHelper();
            
            dynamic purchaseOrderDocs = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseordermanagement/getitemdocuments/" + id.ToString() + "/1");
        
            return JsonConvert.SerializeObject(purchaseOrderDocs);
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: delete Purchase Order tracking by id by using badger api helper and login helper  
        URL: /purchaseorders/trackingdelete/id
        Request: Post
        Input: int id, FromBody json object
        output: string of purchase orders tracking delete
        */
        [Authorize]
        [HttpPost("purchaseorders/trackingdelete/{id}")]
        public async Task<string> TrackingDelete(int id, [FromBody] JObject json)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            JObject purchaseOrdersTrackingData = new JObject();
            purchaseOrdersTrackingData.Add("po_id", json.Value<string>("po_id"));
            purchaseOrdersTrackingData.Add("created_by", Int32.Parse(loginUserId));

            return await _BadgerApiHelper.GenericPostAsyncString<string>(purchaseOrdersTrackingData.ToString(Formatting.None), "/purchaseorderstracking/delete/" + id.ToString());
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Delete Purchase Order by id by using badger api helper and login helper  
        URL: /purchaseorders/delete/id
        Request: Post
        Input: int id, FromBody json object
        output: string of purchase orders delete
        */
        [Authorize]
        [HttpPost("purchaseorders/delete/{id}")]
        public async Task<string> PurchaseOrdersDelete(int id, [FromBody] JObject json)
        {
            SetBadgerHelper();

            string updatePurchaseOrderID = "0";

            string loginUserId = await _LoginHelper.GetLoginUserId();
            try
            {
                JObject purchaseOrdersData = new JObject();
                purchaseOrdersData.Add("po_id", id);
                purchaseOrdersData.Add("po_status", 4);
                purchaseOrdersData.Add("updated_by", Int32.Parse(loginUserId));

                updatePurchaseOrderID = await _BadgerApiHelper.GenericPutAsyncString<String>(purchaseOrdersData.ToString(Formatting.None), "/purchaseorders/updatespecific/" + id.ToString());
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating new delete purchaseorders with message" + ex.Message);
                updatePurchaseOrderID = "Failed";
            }
            return updatePurchaseOrderID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get Purchase Order Line Item by product id and by purchase order id by using badger api helper 
        URL: /purchaseorders/lineitems/productid/poid
        Request: Get
        Input: int product id, int purchase order id
        output: string of purchase orders line items
        */
        [HttpGet("purchaseorders/lineitems/{product_id}/{PO_id}")]
        public async Task<string> GetAsyncLineitems(Int32 product_id, Int32 PO_id)
        {

            SetBadgerHelper();
            dynamic poLineitems = new ExpandoObject();

            poLineitems = await _BadgerApiHelper.GenericGetAsync<object>("/purchaseorders/lineitems/" + product_id.ToString()+"/" + PO_id.ToString());
            
            return JsonConvert.SerializeObject(poLineitems);
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Create Purchase Order item document by using badger api helper and login helper 
        URL: /purchaseorders/lineitems/productid/poid
        Request: Post
        Input: FromBody string file
        output: string of purchase orders item document
        */
        [Authorize]
        [HttpPost("purchaseorders/itemdocumentcreate")]
        public async Task<String> CreateNewItemDoc(purchaseOrderFileData purchaseorderfile)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            string messageDocuments = "";
            string messageAlreadyDocuments = "";
            try
            {
                List<IFormFile> files = purchaseorderfile.purchaseOrderDocuments;

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        string Fill_path = formFile.FileName;
                        Fill_path = UploadPath + Fill_path;

                        if (System.IO.File.Exists(Fill_path))
                        {
                            messageAlreadyDocuments += "File Already Exists: " + Fill_path + " \r\n";
                        }
                        else
                        {
                            using (var stream = new FileStream(Fill_path, FileMode.Create))
                            {
                                messageDocuments += Fill_path + " \r\n";

                                await formFile.CopyToAsync(stream);

                                JObject itemDocuments = new JObject();
                                itemDocuments.Add("ref_id", purchaseorderfile.po_id);
                                itemDocuments.Add("url", Fill_path);
                                itemDocuments.Add("created_by", Int32.Parse(loginUserId));
                                await _BadgerApiHelper.GenericPostAsyncString<String>(itemDocuments.ToString(Formatting.None), "/purchaseordermanagement/documentcreate");


                            }
                        }
                    }
                }

                return messageDocuments + " \r\n " + messageAlreadyDocuments;

            }
            catch (Exception ex)
            {
                return "0";
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update Purchase Order item update by id by using badger api helper 
        URL: /purchaseorders/itemupdate/id
        Request: Post
        Input: int id, FromBody json object
        output: string of purchase orders item
        */
        [Authorize]
        [HttpPost("purchaseorders/itemupdate/{id}")]
        public async Task<string> ItemStatusUpdate(int id, [FromBody] JObject json)
        {
            SetBadgerHelper();
            string updateItemID = "0";
            try
            {
                updateItemID = await _BadgerApiHelper.GenericPostAsyncString<String>(json.ToString(Formatting.None), "/purchaseordermanagement/itemupdate/" + id.ToString());
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating new delete purchaseorders with message" + ex.Message);
                updateItemID = "Failed";
            }
            return updateItemID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update Purchase Order sku weight update by id by using badger api helper and login helper 
        URL: /purchaseorders/skuweightupdate/id
        Request: Get
        Input: int id, FromBody json object
        output: string of purchase orders sku
        */
        [Authorize]
        [HttpPost("purchaseorders/skuweightupdate/{id}")]
        public async Task<string> SkuWeightUpdate(int id, [FromBody] JObject json)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            string updateSkuID = "0";
            try
            {
                JObject skuUpdate = new JObject();
                skuUpdate.Add("sku_id", json.Value<string>("sku_id"));
                skuUpdate.Add("weight", json.Value<string>("weight"));
                skuUpdate.Add("updated_by", Int32.Parse(loginUserId));
                skuUpdate.Add("updated_at", _common.GetTimeStemp());

                updateSkuID = await _BadgerApiHelper.GenericPutAsyncString<String>(skuUpdate.ToString(Formatting.None), "/sku/updatespecific/" + id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating new delete purchaseorders with message" + ex.Message);
                updateSkuID = "Failed";
            }
            return updateSkuID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update Purchase Order sku update by id & by product id sku family update & product attribute by id sku update & 
        line item by id sku and quantity update by using badger api helper and login helper  
        URL: /purchaseorders/skuupdate/id
        Request: Post
        Input: int id, FromBody json object
        output: string of purchase orders sku
        */
        [Authorize]
        [HttpPost("purchaseorders/skuupdate/{id}")]
        public async Task<string> SkuUpdate(int id, [FromBody] JObject json)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            string updateSkuID = "0";
            try
            {
                JObject skuUpdate = new JObject();
                skuUpdate.Add("sku_id", json.Value<string>("sku_id"));
                skuUpdate.Add("sku", json.Value<string>("sku"));
                skuUpdate.Add("updated_by", Int32.Parse(loginUserId));
                skuUpdate.Add("updated_at", _common.GetTimeStemp());

                updateSkuID = await _BadgerApiHelper.GenericPutAsyncString<String>(skuUpdate.ToString(Formatting.None), "/sku/updatespecific/" + id);

                JObject productUpdate = new JObject();
                id = Int32.Parse(json.Value<string>("product_id"));
                productUpdate.Add("product_id", json.Value<string>("product_id"));
                productUpdate.Add("sku_family", json.Value<string>("sku"));
                productUpdate.Add("updated_by", Int32.Parse(loginUserId));
                productUpdate.Add("updated_at", _common.GetTimeStemp());

                updateSkuID = await _BadgerApiHelper.GenericPutAsyncString<String>(productUpdate.ToString(Formatting.None), "/product/updatespecific/" + id);

                JObject productAttributeUpdate = new JObject();
                id = Int32.Parse(json.Value<string>("product_attribute_id"));
                productAttributeUpdate.Add("product_attribute_id", json.Value<string>("product_attribute_id"));
                productAttributeUpdate.Add("sku", json.Value<string>("sku"));
                productAttributeUpdate.Add("updated_by", Int32.Parse(loginUserId));
                productAttributeUpdate.Add("updated_at", _common.GetTimeStemp());

                updateSkuID = await _BadgerApiHelper.GenericPutAsyncString<String>(productAttributeUpdate.ToString(Formatting.None), "/product/attribute/updatespecific/" + id);

                JObject poLineItemUpdate = new JObject();
                id = Int32.Parse(json.Value<string>("line_item_id"));
                poLineItemUpdate.Add("line_item_id", json.Value<string>("line_item_id"));
                poLineItemUpdate.Add("sku", json.Value<string>("sku"));
                poLineItemUpdate.Add("line_item_ordered_quantity", json.Value<string>("quantity"));
                poLineItemUpdate.Add("updated_by", Int32.Parse(loginUserId));
                poLineItemUpdate.Add("updated_at", _common.GetTimeStemp());

                updateSkuID = await _BadgerApiHelper.GenericPutAsyncString<String>(poLineItemUpdate.ToString(Formatting.None), "/purchaseorderslineitems/updatespecific/" + id);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating new delete purchaseorders with message" + ex.Message);
                updateSkuID = "Failed";
            }
            return updateSkuID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update Purchase Order line item quantity update by id by using badger api helper and login helper
        URL: /purchaseorders/polineitemupdate/id
        Request: Get
        Input: int id, FromBody json object
        output: string of purchase orders line item
        */
        [Authorize]
        [HttpPost("purchaseorders/polineitemupdate/{id}")]
        public async Task<string> POLineItemUpdate(int id, [FromBody] JObject json)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            string updatePOLineItemID = "0";
            try
            {
                JObject poLineItemUpdate = new JObject();
                poLineItemUpdate.Add("line_item_id", json.Value<string>("line_item_id"));
                poLineItemUpdate.Add("line_item_ordered_quantity", json.Value<string>("line_item_ordered_quantity"));
                poLineItemUpdate.Add("updated_by", Int32.Parse(loginUserId));
                poLineItemUpdate.Add("updated_at", _common.GetTimeStemp());

                updatePOLineItemID = await _BadgerApiHelper.GenericPutAsyncString<String>(poLineItemUpdate.ToString(Formatting.None), "/purchaseorderslineitems/updatespecific/" + id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating new delete purchaseorders with message" + ex.Message);
                updatePOLineItemID = "Failed";
            }
            return updatePOLineItemID;
        }

    }
}
