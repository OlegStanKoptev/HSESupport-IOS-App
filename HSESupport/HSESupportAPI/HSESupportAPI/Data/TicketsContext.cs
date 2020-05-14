using HSESupportAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HSESupportAPI.Data
{
    public class TicketsContext : DbContext
    {
        public DbSet<Ticket> Tickets { get; set; }
        public TicketsContext(DbContextOptions<TicketsContext> options) : base(options)
        {
        }
    }
}