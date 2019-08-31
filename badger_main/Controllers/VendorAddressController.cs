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
    public class VendorAddressController : ControllerBase
    {
        private readonly IVendorAddress _VendorAddressRepo ;
        private IEventRepo _eventRepo;
        private string userEventTableName = "user_events";
        private string vendorEventTableName = "vendor_events";
        string vendor_address_updated = "vendor_address_updated";
        string vendor_address_created = "vendor_address_created";

        private CommonHelper.CommonHelper _common = new CommonHelper.CommonHelper();
        ILoggerFactory _loggerFactory;

        public VendorAddressController(IVendorAddress VendorAddressRepo, ILoggerFactory loggerFactory, IEventRepo eventRepo)
        {
            _eventRepo = eventRepo;
            _VendorAddressRepo = VendorAddressRepo;
            _loggerFactory = loggerFactory;
        }

        /*
           Developer: Azeem Hassan
           Date: 7-5-19 
           Action: create vendor address with vendor and user events 
           URL: api/VendorAddress/create
           Request:POST
           Input: vendor address data
           output: newAddressId
        */
        // POST: api/VendorAddress/create
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                VendorAddress newVendorAddress = JsonConvert.DeserializeObject<VendorAddress>(value);
                int created_by = newVendorAddress.created_by;
                NewInsertionID = await _VendorAddressRepo.Create(newVendorAddress);
                int vendor_id = newVendorAddress.vendor_id;
                var eventModel = new EventModel(vendorEventTableName)
                {
                    EventName = vendor_address_created,
                    EntityId = vendor_id ,
                    RefrenceId = Int32.Parse(NewInsertionID),
                    UserId = created_by,
                    EventNoteId = Int32.Parse(NewInsertionID)
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EventName = vendor_address_created,
                    EntityId = created_by,
                    RefrenceId = Convert.ToInt32(NewInsertionID),
                    UserId = created_by,
                    EventNoteId = Int32.Parse(NewInsertionID)
                };
                await _eventRepo.AddEventAsync(userEvent);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in making new vendor address with message" + ex.Message);
            }
            return NewInsertionID;
        }

        /*
           Developer: Azeem Hassan
           Date: 7-5-19 
           Action: update vendor address with vendor and user events 
           URL: api/VendorAddress/update/5
           Request:PUT
           Input: vendor address data and vendor id
           output: updateResult
        */
        // PUT: api/VendorAddress/update/5
        [HttpPut("update/{id}")]
        public async Task<string> Update(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
            bool UpdateProcessOutput = false;
            try
            {
                VendorAddress VendorToUpdate = JsonConvert.DeserializeObject<VendorAddress>(value);
                VendorToUpdate.vendor_address_id = id;
                int updated_by = VendorToUpdate.updated_by;
                UpdateProcessOutput = await _VendorAddressRepo.Update(VendorToUpdate);
                var eventModel = new EventModel(vendorEventTableName)
                {
                    EventName = vendor_address_updated,
                    EntityId = VendorToUpdate.vendor_id,
                    RefrenceId = id,
                    UserId = updated_by,
                    EventNoteId = id
                };
                await _eventRepo.AddEventAsync(eventModel);

                var userEvent = new EventModel(userEventTableName)
                {
                    EventName = vendor_address_updated,
                    EntityId = updated_by,
                    RefrenceId = id,
                    UserId = updated_by,
                    EventNoteId = id
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
