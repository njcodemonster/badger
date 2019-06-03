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
    public class VendorAddressController : ControllerBase
    {
        private readonly IVendorAddress _VendorAddressRepo ;
        ILoggerFactory _loggerFactory;

        public VendorAddressController(IVendorAddress VendorAddressRepo, ILoggerFactory loggerFactory)
        {
            _VendorAddressRepo = VendorAddressRepo;
            _loggerFactory = loggerFactory;
        }


        // GET: api/VendorAddress/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/VendorAddress/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                VendorAddress newVendorAddress = JsonConvert.DeserializeObject<VendorAddress>(value);
                NewInsertionID = await _VendorAddressRepo.Create(newVendorAddress);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new vendor address with message" + ex.Message);
            }
            return NewInsertionID;
        }

        // PUT: api/VendorAddress/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
