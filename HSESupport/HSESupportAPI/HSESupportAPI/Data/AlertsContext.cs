using HSESupportAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HSESupportAPI.Data
{
    public class AlertsContext : DbContext
    {
        public DbSet<Alert> Alerts { get; set; }
        public AlertsContext(DbContextOptions<AlertsContext> options) : base(options)
        {
        }
    }
}