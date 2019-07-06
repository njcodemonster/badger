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
    public class SkuController : ControllerBase
    {
        private readonly ISkuRepo _SkuRepo;
        ILoggerFactory _loggerFactory;

        public SkuController(ISkuRepo SkuRepo, ILoggerFactory loggerFactory)
        {
            _SkuRepo = SkuRepo;
            _loggerFactory = loggerFactory;
        }

        // GET: api/sku/list
        [HttpGet("list")]
        public async Task<ActionResult<List<Sku>>> GetAsync()
        {
            List<Sku> ToReturn = new List<Sku>();
            try
            {
                return await _SkuRepo.GetAll(0);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get all with message" + ex.Message);
                return ToReturn;
            }

        }

        // GET: api/sku/list/1
        [HttpGet("list/{id}")]
        public async Task<List<Sku>> GetAsync(int id)
        {
            List<Sku> ToReturn = new List<Sku>();
            try
            {
                Sku Res = await _SkuRepo.GetById(id);
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
          Developer: ubaid
          Date:5-7-19
          Action:get HTML Form (New Styles SKUs Data) from VIEW and pass the data to API SKU Repo 
          URL: /sku/create
          Input: HTML form Body Json with the data of new Styles SKU and product_id
          output: New SKU id
          */
        // POST: api/sku/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                Sku newSku = JsonConvert.DeserializeObject<Sku>(value);
                NewInsertionID = await _SkuRepo.Create(newSku);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new attribute with message" + ex.Message);
            }
            return NewInsertionID;
        }

        // PUT: api/sku/update/5
        [HttpPut("update/{id}")]
        public async Task<string> Update(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
            bool UpdateProcessOutput = false;
            try
            {
                Sku SkuToUpdate = JsonConvert.DeserializeObject<Sku>(value);
                SkuToUpdate.sku_id = id;
                UpdateProcessOutput = await _SkuRepo.Update(SkuToUpdate);
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

        // PUT: api/sku/updatespecific/1
        [HttpPut("updatespecific/{id}")]
        public async Task<string> UpdateSpecific(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";

            try
            {
                Sku SkuToUpdate = JsonConvert.DeserializeObject<Sku>(value);
                SkuToUpdate.sku_id = id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();

                if (SkuToUpdate.sku != null)
                {
                    ValuesToUpdate.Add("sku", SkuToUpdate.sku);
                }
                if (SkuToUpdate.vendor_id != 0)
                {
                    ValuesToUpdate.Add("vendor_id", SkuToUpdate.vendor_id.ToString());
                }
                if (SkuToUpdate.product_id != 0)
                {
                    ValuesToUpdate.Add("product_id", SkuToUpdate.product_id.ToString());
                }
                if (SkuToUpdate.weight != 0)
                {
                    ValuesToUpdate.Add("weight", SkuToUpdate.weight.ToString());
                }
                if (SkuToUpdate.updated_by != 0)
                {
                    ValuesToUpdate.Add("updated_by", SkuToUpdate.updated_by.ToString());
                }
                if (SkuToUpdate.updated_at != 0)
                {
                    ValuesToUpdate.Add("updated_at", SkuToUpdate.updated_at.ToString());
                }

                await _SkuRepo.UpdateSpecific(ValuesToUpdate, "sku_id=" + id);
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
