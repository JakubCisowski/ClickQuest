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

		public DbSet<User> Users { get; set; }
	}
}