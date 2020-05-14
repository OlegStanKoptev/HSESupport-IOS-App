using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HSESupportAPI.Data;
using HSESupportAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HSESupportAPI.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketsController : ControllerBase
    {
        TicketsContext db;
        public TicketsController(TicketsContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> Get()
        {
            return await db.Tickets.ToListAsync();
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Ticket>>> Get(string userId)
        {
            IEnumerable<Ticket> tickets = await db.Tickets.Where(x => x.UserId == userId).ToListAsync();
            if (tickets == null)
                return NotFound();
            return new ObjectResult(tickets);
        }

        [HttpPost]
        public async Task<ActionResult<Ticket>> Post(Ticket ticket)
        {
            if (ticket == null)
            {
                return BadRequest();
            }
            db.Tickets.Add(ticket);
            await db.SaveChangesAsync();
            return Ok(ticket);
        }

        [HttpPut]
        public async Task<ActionResult<Ticket>> Put(Ticket ticket)
        {
            if (ticket == null)
            {
                return BadRequest();
            }
            if (!db.Tickets.Any(x => x.Id == ticket.Id))
            {
                return NotFound();
            }
            db.Update(ticket);
            await db.SaveChangesAsync();
            return Ok(ticket);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Ticket>> Delete(int id)
        {
            Ticket ticket = db.Tickets.FirstOrDefault(x => x.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }
            db.Tickets.Remove(ticket);
            await db.SaveChangesAsync();
            return Ok(ticket);
        }
    }
}