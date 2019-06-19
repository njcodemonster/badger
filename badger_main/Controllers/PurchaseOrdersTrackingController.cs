﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using badgerApi.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;


namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersTrackingController : ControllerBase
    {
        private readonly IPurchaseOrdersTrackingRepository _PurchaseOrdersTrackingRepo;
        ILoggerFactory _loggerFactory;

        IEventRepo _eventRepo;
        IUserEventsRepo _userEventsRepo;

        private int event_type_tracking_id = 4;
        private int event_type_tracking_update_id = 14;
        private int event_type_tracking_specific_update_id = 15;

        private string tableName = "purchase_order_events";

        private string event_create_purchase_orders_tracking = "Purchase order tracking created by user =%%userid%% with purchase order tracking id= %%trackingid%%";
        private string event_update_purchase_orders_tracking = "Purchase order tracking updated by user =%%userid%% with purchase order tracking id= %%trackingid%%";
        private string event_updatespecific_purchase_orders_tracking = "Purchase order tracking specific updated by user =%%userid%% with purchase order tracking id= %%trackingid%%";

        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        public PurchaseOrdersTrackingController(IPurchaseOrdersTrackingRepository PurchaseOrdersTrackingRepo, ILoggerFactory loggerFactory, IEventRepo eventRepo, IUserEventsRepo userEventsRepo)
        {
            _userEventsRepo = userEventsRepo;
            _eventRepo = eventRepo;
            _PurchaseOrdersTrackingRepo = PurchaseOrdersTrackingRepo;
            _loggerFactory = loggerFactory;
        }

        // GET: api/purchaseorderstracking/gettracking/10
        [HttpGet("gettracking/{id}")]
        public async Task<object> GetTrackingViewAsync(int id)
        {
            dynamic poPageList = new object();
            try
            {
                poPageList = await _PurchaseOrdersTrackingRepo.GetTrackingById(id);
            }
            catch (Exception ex)
             {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for purchaseorderstracking gettracking with message" + ex.Message);

            }

            return poPageList;

        }

        // POST: api/purchaseorderstracking/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                PurchaseOrdersTracking newPurchaseOrderTracking = JsonConvert.DeserializeObject<PurchaseOrdersTracking>(value);
                NewInsertionID = await _PurchaseOrdersTrackingRepo.Create(newPurchaseOrderTracking);

                event_create_purchase_orders_tracking = event_create_purchase_orders_tracking.Replace("%%userid%%", newPurchaseOrderTracking.created_by.ToString()).Replace("%%trackingid%%", NewInsertionID);

                _eventRepo.AddPurchaseOrdersEventAsync(newPurchaseOrderTracking.po_id, event_type_tracking_id, Int32.Parse(NewInsertionID), event_create_purchase_orders_tracking, newPurchaseOrderTracking.created_by, _common.GetTimeStemp(), tableName);

                _userEventsRepo.AddUserEventAsync(event_type_tracking_id, Int32.Parse(NewInsertionID), event_create_purchase_orders_tracking, newPurchaseOrderTracking.created_by, _common.GetTimeStemp());
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new new Purchase Order Tracking create with message" + ex.Message);
            }
            return NewInsertionID;
        }

        // PUT: api/purchaseorderstracking/update/5
        [HttpPut("update/{id}")]
        public async Task<string> Update(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
            bool UpdateProcessOutput = false;
            try
            {
                PurchaseOrdersTracking PurchaseOrdersTrackingToUpdate = JsonConvert.DeserializeObject<PurchaseOrdersTracking>(value);
                PurchaseOrdersTrackingToUpdate.po_tracking_id = id;
                UpdateProcessOutput = await _PurchaseOrdersTrackingRepo.Update(PurchaseOrdersTrackingToUpdate);

                event_update_purchase_orders_tracking = event_update_purchase_orders_tracking.Replace("%%userid%%", PurchaseOrdersTrackingToUpdate.updated_by.ToString()).Replace("%%trackingid%%", id.ToString());

                _eventRepo.AddPurchaseOrdersEventAsync(PurchaseOrdersTrackingToUpdate.po_id, event_type_tracking_update_id, id, event_update_purchase_orders_tracking , PurchaseOrdersTrackingToUpdate.updated_by, _common.GetTimeStemp(), tableName);

                _userEventsRepo.AddUserEventAsync(event_type_tracking_id, id, event_update_purchase_orders_tracking, PurchaseOrdersTrackingToUpdate.updated_by, _common.GetTimeStemp());
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating  purchaseorderstracking with message" + ex.Message);
                UpdateResult = "Failed";
            }
            if (!UpdateProcessOutput)
            {
                UpdateResult = "Creation failed due to reason: No specific reson";
            }
            return UpdateResult;
        }


        // PUT: api/purchaseorderstracking/updatespecific/1
        [HttpPut("updatespecific/{id}")]
        public async Task<string> UpdateSpecific(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";

            try
            {
                PurchaseOrdersTracking PurchaseOrdersToUpdate = JsonConvert.DeserializeObject<PurchaseOrdersTracking>(value);
                PurchaseOrdersToUpdate.po_tracking_id = id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();
    
                if (PurchaseOrdersToUpdate.po_id != 0)
                {
                    ValuesToUpdate.Add("po_id", PurchaseOrdersToUpdate.po_id.ToString());
                }
                if (PurchaseOrdersToUpdate.tracking_number != 0)
                {
                    ValuesToUpdate.Add("tracking_number", PurchaseOrdersToUpdate.tracking_number.ToString());
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

                await _PurchaseOrdersTrackingRepo.UpdateSpecific(ValuesToUpdate, "po_tracking_id=" + id);

                event_updatespecific_purchase_orders_tracking = event_updatespecific_purchase_orders_tracking.Replace("%%userid%%", PurchaseOrdersToUpdate.updated_by.ToString()).Replace("%%trackingid%%", id.ToString());

                _eventRepo.AddPurchaseOrdersEventAsync(PurchaseOrdersToUpdate.po_id, event_type_tracking_specific_update_id, id, event_updatespecific_purchase_orders_tracking , PurchaseOrdersToUpdate.updated_by, _common.GetTimeStemp(), tableName);

                _userEventsRepo.AddUserEventAsync(event_type_tracking_id, id, event_updatespecific_purchase_orders_tracking, PurchaseOrdersToUpdate.updated_by, _common.GetTimeStemp());

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating new updatespecific purchaseorderstracking with message" + ex.Message);
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
