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
			modelBuilder.Entity<User>(u=>{
				u.HasData(new
					{
						Id = 1,
						Gold = 0
					});

				u.OwnsOne(x=>x.Specialization).HasData(new{
					UserId=1,
					SpecBuyingAmount = 0,
					SpecKillingAmount = 0,
					SpecBlessingAmount = 0,
					SpecCraftingAmount = 0,
					SpecQuestingAmount = 0,
					SpecMeltingAmount = 0,
					SpecBlessingThreshold = 1,
					SpecBlessingBuff = 0,
					SpecBuyingThreshold = 1,
					SpecBuyingBuff = 0,
					SpecQuestingThreshold = 1,
					SpecQuestingBuff = 0,
					SpecKillingThreshold = 1,
					SpecKillingBuff = 0,
					SpecCraftingThreshold = 1,
					SpecCraftingBuff = 0,
					SpecMeltingThreshold = 1,
					SpecMeltingBuff = 0
				});
			});

			base.OnModelCreating(modelBuilder);
		}

		public DbSet<User> Users { get; set; }
	}
}