using System;
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
    [Route("api/alerts")]
    public class AlertsController : ControllerBase
    {
        AlertsContext db;
        public AlertsController(AlertsContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alert>>> Get()
        {
            return await db.Alerts.ToListAsync();
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<IEnumerable<Alert>>> Get(string email)
        {
            IEnumerable<Alert> alerts = await db.Alerts.Where(x => x.UserEmail == email || x.UserEmail == "all").ToListAsync();
            if (alerts == null)
                return NotFound();
            return new ObjectResult(alerts);
        }

        [HttpPost]
        public async Task<ActionResult<Alert>> Post(Alert alert)
        {
            if (alert == null)
            {
                return BadRequest();
            }
            db.Alerts.Add(alert);
            
            await db.SaveChangesAsync();
            return Ok(alert);
        }

        [HttpPut]
        public async Task<ActionResult<Alert>> Put(Alert alert)
        {
            if (alert == null)
            {
                return BadRequest();
            }
            if (!db.Alerts.Any(x => x.Id == alert.Id))
            {
                return NotFound();
            }
            db.Update(alert);
            await db.SaveChangesAsync();
            return Ok(alert);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Alert>> DeleteMessage(int id)
        {
            Alert alert = db.Alerts.FirstOrDefault(x => x.Id == id);
            if (alert == null)
            {
                return NotFound();
            }

            db.Alerts.Remove(alert);
            await db.SaveChangesAsync();
            return Ok(alert);
        }
    }
}