using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using badgerApi.Models;
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
        // GET: api/Vendor
        [HttpGet]
        public async Task<ActionResult<List<Vendor>>> GetAsync()
        {
            return await _VendorRepo.GetAll(0);
        }

        // GET: api/Vendor/5
        [HttpGet("{id}")]
        public async Task<Vendor> GetAsync(int id)
        {
            return await _VendorRepo.GetByID(id);
        }

        // POST: api/Vendor
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Vendor/5
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
