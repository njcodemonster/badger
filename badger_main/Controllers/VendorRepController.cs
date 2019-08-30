using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using GenericModals.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using GenericModals.Event;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorRepController : ControllerBase
    {
       
        private readonly IVendorRepRepository _VendorRepRepository;
        private IEventRepo _eventRepo;
        private string userEventTableName = "user_events";
        private string vendorEventTableName = "vendor_events";
        string create_vendor_repo = "create_vendor_repo";
        string update_vendor_repo = "update_vendor_repo";
        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        ILoggerFactory _loggerFactory;

        public VendorRepController(IVendorRepRepository VendorRepRepository, ILoggerFactory loggerFactory, IEventRepo eventRepo)
        {
            _eventRepo = eventRepo;
            _VendorRepRepository = VendorRepRepository;
            _loggerFactory = loggerFactory;
        }
        
        /*
           Developer: Azeem Hassan
           Date: 7-5-19 
           Action: create vendor repo with user and vendor events
           URL:  api/VendorRep/create
           Request POST
           Input: vendor repo data
           output: rep id
        */
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
                var eventModel = new EventModel(vendorEventTableName)
                {
                    EventName = create_vendor_repo,
                    EntityId = Int32.Parse(NewInsertionID),
                    RefrenceId = newVendorRep.vendor_id,
                    UserId = created_by,
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EventName = create_vendor_repo,
                    EntityId = created_by,
                    RefrenceId = Convert.ToInt32(NewInsertionID),
                    UserId = created_by,
                };
                await _eventRepo.AddEventAsync(userEvent);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new vendor with message" + ex.Message);
            }
            return NewInsertionID;
        }

         /*
            Developer: Azeem Hassan
            Date: 7-5-19 
            Action: update vendor repo with user and vendor events
            URL:  api/VendorRep/create
            Request PUT
            Input: vendor repo data and id
            output: update result
         */
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
                var eventModel = new EventModel(vendorEventTableName)
                {
                    EventName = update_vendor_repo,
                    EntityId = id,
                    RefrenceId = id,
                    UserId = updated_by,
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EventName = update_vendor_repo,
                    EntityId = updated_by,
                    RefrenceId = id,
                    UserId = updated_by,
                };
                await _eventRepo.AddEventAsync(userEvent);
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
