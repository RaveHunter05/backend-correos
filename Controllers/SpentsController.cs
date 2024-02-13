using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace correos_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpentsController : ControllerBase
    {
        private readonly CorreosContext _context;

        public SpentsController(CorreosContext context)
        {
            _context = context;
        }

        // GET: api/Spents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Spent>>> GetSpents()
        {
          if (_context.Spents == null)
          {
              return NotFound();
          }
            return await _context.Spents.ToListAsync();
        }

        // GET: api/Spents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Spent>> GetSpent(int id)
        {
          if (_context.Spents == null)
          {
              return NotFound();
          }
            var spent = await _context.Spents.FindAsync(id);

            if (spent == null)
            {
                return NotFound();
            }

            return spent;
        }

        // PUT: api/Spents/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpent(int id, Spent spent)
        {
            if (id != spent.SpentId)
            {
                return BadRequest();
            }

            _context.Entry(spent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpentExists(id))
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

        // POST: api/Spents
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Spent>> PostSpent(Spent spent)
        {
          if (_context.Spents == null)
          {
              return Problem("Entity set 'CorreosContext.Spents'  is null.");
          }
            _context.Spents.Add(spent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpent", new { id = spent.SpentId }, spent);
        }

        // DELETE: api/Spents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpent(int id)
        {
            if (_context.Spents == null)
            {
                return NotFound();
            }
            var spent = await _context.Spents.FindAsync(id);
            if (spent == null)
            {
                return NotFound();
            }

            _context.Spents.Remove(spent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SpentExists(int id)
        {
            return (_context.Spents?.Any(e => e.SpentId == id)).GetValueOrDefault();
        }
    }
}
