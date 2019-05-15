using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using badgerApi.Interfaces;
using badgerApi.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace badgerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderStatusController : ControllerBase
    {
        private readonly IPurchaseOrderStatusRepository _PurchaseOrderStatusRepo;
        ILoggerFactory _loggerFactory;
        public PurchaseOrderStatusController(IPurchaseOrderStatusRepository PurchaseOrderStatusRepo, ILoggerFactory loggerFactory)
        {
            _PurchaseOrderStatusRepo = PurchaseOrderStatusRepo;
            _loggerFactory = loggerFactory;
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<PurchaseOrderStatus>>> GetAsync()
        {
            List<PurchaseOrderStatus> ToReturn = new List<PurchaseOrderStatus>();
            try
            {
                return await _PurchaseOrderStatusRepo.GetAll(0);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for get all with message" + ex.Message);
                return ToReturn;
            }

        }

        [HttpGet("list/{id}")]
        public async Task<List<PurchaseOrderStatus>> GetAsync(int id)
        {
            List<PurchaseOrderStatus> ToReturn = new List<PurchaseOrderStatus>();
            try
            {
                PurchaseOrderStatus Res = await _PurchaseOrderStatusRepo.GetById(id);
                ToReturn.Add(Res);

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

            }
            return ToReturn;
        }


        [HttpGet("list/name/{name}")]
        public async Task<List<PurchaseOrderStatus>> GetByName(string name)
        {
            List<PurchaseOrderStatus> ToReturn = new List<PurchaseOrderStatus>();
            try
            {
                ToReturn = await _PurchaseOrderStatusRepo.GetByName(name);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("internal_error_log");
                logger.LogInformation("Problem happened in selecting the data for GetAsync with message" + ex.Message);

            }

            return ToReturn;
        }

    }
}
