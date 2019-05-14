using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using itemService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using itemService.Interfaces;

namespace itemService.Controllers
{
    [Route("item/[controller]")]
    [ApiController]
    public class EventTypeController : ControllerBase
    {
        private readonly IItemTypeRepository _EventTyperepo;

        public EventTypeController(IItemTypeRepository EventTyperepo)
        {
            _EventTyperepo = EventTyperepo;
        }
        // GET: api/EventType
        [HttpGet]
        public async Task<ActionResult<List<EventTypes>>> GetEventTypes()
        {
            return await _EventTyperepo.GetAllAsync();
        }

        // GET: api/EventType/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<EventTypes> GetAsync(int id)
        {
            return await _EventTyperepo.GetByID(1);
           
        }
        [HttpGet("ponka")]
        public async Task<ponkaquery> Getponka()
        {
            var z = await _EventTyperepo.GetPonka();
            return z.ToArray()[0];

        }
        // POST: api/EventType
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/EventType/5
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
