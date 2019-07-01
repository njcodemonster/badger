using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using badgerApi.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersLineItemsController : ControllerBase
    {
        private readonly IPurchaseOrdersLineItemsRepo _PurchaseOrdersLineItemsRepo;
        ILoggerFactory _loggerFactory;

        public PurchaseOrdersLineItemsController(IPurchaseOrdersLineItemsRepo PurchaseOrdersLineItemsRepo, ILoggerFactory loggerFactory)
        {
            _PurchaseOrdersLineItemsRepo = PurchaseOrdersLineItemsRepo;
            _loggerFactory = loggerFactory;
        }

        // GET: api/purchaseorderlineitems/list
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

        // GET: api/purchaseorderlineitems/list/1
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

        // POST: api/purchaseorderlineitems/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                PurchaseOrderLineItems NewPoLineItem = JsonConvert.DeserializeObject<PurchaseOrderLineItems>(value);
                NewInsertionID = await _PurchaseOrdersLineItemsRepo.Create(NewPoLineItem);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new attribute with message" + ex.Message);
            }
            return NewInsertionID;
        }

        // PUT: api/purchaseorderlineitems/update/5
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

        // PUT: api/purchaseorderlineitems/updatespecific/1
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
                if (PoLineItemToUpdate.line_item_ordered_quantity != 0)
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
