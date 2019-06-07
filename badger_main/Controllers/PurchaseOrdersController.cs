﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using badgerApi.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Dynamic;
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
        private int note_type = 4;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        public PurchaseOrdersController(IPurchaseOrdersRepository PurchaseOrdersRepo, ILoggerFactory loggerFactory, INotesAndDocHelper NotesAndDoc, IConfiguration config, IItemServiceHelper ItemsHelper)
        {
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
                logger.LogInformation("Problem happened in selecting the data for get all with message" + ex.Message);
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
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

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
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);

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
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new attribute with message" + ex.Message);
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
                double created_at = _common.GetTimeStemp();
                newNoteID = await _NotesAndDoc.GenericPostNote<string>(ref_id, note_type, note, 1, created_at);
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
                double created_at = _common.GetTimeStemp();
                NewInsertionID =  await _NotesAndDoc.GenericPostDoc<string>(ref_id, note_type, document_url, "", 1, created_at);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new Purchase Order Document with message" + ex.Message);
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
                logger.LogInformation("Problem happened in selecting the data for Get Note with message" + ex.Message);

            }

            return notes;

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
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating  attribute with message" + ex.Message);
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
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating new attribute with message" + ex.Message);
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
