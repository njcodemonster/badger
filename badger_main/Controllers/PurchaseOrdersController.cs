using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using GenericModals.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using badgerApi.Helper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Data;
using Dapper;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Collections;
using System.Dynamic;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IPurchaseOrdersRepository _PurchaseOrdersRepo;
        private readonly iBarcodeRangeRepo _BarcodeRangeRepo;
        ILoggerFactory _loggerFactory;

        private INotesAndDocHelper _NotesAndDoc;
        private IItemServiceHelper _ItemsHelper;
        IEventRepo _eventRepo;

        private int note_type = 4;

        private int event_type_po_id = 2;
        private int event_type_po_note_create_id = 6;
        private int event_type_po_document_create_id = 7;
        private int event_type_po_update_id = 8;
        private int event_type_po_specific_update_id = 9;
        private int event_type_po_delete_id = 24;
        private int event_type_po_delete_document_id = 31;

        private string userEventTableName = "user_events";
        private string tableName = "purchase_order_events";

        private string event_create_purchase_orders = "Purchase order created by user =%%userid%% with purchase order id= %%poid%%";
        private string event_create_purchase_orders_notecreate = "Purchase order note created by user =%%userid%% with note id= %%noteid%%";
        private string event_create_purchase_orders_documentcreate = "Purchase order document created by user =%%userid%% with document id= %%documentid%%";
        private string event_update_purchase_orders = "Purchase order updated by user =%%userid%% with purchase order id= %%poid%%";
        private string event_updatespecific_purchase_orders = "Purchase order specific updated by user =%%userid%% with purchase order id= %%poid%%";
        private string event_delete_purchase_orders = "Purchase order deleted by user =%%userid%% with purchase order id= %%poid%%";
        private string event_delete_document_purchase_orders = "Purchase order document deleted by user =%%userid%% with document id= %%documentid%%";

        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();

        public IDbConnection Connection
        {
            get
            {
                return new MySqlConnection(_config.GetConnectionString("ProductsDatabase"));
            }
        }

        public PurchaseOrdersController(IPurchaseOrdersRepository PurchaseOrdersRepo, ILoggerFactory loggerFactory, INotesAndDocHelper NotesAndDoc, IConfiguration config, IItemServiceHelper ItemsHelper, IEventRepo eventRepo, iBarcodeRangeRepo barcodeRangeRepo)
        {
            _eventRepo = eventRepo;
            _config = config;
            _PurchaseOrdersRepo = PurchaseOrdersRepo;
            _loggerFactory = loggerFactory;
            _NotesAndDoc = NotesAndDoc;
            _ItemsHelper = ItemsHelper;
            _BarcodeRangeRepo = barcodeRangeRepo;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get List of Purchase Orders calling from "/purchaseorders/list"
        URL: api/purchaseorders/list
        Request: Get
        Input: /list
        output: List of Purchase Orders
        */
        [HttpGet("list")]
        public async Task<ActionResult<List<PurchaseOrders>>> GetAsync()
        {
            List<PurchaseOrders> ToReturn = new List<PurchaseOrders>();
            try
            {
                return await _PurchaseOrdersRepo.GetAll(0);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseorders list with message" + ex.Message);
                return ToReturn;
            }

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order by id "api/purchaseorders/list/1"
        URL: api/purchaseorders/list/id
        Request: Get
        Input: int id of Purchase Orders
        output: List of Purchase Orders by id
        */
        [HttpGet("list/{id}")]
        public async Task<List<PurchaseOrders>> GetAsync(int id)
        {
            List<PurchaseOrders> ToReturn = new List<PurchaseOrders>();
            try
            {
                PurchaseOrders Res = await _PurchaseOrdersRepo.GetById(id);
                ToReturn.Add(Res);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseorders List by id with message" + ex.Message);

            }
            return ToReturn;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Count of purchase orders "api/purchaseorders/count"
        URL: api/purchaseorders/count
        Request: Get
        Input: /count
        output: Count of Purchase Orders
        */
        [HttpGet("count")]
        public async Task<string> CountAsync()
        {
            return await _PurchaseOrdersRepo.Count();

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order list page view limit and count "api/purchaseorders/listpageview/10/boolean"
        URL: api/purchaseorders/listpageview/10/boolean
        Request: Get
        Input: int limit and boolean (true/false)
        output: list of dynamic Object of Purchase Orders
        */
        [HttpGet("listpageview/{start}/{limit}/{countNeeded}")]
        public async Task<object> ListPageViewAsync(int start, int limit, Boolean countNeeded)
        {
            dynamic poPageList = new object();
            try
            {
                poPageList = await _PurchaseOrdersRepo.GetPurchaseOrdersPageList(start, limit);
                if (countNeeded)
                {
                    string poPageCount = await _PurchaseOrdersRepo.Count();
                    poPageList.Count = poPageCount;
                }
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseorders listpageview with message" + ex.Message);

            }

            return poPageList;

        }
        /*
       Developer: Azeem
       Date: 27-8-19 
       Action: Get purchase order page data
       URL: api/purchaseorders/singlepageview/10
       Request: Get
       Input: int id
       output: list of dynamic Object of Purchase Orders
       */
        [HttpGet("singlepageview/{id}")]
        public async Task<object> singlepageviewAsync(int id)
        {
            dynamic poPageData = new object();
            try
            {
                poPageData = await _PurchaseOrdersRepo.GetPurchaseOrdersPageData(id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseorders listpageview with message" + ex.Message);

            }

            return poPageData;

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Create new purchase orders with events created in purchase order event and user event "api/purchaseorders/create"
        URL: api/purchaseorders/create
        Request: Post
        Input: FromBody, string 
        output: string of Last insert Purchase Orders id
        */
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody] string value)
        {
            string NewInsertionID = "0";
            try
            {
                PurchaseOrders newPurchaseOrder = JsonConvert.DeserializeObject<PurchaseOrders>(value);
                NewInsertionID = await _PurchaseOrdersRepo.Create(newPurchaseOrder);

                event_create_purchase_orders = event_create_purchase_orders.Replace("%%userid%%", newPurchaseOrder.created_by.ToString()).Replace("%%poid%%", NewInsertionID);

                _eventRepo.AddPurchaseOrdersEventAsync(Int32.Parse(NewInsertionID), event_type_po_id, 0, event_create_purchase_orders, newPurchaseOrder.created_by, _common.GetTimeStemp(), tableName);

                _eventRepo.AddEventAsync(event_type_po_id, newPurchaseOrder.created_by, Int32.Parse(NewInsertionID), event_create_purchase_orders, _common.GetTimeStemp(), userEventTableName);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new purchaseorders create with message" + ex.Message);
            }
            return NewInsertionID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Create purchase order note with events created in purchase order event and user event  " api/purchaseorders/notecreate"
        URL: api/purchaseorders/notecreate
        Request: Post
        Input: FromBody, string 
        output: string of Last insert Purchase Orders note id
        */
        [HttpPost("notecreate")]
        public async Task<string> NoteCreate([FromBody]   string value)
        {
            string newNoteID = "0";
            try
            {
                dynamic newPurchaseOrderNote = JsonConvert.DeserializeObject<Object>(value);
                int ref_id = newPurchaseOrderNote.ref_id;
                string note = newPurchaseOrderNote.note;
                int created_by = newPurchaseOrderNote.created_by;
                double created_at = _common.GetTimeStemp();
                newNoteID = await _NotesAndDoc.GenericPostNote<string>(ref_id, note_type, note, created_by, created_at);

                event_create_purchase_orders_notecreate = event_create_purchase_orders_notecreate.Replace("%%userid%%", created_by.ToString()).Replace("%%noteid%%", newNoteID);

                _eventRepo.AddPurchaseOrdersEventAsync(ref_id, event_type_po_note_create_id, Int32.Parse(newNoteID), event_create_purchase_orders_notecreate, created_by, _common.GetTimeStemp(), tableName);

                _eventRepo.AddEventAsync(event_type_po_note_create_id, created_by, Int32.Parse(newNoteID), event_create_purchase_orders_notecreate, _common.GetTimeStemp(), userEventTableName);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Purchase Order Note with message" + ex.Message);
            }
            return newNoteID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Create purchase order document with events created in purchase order event and user event  " api/purchaseorders/documentcreate"
        URL: api/purchaseorders/documentcreate
        Request: Post
        Input: FromBody, string 
        output: string of Last insert Purchase Orders note id
        */
        [HttpPost("documentcreate")]
        public async Task<string> DocumentCreate([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                dynamic PurchaseOrdersToUpdate = JsonConvert.DeserializeObject<JObject>(value);

                int ref_id = PurchaseOrdersToUpdate.ref_id;
                string document_url = PurchaseOrdersToUpdate.url;
                int created_by = PurchaseOrdersToUpdate.created_by;
                double created_at = _common.GetTimeStemp();

                NewInsertionID = await _NotesAndDoc.GenericPostDoc<string>(ref_id, note_type, document_url, "", created_by, created_at);

                event_create_purchase_orders_documentcreate = event_create_purchase_orders_documentcreate.Replace("%%userid%%", created_by.ToString()).Replace("%%documentid%%", NewInsertionID);

                _eventRepo.AddPurchaseOrdersEventAsync(ref_id, event_type_po_document_create_id, Int32.Parse(NewInsertionID), event_create_purchase_orders_documentcreate, created_by, _common.GetTimeStemp(), tableName);

                _eventRepo.AddEventAsync(event_type_po_document_create_id, created_by, Int32.Parse(NewInsertionID), event_create_purchase_orders_documentcreate, _common.GetTimeStemp(), userEventTableName);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Purchase Order Document create with message" + ex.Message);
            }
            return NewInsertionID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order note by note id and limit  "api/purchaseorders/getnote/ref_id/limit"
        URL: api/purchaseorders/getnote/ref_id/limit
        Request: Get
        Input: int ref_id, int limit 
        output: List of Purchase Orders note
        */
        [HttpGet("getnote/{ref_id}/{limit}")]
        public async Task<List<Notes>> GetNoteViewAsync(int ref_id, int limit)
        {
            List<Notes> notes = new List<Notes>();
            try
            {
                notes = await _NotesAndDoc.GenericNote<Notes>(ref_id, note_type, limit);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseorders Get Note with message" + ex.Message);

            }

            return notes;

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order document by doc id and limit  "api/purchaseorders/getdocuments/ref_id/limit"
        URL: api/purchaseorders/getdocuments/ref_id/limit
        Request: Get
        Input: int ref_id, int limit 
        output: List of Purchase Orders document
        */
        [HttpGet("getdocuments/{ref_id}/{limit}")]
        public async Task<List<Documents>> GetDocumentsViewAsync(int ref_id, int limit)
        {
            List<Documents> documents = new List<Documents>();
            try
            {
                documents = await _NotesAndDoc.GenericGetDocAsync<Documents>(ref_id, note_type, limit);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseorders Get documents with message" + ex.Message);

            }

            return documents;

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update purchase order by id with events created in purchase order event and user event  "api/purchaseorders/update/5"
        URL: api/purchaseorders/update/5
        Request: Put
        Input: int id, FormBody string  
        output: string
        */
        [HttpPut("update/{id}")]
        public async Task<string> Update(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
            bool UpdateProcessOutput = false;
            try
            {
                PurchaseOrders PurchaseOrdersToUpdate = JsonConvert.DeserializeObject<PurchaseOrders>(value);
                PurchaseOrdersToUpdate.po_id = id;
                UpdateProcessOutput = await _PurchaseOrdersRepo.Update(PurchaseOrdersToUpdate);

                event_update_purchase_orders = event_update_purchase_orders.Replace("%%userid%%", PurchaseOrdersToUpdate.updated_by.ToString()).Replace("%%poid%%", id.ToString());

                _eventRepo.AddPurchaseOrdersEventAsync(id, event_type_po_update_id, id, event_update_purchase_orders, PurchaseOrdersToUpdate.updated_by, _common.GetTimeStemp(), tableName);

                _eventRepo.AddEventAsync(event_type_po_update_id, PurchaseOrdersToUpdate.updated_by, id, event_update_purchase_orders, _common.GetTimeStemp(), userEventTableName);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating purchaseorders with message" + ex.Message);
                UpdateResult = "Failed";
            }
            if (!UpdateProcessOutput)
            {
                UpdateResult = "Creation failed due to reason: No specific reson";
            }
            return UpdateResult;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update specific purchase order by id with events created in purchase order event and user event  "api/purchaseorders/updatespecific/5"
        URL: api/purchaseorders/updatespecific/5
        Request: Put
        Input: int id, FormBody string  
        output: string
        */
        [HttpPut("updatespecific/{id}")]
        public async Task<string> UpdateSpecific(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";

            try
            {
                PurchaseOrders PurchaseOrdersToUpdate = JsonConvert.DeserializeObject<PurchaseOrders>(value);
                PurchaseOrdersToUpdate.po_id = id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();

                if (PurchaseOrdersToUpdate.vendor_po_number != null)
                {
                    ValuesToUpdate.Add("vendor_po_number", PurchaseOrdersToUpdate.vendor_po_number);
                }
                if (PurchaseOrdersToUpdate.vendor_invoice_number != null)
                {
                    ValuesToUpdate.Add("vendor_invoice_number", PurchaseOrdersToUpdate.vendor_invoice_number);
                }
                if (PurchaseOrdersToUpdate.vendor_order_number != 0)
                {
                    ValuesToUpdate.Add("vendor_order_number", PurchaseOrdersToUpdate.vendor_order_number.ToString());
                }
                if (PurchaseOrdersToUpdate.vendor_id != 0)
                {
                    ValuesToUpdate.Add("vendor_id", PurchaseOrdersToUpdate.vendor_id.ToString());
                }
                if (PurchaseOrdersToUpdate.defected != 0)
                {
                    ValuesToUpdate.Add("defected", PurchaseOrdersToUpdate.defected.ToString());
                }
                if (PurchaseOrdersToUpdate.good_condition != 0)
                {
                    ValuesToUpdate.Add("good_condition", PurchaseOrdersToUpdate.good_condition.ToString());
                }
                if (PurchaseOrdersToUpdate.total_styles != 0)
                {
                    ValuesToUpdate.Add("total_styles", PurchaseOrdersToUpdate.total_styles.ToString());
                }
                if (PurchaseOrdersToUpdate.total_quantity != 0)
                {
                    ValuesToUpdate.Add("total_quantity", PurchaseOrdersToUpdate.total_quantity.ToString());
                }
                if (PurchaseOrdersToUpdate.subtotal != 0)
                {
                    ValuesToUpdate.Add("subtotal", PurchaseOrdersToUpdate.subtotal.ToString());
                }
                if (PurchaseOrdersToUpdate.shipping != 0)
                {
                    ValuesToUpdate.Add("shipping", PurchaseOrdersToUpdate.shipping.ToString());
                }
                if (PurchaseOrdersToUpdate.delivery_window_start != null)
                {
                    ValuesToUpdate.Add("delivery_window_start", PurchaseOrdersToUpdate.delivery_window_start.ToString());
                }
                if (PurchaseOrdersToUpdate.delivery_window_end != null)
                {
                    ValuesToUpdate.Add("delivery_window_end", PurchaseOrdersToUpdate.delivery_window_end.ToString());
                }
                if (PurchaseOrdersToUpdate.po_status != 0)
                {
                    ValuesToUpdate.Add("po_status", PurchaseOrdersToUpdate.po_status.ToString());
                }
                if (PurchaseOrdersToUpdate.po_discount_id != 0)
                {
                    ValuesToUpdate.Add("po_discount_id", PurchaseOrdersToUpdate.po_discount_id.ToString());
                }
                if (PurchaseOrdersToUpdate.deleted != 0)
                {
                    ValuesToUpdate.Add("deleted", PurchaseOrdersToUpdate.deleted.ToString());
                }
                if (PurchaseOrdersToUpdate.ra_flag != 0)
                {
                    ValuesToUpdate.Add("ra_flag", PurchaseOrdersToUpdate.ra_flag.ToString());
                }
                if (PurchaseOrdersToUpdate.has_note != 0)
                {
                    ValuesToUpdate.Add("has_note", PurchaseOrdersToUpdate.has_note.ToString());
                }
                if (PurchaseOrdersToUpdate.has_doc != 0)
                {
                    ValuesToUpdate.Add("has_doc", PurchaseOrdersToUpdate.has_doc.ToString());
                }
                if (PurchaseOrdersToUpdate.created_by != 0)
                {
                    ValuesToUpdate.Add("created_by", PurchaseOrdersToUpdate.created_by.ToString());
                }
                if (PurchaseOrdersToUpdate.updated_by != 0)
                {
                    ValuesToUpdate.Add("updated_by", PurchaseOrdersToUpdate.updated_by.ToString());
                }
                if (PurchaseOrdersToUpdate.order_date != 0)
                {
                    ValuesToUpdate.Add("order_date", PurchaseOrdersToUpdate.order_date.ToString());
                }
                if (PurchaseOrdersToUpdate.created_at != 0)
                {
                    ValuesToUpdate.Add("created_at", PurchaseOrdersToUpdate.created_at.ToString());
                }
                if (PurchaseOrdersToUpdate.updated_at != 0)
                {
                    ValuesToUpdate.Add("updated_at", PurchaseOrdersToUpdate.updated_at.ToString());
                }

                await _PurchaseOrdersRepo.UpdateSpecific(ValuesToUpdate, "po_id=" + id);

                if (PurchaseOrdersToUpdate.po_status == 4)
                {
                    event_delete_purchase_orders = event_delete_purchase_orders.Replace("%%userid%%", PurchaseOrdersToUpdate.updated_by.ToString()).Replace("%%poid%%", id.ToString());

                    _eventRepo.AddPurchaseOrdersEventAsync(id, event_type_po_delete_id, id, event_delete_purchase_orders, PurchaseOrdersToUpdate.updated_by, _common.GetTimeStemp(), tableName);

                    _eventRepo.AddEventAsync(event_type_po_delete_id, PurchaseOrdersToUpdate.updated_by, id, event_delete_purchase_orders, _common.GetTimeStemp(), userEventTableName);
                }
                else
                {
                    event_updatespecific_purchase_orders = event_updatespecific_purchase_orders.Replace("%%userid%%", PurchaseOrdersToUpdate.updated_by.ToString()).Replace("%%poid%%", id.ToString());

                    _eventRepo.AddPurchaseOrdersEventAsync(id, event_type_po_specific_update_id, id, event_updatespecific_purchase_orders, PurchaseOrdersToUpdate.updated_by, _common.GetTimeStemp(), tableName);

                    _eventRepo.AddEventAsync(event_type_po_specific_update_id, PurchaseOrdersToUpdate.updated_by, id, event_updatespecific_purchase_orders, _common.GetTimeStemp(), userEventTableName);
                }
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating new updatespecific purchaseorders with message" + ex.Message);
                UpdateResult = "Failed";
            }

            return UpdateResult;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order line item by product id and purchase order id  "api/purchaseorders/lineitems/productid/poid"
        URL: api/purchaseorders/lineitems/productid/poid
        Request: Get
        Input: int productid, int purchase order id
        output: List of Purchase order line item
        */
        [HttpGet("lineitems/{productid}/{poid}")]
        public async Task<IEnumerable<PurchaseOrderLineItems>> GetAsyncLineitems(Int32 productid, int poid)
        {
            IEnumerable<PurchaseOrderLineItems> ToReturn = new List<PurchaseOrderLineItems>();
            try
            {
                ToReturn = await _PurchaseOrdersRepo.GetPOLineitems(productid, poid);
                //ToReturn.Add(Res);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseorders Line items by id with message" + ex.Message);

            }
            return ToReturn;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: delete purchase order document by id "api/purchaseorders/documentdelete/1"  with events created in purchase order event and user event
        URL: api/purchaseorders/documentdelete/5
        Request: Delete
        Input: int id, FromBody string
        output: string of Purchase order document delete
        */
        [HttpPost("documentdelete/{id}")]
        public async Task<bool> DocumentDelete(int id, [FromBody] string value)
        {
            Boolean res = false;
            try
            {
                using (IDbConnection conn = Connection)
                {
                    String DeleteQuery = "Delete From documents WHERE doc_id =" + id.ToString();
                    var docDetails = await conn.QueryAsync<object>(DeleteQuery);
                    res = true;

                    dynamic PurchaseOrdersToUpdate = JsonConvert.DeserializeObject<JObject>(value);
                    int po_id = PurchaseOrdersToUpdate.po_id;
                    int updated_by = PurchaseOrdersToUpdate.updated_by;

                    event_delete_document_purchase_orders = event_delete_document_purchase_orders.Replace("%%userid%%", PurchaseOrdersToUpdate.updated_by.ToString()).Replace("%%documentid%%", id.ToString());

                    _eventRepo.AddPurchaseOrdersEventAsync(po_id, event_type_po_delete_document_id, id, event_delete_document_purchase_orders, updated_by, _common.GetTimeStemp(), tableName);

                    _eventRepo.AddEventAsync(event_type_po_delete_document_id, updated_by, id, event_delete_document_purchase_orders, _common.GetTimeStemp(), userEventTableName);

                }
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseorders document deleted by id with message" + ex.Message);
            }
            return res;
        }

        /*
        Developer: Sajid Khan
        Date: 7-20-19 
        Action: Check Barcode already Exist or not by barcode "api/purchaseorders/checkbarcodeexist/1"
        URL: api/purchaseorders/checkbarcodeexist/1
        Request: Get
        Input: int barcode
        output: boolean
        */
        [HttpGet("checkbarcodeexist/{barcode}")]
        public async Task<Boolean> CheckBarcodeExist(int barcode)
        {
            Boolean result = false;
            try
            {
                result = await _ItemsHelper.CheckBarcodeExist(barcode);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

            }
            return result;
        }

        /*
        Developer: Sajid Khan
        Date: 7-20-19 
        Action: Check Barcode already Exist or not by barcode "api/purchaseorders/checkbarcodeexist/1"
        URL: api/purchaseorders/checkbarcodeexist/1
        Request: Get
        Input: int barcode
        output: boolean
        */
        [HttpGet("checkpoexist/{colname}/{colvalue}")]
        public async Task<Boolean> CheckPOExist(string colname, string colvalue)
        {
            Boolean result = false;
            List<PurchaseOrders> ToReturn = new List<PurchaseOrders>();
            try
            {
                ToReturn = await _PurchaseOrdersRepo.CheckPOExist(colname, colvalue);

                if (ToReturn.Count > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

            }
            return result;
        }

        /*
        Developer: Sajid Khan
        Date: 7-24-19 
        Action: Get smallest sku by product ids with comma separate "api/purchaseorders/smallestsku/1"
        URL: api/purchaseorders/smallestsku/ids
        Request: Get
        Input: string product_ids
        output: string product id and sku
        */
        [HttpGet("smallestsku/{product_ids}")]
        public async Task<string> SmallestSku(string product_ids)
        {
            string result = "";
            string sku;
            List<int> skuIdReturnList = new List<int>();
            JObject AllSkuIdsList = new JObject();
            try
            {
                int countComma = product_ids.Count(c => c == ',');
                if (countComma > 0)
                {
                    var ids = product_ids.Split(",");
                    foreach (var productID in ids)
                    {
                        object SkuList = await _PurchaseOrdersRepo.GetSkuByProduct(productID);
                        IEnumerable SkuListEnum = SkuList as IEnumerable;
                        List<string> skuListInt = new List<string>();
                        IDictionary<string, string> skuSortLink = new Dictionary<string, string>();
                        if (SkuListEnum != null)
                        {
                            foreach (dynamic element in SkuListEnum)
                            {
                                sku = element.sku;
                                int countDash = sku.Count(c => c == '-');
                                if (countDash > 0)
                                {
                                    skuListInt.Add(sku.Split('-').Last().ToString());
                                    skuSortLink[sku.Split('-').Last()] = sku;
                                }
                                else
                                {
                                    skuListInt.Add(element.sku_id.ToString());
                                    skuSortLink[element.sku_id.ToString()] = sku;
                                }
                            }
                            string[] skuArrayInt = skuListInt.ToArray();
                            Array.Sort(skuArrayInt);

                            if (skuArrayInt.Count() > 0)
                            {
                                int i = 0;
                                foreach (var sku_id in skuArrayInt)
                                {
                                    if (i == 0)
                                    {
                                        string index = sku_id.ToString();
                                        AllSkuIdsList.Add(productID, skuSortLink[index]);
                                    }
                                    i++;

                                }

                            }

                        }
                    }
                }
                else
                {
                    object SkuList = await _PurchaseOrdersRepo.GetSkuByProduct(product_ids);
                    IEnumerable SkuListEnum = SkuList as IEnumerable;
                    List<string> skuListInt = new List<string>();
                    IDictionary<string, string> skuSortLink = new Dictionary<string, string>();
                    if (SkuListEnum != null)
                    {
                        foreach (dynamic element in SkuListEnum)
                        {
                            sku = element.sku;
                            int countDash = sku.Count(c => c == '-');
                            if (countDash > 0)
                            {
                                skuListInt.Add(sku.Split('-').Last().ToString());
                                skuSortLink[sku.Split('-').Last()] = sku;
                            }
                            else
                            {
                                skuListInt.Add(element.sku_id.ToString());
                                skuSortLink[element.sku_id.ToString()] = sku;
                            }
                        }
                        string[] skuArrayInt = skuListInt.ToArray();
                        Array.Sort(skuArrayInt);

                        if (skuArrayInt.Count() > 0)
                        {
                            int i = 0;
                            foreach (var sku_id in skuArrayInt)
                            {
                                if (i == 0)
                                {
                                    string index = sku_id.ToString();
                                    AllSkuIdsList.Add(product_ids, skuSortLink[index]);
                                }
                                i++;

                            }

                        }

                    }
                }
                result = AllSkuIdsList.ToString(Formatting.None);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

            }
            return result;
        }

        /*
        Developer: Sajid Khan
        Date: 7-20-19 
        Action: Get Product Name And Size By Product Id And Sku Id "api/purchaseorders/GetNameAndSizeByProductAndSku/productid/sku"
        URL: api/purchaseorders/GetNameAndSizeByProductAndSku/productid/sku
        Request: Get
        Input: string product_id, string sku
        output: Dynamic object list of product detail
        */
        [HttpGet("GetNameAndSizeByProductAndSku/{product_id}/{sku}")]
        public async Task<object> GetNameAndSizeByProductAndSku(string product_id, string sku)
        {
            dynamic ProductList = new object();
            try
            {
                ProductList = await _PurchaseOrdersRepo.GetNameAndSizeByProductAndSku(product_id, sku);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

            }
            return ProductList;
        }

        /*
        Developer: Sajid Khan
        Date: 7-24-19 
        Action: Item barcode update by id 
        URL: api/purchaseorders/ItemBarcodeUpdateWithStatus/id
        Request: Post
        Input: int id, FromBody string value
        output: string of item update
        */
        [HttpPost("ItemSpecificUpdateById/{id}")]
        public async Task<string> ItemSpecificUpdateById(int id, [FromBody] string value)
        {
            string ItemUpdate = "0";
            try
            {
                ItemUpdate = await _ItemsHelper.ItemSpecificUpdateById(id, value);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Purchase Order Document create with message" + ex.Message);
            }
            return ItemUpdate;
        }

        /*
        Developer: Sajid Khan
        Date: 7-20-19 
        Action: Get Product Name And Size By Product Id And Sku Id "api/purchaseorders/GetNameAndSizeByProductAndSku/productid/sku"
        URL: api/purchaseorders/GetNameAndSizeByProductAndSku/productid/sku
        Request: Get
        Input: string product_id, string sku
        output: Dynamic object list of product detail
        */
        [HttpGet("searchbypoandinvoice/{search}")]
        public async Task<object> SearchByPOAndInvoice(string search)
        {
            dynamic ProductList = new object();
            try
            {
                ProductList = await _PurchaseOrdersRepo.SearchByPOAndInvoice(search);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

            }
            return ProductList;
        }

        /*
        Developer: Sajid Khan
        Date: 7-27-19 
        Action: Get Item status count response "api/purchaseorders/GetItemsByPurchaseOrderStatusCountResponse/poid"
        URL: api/purchaseorders/GetItemsByPurchaseOrderStatusCountResponse/poid
        Request: Get
        Input: int poid
        output: Dynamic count item status response
        */
        [HttpGet("GetItemsByPurchaseOrderStatusCountResponse/{poid}")]
        public async Task<object> GetItemsByPurchaseOrderStatusCountResponse(int poid)
        {
            dynamic countData = new ExpandoObject();
            dynamic ProductList = new object();

            int CountRaStatusOne = 0;
            int CountRaStatusZero = 0;
            int CountItemStatusDefault = 0;
            int CountItemStatus = 0;

            int rastatus = 0;
            int itemstatus = 0;
            try
            {
                ProductList = await _ItemsHelper.GetItemsByOrder(poid);
                int TotalItemCount = ProductList.Count;
                for (var i = 0; i < (TotalItemCount - 1); i++)
                {
                    rastatus = ProductList[i].ra_status;
                    itemstatus = ProductList[i].item_status_id;
                    if (rastatus == 0)
                    {
                        CountRaStatusZero++;
                    }
                    if (rastatus > 0)
                    {
                        CountRaStatusOne++;
                    }

                    if (itemstatus == 1)
                    {
                        CountItemStatusDefault++;
                    }

                    if (itemstatus > 1)
                    {
                        CountItemStatus++;
                    }

                }
                countData.rastatuszero = CountRaStatusZero;
                countData.rastatusone = CountRaStatusOne;
                countData.itemstatusdefault = CountItemStatusDefault;
                countData.itemstatus = CountItemStatus;
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

            }
            return countData;
        }



        /*
        Developer: Sajid Khan
        Date: 08-09-19 
        Action:Get item with barcode data by barcode "api/purchaseorders/getbarcode/12345678"
        URL: api/purchaseorders/getbarcode/12346578
        Request: Get
        Input: int barcode
        output: dynamic list of barcode
        */
        [HttpGet("getbarcode/{barcode}")]
        public async Task<object> GetBarcode(int barcode)
        {
            dynamic barcodeDetails = new object();
            try
            {
                barcodeDetails = await _ItemsHelper.GetBarcode(barcode);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get sku with message" + ex.Message);

            }

            return barcodeDetails;
        }

        /*
        Developer: Sajid Khan
        Date: 08-09-19 
        Action: Get PO data by search "api/purchaseorders/getpolist/sk100-1"
        URL: api/purchaseorders/getpolist/search
        Request: Get
        Input: string search
        output: dynamic list of po
        */
        [HttpGet("getpolist/{search}")]
        public async Task<Object> GetPOList(string search)
        {
            dynamic poList = new object();
            try
            {
                poList = await _PurchaseOrdersRepo.GetPOList(search);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get sku with message" + ex.Message);

            }

            return poList;
        }



        /*
        Developer: Rizwan Ali
        Date: 9-8-19 
        Action: Get barcode ranges all that are in the system  "api/purchaseorders/getBarcodeRange"
        URL: api/purchaseorders/listpageview/10/boolean
        Request: Get
        Input: none
        output: list of barcode ranges all that are in the system
        */
        [HttpGet("getBarcodeRange/")]
        public async Task<object> GetBarcodeRange()
        {
            dynamic poPageList = new object();
            try
            {
                poPageList = await _BarcodeRangeRepo.GetBarcodeRangeList();

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseorders listpageview with message" + ex.Message);

            }

            return poPageList;

        }
    }
    
}
