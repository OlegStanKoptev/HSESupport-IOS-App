using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using HSESupportAPI.Data;
using HSESupportAPI.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Internal;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System;
using System.Text;

namespace HSESupportAPI.Controllers
{
    [ApiController]
    [Route("api/pictures")]
    public class PicturesController : ControllerBase
    {
        PicturesContext db;

        public PicturesController(PicturesContext context)
        {
            db = context;
        }

        [HttpPost("user/")]
        public async Task<ActionResult<Picture>> SetUserProfile(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                if (!Directory.Exists("wwwroot/files/pictures/user_pictures/"))
                {
                    Directory.CreateDirectory("wwwroot/files/pictures/user_pictures/");
                }

                string path = "wwwroot/files/pictures/user_pictures/" + uploadedFile.FileName;
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                string userId = uploadedFile.FileName.Split('.')[0];
                Picture file = new Picture
                {
                    Name = uploadedFile.FileName,
                    Path = path,
                    UserId = userId,
                    MessageId = 0,
                    Type = "avatar"
                };
                Picture pictureInDb = await db.Pictures.FirstOrDefaultAsync(x => x.UserId == userId);
                if (pictureInDb != null)
                {
                    db.Pictures.Remove(pictureInDb);
                    db.SaveChanges();
                }
                db.Pictures.Add(file);
                db.SaveChanges();
                return Ok(file);
            }
            return BadRequest();
        }

        [HttpPost("message/{userId}/{messageId}")]
        public async Task<ActionResult<Picture>> SendPictureAsMessage(string userId, string messageId, IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {

                if (!Directory.Exists("wwwroot/files/pictures/messages/"))
                {
                    Directory.CreateDirectory("wwwroot/files/pictures/messages/");
                }
                string path = "wwwroot/files/pictures/messages/" + uploadedFile.FileName;
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                try
                {
                    int intmessageId = Convert.ToInt32(messageId);
                    Picture file = new Picture
                    {
                        Name = uploadedFile.FileName,
                        Path = path,
                        UserId = userId,
                        MessageId = Convert.ToInt32(messageId),
                        Type = "attachment"
                    };
                    db.Pictures.Add(file);
                    db.SaveChanges();
                    return Ok(file);
                } catch (Exception)
                {
                    return NoContent();
                }
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Picture>>> GetFiles()
        {
            return await db.Pictures.ToListAsync();
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetSpecificPicture(string userId)
        {
            Picture picture = await db.Pictures.FirstOrDefaultAsync(x => x.UserId == userId && x.Type == "avatar");
            if (picture == null || !System.IO.File.Exists(picture.Path))
            {
                return NotFound();
            }
            var bytesArray = System.IO.File.ReadAllBytes(picture.Path);
            var content = new MemoryStream(bytesArray);
            var contentType = "application/octet-stream";
            var fileName = userId + ".jpg";
            return File(content, contentType, fileName);
        }

        [HttpGet("attachment/{name}")]
        public async Task<IActionResult> GetAttachmentPicture(string name)
        {
            Picture picture = await db.Pictures.FirstOrDefaultAsync(x => x.Name.Contains(name) && x.Type == "attachment");
            if (picture == null || !System.IO.File.Exists(picture.Path))
            {
                return NotFound();
            }
            var bytesArray = System.IO.File.ReadAllBytes(picture.Path);
            var content = new MemoryStream(bytesArray);
            var contentType = "application/octet-stream";
            var fileName = name + ".jpg";
            return File(content, contentType, fileName);
        }

        [HttpDelete("{name}")]
        public async Task<ActionResult<Picture>> DeleteSpecificPicture(string name)
        {
            Picture picture = await db.Pictures.FirstOrDefaultAsync(x => x.Name.Contains(name));
            if (picture == null)
            {
                return NotFound();
            }
            if (System.IO.File.Exists(picture.Path))
            {
                System.IO.File.Delete(picture.Path);
            }
            db.Pictures.Remove(picture);
            await db.SaveChangesAsync();
            return Ok(picture);
        }

        [HttpDelete]
        public ActionResult DeleteAllPictures()
        {
            string[] files;
            if ((files = Directory.GetFiles("wwwroot/files/pictures/user_pictures/")).Length != 0)
            {
                foreach (var file in files)
                {
                    System.IO.File.Delete(file);
                }
            }
            return Ok();
        }
    }
}
