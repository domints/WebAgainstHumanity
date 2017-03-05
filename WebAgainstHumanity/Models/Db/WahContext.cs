using Microsoft.EntityFrameworkCore;

namespace WebAgainstHumanity.Models.Db
{
    public class WahContext : DbContext
    {
        public WahContext(DbContextOptions<WahContext> options)
        : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<CardSet> CardSets { get; set; }
        public DbSet<Card> Cards { get; set; }
    }
}