using ClickQuest.Player;
using Microsoft.EntityFrameworkCore;

namespace ClickQuest.Entity
{
	public class UserContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=ClickQuest.Entity.UserContext;");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Seed the database with a sample, empty user.
			modelBuilder.Entity<User>(u =>
			{
				u.HasData(new
				{
					Id = 1,
					Gold = 0
				});
			});

			base.OnModelCreating(modelBuilder);
		}

		public DbSet<User> Users { get; set; }
	}
}