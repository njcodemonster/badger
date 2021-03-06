﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using GenericModals.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Dynamic;
using badgerApi.Helper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using GenericModals.Event;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersDiscountsController : ControllerBase
    {
        private readonly IPurchaseOrdersDiscountsRepository _PurchaseOrdersDiscountsRepo;
        ILoggerFactory _loggerFactory;

        IEventRepo _eventRepo;

        private string userEventTableName = "user_events";
        private string tableName = "purchase_order_events";

        string discount_create = "discount_create";
        string discount_update = "discount_update";
        string discount_specificupdate = "discount_specificupdate";

        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        public PurchaseOrdersDiscountsController(IPurchaseOrdersDiscountsRepository PurchaseOrdersDiscountsRepo, ILoggerFactory loggerFactory, IEventRepo eventRepo)
        {
            _eventRepo = eventRepo;
            _PurchaseOrdersDiscountsRepo = PurchaseOrdersDiscountsRepo;
            _loggerFactory = loggerFactory;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order discount list "api/purchaseordersdiscounts/list"
        URL: api/purchaseordersdiscounts/list
        Request: Get
        Input: /list
        output: List of Purchase order discount
        */
        [HttpGet("list")]
        public async Task<ActionResult<List<PurchaseOrderDiscounts>>> GetAsync()
        {
            List<PurchaseOrderDiscounts> ToReturn = new List<PurchaseOrderDiscounts>();
            try
            {
                return await _PurchaseOrdersDiscountsRepo.GetAll(0);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseordersdiscounts list get all with message" + ex.Message);
                return ToReturn;
            }

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order discount by id "api/purchaseordersdiscounts/list/1"
        URL: api/purchaseordersdiscounts/list/id
        Request: Get
        Input: int id
        output: List of Purchase order discount
        */
        [HttpGet("list/{id}")]
        public async Task<List<PurchaseOrderDiscounts>> GetAsync(int id)
        {
            List<PurchaseOrderDiscounts> ToReturn = new List<PurchaseOrderDiscounts>();
            try
            {
                PurchaseOrderDiscounts Res = await _PurchaseOrdersDiscountsRepo.GetById(id);
                ToReturn.Add(Res);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseordersdiscounts list by id with message" + ex.Message);

            }
            return ToReturn;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order discount by id limit 1 "api/purchaseordersdiscounts/getdiscount/1"
        URL: api/purchaseordersdiscounts/getdiscount/id
        Request: Get
        Input: int id
        output: count of Purchase order discount
        */
        [HttpGet("getdiscount/{id}")]
        public async Task<object> GetDiscount(int id)
        {
            dynamic poPageList = new object();
            try
            {
                poPageList = await _PurchaseOrdersDiscountsRepo.GetPurchaseOrdersDiscount(id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseordersdiscounts  getdiscount with message" + ex.Message);

            }

            return poPageList;

        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order discount count "api/purchaseordersdiscounts/count"
        URL: api/purchaseordersdiscounts/count
        Request: Get
        Input: /count
        output: List of Purchase order discount
        */
        [HttpGet("count")]
        public async Task<string> CountAsync()
        {
            return await _PurchaseOrdersDiscountsRepo.Count();
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Create purchase order discount "api/purchaseordersdiscounts/create"  with events created in purchase order event and user event
        URL: api/purchaseordersdiscounts/create 
        Request: Post
        Input: FormBody String
        output: string purchase order discount id
        */
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                PurchaseOrderDiscounts newPurchaseOrder = JsonConvert.DeserializeObject<PurchaseOrderDiscounts>(value);
                NewInsertionID = await _PurchaseOrdersDiscountsRepo.Create(newPurchaseOrder);

                var eventModel = new EventModel(tableName)
                {
                    EntityId = newPurchaseOrder.po_id,
                    EventName = discount_create,
                    RefrenceId = Int32.Parse(NewInsertionID),
                    UserId = newPurchaseOrder.created_by,
                    EventNoteId = Int32.Parse(NewInsertionID)
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EntityId = newPurchaseOrder.created_by,
                    EventName = discount_create,
                    RefrenceId = Convert.ToInt32(NewInsertionID),
                    UserId = newPurchaseOrder.created_by,
                    EventNoteId = Int32.Parse(NewInsertionID)
                };
                await _eventRepo.AddEventAsync(userEvent);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new purchaseordersdiscounts create with message" + ex.Message);
            }
            return NewInsertionID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update purchase order discount by id "api/purchaseordersdiscounts/update/1"  with events created in purchase order event and user event
        URL: api/purchaseordersdiscounts/update/5
        Request: Put
        Input: FormBody String
        output: string 
        */
        [HttpPut("update/{id}")]
        public async Task<string> Update(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
            bool UpdateProcessOutput = false;
            try
            {
                PurchaseOrderDiscounts PurchaseOrdersToUpdate = JsonConvert.DeserializeObject<PurchaseOrderDiscounts>(value);
                PurchaseOrdersToUpdate.po_discount_id = id;
                UpdateProcessOutput = await _PurchaseOrdersDiscountsRepo.Update(PurchaseOrdersToUpdate);
                
                var eventModel = new EventModel(tableName)
                {
                    EntityId = PurchaseOrdersToUpdate.po_id,
                    EventName = discount_update,
                    RefrenceId = id,
                    UserId = PurchaseOrdersToUpdate.updated_by,
                    EventNoteId = id
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EntityId = PurchaseOrdersToUpdate.updated_by,
                    EventName = discount_update,
                    RefrenceId = id,
                    UserId = PurchaseOrdersToUpdate.updated_by,
                    EventNoteId = id
                };
                await _eventRepo.AddEventAsync(userEvent);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating  purchaseordersdiscounts with message" + ex.Message);
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
        Action: update specific purchase order discount by id "api/purchaseordersdiscounts/updatespecific/1"  with events created in purchase order event and user event
        URL: api/purchaseordersdiscounts/updatespecific/5
        Request: Put
        Input: FormBody String
        output: string 
        */
        [HttpPut("updatespecific/{id}")]
        public async Task<string> UpdateSpecific(int id, [FromBody] string value)
        {
            string UpdateResult = "Success";
            try
            {
                PurchaseOrderDiscounts PurchaseOrdersToUpdate = JsonConvert.DeserializeObject<PurchaseOrderDiscounts>(value);
                PurchaseOrdersToUpdate.po_discount_id = id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();

                if (PurchaseOrdersToUpdate.po_id != 0)
                {
                    ValuesToUpdate.Add("po_id", PurchaseOrdersToUpdate.po_id.ToString());
                }
                if (PurchaseOrdersToUpdate.discount_percentage != 0)
                {
                    ValuesToUpdate.Add("discount_percentage", PurchaseOrdersToUpdate.discount_percentage.ToString());
                }
                if (PurchaseOrdersToUpdate.discount_note != null)
                {
                    ValuesToUpdate.Add("discount_note", PurchaseOrdersToUpdate.discount_note.ToString());
                }
                if (PurchaseOrdersToUpdate.completed_status != 0)
                {
                    ValuesToUpdate.Add("completed_status", PurchaseOrdersToUpdate.completed_status.ToString());
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

                await _PurchaseOrdersDiscountsRepo.UpdateSpecific(ValuesToUpdate, "po_discount_id=" + id);
                
                var eventModel = new EventModel(tableName)
                {
                    EntityId = PurchaseOrdersToUpdate.po_id,
                    EventName = discount_specificupdate,
                    RefrenceId = id,
                    UserId = PurchaseOrdersToUpdate.updated_by,
                    EventNoteId = id
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EntityId = PurchaseOrdersToUpdate.updated_by,
                    EventName = discount_specificupdate,
                    RefrenceId = id,
                    UserId = PurchaseOrdersToUpdate.updated_by,
                    EventNoteId = id
                };
                await _eventRepo.AddEventAsync(userEvent);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating updatespecific purchaseordersdiscounts with message" + ex.Message);
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
