using ClickQuest.Heroes;
using ClickQuest.Items;
using Microsoft.EntityFrameworkCore;

namespace ClickQuest.Entity
{
    public class UserContext : DbContext
    {
        public UserContext()
        {
        }

        public UserContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Hero> Heroes { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Artifact> Artifacts { get; set; }
    }
}