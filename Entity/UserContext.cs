using ClickQuest.Heroes;
using ClickQuest.Items;
using Microsoft.EntityFrameworkCore;

namespace ClickQuest.Entity
{
	public class UserContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=ClickQuest.Entity.UserContext;");
		}

		public DbSet<Hero> Heroes { get; set; }
		public DbSet<Material> Materials { get; set; }
		public DbSet<Recipe> Recipes { get; set; }
		public DbSet<Artifact> Artifacts { get; set; }
	}
}