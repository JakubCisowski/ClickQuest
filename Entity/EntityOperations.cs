using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ClickQuest.Entity
{
	public static class EntityOperations
	{
		public static void SaveGame()
		{
			using (var db = new UserContext())
			{
				db.Users.RemoveRange(db.Users);

				db.Users.AddRange(Account.User.Instance);
				db.Entry(Account.User.Instance).State = EntityState.Modified;

				db.SaveChanges();
			}
		}

		public static void LoadGame()
		{
			using (var db = new UserContext())
			{
				// Load user.
				var user = db.Users.Include(x => x.Materials).Include(x => x.Heroes).Include(x => x.Artifacts).Include(x => x.Recipes).Include(x => x.Ingots)
					.FirstOrDefault();
				Account.User.Instance = user;
			}
		}
	}
}