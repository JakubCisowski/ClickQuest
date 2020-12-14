using ClickQuest.Account;
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
			modelBuilder.Entity<User>().HasData(new User()
			{
				Id = 1
			});

			base.OnModelCreating(modelBuilder);
		}

		public DbSet<User> Users { get; set; }
	}
}