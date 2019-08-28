using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using itemService.Interfaces;
using GenericModals.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace itemService.Controllers
{
    [Route("api/item/status")]
    [ApiController]
    public class ItemStatusController : ControllerBase
    {

        private readonly IItemStatusRepository _ItemStatusRepository;

        public ItemStatusController(IItemStatusRepository _ItemStatusReposit)
        {
            _ItemStatusRepository = _ItemStatusReposit;
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get item status list by id "api/ItemStatus/list/id/2"
        URL: api/ItemStatus/list/id/2
        Request: Get
        Input: int item_status_id
        output: List of item status
        */
        [HttpGet("list/id/{item_status_id}")]
        public async Task<List<ItemStatus>> id(int item_status_id)
        {   
            List<ItemStatus> ToRetrunItemStatus = new List<ItemStatus>();
            try
            {
                ToRetrunItemStatus = await _ItemStatusRepository.GetStatusByID(item_status_id);
                return ToRetrunItemStatus;
            }
            catch (Exception IdException)
            {
                return ToRetrunItemStatus;
            }
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get all item status list "api/ItemStatus/list"
        URL: api/ItemStatus/list
        Request: Get
        Input: null
        output: List of item status
        */
        [HttpGet("list")]
        public async Task<List<ItemStatus>> GetAll()
        {
            List<ItemStatus> ToRetrunItemStatus = new List<ItemStatus>();
            try
            {
                ToRetrunItemStatus = await _ItemStatusRepository.GetAllStatus();
                return ToRetrunItemStatus;
            }
            catch (Exception IdException)
            {
                return ToRetrunItemStatus;
            }
        }


        /*
        // GET: api/Items
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        
        // GET: api/Items/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Items
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Items/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/
    }
}
