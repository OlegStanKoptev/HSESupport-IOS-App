using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HSESupportAPI.Data;
using HSESupportAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HSESupportAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        UsersContext db;
        public UsersController(UsersContext context)
        {
            db = context;
            if (db.Users.Any())
            {
                db.Users.ForEachAsync(x =>
                {
                    if (System.IO.File.Exists("wwwroot/files/pictures/user_pictures/" + x.UserId + ".jpg"))
                    {
                        x.HasPicture = 1;
                    }
                    else
                    {
                        x.HasPicture = 0;
                    }
                    db.Users.Update(x);
                });
                db.SaveChanges();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            return await db.Users.ToListAsync();
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> Get(string userId)
        {
            User user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            Console.WriteLine();
            if (user == null)
                return NotFound();
            return new ObjectResult(user);
        }

        [HttpGet("search/{nameOrEmail}")]
        public async Task<ActionResult<List<User>>> GetWithNameOrEmail (string nameOrEmail)
        {
            List<User> users = await db.Users.Where(x =>
                x.Name.ToLower().Contains(nameOrEmail.ToLower()) ||
                x.Email.ToLower().Contains(nameOrEmail.ToLower())).ToListAsync();
            Console.WriteLine();
            if (users == null)
                return NotFound();
            return new ObjectResult(users);
        }

        [HttpPost]
        public async Task<ActionResult<User>> Post(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPut]
        public async Task<ActionResult<User>> Put(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            User selectedUser = db.Users.Where(x => x.Id == user.Id).FirstOrDefault();
            if (selectedUser != null)
            {
                selectedUser.Name = user.Name;
                selectedUser.Status = user.Status;
                selectedUser.UserId = user.UserId;
                selectedUser.Email = user.Email;
                selectedUser.HasPicture = user.HasPicture;
                db.Users.Update(selectedUser);
                await db.SaveChangesAsync();
                return Ok(user);
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> Delete(string id)
        {
            User user = db.Users.FirstOrDefault(x => x.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return Ok(user);
        }
    }
}