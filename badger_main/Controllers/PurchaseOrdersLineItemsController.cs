using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using GenericModals.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using GenericModals.PurchaseOrder;
using GenericModals.Event;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersLineItemsController : ControllerBase
    {
        private readonly IPurchaseOrdersLineItemsRepo _PurchaseOrdersLineItemsRepo;
        ILoggerFactory _loggerFactory;

        IEventRepo _eventRepo;

        private string userEventTableName = "user_events";
        private string tableName = "purchase_order_events";

        string po_lineitem_created = "po_lineitem_created";
        string po_lineitem_update = "po_lineitem_update";
        string po_lineitem_specific_update = "po_lineitem_specific_update";
        public PurchaseOrdersLineItemsController(IPurchaseOrdersLineItemsRepo PurchaseOrdersLineItemsRepo, ILoggerFactory loggerFactory, IEventRepo eventRepo)
        {
            _eventRepo = eventRepo;
            _PurchaseOrdersLineItemsRepo = PurchaseOrdersLineItemsRepo;
            _loggerFactory = loggerFactory;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order line item list "api/purchaseorderlineitems/list"
        URL: api/purchaseorderlineitems/list
        Request: Get
        Input: /list
        output: List of Purchase order ledger
        */
        [HttpGet("list")]
        public async Task<ActionResult<List<PurchaseOrderLineItems>>> GetAsync()
        {
            List<PurchaseOrderLineItems> ToReturn = new List<PurchaseOrderLineItems>();
            try
            {
                return await _PurchaseOrdersLineItemsRepo.GetAll(0);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get all with message" + ex.Message);
                return ToReturn;
            }

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order line item by id "api/purchaseorderlineitems/list/1"
        URL: api/purchaseorderlineitems/list/1
        Request: Get
        Input: int id
        output: List of Purchase order ledger
        */
        [HttpGet("list/{id}")]
        public async Task<List<PurchaseOrderLineItems>> GetAsync(int id)
        {
            List<PurchaseOrderLineItems> ToReturn = new List<PurchaseOrderLineItems>();
            try
            {
                PurchaseOrderLineItems Res = await _PurchaseOrdersLineItemsRepo.GetById(id);
                ToReturn.Add(Res);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

            }
            return ToReturn;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Create purchase order line item "api/purchaseorderlineitems/create"  with events created in purchase order event and user event
        URL: api/purchaseorderlineitems/create
        Request: Post
        Input: FromBody string
        output: string of purchase order line item id
        */
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                PurchaseOrderLineItems NewPoLineItem = JsonConvert.DeserializeObject<PurchaseOrderLineItems>(value);
                NewInsertionID = await _PurchaseOrdersLineItemsRepo.Create(NewPoLineItem);

                var eventModel = new EventModel(tableName)
                {
                    EntityId = NewPoLineItem.po_id,
                    EventName = po_lineitem_created,
                    RefrenceId = Int32.Parse(NewInsertionID),
                    UserId = NewPoLineItem.created_by,
                    EventNoteId = Convert.ToInt32(NewInsertionID)
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EntityId = NewPoLineItem.created_by,
                    EventName = po_lineitem_created,
                    RefrenceId = Convert.ToInt32(NewInsertionID),
                    UserId = NewPoLineItem.created_by,
                    EventNoteId = Convert.ToInt32(NewInsertionID)
                };
                await _eventRepo.AddEventAsync(userEvent);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new attribute with message" + ex.Message);
            }
            return NewInsertionID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update purchase order line item by id "api/purchaseorderlineitems/update/1"  with events created in purchase order event and user event
        URL: api/purchaseorderlineitems/update/5
        Request: Put
        Input: FromBody string
        output: string of purchase order line item
        */
        [HttpPut("update/{id}")]
        public async Task<string> Update(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
            bool UpdateProcessOutput = false;
            try
            {
                PurchaseOrderLineItems PoLineItemToUpdate = JsonConvert.DeserializeObject<PurchaseOrderLineItems>(value);
                PoLineItemToUpdate.line_item_id = id;
                UpdateProcessOutput = await _PurchaseOrdersLineItemsRepo.Update(PoLineItemToUpdate);

                var eventModel = new EventModel(tableName)
                {
                    EntityId = PoLineItemToUpdate.po_id,
                    EventName = po_lineitem_update,
                    RefrenceId = id,
                    UserId = PoLineItemToUpdate.updated_by,
                    EventNoteId = id
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EntityId = PoLineItemToUpdate.updated_by,
                    EventName = po_lineitem_update,
                    RefrenceId = id,
                    UserId = PoLineItemToUpdate.updated_by,
                    EventNoteId = id
                };
                await _eventRepo.AddEventAsync(userEvent);
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

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update specific purchase order line item by id "api/purchaseorderlineitems/updatespecific/1"  with events created in purchase order event and user event
        URL: api/purchaseorderlineitems/updatespecific/5
        Request: Put
        Input: FromBody string
        output: string of purchase order line item
        */
        [HttpPut("updatespecific/{id}")]
        public async Task<string> UpdateSpecific(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";

            try
            {
                PurchaseOrderLineItems PoLineItemToUpdate = JsonConvert.DeserializeObject<PurchaseOrderLineItems>(value);
                PoLineItemToUpdate.line_item_id = id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();

                if (PoLineItemToUpdate.po_id != 0)
                {
                    ValuesToUpdate.Add("po_id", PoLineItemToUpdate.po_id.ToString());
                }
                if (PoLineItemToUpdate.vendor_id != 0)
                {
                    ValuesToUpdate.Add("vendor_id", PoLineItemToUpdate.vendor_id.ToString());
                }
                if (PoLineItemToUpdate.sku != null)
                {
                    ValuesToUpdate.Add("sku", PoLineItemToUpdate.sku);
                }
                if (PoLineItemToUpdate.product_id != 0)
                {
                    ValuesToUpdate.Add("product_id", PoLineItemToUpdate.product_id.ToString());
                }
                if (PoLineItemToUpdate.line_item_cost != 0)
                {
                    ValuesToUpdate.Add("line_item_cost", PoLineItemToUpdate.line_item_cost.ToString());
                }
                if (PoLineItemToUpdate.line_item_retail != 0)
                {
                    ValuesToUpdate.Add("line_item_retail", PoLineItemToUpdate.line_item_retail.ToString());
                }
                if (PoLineItemToUpdate.line_item_type != 0)
                {
                    ValuesToUpdate.Add("line_item_type", PoLineItemToUpdate.line_item_type.ToString());
                }
                if (PoLineItemToUpdate.line_item_ordered_quantity >= 0)
                {
                    ValuesToUpdate.Add("line_item_ordered_quantity", PoLineItemToUpdate.line_item_ordered_quantity.ToString());
                }
                if (PoLineItemToUpdate.line_item_accepted_quantity != 0)
                {
                    ValuesToUpdate.Add("line_item_accepted_quantity", PoLineItemToUpdate.line_item_accepted_quantity.ToString());
                }
                if (PoLineItemToUpdate.line_item_rejected_quantity != 0)
                {
                    ValuesToUpdate.Add("line_item_rejected_quantity", PoLineItemToUpdate.line_item_rejected_quantity.ToString());
                }
                if (PoLineItemToUpdate.updated_by != 0)
                {
                    ValuesToUpdate.Add("updated_by", PoLineItemToUpdate.updated_by.ToString());
                }
                if (PoLineItemToUpdate.updated_at != 0)
                {
                    ValuesToUpdate.Add("updated_at", PoLineItemToUpdate.updated_at.ToString());
                }

                await _PurchaseOrdersLineItemsRepo.UpdateSpecific(ValuesToUpdate, "line_item_id=" + id);

                var eventModel = new EventModel(tableName)
                {
                    EntityId = PoLineItemToUpdate.po_id,
                    EventName = po_lineitem_specific_update,
                    RefrenceId = id,
                    UserId = PoLineItemToUpdate.updated_by,
                    EventNoteId = id
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EntityId = PoLineItemToUpdate.updated_by,
                    EventName = po_lineitem_specific_update,
                    RefrenceId = id,
                    UserId = PoLineItemToUpdate.updated_by,
                    EventNoteId = id
                };
                await _eventRepo.AddEventAsync(userEvent);
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
