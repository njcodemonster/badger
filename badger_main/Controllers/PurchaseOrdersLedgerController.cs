using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using GenericModals.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using GenericModals.Event;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersLedgerController : ControllerBase
    {
        private readonly IPurchaseOrdersLedgerRepository _PurchaseOrdersLedgerRepo;
        ILoggerFactory _loggerFactory;

        IEventRepo _eventRepo;

        private string userEventTableName = "user_events";
        private string tableName = "purchase_order_events";

        string ledger_create = "ledger_create";
        string ledger_update = "ledger_update";
        string ledger_specificupdate = "ledger_specificupdate";

        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        public PurchaseOrdersLedgerController(IPurchaseOrdersLedgerRepository PurchaseOrdersLedgerRepo, ILoggerFactory loggerFactory, IEventRepo eventRepo)
        {
            _eventRepo = eventRepo;
            _PurchaseOrdersLedgerRepo = PurchaseOrdersLedgerRepo;
            _loggerFactory = loggerFactory;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order ledger list "api/purchaseordersledger/list"
        URL: api/purchaseordersledger/list
        Request: Get
        Input: /list
        output: List of Purchase order ledger
        */
        [HttpGet("list")]
        public async Task<ActionResult<List<PurchaseOrderLedger>>> GetAsync()
        {
            List<PurchaseOrderLedger> ToReturn = new List<PurchaseOrderLedger>();
            try
            {
                return await _PurchaseOrdersLedgerRepo.GetAll(0);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseordersledger list get all with message" + ex.Message);
                return ToReturn;
            }

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order ledger list "api/purchaseordersledger/list/1"
        URL: api/purchaseordersledger/list/1
        Request: Get
        Input: int id
        output: List of Purchase order ledger
        */
        [HttpGet("list/{id}")]
        public async Task<List<PurchaseOrderLedger>> GetAsync(int id)
        {
            List<PurchaseOrderLedger> ToReturn = new List<PurchaseOrderLedger>();
            try
            {
                PurchaseOrderLedger Res = await _PurchaseOrdersLedgerRepo.GetById(id);
                ToReturn.Add(Res);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseordersledger list by id with message" + ex.Message);

            }
            return ToReturn;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order ledger by id limit 1 "api/purchaseordersledger/getledger/1"
        URL: api/purchaseordersledger/getledger/1
        Request: Get
        Input: int id
        output: List of Purchase order ledger
        */
        [HttpGet("getledger/{id}")]
        public async Task<object> GetLedger(int id)
        {
            dynamic poPageList = new object();
            try
            {
                poPageList = await _PurchaseOrdersLedgerRepo.GetPurchaseOrdersLedger(id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseordersledger getledger with message" + ex.Message);

            }

            return poPageList;

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order ledger count "api/purchaseordersledger/count"
        URL: api/purchaseordersledger/count
        Request: Get
        Input: /count
        output: Count of Purchase order ledger
        */
        [HttpGet("count")]
        public async Task<string> CountAsync()
        {
            return await _PurchaseOrdersLedgerRepo.Count();

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Create purchase order ledger "api/purchaseordersledger/create"  with events created in purchase order event and user event
        URL: api/purchaseordersledger/create
        Request: Post
        Input: FromBody String
        output: string of Purchase order ledger id
        */
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                PurchaseOrderLedger newPurchaseOrder = JsonConvert.DeserializeObject<PurchaseOrderLedger>(value);
                NewInsertionID = await _PurchaseOrdersLedgerRepo.Create(newPurchaseOrder);

                var eventModel = new EventModel(tableName)
                {
                    EntityId = newPurchaseOrder.po_id,
                    EventName = ledger_create,
                    RefrenceId = Int32.Parse(NewInsertionID),
                    UserId = newPurchaseOrder.created_by,
                    EventNoteId = Convert.ToInt32(NewInsertionID)
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EntityId = newPurchaseOrder.created_by,
                    EventName = ledger_create,
                    RefrenceId = Convert.ToInt32(NewInsertionID),
                    UserId = newPurchaseOrder.created_by,
                    EventNoteId = Convert.ToInt32(NewInsertionID)
                };
                await _eventRepo.AddEventAsync(userEvent);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new purchaseordersledger create with message" + ex.Message);
            }
            return NewInsertionID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update purchase order ledger by id "api/purchaseordersledger/update/1"  with events created in purchase order event and user event
        URL: api/purchaseordersledger/update/5
        Request: Put
        Input: int id, FromBody String
        output: string
        */
        [HttpPut("update/{id}")]
        public async Task<string> Update(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
            bool UpdateProcessOutput = false;
            try
            {
                PurchaseOrderLedger PurchaseOrdersToUpdate = JsonConvert.DeserializeObject<PurchaseOrderLedger>(value);
                PurchaseOrdersToUpdate.transaction_id = id;
                UpdateProcessOutput = await _PurchaseOrdersLedgerRepo.Update(PurchaseOrdersToUpdate);
                
                var eventModel = new EventModel(tableName)
                {
                    EntityId = PurchaseOrdersToUpdate.po_id,
                    EventName = ledger_update,
                    RefrenceId = id,
                    UserId = PurchaseOrdersToUpdate.updated_by,
                    EventNoteId = id
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EntityId = PurchaseOrdersToUpdate.updated_by,
                    EventName = ledger_update,
                    RefrenceId = id,
                    UserId = PurchaseOrdersToUpdate.updated_by,
                    EventNoteId = id
                };
                await _eventRepo.AddEventAsync(userEvent);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating purchaseordersledger with message" + ex.Message);
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
        Action: update specific purchase order ledger by id "api/purchaseordersledger/updatespecific/1"  with events created in purchase order event and user event
        URL: api/purchaseordersledger/updatespecific/5
        Request: Put
        Input: int id, FromBody String
        output: string
        */
        [HttpPut("updatespecific/{id}")]
        public async Task<string> UpdateSpecific(int id, [FromBody] string value)
        {
            string UpdateResult = "Success";
            try
            {
                PurchaseOrderLedger PurchaseOrdersToUpdate = JsonConvert.DeserializeObject<PurchaseOrderLedger>(value);
                PurchaseOrdersToUpdate.transaction_id = id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();

                if (PurchaseOrdersToUpdate.po_id != 0)
                {
                    ValuesToUpdate.Add("po_id", PurchaseOrdersToUpdate.po_id.ToString());
                }
                if (PurchaseOrdersToUpdate.description != null)
                {
                    ValuesToUpdate.Add("description", PurchaseOrdersToUpdate.description.ToString());
                }
                if (PurchaseOrdersToUpdate.credit != 0)
                {
                    ValuesToUpdate.Add("credit", PurchaseOrdersToUpdate.credit.ToString());
                }
                if (PurchaseOrdersToUpdate.debit != 0)
                {
                    ValuesToUpdate.Add("debit", PurchaseOrdersToUpdate.debit.ToString());
                }               
                if (PurchaseOrdersToUpdate.created_by != 0)
                {
                    ValuesToUpdate.Add("created_by", PurchaseOrdersToUpdate.created_by.ToString());
                }
                if (PurchaseOrdersToUpdate.updated_by != 0)
                {
                    ValuesToUpdate.Add("updated_by", PurchaseOrdersToUpdate.updated_by.ToString());
                }
                if (PurchaseOrdersToUpdate.created_at != 0)
                {
                    ValuesToUpdate.Add("created_at", PurchaseOrdersToUpdate.created_at.ToString());
                }
                if (PurchaseOrdersToUpdate.updated_at != 0)
                {
                    ValuesToUpdate.Add("updated_at", PurchaseOrdersToUpdate.updated_at.ToString());
                }

                await _PurchaseOrdersLedgerRepo.UpdateSpecific(ValuesToUpdate, "transaction_id=" + id);
                
                var eventModel = new EventModel(tableName)
                {
                    EntityId = PurchaseOrdersToUpdate.po_id,
                    EventName = ledger_specificupdate,
                    RefrenceId = id,
                    UserId = PurchaseOrdersToUpdate.updated_by,
                    EventNoteId = id
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EntityId = PurchaseOrdersToUpdate.updated_by,
                    EventName = ledger_specificupdate,
                    RefrenceId = id,
                    UserId = PurchaseOrdersToUpdate.updated_by,
                    EventNoteId = id
                };
                await _eventRepo.AddEventAsync(userEvent);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating updatespecific purchaseordersledger with message" + ex.Message);
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
