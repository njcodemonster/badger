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
    public class CalculationValuesController : ControllerBase
    {
        private readonly ICalculationValuesRepository _CalculationValuesRepo;
        ILoggerFactory _loggerFactory;

        public CalculationValuesController(ICalculationValuesRepository CalculationValuesRepo, ILoggerFactory loggerFactory)
        {
            _CalculationValuesRepo = CalculationValuesRepo;
            _loggerFactory = loggerFactory;
        }

        /*
           Developer: Azeem Hassan
           Date:7-8-19
           Action: getting CalculationValues list
           Request:GET
           URL: api/CalculationValues/list
           Input: null
           output: list of CalculationValues
        */
        [HttpGet("list")]
        public async Task<ActionResult<List<CalculationValues>>> GetAsync()
        {
            List<CalculationValues> ToReturn = new List<CalculationValues>();
            try
            {
                return await _CalculationValuesRepo.GetAll(0);
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
           Action: getting CalculationValues by id
           Request:GET
           URL: api/CalculationValues/list/1
           Input: int id
           output: CalculationValues
        */
        [HttpGet("list/{id}")]
        public async Task<List<CalculationValues>> GetAsync(int id)
        {
            List<CalculationValues> ToReturn = new List<CalculationValues>();
            try
            {
                CalculationValues Res = await _CalculationValuesRepo.GetById(id);
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
           Developer: Azeem Hassan
           Date:7-8-19
           Action: Inserting new CalculationValues
           Request:POST
           URL: api/CalculationValues/create
           Input: FormBody data
           output: NewInsertionID
        */
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                CalculationValues newAttributes = JsonConvert.DeserializeObject<CalculationValues>(value);
                NewInsertionID = await _CalculationValuesRepo.Create(newAttributes);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new attribute type with message" + ex.Message);
            }
            return NewInsertionID;
        }

        /*
           Developer: Azeem Hassan
           Date:7-8-19
           Action: Update CalculationValues by id
           Request:PUT
           URL: api/CalculationValues/update/5
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
                CalculationValues CalculationValuesToUpdate = JsonConvert.DeserializeObject<CalculationValues>(value);
                CalculationValuesToUpdate.calculation_id = id;
                UpdateProcessOutput = await _CalculationValuesRepo.Update(CalculationValuesToUpdate);
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

       
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
