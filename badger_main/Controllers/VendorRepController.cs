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
        private IEventRepo _eventRepo;
        private int event_vendor_repo_created_id = 25;
        private int event_vendor_repo_updated_id = 28;
        private string event_create_vendor_repo = "Vendor repo created by user =%%userid%% with vendor id= %%vendorid%%";
        private string event_update_vendor_repo = "Vendor repo updated by user =%%userid%% with vendor id= %%vendorid%%";
        private string userEventTableName = "user_events";
        private string vendorEventTableName = "vendor_events";
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        ILoggerFactory _loggerFactory;

        public VendorRepController(IVendorRepRepository VendorRepRepository, ILoggerFactory loggerFactory, IEventRepo eventRepo)
        {
            _eventRepo = eventRepo;
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
                int created_by = newVendorRep.created_by;
                NewInsertionID = await _VendorRepRepository.Create(newVendorRep);
              
                event_create_vendor_repo = event_create_vendor_repo.Replace("%%userid%%", created_by.ToString()).Replace("%%vendorid%%", NewInsertionID);

                _eventRepo.AddVendorEventAsync(newVendorRep.vendor_id, event_vendor_repo_created_id, Int32.Parse(NewInsertionID), created_by, event_create_vendor_repo, _common.GetTimeStemp(), vendorEventTableName);

                _eventRepo.AddEventAsync(event_vendor_repo_created_id,created_by, Int32.Parse(NewInsertionID), event_create_vendor_repo, _common.GetTimeStemp(), userEventTableName);

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
                int updated_by = VendorToUpdate.updated_by;
                UpdateProcessOutput = await _VendorRepRepository.Update(VendorToUpdate);

                event_update_vendor_repo = event_update_vendor_repo.Replace("%%userid%%", updated_by.ToString()).Replace("%%vendorid%%", id.ToString());

                _eventRepo.AddVendorEventAsync(id, event_vendor_repo_updated_id, id, updated_by, event_update_vendor_repo, _common.GetTimeStemp(), vendorEventTableName);

                _eventRepo.AddEventAsync(event_vendor_repo_updated_id, updated_by, id, event_update_vendor_repo, _common.GetTimeStemp(), userEventTableName);

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
