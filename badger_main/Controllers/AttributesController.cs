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
    public class AttributesController : ControllerBase
    {
        private readonly IAttributesRepository _AttributesRepo;
        ILoggerFactory _loggerFactory;

        public AttributesController(IAttributesRepository AttributesRepo, ILoggerFactory loggerFactory)
        {
            _AttributesRepo = AttributesRepo;
            _loggerFactory = loggerFactory;
        }

        // GET: api/attributes/list
        [HttpGet("list")]
        public async Task<ActionResult<List<Attributes>>> GetAsync()
        {
            List<Attributes> ToReturn = new List<Attributes>();
            try
            {
                return await _AttributesRepo.GetAll(0);
            }
            catch(Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get all with message"+ex.Message);
                return ToReturn;
            }
           
        }

        // GET: api/attributes/list/1
        [HttpGet("list/{id}")]
        public async Task<List<Attributes>> GetAsync(int id)
        {
            List<Attributes> ToReturn = new List<Attributes>();
            try
            {
                Attributes Res =  await _AttributesRepo.GetById(id);
                ToReturn.Add(Res);
               
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);
               
            }
            return ToReturn;
        }

        // POST: api/attributes/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                Attributes newAttributes = JsonConvert.DeserializeObject<Attributes>(value);
                NewInsertionID = await _AttributesRepo.Create(newAttributes);
            }
            catch(Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new attribute with message" + ex.Message);
            }
            return NewInsertionID;
        }

        // PUT: api/attributes/update/5
        [HttpPut("update/{id}")]
        public async Task<string> Update(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
            bool UpdateProcessOutput = false;
            try
            {
                Attributes AttributesToUpdate = JsonConvert.DeserializeObject<Attributes>(value);
                AttributesToUpdate.attribute_id = id;
                UpdateProcessOutput = await _AttributesRepo.Update(AttributesToUpdate);
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

        // PUT: api/attributes/updatespecific/1
        [HttpPut("updatespecific/{id}")]
        public async Task<string> UpdateSpecific(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
           
            try
            {
                Attributes AttributesToUpdate = JsonConvert.DeserializeObject<Attributes>(value);
                AttributesToUpdate.attribute_id = id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();

                if (AttributesToUpdate.attribute != null)
                {
                    ValuesToUpdate.Add("attribute", AttributesToUpdate.attribute);
                }
                if (AttributesToUpdate.attribute_type_id != 0)
                {
                    ValuesToUpdate.Add("attribute_type_id", AttributesToUpdate.attribute_type_id.ToString());
                }
                if (AttributesToUpdate.attribute_display_name != null)
                {
                    ValuesToUpdate.Add("attribute_display_name", AttributesToUpdate.attribute_display_name);
                }
                if (AttributesToUpdate.data_type != null)
                {
                    ValuesToUpdate.Add("data_type", AttributesToUpdate.data_type);
                }
                if (AttributesToUpdate.created_at != 0)
                {
                    ValuesToUpdate.Add("created_at", AttributesToUpdate.created_at.ToString());
                }

                await _AttributesRepo.UpdateSpecific(ValuesToUpdate, "attribute_id=" + id);
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
