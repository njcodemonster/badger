using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using badgerApi.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using badgerApi.Helper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;


namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IPurchaseOrdersRepository _PurchaseOrdersRepo;
        ILoggerFactory _loggerFactory;
        private INotesAndDocHelper _NotesAndDoc;
        private IItemServiceHelper _ItemsHelper;
        IEventRepo _eventRepo;
        IUserEventsRepo _userEventsRepo;

        private int note_type = 4;

        private int event_type_po_id = 2;
        private int event_type_po_note_create_id = 6;
        private int event_type_po_document_create_id = 7;
        private int event_type_po_update_id = 8;
        private int event_type_po_specific_update_id = 9;

        private string tableName = "purchase_order_events";

        private string event_create_purchase_orders = "Purchase order created by user =%%userid%% with purchase order id= %%poid%%";
        private string event_create_purchase_orders_notecreate = "Purchase order note created by user =%%userid%% with note id= %%noteid%%";
        private string event_create_purchase_orders_documentcreate = "Purchase order document created by user =%%userid%% with document id= %%documentid%%";
        private string event_update_purchase_orders = "Purchase order updated by user =%%userid%% with purchase order id= %%poid%%";
        private string event_updatespecific_purchase_orders = "Purchase order specific updated by user =%%userid%% with purchase order id= %%poid%%";
        
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        public PurchaseOrdersController(IPurchaseOrdersRepository PurchaseOrdersRepo, ILoggerFactory loggerFactory, INotesAndDocHelper NotesAndDoc, IConfiguration config, IItemServiceHelper ItemsHelper, IEventRepo eventRepo, IUserEventsRepo userEventsRepo)
        {
            _userEventsRepo = userEventsRepo;
            _eventRepo = eventRepo;
            _config = config;
            _PurchaseOrdersRepo = PurchaseOrdersRepo;
            _loggerFactory = loggerFactory;
            _NotesAndDoc = NotesAndDoc;
            _ItemsHelper = ItemsHelper;
        }

        // GET: api/purchaseorders/list
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

        // GET: api/purchaseorders/list/1
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

        // GET: api/purchaseorders/count
        [HttpGet("count")]
        public async Task<string> CountAsync()
        {
            return await _PurchaseOrdersRepo.Count();

        }

        // GET: api/purchaseorders/listpageview/10/boolean
        [HttpGet("listpageview/{limit}/{countNeeded}")]
        public async Task<object> ListPageViewAsync(int limit,Boolean countNeeded)
        {
            dynamic poPageList = new object();
            try
            {
                poPageList = await _PurchaseOrdersRepo.GetPurchaseOrdersPageList(limit);
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

        // POST: api/purchaseorders/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                PurchaseOrders newPurchaseOrder = JsonConvert.DeserializeObject<PurchaseOrders>(value);
                NewInsertionID = await _PurchaseOrdersRepo.Create(newPurchaseOrder);

                event_create_purchase_orders = event_create_purchase_orders.Replace("%%userid%%", newPurchaseOrder.created_by.ToString()).Replace("%%poid%%", NewInsertionID);

                _eventRepo.AddPurchaseOrdersEventAsync(Int32.Parse(NewInsertionID), event_type_po_id, 0, event_create_purchase_orders, newPurchaseOrder.created_by, _common.GetTimeStemp(), tableName);

                _userEventsRepo.AddUserEventAsync(event_type_po_id, Int32.Parse(NewInsertionID), event_create_purchase_orders, newPurchaseOrder.created_by, _common.GetTimeStemp());
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new purchaseorders create with message" + ex.Message);
            }
            return NewInsertionID;
        }

        // POST: api/purchaseorders/notecreate
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

                _userEventsRepo.AddUserEventAsync(event_type_po_note_create_id, Int32.Parse(newNoteID), event_create_purchase_orders_notecreate, created_by, _common.GetTimeStemp());
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Purchase Order Note with message" + ex.Message);
            }
            return newNoteID;
        }

        // POST: api/purchaseorders/documentcreate
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

                NewInsertionID =  await _NotesAndDoc.GenericPostDoc<string>(ref_id, note_type, document_url, "", created_by, created_at);

                event_create_purchase_orders_documentcreate = event_create_purchase_orders_documentcreate.Replace("%%userid%%", created_by.ToString()).Replace("%%documentid%%", NewInsertionID);

                _eventRepo.AddPurchaseOrdersEventAsync(ref_id, event_type_po_document_create_id, Int32.Parse(NewInsertionID), event_create_purchase_orders_documentcreate, created_by, _common.GetTimeStemp(), tableName);

                _userEventsRepo.AddUserEventAsync(event_type_po_document_create_id, Int32.Parse(NewInsertionID), event_create_purchase_orders_documentcreate, created_by, _common.GetTimeStemp());
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Purchase Order Document create with message" + ex.Message);
            }
            return NewInsertionID;
        }

        // GET: api/purchaseorders/getnote/ref_id
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

        // GET: api/purchaseorders/getdocuments/ref_id
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

        // PUT: api/purchaseorders/update/5
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
                
                _eventRepo.AddPurchaseOrdersEventAsync(id, event_type_po_update_id, id, event_update_purchase_orders ,PurchaseOrdersToUpdate.updated_by, _common.GetTimeStemp(), tableName);

                _userEventsRepo.AddUserEventAsync(event_type_po_update_id, id, event_update_purchase_orders, PurchaseOrdersToUpdate.updated_by, _common.GetTimeStemp());
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


        // PUT: api/purchaseorders/updatespecific/1
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

                event_updatespecific_purchase_orders = event_updatespecific_purchase_orders.Replace("%%userid%%", PurchaseOrdersToUpdate.updated_by.ToString()).Replace("%%poid%%", id.ToString());

                _eventRepo.AddPurchaseOrdersEventAsync(id, event_type_po_specific_update_id, id, event_updatespecific_purchase_orders, PurchaseOrdersToUpdate.updated_by, _common.GetTimeStemp(), tableName);

                _userEventsRepo.AddUserEventAsync(event_type_po_specific_update_id, id, event_updatespecific_purchase_orders, PurchaseOrdersToUpdate.updated_by, _common.GetTimeStemp());
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating new updatespecific purchaseorders with message" + ex.Message);
                UpdateResult = "Failed";
            }

            return UpdateResult;
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
