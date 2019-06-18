using System;
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
    public class PurchaseOrdersDiscountsController : ControllerBase
    {
        private readonly IPurchaseOrdersDiscountsRepository _PurchaseOrdersDiscountsRepo;
        ILoggerFactory _loggerFactory;

        private IEventRepo _eventRepo;

        private int event_type_discount_id = 5;
        private int event_type_discount_update_id = 12;
        private int event_type_discount_specificupdate_id = 13;

        private string tableName = "purchase_order_events";

        private string event_create_purchase_orders_discount = "Purchase order discount created by user =%%userid%% with purchase order discount id= %%discountid%%";
        private string event_update_purchase_orders_discount = "Purchase order discount updated by user =%%userid%% with purchase order discount id= %%discountid%%";
        private string event_updatespecific_purchase_orders_discount = "Purchase order discount specific updated by user =%%userid%% with purchase order discount id= %%discountid%%";

        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        public PurchaseOrdersDiscountsController(IPurchaseOrdersDiscountsRepository PurchaseOrdersDiscountsRepo, ILoggerFactory loggerFactory, IEventRepo eventRepo)
        {
            _eventRepo = eventRepo;
            _PurchaseOrdersDiscountsRepo = PurchaseOrdersDiscountsRepo;
            _loggerFactory = loggerFactory;
        }

        // GET: api/purchaseordersdiscounts/list
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

        // GET: api/purchaseordersdiscounts/list/1
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

        // GET: api/purchaseordersdiscounts/getdiscount/10
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

        // GET: api/purchaseordersdiscounts/count
        [HttpGet("count")]
        public async Task<string> CountAsync()
        {
            return await _PurchaseOrdersDiscountsRepo.Count();

        }

        // POST: api/purchaseordersdiscounts/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                PurchaseOrderDiscounts newPurchaseOrder = JsonConvert.DeserializeObject<PurchaseOrderDiscounts>(value);
                NewInsertionID = await _PurchaseOrdersDiscountsRepo.Create(newPurchaseOrder);

                event_create_purchase_orders_discount = event_create_purchase_orders_discount.Replace("%%userid%%", newPurchaseOrder.created_by.ToString()).Replace("%%discountid%%", NewInsertionID);

                _eventRepo.AddPurchaseOrdersEventAsync(newPurchaseOrder.po_id, event_type_discount_id, Int32.Parse(NewInsertionID), event_create_purchase_orders_discount, newPurchaseOrder.created_by, _common.GetTimeStemp(), tableName);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new purchaseordersdiscounts create with message" + ex.Message);
            }
            return NewInsertionID;
        }

        // PUT: api/purchaseordersdiscounts/update/5
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

                event_update_purchase_orders_discount = event_update_purchase_orders_discount.Replace("%%userid%%", PurchaseOrdersToUpdate.updated_by.ToString()).Replace("%%discountid%%", id.ToString());

                _eventRepo.AddPurchaseOrdersEventAsync(PurchaseOrdersToUpdate.po_id, event_type_discount_update_id, id, event_update_purchase_orders_discount, PurchaseOrdersToUpdate.updated_by, _common.GetTimeStemp(), tableName);
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


        // PUT: api/purchaseordersdiscounts/updatespecific/1
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

                event_updatespecific_purchase_orders_discount = event_updatespecific_purchase_orders_discount.Replace("%%userid%%", PurchaseOrdersToUpdate.updated_by.ToString()).Replace("%%discountid%%", id.ToString());

                _eventRepo.AddPurchaseOrdersEventAsync(PurchaseOrdersToUpdate.po_id, event_type_discount_specificupdate_id, id, event_updatespecific_purchase_orders_discount, PurchaseOrdersToUpdate.updated_by, _common.GetTimeStemp(), tableName);
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
