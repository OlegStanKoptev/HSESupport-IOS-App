using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HSESupportAPI.Data;
using HSESupportAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.EntityFrameworkCore;

namespace HSESupportAPI.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessagesController : ControllerBase
    {
        MessagesContext db;
        public MessagesController(MessagesContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> Get()
        {
            return await db.Messages.ToListAsync();
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Message>>> Get(string userId)
        {
            IEnumerable<Message> messages = await db.Messages.Where(x => x.UserId == userId).ToListAsync();
            if (messages == null)
                return NotFound();
            return new ObjectResult(messages);
        }

        [HttpPost]
        public async Task<ActionResult<Message>> Post(Message message)
        {            
            if (message == null)
            {
                return BadRequest();
            }
            db.Messages.Add(message);
            if (message.Sender == "Admin" || message.Sender == "Manager")
            {
                if (message.Type == "Text")
                {
                    string alert = "{\"aps\":{\"alert\":\"" + "Support team: " + message.Text + "\",\"sound\":\"default\"}}";
                    string[] tags = new string[] { "username:" + message.UserId };
                    await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, tags);
                }
                else
                {
                    string alert = "{\"aps\":{\"alert\":\"" + "Support team sent you a picture\",\"sound\":\"default\"}}";
                    string[] tags = new string[] { "username:" + message.UserId };
                    await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, tags);
                }
            }
            else
            {
                //Background update for an admin device "content-available" : 1
                //string alert = "{\"aps\":{\"content-available\":1},\"type\":\"message\"}";
                string alert = "{\"aps\":{\"alert\":\"" + "Student: " + message.Text + "\",\"sound\":\"default\"}}";
                string[] tags = new string[] { "usertype:admin" };
                await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, tags);
            }
            await db.SaveChangesAsync();
            return Ok(message);
        }

        [HttpPost("globalalert")]
        public async Task<ActionResult> GlobalAlertPost(string text)
        {
            string alert = "{\"aps\":{\"alert\":\"" + text + "\",\"sound\":\"default\"}}";
            await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult<Message>> Put(Message message)
        {
            if (message == null)
            {
                return BadRequest();
            }
            if (!db.Messages.Any(x => x.Id == message.Id))
            {
                return NotFound();
            }
            db.Update(message);
            await db.SaveChangesAsync();
            return Ok(message);
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult<Message>> DeleteMessage(int id)
        {
            Message message = db.Messages.FirstOrDefault(x => x.Id == id);
            if (message == null)
            {
                return NotFound();
            }
            db.Messages.Remove(message);
            await db.SaveChangesAsync();
            return Ok(message);
        }
        
        [HttpDelete("ticket/{ticketId}")]
        public async Task<ActionResult> DeleteMessagesWithTicket(int ticketId)
        {
            List<Message> messages = await db.Messages.Where(x => x.TicketId == ticketId).ToListAsync();
            Console.WriteLine();
            foreach (var message in messages)
            {
                db.Remove(message);
            }
            await db.SaveChangesAsync();
            return Ok();
        }
        
    }
}