using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using GenericModals.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;
using GenericModals.Event;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersTrackingController : ControllerBase
    {
        private readonly IPurchaseOrdersTrackingRepository _PurchaseOrdersTrackingRepo;
        ILoggerFactory _loggerFactory;

        IEventRepo _eventRepo;

        private string userEventTableName = "user_events";
        private string tableName = "purchase_order_events";

        string tracking_create = "tracking_create";
        string tracking_update = "tracking_update";
        string tracking_specific_update = "tracking_specific_update";
        string tracking_delete = "tracking_delete";

        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();

        private readonly IConfiguration _config;
        public IDbConnection Connection
        {
            get
            {
                return new MySqlConnection(_config.GetConnectionString("ProductsDatabase"));
            }
        }

        public PurchaseOrdersTrackingController(IPurchaseOrdersTrackingRepository PurchaseOrdersTrackingRepo, ILoggerFactory loggerFactory, IEventRepo eventRepo, IConfiguration config)
        {
            _config = config;
            _eventRepo = eventRepo;
            _PurchaseOrdersTrackingRepo = PurchaseOrdersTrackingRepo;
            _loggerFactory = loggerFactory;
        }
    
        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get purchase order tracking by id "api/purchaseorderstracking/gettracking/1"
        URL: api/purchaseorderstracking/gettracking/10
        Request: Get
        Input: int id
        output: dynamic object of Purchase order tracking
        */
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

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Create purchase order tracking "api/purchaseorderstracking/create"  with events created in purchase order event and user event
        URL: api/purchaseorderstracking/create
        Request: Post
        Input: FromBody string
        output: string of Purchase order tracking id
        */
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                PurchaseOrdersTracking newPurchaseOrderTracking = JsonConvert.DeserializeObject<PurchaseOrdersTracking>(value);
                NewInsertionID = await _PurchaseOrdersTrackingRepo.Create(newPurchaseOrderTracking);

                var eventModel = new EventModel(tableName)
                {
                    EntityId = Int32.Parse(NewInsertionID),
                    EventName = tracking_create,
                    RefrenceId = newPurchaseOrderTracking.po_id,
                    UserId = newPurchaseOrderTracking.created_by,
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EntityId = newPurchaseOrderTracking.created_by,
                    EventName = tracking_create,
                    RefrenceId = Convert.ToInt32(NewInsertionID),
                    UserId = newPurchaseOrderTracking.created_by,
                };
                await _eventRepo.AddEventAsync(userEvent);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new new Purchase Order Tracking create with message" + ex.Message);
            }
            return NewInsertionID;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update purchase order tracking by id "api/purchaseorderstracking/update/1"  with events created in purchase order event and user event
        URL: api/purchaseorderstracking/update/5
        Request: Put
        Input: int id, FromBody string
        output: string of Purchase order tracking
        */
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
               
                var eventModel = new EventModel(tableName)
                {
                    EntityId = id,
                    EventName = tracking_update,
                    RefrenceId = PurchaseOrdersTrackingToUpdate.po_id,
                    UserId = PurchaseOrdersTrackingToUpdate.updated_by,
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EntityId = PurchaseOrdersTrackingToUpdate.updated_by,
                    EventName = tracking_update,
                    RefrenceId = id,
                    UserId = PurchaseOrdersTrackingToUpdate.updated_by,
                };
                await _eventRepo.AddEventAsync(userEvent);

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

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: update specific purchase order tracking by id "api/purchaseorderstracking/updatespecific/1"  with events created in purchase order event and user event
        URL: api/purchaseorderstracking/updatespecific/5
        Request: Put
        Input: int id, FromBody string
        output: string of Purchase order tracking
        */
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
                
                var eventModel = new EventModel(tableName)
                {
                    EntityId = id,
                    EventName = tracking_specific_update,
                    RefrenceId = PurchaseOrdersToUpdate.po_id,
                    UserId = PurchaseOrdersToUpdate.updated_by,
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EntityId = PurchaseOrdersToUpdate.updated_by,
                    EventName = tracking_specific_update,
                    RefrenceId = id,
                    UserId = PurchaseOrdersToUpdate.updated_by,
                };
                await _eventRepo.AddEventAsync(userEvent);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating new updatespecific purchaseorderstracking with message" + ex.Message);
                UpdateResult = "Failed";
            }

            return UpdateResult;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: delete purchase order tracking by id "api/purchaseorderstracking/delete/1"  with events created in purchase order event and user event
        URL: api/purchaseorderstracking/delete/5
        Request: Delete
        Input: int id, FromBody string
        output: string of Purchase order tracking
        */
        [HttpPost("delete/{id}")]
        public async Task<bool> Delete(int id, [FromBody] string value)
        {
            Boolean res = false;
            try
            {
                using (IDbConnection conn = Connection)
                {
                    String DeleteQuery = "Delete From purchase_order_tracking WHERE po_tracking_id ="+id.ToString();
                    var vendorDetails = await conn.QueryAsync<object>(DeleteQuery);
                    res = true;

                    PurchaseOrdersTracking PurchaseOrdersToUpdate = JsonConvert.DeserializeObject<PurchaseOrdersTracking>(value);
                   
                    var eventModel = new EventModel(tableName)
                    {
                        EntityId = id,
                        EventName = tracking_delete,
                        RefrenceId = PurchaseOrdersToUpdate.po_id,
                        UserId = PurchaseOrdersToUpdate.updated_by,
                    };
                    await _eventRepo.AddEventAsync(eventModel);

                    var userEvent = new EventModel(userEventTableName)
                    {
                        EntityId = PurchaseOrdersToUpdate.updated_by,
                        EventName = tracking_delete,
                        RefrenceId = id,
                        UserId = PurchaseOrdersToUpdate.updated_by,
                    };
                    await _eventRepo.AddEventAsync(userEvent);
                }
            }
            catch (Exception ex)
            {

            }
            return res;
        }
    }
}
