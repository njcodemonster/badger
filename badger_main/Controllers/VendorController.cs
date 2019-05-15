using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using badgerApi.Models;
using Newtonsoft.Json;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IVendorRepository _VendorRepo;

        public VendorController(IVendorRepository VendorRepo)
        {
            _VendorRepo = VendorRepo;
        }
       
        [HttpGet("list")]
        public async Task<ActionResult<List<Vendor>>> GetAsync()
        {
            return await _VendorRepo.GetAll(0);
          
        }

       
        [HttpGet("list/{id}")]
        public async Task<Vendor> GetAsync(int id)
        {
            return await _VendorRepo.GetById(id);
        }
      
        [HttpPost("create")]
        public async Task<string> PostAsync([FromBody]   string value)
        {
            string NewInsertionID = "0";
            try
            {
                Vendor newVendor = JsonConvert.DeserializeObject<Vendor>(value);
                NewInsertionID = await _VendorRepo.Create(newVendor);
            }
            catch(Exception ex)
            {
                NewInsertionID = "Creation failed due to reason:" +ex.Message;
            }
            return NewInsertionID;
        }

        // PUT: api/Vendor/5
        [HttpPut("update/{id}")]
        public async Task<string> Update(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
            bool UpdateProcessOutput = false;
            try
            {
                Vendor VendorToUpdate = JsonConvert.DeserializeObject<Vendor>(value);
                VendorToUpdate.vendor_id = id;
                UpdateProcessOutput = await _VendorRepo.Update(VendorToUpdate);
            }
            catch (Exception ex)
            {
                UpdateResult = "Creation failed due to reason:" + ex.Message;
            }
            if (!UpdateProcessOutput)
            {
                UpdateResult = "Creation failed due to reason: No specific reson";
            }
            return UpdateResult;
        }
        [HttpPut("specificUpdate/{id}")]
        public async Task<string> UpdateSpecific(int id, [FromBody] string value)
        {

            string UpdateResult = "Success";
           
            try
            {
                Vendor VendorToUpdate = JsonConvert.DeserializeObject<Vendor>(value);
                VendorToUpdate.vendor_id = id;
                Dictionary<String, String> ValuesToUpdate = new Dictionary<string, string>();
                if(VendorToUpdate.vendor_type != 0)
                {
                    ValuesToUpdate.Add("vendor_type", VendorToUpdate.vendor_type.ToString());
                }
                if(VendorToUpdate.vendor_name != null)
                {
                    ValuesToUpdate.Add("vendor_name", VendorToUpdate.vendor_name);
                }
                if (VendorToUpdate.vendor_description != null)
                {
                    ValuesToUpdate.Add("vendor_description", VendorToUpdate.vendor_description);
                }
                if (VendorToUpdate.corp_name != null)
                {
                    ValuesToUpdate.Add("corp_name", VendorToUpdate.corp_name);
                }
                if (VendorToUpdate.statement_name != null)
                {
                    ValuesToUpdate.Add("statement_name", VendorToUpdate.statement_name);
                }
                if (VendorToUpdate.our_customer_number != null)
                {
                    ValuesToUpdate.Add("our_customer_number", VendorToUpdate.our_customer_number);
                }
                if (VendorToUpdate.created_by != 0)
                {
                    ValuesToUpdate.Add("created_by", VendorToUpdate.created_by.ToString());
                }
                if (VendorToUpdate.updated_by != 0)
                {
                    ValuesToUpdate.Add("updated_by", VendorToUpdate.updated_by.ToString());
                }
                if (VendorToUpdate.active_status != 0)
                {
                    ValuesToUpdate.Add("active_status", VendorToUpdate.active_status.ToString());
                }
                if (VendorToUpdate.vendor_since != 0)
                {
                    ValuesToUpdate.Add("vendor_since", VendorToUpdate.vendor_since.ToString());
                }
                if (VendorToUpdate.created_at != 0)
                {
                    ValuesToUpdate.Add("created_at", VendorToUpdate.created_at.ToString());
                }
                if (VendorToUpdate.updated_at != 0)
                {
                    ValuesToUpdate.Add("updated_at", VendorToUpdate.updated_at.ToString());
                }


                await _VendorRepo.UpdateSpeific(ValuesToUpdate, "vendor_id="+id);
            }
            catch (Exception ex)
            {
                UpdateResult = "Creation failed due to reason:" + ex.Message;
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
