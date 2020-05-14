using System;
using HSESupportAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HSESupportAPI.Data
{
    public class PicturesContext : DbContext
    {
        public DbSet<Picture> Pictures { get; set; }
        public PicturesContext(DbContextOptions<PicturesContext> options)
            : base(options)
        {
        }
    }
}
