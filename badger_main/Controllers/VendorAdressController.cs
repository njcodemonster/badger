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
    public class VendorAdressController : ControllerBase
    {
        private readonly IVendorAdress _VendorAdressRepo ;
        ILoggerFactory _loggerFactory;

        public VendorAdressController(IVendorAdress VendorAdressRepo, ILoggerFactory loggerFactory)
        {
            _VendorAdressRepo = VendorAdressRepo;
            _loggerFactory = loggerFactory;
        }


        // GET: api/VendorAdress/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/VendorAdress/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                VendorAddress newVendorAddress = JsonConvert.DeserializeObject<VendorAddress>(value);
                NewInsertionID = await _VendorAdressRepo.Create(newVendorAddress);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new vendor with message" + ex.Message);
            }
            return NewInsertionID;
        }

        // PUT: api/VendorAdress/5
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
