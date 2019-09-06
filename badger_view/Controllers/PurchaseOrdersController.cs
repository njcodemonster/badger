using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using badger_view.Helpers;
using GenericModals.Models;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CommonHelper;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using GenericModals.Claim;
using GenericModals.PurchaseOrder;
using GenericModals;
using CommonHelper.Extensions;

namespace badger_view.Controllers
{
    public class purchaseOrderFileData
    {
        public List<IFormFile> purchaseOrderDocuments { get; set; }
        public string po_id { get; set; }
        public string doc_type { get; set; }
    }
    public class PurchaseOrdersController : Controller
    {
        private readonly IConfiguration _config;

        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        private CommonHelper.awsS3helper awsS3Helper = new CommonHelper.awsS3helper();
        private String UploadPath = "";
        private String S3bucket = "";
        private String S3folder = "";

        private ILoginHelper _LoginHelper;

        ILoggerFactory _loggerFactory;

        public PurchaseOrdersController(IConfiguration config, ILoginHelper LoginHelper, ILoggerFactory loggerFactory)
        {
            _LoginHelper = LoginHelper;
            _config = config;
            UploadPath = _config.GetValue<string>("UploadPath:path");
            S3bucket = _config.GetValue<string>("S3config:Bucket_Name");
            S3folder = _config.GetValue<string>("S3config:Folder");
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

            // PurchaseOrdersPagerList purchaseOrdersPagerList = await _BadgerApiHelper.GenericGetAsync<PurchaseOrdersPagerList>("/purchaseorders/listpageview/0/0/true");
            var purchaseOrdersPagerList = await _BadgerApiHelper.GetAsync<PurchaseOrdersPagerList>("/purchaseorders/listpageview/0/0/true");

            String poIdsList = "";

            dynamic ProductIdsList = await _BadgerApiHelper.GenericGetAsync<object>("/product/getproductidsbypurchaseorder/");

            List<VendorType> getVendorTypes = await _BadgerApiHelper.GenericGetAsync<List<VendorType>>("/vendor/getvendortypes");

            string DeliveryStartEnd = "";

            string NewDateFormat = "";
            string NumDays = "";

            var TotalList = purchaseOrdersPagerList.purchaseOrdersInfo;

            List<PurchaseOrdersInfo> newPurchaseOrderInfoList = new List<PurchaseOrdersInfo>();
            List<PurchaseOrdersInfo> newPurchaseOrderInfoList2 = new List<PurchaseOrdersInfo>();

            int PhotosCount = 0;
            int TotalPublishedProducts = 0;
            foreach (PurchaseOrdersInfo poList in TotalList)
            {
                PhotosCount = 0;
                TotalPublishedProducts = 0;

                foreach (dynamic ids in ProductIdsList)
                {
                    if (ids.po_id == poList.po_id)
                    {
                        PhotosCount++;
                        poIdsList += ids.product_id + ",";
                    }
                }

                if (PhotosCount != 0)
                {
                    dynamic TotalData = await _BadgerApiHelper.GenericGetAsync<object>("/product/getpublishedproductCount/" + poIdsList.TrimEnd(','));
                    TotalPublishedProducts = TotalData.Count;
                }

                DeliveryStartEnd = _common.MultiDatePickerFormat(poList.delivery_window_start, poList.delivery_window_end);

                double DateToCheck = _common.GetTimeStemp();

                bool CheckDaysRange = false;

                if (DateToCheck <= poList.delivery_window_end || poList.delivery_window_end == 0)
                {
                    CheckDaysRange = true;
                }

                NewDateFormat = _common.ConvertToDate(poList.order_date);
                NumDays = _common.NumberOfDays(poList.updated_at);


                if (CheckDaysRange == true && poList.po_status == 5)
                {
                    newPurchaseOrderInfoList2.Add(new PurchaseOrdersInfo
                    {
                        po_id = poList.po_id,
                        vendor_po_number = poList.vendor_po_number,
                        vendor_invoice_number = poList.vendor_invoice_number,
                        vendor_order_number = poList.vendor_order_number,
                        vendor_id = poList.vendor_id,
                        total_styles = poList.total_styles,
                        shipping = poList.shipping,
                        order_date = poList.order_date,
                        vendor = poList.vendor,
                        custom_delivery_window_start_end = DeliveryStartEnd,
                        po_status = poList.po_status,
                        ra_flag = poList.ra_flag,
                        updated_at = poList.updated_at,
                        custom_order_date = NewDateFormat,
                        num_of_days = NumDays,
                        check_days_range = CheckDaysRange,
                        has_note = poList.has_note,
                        has_doc = poList.has_doc,
                        photos = TotalPublishedProducts,
                        remaining = (PhotosCount - TotalPublishedProducts)
                    });
                }
                else
                {
                    newPurchaseOrderInfoList.Add(new PurchaseOrdersInfo
                    {
                        po_id = poList.po_id,
                        vendor_po_number = poList.vendor_po_number,
                        vendor_invoice_number = poList.vendor_invoice_number,
                        vendor_order_number = poList.vendor_order_number,
                        vendor_id = poList.vendor_id,
                        total_styles = poList.total_styles,
                        shipping = poList.shipping,
                        order_date = poList.order_date,
                        vendor = poList.vendor,
                        custom_delivery_window_start_end = DeliveryStartEnd,
                        po_status = poList.po_status,
                        ra_flag = poList.ra_flag,
                        updated_at = poList.updated_at,
                        custom_order_date = NewDateFormat,
                        num_of_days = NumDays,
                        check_days_range = CheckDaysRange,
                        has_note = poList.has_note,
                        has_doc = poList.has_doc,
                        photos = TotalPublishedProducts,
                        remaining = (PhotosCount - TotalPublishedProducts)
                    });
                }
                NewDateFormat = "";
                NumDays = "";
            }

            dynamic PurchaseOrdersPageModal = new ExpandoObject();

            var list = newPurchaseOrderInfoList.Concat(newPurchaseOrderInfoList2).ToList();

            PurchaseOrdersPageModal.PurchaseOrdersCount = purchaseOrdersPagerList.Count;
            PurchaseOrdersPageModal.PurchaseOrdersLists = list;
            PurchaseOrdersPageModal.VendorType = getVendorTypes;

            return View("Index", PurchaseOrdersPageModal);
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: View Purchase Orders List & Get Note,Document,Tracking,Ledger and Discount by using badger api helper   
        URL: /purchaseorders/listpagination/start/end/boolean
        Request: Get
        Input: int start, int limit, boolean
        output: Dynamic object of purchase orders
        */
        [Authorize]
        [HttpGet("purchaseorders/listpagination/{start}/{limit}/{count}")]
        public async Task<string> ListPagination(int start, int limit, Boolean count)
        {
            SetBadgerHelper();

            PurchaseOrdersPagerList purchaseOrdersPagerList = await _BadgerApiHelper.GenericGetAsync<PurchaseOrdersPagerList>("/purchaseorders/listpageview/" + start + "/" + limit + "/" + count);

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
                    ra_flag = poList.ra_flag,
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

            return JsonConvert.SerializeObject(PurchaseOrdersPageModal);
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

            string vendor_id = purchaseOrder[0].vendor_id;

            dynamic vendorData = await _BadgerApiHelper.GenericGetAsync<Object>("/vendor/list/" + vendor_id);

            dynamic purchaseOrderNote = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseorders/getnote/" + id.ToString() + "/1");

            dynamic purchaseOrderDocs = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseorders/getdocuments/" + id.ToString() + "/0");

            dynamic purchaseOrderTracking = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseorderstracking/gettracking/" + id.ToString());

            dynamic getLedger = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseordersledger/getledger/" + id.ToString());

            dynamic getDiscount = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseordersdiscounts/getdiscount/" + id.ToString());

            dynamic LineItemsDetails = await _BadgerApiHelper.GenericGetAsync<Object>("/PurchaseOrderManagement/GetLineItemDetails/" + id.ToString() + "/" + "0");

            purchaseOrdersData.purchase_order = purchaseOrder;
            purchaseOrdersData.vendor = vendorData;
            purchaseOrdersData.notes = purchaseOrderNote;
            purchaseOrdersData.documents = purchaseOrderDocs;
            purchaseOrdersData.tracking = purchaseOrderTracking;
            purchaseOrdersData.ledger = getLedger;
            purchaseOrdersData.discount = getDiscount;
            purchaseOrdersData.Items = LineItemsDetails;
            return JsonConvert.SerializeObject(purchaseOrdersData);
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get Purchase Orders data by id 
        URL: /purchaseorders/single/649
        Request: Get
        Input: int id
        output: dynamic object of purchase orders
        */
        [Authorize]
        public async Task<IActionResult> Single()
        {
            SetBadgerHelper();
            List<Vendor> getVendorsNameAndId = await _BadgerApiHelper.GenericGetAsync<List<Vendor>>("/vendor/getvendorsnameandid");
            List<VendorType> getVendorTypes = await _BadgerApiHelper.GenericGetAsync<List<VendorType>>("/vendor/getvendortypes");

            dynamic vendor = new ExpandoObject();
            vendor.GetVendorsNameAndId = getVendorsNameAndId;
            vendor.VendorType = getVendorTypes;
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

            if (json.Value<string>("vendor_po_number") != "")
            {
                purchaseOrder.Add("vendor_po_number", json.Value<string>("vendor_po_number"));
            }
            else
            {
                purchaseOrder.Add("vendor_po_number", "");
            }

            if (json.Value<string>("vendor_invoice_number") != "")
            {
                purchaseOrder.Add("vendor_invoice_number", json.Value<string>("vendor_invoice_number"));
            }
            else
            {
                purchaseOrder.Add("vendor_invoice_number", "");
            }

            if (json.Value<string>("vendor_order_number") != "")
            {
                purchaseOrder.Add("vendor_order_number", json.Value<string>("vendor_order_number"));
            }
            else
            {
                purchaseOrder.Add("vendor_order_number", "");
            }

            if (json.Value<string>("total_styles") != "")
            {
                purchaseOrder.Add("total_styles", json.Value<string>("total_styles"));
            }
            else
            {
                purchaseOrder.Add("total_styles", 0);
            }

            if (json.Value<string>("total_quantity") != "")
            {
                purchaseOrder.Add("total_quantity", json.Value<string>("total_quantity"));
            }
            else
            {
                purchaseOrder.Add("total_quantity", 0);
            }

            if (json.Value<string>("subtotal") != "")
            {
                purchaseOrder.Add("subtotal", json.Value<string>("subtotal"));
            }
            else
            {
                purchaseOrder.Add("subtotal", 0);
            }

            purchaseOrder.Add("vendor_id", json.Value<string>("vendor_id"));

            if (json.Value<string>("shipping") != "")
            {
                purchaseOrder.Add("shipping", json.Value<string>("shipping"));
            }
            else
            {
                purchaseOrder.Add("shipping", 0);
            }

            string daterange = json.Value<string>("vendor_po_delievery_range");
            if (daterange != "")
            {
                string[] dateRangeList = daterange.Split(" - ");

                string startDate = dateRangeList[0].ToString();
                string endDate = dateRangeList[1].ToString();

                purchaseOrder.Add("delivery_window_start", _common.DateConvertToTimeStamp(startDate));
                purchaseOrder.Add("delivery_window_end", _common.DateConvertToTimeStamp(endDate));
            }
            else
            {
                purchaseOrder.Add("delivery_window_start", 0);
                purchaseOrder.Add("delivery_window_end", 0);
            }

            purchaseOrder.Add("po_status", 5);
            purchaseOrder.Add("deleted", 0);
            purchaseOrder.Add("has_note", 2);
            purchaseOrder.Add("created_by", Int32.Parse(loginUserId));

            string orderDate = json.Value<string>("order_date");
            if (orderDate != "")
            {
                purchaseOrder.Add("order_date", _common.DateConvertToTimeStamp(orderDate));
            }
            else
            {
                purchaseOrder.Add("order_date", 0);
            }
            purchaseOrder.Add("created_at", _common.GetTimeStemp());

            String newPurchaseOrderID = await _BadgerApiHelper.GenericPostAsyncString<String>(purchaseOrder.ToString(Formatting.None), "/purchaseorders/create");

            if (newPurchaseOrderID != "0")
            {
                if (json.Value<string>("note") != "")
                {
                    JObject purchaseOrderNote = new JObject();
                    purchaseOrderNote.Add("ref_id", newPurchaseOrderID);
                    purchaseOrderNote.Add("note", json.Value<string>("note"));
                    purchaseOrderNote.Add("created_by", Int32.Parse(loginUserId));

                    await _BadgerApiHelper.GenericPostAsyncString<String>(purchaseOrderNote.ToString(Formatting.None), "/purchaseorders/notecreate");

                    JObject purchaseOrderStatusNote = new JObject();
                    purchaseOrderStatusNote.Add("has_note", 1);
                    purchaseOrderStatusNote.Add("updated_by", Int32.Parse(loginUserId));
                    await _BadgerApiHelper.GenericPutAsyncString<String>(purchaseOrderStatusNote.ToString(Formatting.None), "/purchaseorders/updatespecific/" + newPurchaseOrderID);
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
                                //awsS3Helper.UploadToS3(formFile.FileName, formFile.OpenReadStream(), S3bucket, S3folder);
                                await formFile.CopyToAsync(stream);

                                JObject purchaseOrderDocuments = new JObject();
                                purchaseOrderDocuments.Add("ref_id", purchaseorderfile.po_id);
                                purchaseOrderDocuments.Add("url", formFile.FileName);
                                purchaseOrderDocuments.Add("doc_type", purchaseorderfile.doc_type);
                                purchaseOrderDocuments.Add("created_by", Int32.Parse(loginUserId));
                                await _BadgerApiHelper.GenericPostAsyncString<String>(purchaseOrderDocuments.ToString(Formatting.None), "/purchaseorders/documentcreate");

                                JObject purchaseOrderStatusDoc = new JObject();
                                purchaseOrderStatusDoc.Add("has_doc", 1);
                                purchaseOrderStatusDoc.Add("updated_by", Int32.Parse(loginUserId));
                                await _BadgerApiHelper.GenericPutAsyncString<String>(purchaseOrderStatusDoc.ToString(Formatting.None), "/purchaseorders/updatespecific/" + purchaseorderfile.po_id);

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

            if (json.Value<string>("vendor_po_number") != "")
            {
                purchaseOrder.Add("vendor_po_number", json.Value<string>("vendor_po_number"));
            }
            else
            {
                purchaseOrder.Add("vendor_po_number", "");
            }

            if (json.Value<string>("vendor_invoice_number") != "")
            {
                purchaseOrder.Add("vendor_invoice_number", json.Value<string>("vendor_invoice_number"));
            }
            else
            {
                purchaseOrder.Add("vendor_invoice_number", "");
            }

            if (json.Value<string>("vendor_order_number") != "")
            {
                purchaseOrder.Add("vendor_order_number", json.Value<string>("vendor_order_number"));
            }
            else
            {
                purchaseOrder.Add("vendor_order_number", "");
            }

            if (json.Value<string>("total_styles") != "")
            {
                purchaseOrder.Add("total_styles", json.Value<string>("total_styles"));
            }
            else
            {
                purchaseOrder.Add("total_styles", 0);
            }

            if (json.Value<string>("total_quantity") != "")
            {
                purchaseOrder.Add("total_quantity", json.Value<string>("total_quantity"));
            }
            else
            {
                purchaseOrder.Add("total_quantity", 0);
            }

            if (json.Value<string>("subtotal") != "")
            {
                purchaseOrder.Add("subtotal", json.Value<string>("subtotal"));
            }
            else
            {
                purchaseOrder.Add("subtotal", 0);
            }

            purchaseOrder.Add("vendor_id", json.Value<string>("vendor_id"));

            if (json.Value<string>("shipping") != "")
            {
                purchaseOrder.Add("shipping", json.Value<string>("shipping"));
            }
            else
            {
                purchaseOrder.Add("shipping", 0);
            }

            string daterange = json.Value<string>("vendor_po_delievery_range");
            if (daterange != "")
            {
                string[] dateRangeList = daterange.Split(" - ");

                string startDate = dateRangeList[0].ToString();
                string endDate = dateRangeList[1].ToString();

                purchaseOrder.Add("delivery_window_start", _common.DateConvertToTimeStamp(startDate));
                purchaseOrder.Add("delivery_window_end", _common.DateConvertToTimeStamp(endDate));
            }
            else
            {
                purchaseOrder.Add("delivery_window_start", 0);
                purchaseOrder.Add("delivery_window_end", 0);
            }
            string orderDate = json.Value<string>("order_date");
            if (orderDate != "")
            {
                purchaseOrder.Add("order_date", _common.DateConvertToTimeStamp(orderDate));
            }
            else
            {
                purchaseOrder.Add("order_date", 0);
            }

            purchaseOrder.Add("updated_by", Int32.Parse(loginUserId));
            purchaseOrder.Add("updated_at", _common.GetTimeStemp());

            String newPurchaseOrderID = await _BadgerApiHelper.GenericPutAsyncString<String>(purchaseOrder.ToString(Formatting.None), "/purchaseorders/updatespecific/" + id);

            if (newPurchaseOrderID == "Success")
            {
                JObject purchaseOrderStatusNote = new JObject();

                if (json.Value<string>("old_note") != "")
                {
                    if (json.Value<string>("old_note") == json.Value<string>("note"))
                    {
                    }
                    else
                    {
                        JObject purchaseOrderNote = new JObject();
                        purchaseOrderNote.Add("ref_id", id);
                        purchaseOrderNote.Add("note", json.Value<string>("note"));
                        purchaseOrderNote.Add("created_by", Int32.Parse(loginUserId));

                        await _BadgerApiHelper.GenericPostAsyncString<String>(purchaseOrderNote.ToString(Formatting.None), "/purchaseorders/notecreate");
                    }

                    if (json.Value<string>("note") != "")
                    {
                        purchaseOrderStatusNote.Add("has_note", 1);
                    }
                    else
                    {
                        purchaseOrderStatusNote.Add("has_note", 2);
                    }
                    purchaseOrderStatusNote.Add("updated_by", Int32.Parse(loginUserId));
                    await _BadgerApiHelper.GenericPutAsyncString<String>(purchaseOrderStatusNote.ToString(Formatting.None), "/purchaseorders/updatespecific/" + id);

                }
                else
                {
                    JObject purchaseOrderNote = new JObject();
                    purchaseOrderNote.Add("ref_id", id);
                    purchaseOrderNote.Add("note", json.Value<string>("note"));
                    purchaseOrderNote.Add("created_by", Int32.Parse(loginUserId));

                    await _BadgerApiHelper.GenericPostAsyncString<String>(purchaseOrderNote.ToString(Formatting.None), "/purchaseorders/notecreate");

                    if (json.Value<string>("note") != "")
                    {
                        purchaseOrderStatusNote.Add("has_note", 1);
                    }
                    else
                    {
                        purchaseOrderStatusNote.Add("has_note", 2);
                    }
                    purchaseOrderStatusNote.Add("updated_by", Int32.Parse(loginUserId));
                    await _BadgerApiHelper.GenericPutAsyncString<String>(purchaseOrderStatusNote.ToString(Formatting.None), "/purchaseorders/updatespecific/" + id);
                }

                if (json.Value<string>("old_note") == "0" && json.Value<string>("note") != null)
                {
                    JObject purchaseOrderNote = new JObject();
                    purchaseOrderNote.Add("ref_id", id);
                    purchaseOrderNote.Add("note", json.Value<string>("note"));
                    purchaseOrderNote.Add("created_by", Int32.Parse(loginUserId));

                    await _BadgerApiHelper.GenericPostAsyncString<String>(purchaseOrderNote.ToString(Formatting.None), "/purchaseorders/notecreate");

                    purchaseOrderStatusNote.Add("has_note", 1);
                    purchaseOrderStatusNote.Add("updated_by", Int32.Parse(loginUserId));
                    await _BadgerApiHelper.GenericPutAsyncString<String>(purchaseOrderStatusNote.ToString(Formatting.None), "/purchaseorders/updatespecific/" + id);
                }

                JObject allData = JObject.Parse(json.ToString());
                JArray trackings = (JArray)allData["tracking"];
                foreach (var track in trackings)
                {
                    if (track.Value<string>("track") != "")
                    {
                        if (track.Value<string>("id") == null)
                        {
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
                            await _BadgerApiHelper.GenericPutAsyncString<String>(PurchaseOrdersTracking.ToString(Formatting.None), "/purchaseorderstracking/update/" + track.Value<string>("id").ToString());
                        }

                    }

                    if (track.Value<string>("track") == "" && track.Value<string>("id") != null)
                    {
                        JObject purchaseOrdersTrackingData = new JObject();
                        purchaseOrdersTrackingData.Add("po_id", id);
                        await _BadgerApiHelper.GenericPostAsyncString<string>(purchaseOrdersTrackingData.ToString(Formatting.None), "/purchaseorderstracking/delete/" + track.Value<string>("id").ToString());
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
        public async Task<String> DiscountUpdate(int id, [FromBody] JObject json)
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

            updatePurchaseDiscountID = await _BadgerApiHelper.GenericPutAsyncString<String>(PurchaseOrdersDiscount.ToString(Formatting.None), "/purchaseordersdiscounts/update/" + id.ToString());


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

            if (newPurchaseLedgerID != "0")
            {

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

            updatePurchaseLedgerID = await _BadgerApiHelper.GenericPutAsyncString<String>(PurchaseOrdersLedger.ToString(Formatting.None), "/purchaseordersledger/update/" + id.ToString());

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
            PurchaseOrdersPagerList purchaseOrdersPagerList = await _BadgerApiHelper.GenericGetAsync<PurchaseOrdersPagerList>("/purchaseorders/listpageview/0/50/false");
            PageModal.POList = purchaseOrdersPagerList.purchaseOrdersInfo;
            int purchase_order_id = PageModal.POList[0].po_id;
            PageModal.FirstPOInfor = await PurchaseOrderLineItemDetails(purchase_order_id, 0);
            PageModal.AllItemStatus = await _BadgerApiHelper.GenericGetAsync<Object>("/PurchaseOrderManagement/ListAllItemStatus");

            return View("PurchaseOrdersManagement", PageModal);
        }

        //purchaseorders/lineitemsdetails/poid
        [Authorize]
        [HttpGet("PurchaseOrders/lineitemsdetails/{po_id}")]
        public async Task<IActionResult> GetLineItemsByPOID(int po_id)
        {
            SetBadgerHelper();

            dynamic PageModal = new ExpandoObject();

            PageModal.FirstPOInfor = await PurchaseOrderLineItemDetails(po_id, 0);
            PageModal.AllItemStatus = await _BadgerApiHelper.GenericGetAsync<Object>("/PurchaseOrderManagement/ListAllItemStatus");
            PageModal.AllRaStatus = await _BadgerApiHelper.GenericGetAsync<Object>("/PurchaseOrderManagement/ListAllRaStatus");

            return View("PurchaseOrdersManagementViewAjax", PageModal);
        }

        /*
        Developer: Sajid Khan
        Date: 7-16-19 
        Action: Purchase Order List & Get first purchase order by id and Get purchase order line item by id & 
        List all status by purchase order for itemm by using badger api helper
        URL: /purchaseorders/PurchaseOrdersCheckIn
        Request: Get
        Input: Null
        output: dynamic object of purchase orders management list
        */
        public async Task<IActionResult> PurchaseOrdersCheckIn(int id)
        {
            SetBadgerHelper();

            dynamic PageModal = new ExpandoObject();
            PurchaseOrdersPagerList purchaseOrdersPagerList = new PurchaseOrdersPagerList();
            if (id == 0)
            {
                purchaseOrdersPagerList = await _BadgerApiHelper.GetAsync<PurchaseOrdersPagerList>("/purchaseorders/listpageview/0/200/false");

            }
            else
            {
                purchaseOrdersPagerList = await _BadgerApiHelper.GenericGetAsync<PurchaseOrdersPagerList>("/purchaseorders/singlepageview/" + id);
                if (purchaseOrdersPagerList.purchaseOrdersInfo.Count() == 0)
                {

                    return Redirect("~/PurchaseOrders");
                }
            }


            PageModal.POList = purchaseOrdersPagerList.purchaseOrdersInfo;
            //int purchase_order_id = purchaseOrdersPagerList.purchaseOrdersInfo.First().po_id;
            //PageModal.FirstPOInfor = await PurchaseOrderLineItemDetails(purchase_order_id, 0);
            PageModal.AllItemStatus = await _BadgerApiHelper.GenericGetAsync<Object>("/PurchaseOrderManagement/ListAllItemStatus");
            List<Barcode> allBarcodeRanges = await _BadgerApiHelper.GenericGetAsync<List<Barcode>>("/purchaseorders/getBarcodeRange/");
            ViewBag.allBarcodeRanges = JsonConvert.SerializeObject(allBarcodeRanges);

            ViewBag.Sizes = await _BadgerApiHelper.GenericGetAsync<object>("/attributes/list/type/1");
            ViewBag.categories = await _BadgerApiHelper.GenericGetAsync<object>("/categories/list");
            return View("PurchaseOrdersCheckIn", PageModal);
        }

        //purchaseorders/lineitemsdetails/poid
        //[Authorize]
        [HttpGet("PurchaseOrders/itemsdetails/{po_id}")]
        public async Task<IActionResult> GetItemsDetailsByPOID(int po_id)
        {
            SetBadgerHelper();

            dynamic PageModal = new ExpandoObject();

            PageModal.FirstPOInfor = await PurchaseOrderLineItemDetails(po_id, 0);
            PageModal.AllItemStatus = await _BadgerApiHelper.GenericGetAsync<Object>("/PurchaseOrderManagement/ListAllItemStatus");
            PageModal.AllRaStatus = await _BadgerApiHelper.GenericGetAsync<Object>("/PurchaseOrderManagement/ListAllRaStatus");
            PageModal.AllWashTypes = await _BadgerApiHelper.GenericGetAsync<Object>("/PurchaseOrderManagement/ListAllWashTypes");

            return View("PurchaseOrdersCheckInViewAjax", PageModal);
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

            JObject purchaseOrderNote = new JObject();
            purchaseOrderNote.Add("ref_id", json.Value<string>("po_id"));
            purchaseOrderNote.Add("note", json.Value<string>("po_notes"));
            purchaseOrderNote.Add("created_by", Int32.Parse(loginUserId));

            newPurchaseLedgerID = await _BadgerApiHelper.GenericPostAsyncString<String>(purchaseOrderNote.ToString(Formatting.None), "/purchaseorders/notecreate");

            if (json.Value<string>("po_notes") != "")
            {
                JObject purchaseOrderStatusNote = new JObject();
                purchaseOrderStatusNote.Add("has_note", 1);
                purchaseOrderStatusNote.Add("updated_by", Int32.Parse(loginUserId));
                await _BadgerApiHelper.GenericPutAsyncString<String>(purchaseOrderStatusNote.ToString(Formatting.None), "/purchaseorders/updatespecific/" + json.Value<string>("po_id"));
            }
            else
            {
                JObject purchaseOrderStatusNote = new JObject();
                purchaseOrderStatusNote.Add("has_note", 2);
                purchaseOrderStatusNote.Add("updated_by", Int32.Parse(loginUserId));
                await _BadgerApiHelper.GenericPutAsyncString<String>(purchaseOrderStatusNote.ToString(Formatting.None), "/purchaseorders/updatespecific/" + json.Value<string>("po_id"));

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

            if (json.Value<string>("item_note") != "")
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

            poLineitems = await _BadgerApiHelper.GenericGetAsync<object>("/purchaseorders/lineitems/" + product_id.ToString() + "/" + PO_id.ToString());

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
                                itemDocuments.Add("url", formFile.FileName);
                                itemDocuments.Add("created_by", Int32.Parse(loginUserId));
                                await _BadgerApiHelper.GenericPostAsyncString<String>(itemDocuments.ToString(Formatting.None), "/purchaseordermanagement/documentcreate");

                                JObject itemDocStatus = new JObject();
                                itemDocStatus.Add("has_doc", 1);
                                itemDocStatus.Add("updated_by", Int32.Parse(loginUserId));
                                await _BadgerApiHelper.GenericPostAsyncString<String>(itemDocStatus.ToString(Formatting.None), "/purchaseordermanagement/itemupdate/" + purchaseorderfile.po_id);
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
            string loginUserId = await _LoginHelper.GetLoginUserId();
            string updateItemID = "0";
            try
            {
                updateItemID = await _BadgerApiHelper.GenericPostAsyncString<String>(json.ToString(Formatting.None), "/purchaseordermanagement/itemupdate/" + id.ToString());

                if (updateItemID == "Success")
                {

                    int po_id = json.Value<int>("pO_id");
                    int ra_status = json.Value<int>("ra_status");

                    dynamic result = await _BadgerApiHelper.GenericGetAsync<object>("/purchaseorders/GetItemsByPurchaseOrderStatusCountResponse/" + po_id);

                    JObject purchaseOrdersData = new JObject();
                    purchaseOrdersData.Add("po_id", po_id);

                    if (result.itemstatus > 0)
                    {
                        purchaseOrdersData.Add("po_status", 3);
                    }

                    if (result.rastatusone > 0)
                    {
                        purchaseOrdersData.Add("ra_flag", 1);
                    }
                    else
                    {
                        purchaseOrdersData.Add("ra_flag", 2);
                    }

                    purchaseOrdersData.Add("updated_by", Int32.Parse(loginUserId));
                    purchaseOrdersData.Add("updated_at", _common.GetTimeStemp());

                    updateItemID = await _BadgerApiHelper.GenericPutAsyncString<String>(purchaseOrdersData.ToString(Formatting.None), "/purchaseorders/updatespecific/" + po_id);
                }
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
      Developer: Azeem hassan
      Date: 7-5-19 
      Action: update Purchase Order sku weight update by id by using badger api helper and login helper 
      URL: /purchaseorders/MultipleskuWeightUpdate/id
      Request: Get
      Input: int id, FromBody json object
      output: string of purchase orders sku
      */
        [Authorize]
        [HttpPost("purchaseorders/MultipleskuWeightUpdate")]
        public async Task<string> MultipleskuWeightUpdate([FromBody] JObject json)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            string updateSkuID = "0";
            try
            {
                JObject Data = JObject.Parse(json.ToString());
                JArray skuData = (JArray)Data["skuData"];
                for (int i = 0; i < skuData.Count; i++)
                {
                    JObject skuUpdate = new JObject();
                    skuUpdate.Add("sku_id", skuData[i].Value<string>("sku_id"));
                    skuUpdate.Add("weight", skuData[i].Value<string>("weight"));
                    skuUpdate.Add("updated_by", Int32.Parse(loginUserId));
                    skuUpdate.Add("updated_at", _common.GetTimeStemp());

                    updateSkuID = await _BadgerApiHelper.GenericPutAsyncString<String>(skuUpdate.ToString(Formatting.None), "/sku/updatespecific/" + skuData[i].Value<string>("sku_id"));
                }
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
        /*public IActionResult PurchaseOrdersCheckIn()
        {
            return View();
        }*/

        /*
        Developer: Sajid Khan
        Date: 7-11-19 
        Action: delete Purchase Order document by id by using badger api helper and login helper  
        URL: /purchaseorders/documentdelete/id
        Request: Post
        Input: int id, FromBody json object
        output: string of purchase orders delete
        */
        [Authorize]
        [HttpPost("purchaseorders/documentsdelete/{id}")]
        public async Task<string> DocumentsDelete(int id, [FromBody] JObject json)
        {
            SetBadgerHelper();
            string res = "0";
            string loginUserId = await _LoginHelper.GetLoginUserId();

            JObject purchaseOrderDocumentDelete = new JObject();
            purchaseOrderDocumentDelete.Add("doc_id", json.Value<string>("doc_id"));
            purchaseOrderDocumentDelete.Add("po_id", json.Value<string>("po_id"));
            purchaseOrderDocumentDelete.Add("updated_by", Int32.Parse(loginUserId));

            string fileName = json.Value<string>("url");

            if (fileName != null || fileName != string.Empty)
            {
                if ((System.IO.File.Exists(UploadPath + fileName)))
                {
                    System.IO.File.Delete(UploadPath + fileName);
                }

            }
            res = await _BadgerApiHelper.GenericPostAsyncString<string>(purchaseOrderDocumentDelete.ToString(Formatting.None), "/purchaseorders/documentdelete/" + id.ToString());

            if (res != "0")
            {
                if (json.Value<string>("item") == "item")
                {
                    dynamic purchaseOrderItemDocs = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseordermanagement/getitemdocuments/" + json.Value<string>("itemid") + "/0");
                    if (purchaseOrderItemDocs.Count == 0)
                    {
                        JObject itemDocStatus = new JObject();
                        itemDocStatus.Add("has_doc", 2);
                        itemDocStatus.Add("updated_by", Int32.Parse(loginUserId));
                        await _BadgerApiHelper.GenericPostAsyncString<String>(itemDocStatus.ToString(Formatting.None), "/purchaseordermanagement/itemupdate/" + json.Value<string>("itemid"));
                    }
                }
                else
                {
                    dynamic purchaseOrderDocs = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseorders/getdocuments/" + json.Value<string>("po_id") + "/0");
                    if (purchaseOrderDocs.Count == 0)
                    {
                        JObject purchaseOrderStatusDoc = new JObject();
                        purchaseOrderStatusDoc.Add("has_doc", 2);
                        purchaseOrderStatusDoc.Add("updated_by", Int32.Parse(loginUserId));
                        await _BadgerApiHelper.GenericPutAsyncString<String>(purchaseOrderStatusDoc.ToString(Formatting.None), "/purchaseorders/updatespecific/" + json.Value<string>("po_id"));
                    }
                }


            }

            return res;
        }

        /*
        Developer: Sajid Khan
        Date: 7-19-19 
        Action: update Product wash type by id by using badger api helper and login helper  
        URL: /purchaseorders/productwashtypeupdate/id
        Request: Post
        Input: int id, FromBody json object
        output: string of Product
        */
        [Authorize]
        [HttpPost("purchaseorders/productwashtypeupdate/{id}")]
        public async Task<string> ProductUpdate(int id, [FromBody] JObject json)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            string updateSkuID = "0";
            try
            {
                JObject productUpdate = new JObject();
                id = Int32.Parse(json.Value<string>("product_id"));
                productUpdate.Add("product_id", json.Value<string>("product_id"));
                productUpdate.Add("wash_type_id", json.Value<string>("wash_type_id"));
                productUpdate.Add("updated_by", Int32.Parse(loginUserId));
                productUpdate.Add("updated_at", _common.GetTimeStemp());

                updateSkuID = await _BadgerApiHelper.GenericPutAsyncString<String>(productUpdate.ToString(Formatting.None), "/product/updatespecific/" + id);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating product wash type with message" + ex.Message);
                updateSkuID = "Failed";
            }
            return updateSkuID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-20-19 
        Action: Check sku Already exist or not by sku by using badger api helper 
        URL: /purchaseorders/checkskuexist/sku
        Request: Get
        Input: string sku
        output: string true/false
        */
        [HttpGet("purchaseorders/checkskuexist/{sku}")]
        public async Task<string> CheckSkuExist(string sku)
        {
            SetBadgerHelper();

            string result = "false";
            try
            {
                result = await _BadgerApiHelper.GenericGetAsync<string>("/sku/checkskuexist/" + sku);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating product wash type with message" + ex.Message);
            }
            return result;
        }

        /*
        Developer: Sajid Khan
        Date: 7-20-19 
        Action: Check Barcode Already exist or not by barcode by using badger api helper 
        URL: /purchaseorders/checkbarcodeexist/12345678
        Request: Get
        Input: int barcode
        output: string true/false
        */
        [HttpGet("purchaseorders/checkbarcodeexist/{barcode}")]
        public async Task<string> CheckBarcodeExist(int barcode)
        {
            SetBadgerHelper();

            string result = "false";
            try
            {
                result = await _BadgerApiHelper.GenericGetAsync<string>("/purchaseorders/checkbarcodeexist/" + barcode);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating product wash type with message" + ex.Message);
            }
            return result;
        }

        /*
        Developer: Sajid Khan
        Date: 7-20-19 
        Action: Check Barcode Already exist or not by barcode by using badger api helper 
        URL: /purchaseorders/checkbarcodeexist/12345678
        Request: Get
        Input: int barcode
        output: string true/false
        */
        [HttpGet("purchaseorders/checkpoexist/{colname}/{colvalue}")]
        public async Task<string> CheckPOExist(string colname, string colvalue)
        {
            SetBadgerHelper();

            string result = "false";
            try
            {
                result = await _BadgerApiHelper.GenericGetAsync<string>("/purchaseorders/checkpoexist/" + colname + "/" + colvalue);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating product wash type with message" + ex.Message);
            }
            return result;
        }

        /*
        Developer: Sajid Khan
        Date: 7-24-19 
        Action: Get Purchase Order item data with tracking and document and smallest sku with product name  by using badger api helper
        URL: /purchaseorders/PurchaseOrderItemDetails/poid
        Request: Get
        Input:int poid
        output: dynamic object of purchase orders line item
        */
        [HttpGet("purchaseorders/PurchaseOrderItemDetails/{poid}")]
        public async Task<string> PurchaseOrderItemDetails(int poid)
        {
            SetBadgerHelper();

            dynamic purchaseOrdersData = new ExpandoObject();

            dynamic purchaseOrderTracking = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseorderstracking/gettracking/" + poid.ToString());
            dynamic purchaseOrderDocs = await _BadgerApiHelper.GenericGetAsync<Object>("/purchaseorders/getdocuments/" + poid.ToString() + "/0");

            dynamic ItemsDetails = await _BadgerApiHelper.GenericGetAsync<Object>("/PurchaseOrderManagement/GetItemsGroupByProductId/" + poid.ToString());

            string product_ids = "";
            int i = 0;

            List<Items> newItemsList = new List<Items>();

            foreach (dynamic element in ItemsDetails)
            {
                if (i == 0)
                {
                    product_ids += element.product_id;
                }
                else
                {
                    product_ids += "," + element.product_id;
                }

                i++;
            }

            if (product_ids != "")
            {
                string smallestSku = await _BadgerApiHelper.GenericGetsAsync("/PurchaseOrders/smallestsku/" + product_ids);

                dynamic ProductSkuList = JsonConvert.DeserializeObject(smallestSku);
                foreach (var ExpendJson in ProductSkuList)
                {
                    string SkuListString = ExpendJson.ToString();

                    string product_id = SkuListString.Split(":").First();
                    product_id = product_id.Replace("\"", "").Trim();

                    string sku = SkuListString.Split(":").Last();
                    sku = sku.Replace("\"", "").Trim();

                    dynamic smallestSkus = await _BadgerApiHelper.GenericGetAsync<Object>("/PurchaseOrders/GetNameAndSizeByProductAndSku/" + product_id + "/" + sku);

                    foreach (dynamic s in smallestSkus)
                    {
                        foreach (dynamic newItemList in ItemsDetails)
                        {
                            string pro_id = newItemList.product_id;

                            string pid = s.product_id;
                            string pname = s.product_name;
                            string size = s.size;

                            if (pid == pro_id)
                            {
                                newItemsList.Add(new Items
                                {
                                    item_id = newItemList.item_id,
                                    barcode = newItemList.barcode,
                                    slot_number = newItemList.slot_number,
                                    bag_code = newItemList.bag_code,
                                    item_status_id = newItemList.item_status_id,
                                    ra_status = newItemList.ra_status,
                                    sku = newItemList.sku,
                                    sku_id = newItemList.sku_id,
                                    product_id = newItemList.product_id,
                                    vendor_id = newItemList.vendor_id,
                                    sku_family = newItemList.sku_family,
                                    PO_id = newItemList.PO_id,
                                    small_sku = sku,
                                    product_name = pname,
                                    size = size
                                }); ;
                            }


                        }
                    }
                }
            }



            purchaseOrdersData.itemsList = newItemsList;
            purchaseOrdersData.documents = purchaseOrderDocs;
            purchaseOrdersData.tracking = purchaseOrderTracking;

            return JsonConvert.SerializeObject(purchaseOrdersData);
        }

        /*
        Developer: Sajid Khan
        Date: 7-24-19 
        Action: update Purchase Order checkin form data by using badger api helper and login helper
        URL: /purchaseorders/updatepurchaseordercheckin/id
        Request: Post
        Input: int id, FromBody json object
        output: string
        */
        [Authorize]
        [HttpPost("purchaseorders/updatepurchaseordercheckin/{id}")]
        public async Task<String> updatepurchaseordercheckin(int id, [FromBody] JObject json)
        {
            SetBadgerHelper();

            string loginUserId = await _LoginHelper.GetLoginUserId();

            JObject purchaseOrder = new JObject();

            purchaseOrder.Add("shipping", json.Value<string>("shipping"));
            purchaseOrder.Add("po_status", 6);
            purchaseOrder.Add("updated_by", Int32.Parse(loginUserId));
            purchaseOrder.Add("updated_at", _common.GetTimeStemp());

            String newPurchaseOrderID = await _BadgerApiHelper.GenericPutAsyncString<String>(purchaseOrder.ToString(Formatting.None), "/purchaseorders/updatespecific/" + id);

            if (newPurchaseOrderID == "Success")
            {
                JObject allData = JObject.Parse(json.ToString());
                JArray trackings = (JArray)allData["tracking"];
                foreach (var track in trackings)
                {
                    if (track.Value<string>("track") != "")
                    {
                        if (track.Value<string>("id") == null)
                        {
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
                            await _BadgerApiHelper.GenericPutAsyncString<String>(PurchaseOrdersTracking.ToString(Formatting.None), "/purchaseorderstracking/update/" + track.Value<string>("id").ToString());
                        }

                    }
                }

                JArray items_barcodes = (JArray)allData["items_barcodes"];
                foreach (dynamic item in items_barcodes)
                {
                    if (item.item_id != "")
                    {
                        if (item.barcode != "")
                        {
                            string item_id = item.item_id;
                            JObject UpdatePurchaseOrdersItemBarcode = new JObject();
                            UpdatePurchaseOrdersItemBarcode.Add("item_id", item_id);
                            UpdatePurchaseOrdersItemBarcode.Add("barcode", item.barcode);
                            UpdatePurchaseOrdersItemBarcode.Add("item_status_id", 6);
                            UpdatePurchaseOrdersItemBarcode.Add("updated_by", Int32.Parse(loginUserId));
                            UpdatePurchaseOrdersItemBarcode.Add("updated_at", _common.GetTimeStemp());
                            await _BadgerApiHelper.GenericPostAsyncString<String>(UpdatePurchaseOrdersItemBarcode.ToString(Formatting.None), "/purchaseorders/ItemSpecificUpdateById/" + item_id);
                        }
                    }
                }

            }

            return newPurchaseOrderID;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Claim([FromBody] ClaimModel claim)
        {
            try
            {
                SetBadgerHelper();
                var userId = await _LoginHelper.GetLoginUserId();
                BindClaimerType(claim, userId);
                var response = await _BadgerApiHelper.GenericPostAsync(claim, "/PurchaseOrders/Claim/");
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private static void BindClaimerType(ClaimModel claim, string userId)
        {
            if (claim.claim_type == ClaimerType.InspectClaimer)
                claim.inspect_claimer = Convert.ToInt32(userId);
            else
                claim.publish_claimer = Convert.ToInt32(userId);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveClaim([FromBody] ClaimModel claim)
        {
            try
            {
                SetBadgerHelper();
                var userId = await _LoginHelper.GetLoginUserId();
                BindClaimerType(claim, userId);
                var response = await _BadgerApiHelper.GenericPostAsync(claim, "/PurchaseOrders/removeclaim/");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }

        [Authorize]
        [HttpGet("purchaseorders/loadclaim/{poId:int}")]
        public async Task<IActionResult> LoadClaim(int poId)
        {
            SetBadgerHelper();
            var userId = await _LoginHelper.GetLoginUserId();
            string a = "aa";
            int i = int.Parse(a);
            var response = await _BadgerApiHelper.GetAsync<PoClaim>("/PurchaseOrders/loadclaim/" + poId);
            return Ok(response);
        }

        /*
        Developer:  Rizvan Ali
        Date: 7-5-19 
        Action: get and Verify Total Styles available in po as well as in list of items  
        URL: /purchaseorders/verifyStylesQuantity/id
        Request: Get
        Input: int id
        output: bool
        */
        [Authorize]
        [HttpGet("purchaseorders/verifyStylesQuantity/{poId}")]
        public async Task<bool> VerifyTotalStyle(int poId)
        {
            SetBadgerHelper();

            bool result = false;
            try
            {
                result = await _BadgerApiHelper.GenericGetAsync<bool>("/purchaseorders/verifyStylesQuantity/" + poId);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in verifyng total Style in PO" + ex.Message);
            }
            return result;
        }
    }
}
