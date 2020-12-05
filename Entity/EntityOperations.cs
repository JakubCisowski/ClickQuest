using ClickQuest.Account;
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
				// Make sure we get the right user by Id
				var user = db.Users.FirstOrDefault(x => x.Id == User.Instance.Id);

				user.Gold = User.Instance.Gold;
				user.Artifacts = User.Instance.Artifacts;
				user.Recipes = User.Instance.Recipes;
				user.Materials = User.Instance.Materials;
				user.Heroes = User.Instance.Heroes;

				db.Users.Update(user);
				db.SaveChanges();
			}
		}

		public static void LoadGame()
		{
			using (var db = new UserContext())
			{
				// Load user. Include all collections in it.
				var user = db.Users.Include(x => x.Materials).Include(x => x.Heroes).Include(x => x.Artifacts).Include(x => x.Recipes).Include(x => x.Ingots)
					.FirstOrDefault();
				User.Instance = user;
			}
		}
	}
}