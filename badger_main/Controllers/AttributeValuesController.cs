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

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeValuesController : ControllerBase
    {
        private readonly IAttributeValuesRepository _AttributeValuesRepo;
        ILoggerFactory _loggerFactory;

        public AttributeValuesController(IAttributeValuesRepository AttributeValuesRepo, ILoggerFactory loggerFactory)
        {
            _AttributeValuesRepo = AttributeValuesRepo;
            _loggerFactory = loggerFactory;
        }

        /*
          Developer: Azeem Hassan
          Date:7-8-19
          Action: getting attributevalues list
          Request:GET
          URL: api/attributevalues/list
          Input: null
          output: list of attributevalues
       */
        [HttpGet("list")]
        public async Task<ActionResult<List<AttributeValues>>> GetAsync()
        {
            List<AttributeValues> ToReturn = new List<AttributeValues>();
            try
            {
                return await _AttributeValuesRepo.GetAll(0);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get all with message" + ex.Message);
                return ToReturn;
            }


        }

        /*
          Developer: Azeem Hassan
          Date:7-8-19
          Action: getting attributevalues by id
          Request:GET
          URL: api/attributevalues/list/1
          Input: int id
          output: attributevalues
       */
        [HttpGet("list/{id}")]
        public async Task<List<AttributeValues>> GetAsync(int id)
        {
            List<AttributeValues> ToReturn = new List<AttributeValues>();
            try
            {
                AttributeValues Res = await _AttributeValuesRepo.GetById(id);
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
         Action:get HTML Form (New Styles attributes values Data) from VIEW and pass the data to API Attributes values Repo 
         URL: /attributevalues/create
         Input: HTML form Body Json with the data of new Style attributes values and product_id
         output: New attribute values id
         */

        // POST: api/attributevalues/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                AttributeValues newAttributeValue = JsonConvert.DeserializeObject<AttributeValues>(value);
                NewInsertionID = await _AttributeValuesRepo.Create(newAttributeValue);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new attribute values with message" + ex.Message);
            }
            return NewInsertionID;
        }

        /*
          Developer: Azeem Hassan
          Date:7-8-19
          Action: Update attributevalues by id
          Request:PUT
          URL: api/attributevalues/update/5
          Input: FormBody data and int id
          output: Success/Failed
       */
        [HttpPut("update/{id}")]
        public async Task<string> Update(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
            bool UpdateProcessOutput = false;
            try
            {
                AttributeValues AttributeValuesToUpdate = JsonConvert.DeserializeObject<AttributeValues>(value);
                AttributeValuesToUpdate.value_id = id;
                UpdateProcessOutput = await _AttributeValuesRepo.Update(AttributeValuesToUpdate);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating  attribute values with message" + ex.Message);
                UpdateResult = "Failed";
            }
            if (!UpdateProcessOutput)
            {
                UpdateResult = "Creation failed due to reason: No specific reson";
            }
            return UpdateResult;
        }

        // PUT: api/attributevalues/updatespecific/1
        /*
           Developer: Azeem Hassan
           Date:7-8-19
           Action: Update specific attributevalues by id
           Request:PUT
           URL:  api/attributevalues/updatespecific/1
           Input: FormBody data and int id
           output: Success/Failed
        */
        [HttpPut("updatespecific/{id}")]
        public async Task<string> UpdateSpecific(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";

            try
            {
                AttributeValues AttributeValuesToUpdate = JsonConvert.DeserializeObject<AttributeValues>(value);
                AttributeValuesToUpdate.value_id = id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();

                if (AttributeValuesToUpdate.attribute_id != 0)
                {
                    ValuesToUpdate.Add("attribute_id", AttributeValuesToUpdate.attribute_id.ToString());
                }
                if (AttributeValuesToUpdate.value != null)
                {
                    ValuesToUpdate.Add("value", AttributeValuesToUpdate.value);
                }
                if (AttributeValuesToUpdate.created_by != 0)
                {
                    ValuesToUpdate.Add("created_by", AttributeValuesToUpdate.created_by.ToString());
                }
                if (AttributeValuesToUpdate.updated_by != 0)
                {
                    ValuesToUpdate.Add("updated_by", AttributeValuesToUpdate.updated_by.ToString());
                }
                if (AttributeValuesToUpdate.created_at != 0)
                {
                    ValuesToUpdate.Add("created_at", AttributeValuesToUpdate.created_at.ToString());
                }
                if (AttributeValuesToUpdate.updated_at != 0)
                {
                    ValuesToUpdate.Add("updated_at", AttributeValuesToUpdate.updated_at.ToString());
                }


                await _AttributeValuesRepo.UpdateSpecific(ValuesToUpdate, "value_id=" + id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating new attribute values with message" + ex.Message);
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
