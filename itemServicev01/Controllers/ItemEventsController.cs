using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using itemService.Interfaces;
using itemService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace itemService.Controllers
{
    [Route("api/item/history")]
    [ApiController]
    public class ItemEventsController : ControllerBase
    {

        private readonly IItemEventsRepository _ItemEventsRepository;

        public ItemEventsController(IItemEventsRepository _ItemEventsReposit)
        {
            _ItemEventsRepository = _ItemEventsReposit;
        }
        
        // GET: /id/5
        [HttpGet("list/id/{item_id}")]
        public async Task<List<ItemEvents>> id(int item_id, string StartDate, string EndDate, int Limit)
        {   
            List<ItemEvents> ToRetrunItemEvents = new List<ItemEvents>();
            try
            {
                ToRetrunItemEvents = await _ItemEventsRepository.GetItemHistoryAll(item_id,  StartDate,  EndDate,  Limit);
                return ToRetrunItemEvents;
            }
            catch (Exception IdException)
            {
                return ToRetrunItemEvents;
            }
        }

        // GET: /id/
        [HttpGet("list/id/{item_id}/DateRange/{StartDate}/{EndDate}/{Limit}")]
        public async Task<List<ItemEvents>> GetAll(int item_id, string StartDate, string EndDate, int Limit)
        {
            List<ItemEvents> ToRetrunItemEvents = new List<ItemEvents>();
            try
            {
                ToRetrunItemEvents = await _ItemEventsRepository.GetItemHistoryAll(item_id,StartDate, EndDate, Limit);
                return ToRetrunItemEvents;
            }
            catch (Exception IdException)
            {
                return ToRetrunItemEvents;
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
