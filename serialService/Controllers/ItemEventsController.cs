using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using itemService.Models;

namespace itemService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemEventsController : ControllerBase
    {
        private readonly itemsdbContext _context;

        public ItemEventsController(itemsdbContext context)
        {
            _context = context;
        }

        // GET: api/ItemEvents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemEvents>>> GetItemEvents()
        {
            return await _context.ItemEvents.ToListAsync();
        }

        // GET: api/ItemEvents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemEvents>> GetItemEvents(int id)
        {
            var itemEvents = await _context.ItemEvents.FindAsync(id);

            if (itemEvents == null)
            {
                return NotFound();
            }

            return itemEvents;
        }

        // PUT: api/ItemEvents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItemEvents(int id, ItemEvents itemEvents)
        {
            if (id != itemEvents.ItemEventId)
            {
                return BadRequest();
            }

            _context.Entry(itemEvents).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemEventsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ItemEvents
        [HttpPost]
        public async Task<ActionResult<ItemEvents>> PostItemEvents(ItemEvents itemEvents)
        {
            _context.ItemEvents.Add(itemEvents);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ItemEventsExists(itemEvents.ItemEventId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetItemEvents", new { id = itemEvents.ItemEventId }, itemEvents);
        }

        // DELETE: api/ItemEvents/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ItemEvents>> DeleteItemEvents(int id)
        {
            var itemEvents = await _context.ItemEvents.FindAsync(id);
            if (itemEvents == null)
            {
                return NotFound();
            }

            _context.ItemEvents.Remove(itemEvents);
            await _context.SaveChangesAsync();

            return itemEvents;
        }

        private bool ItemEventsExists(int id)
        {
            return _context.ItemEvents.Any(e => e.ItemEventId == id);
        }
    }
}
