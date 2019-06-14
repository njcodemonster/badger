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
        private readonly IConfiguration _config;
        private readonly IPurchaseOrdersDiscountsRepository _PurchaseOrdersDiscountsRepo;
        ILoggerFactory _loggerFactory;
        private INotesAndDocHelper _NotesAndDoc;
        private IItemServiceHelper _ItemsHelper;
        private int note_type = 4;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        public PurchaseOrdersDiscountsController(IPurchaseOrdersDiscountsRepository PurchaseOrdersDiscountsRepo, ILoggerFactory loggerFactory, INotesAndDocHelper NotesAndDoc, IConfiguration config, IItemServiceHelper ItemsHelper)
        {
            _config = config;
            _PurchaseOrdersDiscountsRepo = PurchaseOrdersDiscountsRepo;
            _loggerFactory = loggerFactory;
            _NotesAndDoc = NotesAndDoc;
            _ItemsHelper = ItemsHelper;
        }

        // GET: api/purchaseordersledger/list
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
                logger.LogInformation("Problem happened in selecting the data for get all with message" + ex.Message);
                return ToReturn;
            }

        }

        // GET: api/purchaseorders/list/1
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
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

            }
            return ToReturn;
        }

        // GET: api/purchaseorders/getdiscount/10
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
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);

            }

            return poPageList;

        }

        // GET: api/purchaseorders/count
        [HttpGet("count")]
        public async Task<string> CountAsync()
        {
            return await _PurchaseOrdersDiscountsRepo.Count();

        }

        // POST: api/purchaseorders/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                PurchaseOrderDiscounts newPurchaseOrder = JsonConvert.DeserializeObject<PurchaseOrderDiscounts>(value);
                NewInsertionID = await _PurchaseOrdersDiscountsRepo.Create(newPurchaseOrder);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new attribute with message" + ex.Message);
            }
            return NewInsertionID;
        }

        // PUT: api/purchaseorders/update/5
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
                PurchaseOrderDiscounts PurchaseOrdersToUpdate = JsonConvert.DeserializeObject<PurchaseOrderDiscounts>(value);
                PurchaseOrdersToUpdate.po_discount_id = id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();


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

                await _PurchaseOrdersDiscountsRepo.UpdateSpecific(ValuesToUpdate, "po_id=" + id);
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
