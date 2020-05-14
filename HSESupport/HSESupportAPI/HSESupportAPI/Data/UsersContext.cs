using HSESupportAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HSESupportAPI.Data
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public UsersContext(DbContextOptions<UsersContext> options) : base(options)
        {
        }
    }
}