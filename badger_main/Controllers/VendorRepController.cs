using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using badgerApi.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;


namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorRepController : ControllerBase
    {
       
        private readonly IVendorRepRepository _VendorRepRepository;
        ILoggerFactory _loggerFactory;

        public VendorRepController(IVendorRepRepository VendorRepRepository, ILoggerFactory loggerFactory)
        {
            _VendorRepRepository = VendorRepRepository;
            _loggerFactory = loggerFactory;
        }



        // GET: api/VendorRep/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/VendorRep/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                VendorContactPerson newVendorRep = JsonConvert.DeserializeObject<VendorContactPerson>(value);
                NewInsertionID = await _VendorRepRepository.Create(newVendorRep);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new vendor with message" + ex.Message);
            }
            return NewInsertionID;
        }

       
        // PUT: api/VendorRep/update/5
        [HttpPut("update/{id}")]
        public async Task<string> Update(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
            bool UpdateProcessOutput = false;
            try
            {
                VendorContactPerson VendorToUpdate = JsonConvert.DeserializeObject<VendorContactPerson>(value);
                VendorToUpdate.contact_id = id;
                UpdateProcessOutput = await _VendorRepRepository.Update(VendorToUpdate);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in updating  vendor with message" + ex.Message);
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
