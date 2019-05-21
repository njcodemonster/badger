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
    public class AttributeTypeController : ControllerBase
    {
        private readonly IAttributeTypeRepository _AttributeTypeRepo;
        ILoggerFactory _loggerFactory;

        public AttributeTypeController(IAttributeTypeRepository AttributeTypeRepo, ILoggerFactory loggerFactory)
        {
            _AttributeTypeRepo = AttributeTypeRepo;
            _loggerFactory = loggerFactory;
        }

        // GET: api/attributetype/list
        [HttpGet("list")]
        public async Task<ActionResult<List<AttributeType>>> GetAsync()
        {
            List<AttributeType> ToReturn = new List<AttributeType>();
            try
            {
                return await _AttributeTypeRepo.GetAll(0);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get all with message" + ex.Message);
                return ToReturn;
            }

        }

        // GET: api/attributetype/list/1
        [HttpGet("list/{id}")]
        public async Task<List<AttributeType>> GetAsync(int id)
        {
            List<AttributeType> ToReturn = new List<AttributeType>();
            try
            {
                AttributeType Res = await _AttributeTypeRepo.GetById(id);
                ToReturn.Add(Res);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

            }
            return ToReturn;
        }

        // POST: api/attributetype/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                AttributeType newAttributes = JsonConvert.DeserializeObject<AttributeType>(value);
                NewInsertionID = await _AttributeTypeRepo.Create(newAttributes);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new attribute type with message" + ex.Message);
            }
            return NewInsertionID;
        }

        // PUT: api/attributetype/update/5
        [HttpPut("update/{id}")]
        public async Task<string> Update(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
            bool UpdateProcessOutput = false;
            try
            {
                AttributeType AttributeTypeToUpdate = JsonConvert.DeserializeObject<AttributeType>(value);
                AttributeTypeToUpdate.attribute_type_id = id;
                UpdateProcessOutput = await _AttributeTypeRepo.Update(AttributeTypeToUpdate);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating  attribute type with message" + ex.Message);
                UpdateResult = "Failed";
            }
            if (!UpdateProcessOutput)
            {
                UpdateResult = "Creation failed due to reason: No specific reson";
            }
            return UpdateResult;
        }

        // PUT: api/attributetype/updatespecific/1
        [HttpPut("updatespecific/{id}")]
        public async Task<string> UpdateSpecific(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";

            try
            {
                AttributeType AttributeTypeToUpdate = JsonConvert.DeserializeObject<AttributeType>(value);
                AttributeTypeToUpdate.attribute_type_id = id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();

                if (AttributeTypeToUpdate.attribute_type_name != null)
                {
                    ValuesToUpdate.Add("attribute_type_name", AttributeTypeToUpdate.attribute_type_name);
                }
                if (AttributeTypeToUpdate.attribute_type_description != null)
                {
                    ValuesToUpdate.Add("attribute_type_description", AttributeTypeToUpdate.attribute_type_description);
                }
                if (AttributeTypeToUpdate.created_by != 0)
                {
                    ValuesToUpdate.Add("created_by", AttributeTypeToUpdate.created_by.ToString());
                }
                if (AttributeTypeToUpdate.updated_by != 0)
                {
                    ValuesToUpdate.Add("updated_by", AttributeTypeToUpdate.updated_by.ToString());
                }
                if (AttributeTypeToUpdate.created_at != 0)
                {
                    ValuesToUpdate.Add("created_at", AttributeTypeToUpdate.created_at.ToString());
                }
                if (AttributeTypeToUpdate.updated_at != 0)
                {
                    ValuesToUpdate.Add("updated_at", AttributeTypeToUpdate.updated_at.ToString());
                }

                await _AttributeTypeRepo.UpdateSpecific(ValuesToUpdate, "attribute_type_id=" + id);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating new attribute type with message" + ex.Message);
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
