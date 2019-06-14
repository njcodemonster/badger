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
    public class PurchaseOrdersLedgerController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IPurchaseOrdersLedgerRepository _PurchaseOrdersLedgerRepo;
        ILoggerFactory _loggerFactory;
        private INotesAndDocHelper _NotesAndDoc;
        private IItemServiceHelper _ItemsHelper;
        private int note_type = 4;
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        public PurchaseOrdersLedgerController(IPurchaseOrdersLedgerRepository PurchaseOrdersLedgerRepo, ILoggerFactory loggerFactory, INotesAndDocHelper NotesAndDoc, IConfiguration config, IItemServiceHelper ItemsHelper)
        {
            _config = config;
            _PurchaseOrdersLedgerRepo = PurchaseOrdersLedgerRepo;
            _loggerFactory = loggerFactory;
            _NotesAndDoc = NotesAndDoc;
            _ItemsHelper = ItemsHelper;
        }

        // GET: api/purchaseordersledger/list
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
                logger.LogInformation("Problem happened in selecting the data for get all with message" + ex.Message);
                return ToReturn;
            }

        }

        // GET: api/purchaseorders/list/1
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
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

            }
            return ToReturn;
        }

        // GET: api/purchaseorders/getledger/10
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
                logger.LogInformation("Problem happened in selecting the data for listpageviewAsync with message" + ex.Message);

            }

            return poPageList;

        }

        // GET: api/purchaseorders/count
        [HttpGet("count")]
        public async Task<string> CountAsync()
        {
            return await _PurchaseOrdersLedgerRepo.Count();

        }

        // POST: api/purchaseorders/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                PurchaseOrderLedger newPurchaseOrder = JsonConvert.DeserializeObject<PurchaseOrderLedger>(value);
                NewInsertionID = await _PurchaseOrdersLedgerRepo.Create(newPurchaseOrder);
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
                PurchaseOrderLedger PurchaseOrdersToUpdate = JsonConvert.DeserializeObject<PurchaseOrderLedger>(value);
                PurchaseOrdersToUpdate.transaction_id = id;
                UpdateProcessOutput = await _PurchaseOrdersLedgerRepo.Update(PurchaseOrdersToUpdate);
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
                PurchaseOrderLedger PurchaseOrdersToUpdate = JsonConvert.DeserializeObject<PurchaseOrderLedger>(value);
                PurchaseOrdersToUpdate.transaction_id = id;
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

                await _PurchaseOrdersLedgerRepo.UpdateSpecific(ValuesToUpdate, "po_id=" + id);
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
