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
    public class PurchaseOrderStatusController : ControllerBase
    {
        private readonly IPurchaseOrderStatusRepository _PurchaseOrderStatusRepo;

        public PurchaseOrderStatusController(IPurchaseOrderStatusRepository PurchaseOrderStatusRepo)
        {
            _PurchaseOrderStatusRepo = PurchaseOrderStatusRepo;
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<PurchaseOrderStatus>>> GetAsync()
        {
            return await _PurchaseOrderStatusRepo.GetAll(0);

        }

        [HttpGet("list/{id}")]
        public async Task<PurchaseOrderStatus> GetAsync(int id)
        {
            return await _PurchaseOrderStatusRepo.GetById(id);
        }


        [HttpGet("list/name/{name}")]
        public async Task<List<PurchaseOrderStatus>> GetByName(string name)
        {
            return await _PurchaseOrderStatusRepo.GetByName(name);
        }

    }
}
