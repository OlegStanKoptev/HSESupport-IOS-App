using HSESupportAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HSESupportAPI.Data
{
    public class MessagesContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public MessagesContext(DbContextOptions<MessagesContext> options) : base(options)
        {
        }
    }
}