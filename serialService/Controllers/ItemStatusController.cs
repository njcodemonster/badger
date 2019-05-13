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
    [Route("items/[controller]")]
    [ApiController]
    public class ItemStatusController : ControllerBase
    {
        private readonly itemsdbContext _context;

        public ItemStatusController(itemsdbContext context)
        {
            _context = context;
        }

        // GET: api/ItemStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemStatus>>> GetItemStatus()
        {
            return await _context.ItemStatus.ToListAsync();
        }

        // GET: api/ItemStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemStatus>> GetItemStatus(int id)
        {
            var itemStatus = await _context.ItemStatus.FindAsync(id);

            if (itemStatus == null)
            {
                return NotFound();
            }

            return itemStatus;
        }

        // PUT: api/ItemStatus/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItemStatus(int id, ItemStatus itemStatus)
        {
            if (id != itemStatus.ItemStatusId)
            {
                return BadRequest();
            }

            _context.Entry(itemStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemStatusExists(id))
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

        // POST: api/ItemStatus
        [HttpPost]
        public async Task<ActionResult<ItemStatus>> PostItemStatus(ItemStatus itemStatus)
        {
            _context.ItemStatus.Add(itemStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItemStatus", new { id = itemStatus.ItemStatusId }, itemStatus);
        }

        // DELETE: api/ItemStatus/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ItemStatus>> DeleteItemStatus(int id)
        {
            var itemStatus = await _context.ItemStatus.FindAsync(id);
            if (itemStatus == null)
            {
                return NotFound();
            }

            _context.ItemStatus.Remove(itemStatus);
            await _context.SaveChangesAsync();

            return itemStatus;
        }

        private bool ItemStatusExists(int id)
        {
            return _context.ItemStatus.Any(e => e.ItemStatusId == id);
        }
    }
}
