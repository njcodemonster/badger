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
       
        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get event type "api/eventtype"
        URL: api/eventtype
        Request: Get
        Input: null
        output: list of events type
        */
        [HttpGet]
        public async Task<ActionResult<List<EventTypes>>> GetEventTypes()
        {
            return await _EventTyperepo.GetAllAsync();
        }

        /*
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get event type by id "api/EventType/5"
        URL: api/EventType/5
        Request: Get
        Input: int id
        output: list of events type
        */
        [HttpGet("{id}", Name = "Get")]
        public async Task<EventTypes> GetAsync(int id)
        {
            return await _EventTyperepo.GetByID(1);
           
        }

        /*
         * * (function name is wrong)
        Developer: Sajid Khan
        Date: 7-5-19 
        Action: Get event type by id "api/EventType/ponka"
        URL: api/EventType/ponka
        Request: Get
        Input: 
        output: list of events type
        */
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
