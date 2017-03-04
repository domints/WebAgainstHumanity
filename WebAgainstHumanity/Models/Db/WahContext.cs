using Microsoft.EntityFrameworkCore;

namespace WebAgainstHumanity.Models.Db
{
    public class WahContext : DbContext
    {
        public WahContext(DbContextOptions<WahContext> options)
        : base(options)
        { }

        public DbSet<User> Users { get; set; }
    }
}