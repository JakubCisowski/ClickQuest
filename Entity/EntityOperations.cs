using ClickQuest.Account;
using ClickQuest.Items;
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

		public static void RemoveItem(Item item)
		{
			using (var db = new UserContext())
			{
				var user = db.Users.Include(x => x.Materials).Include(x => x.Heroes).Include(x => x.Artifacts).Include(x => x.Recipes).Include(x => x.Ingots)
					.FirstOrDefault();

				if (item is Material)
				{
					var material = user.Materials.FirstOrDefault(x => x.Id == item.Id);
					if (material != null)
					{
						user.Materials.Remove(material);
					}
				}
				else if (item is Recipe)
				{
					var recipe = user.Recipes.FirstOrDefault(x => x.Id == item.Id);
					if (recipe != null)
					{
						user.Recipes.Remove(recipe);
					}
				}
				else if (item is Artifact)
				{
					var artifact = user.Artifacts.FirstOrDefault(x => x.Id == item.Id);
					if (artifact != null)
					{
						user.Artifacts.Remove(artifact);
					}
				}

				db.SaveChanges();
			}
		}

		public static void ResetProgress()
		{
			using (var db = new UserContext())
			{
				var user = db.Users.Include(x => x.Materials).Include(x => x.Heroes).Include(x => x.Artifacts).Include(x => x.Recipes).Include(x => x.Ingots)
					.FirstOrDefault();
					while(user.Materials.Count > 0)
					{
						user.Materials.RemoveAt(0);
					}
					while(user.Artifacts.Count > 0)
					{
						user.Artifacts.RemoveAt(0);
					}
					while(user.Recipes.Count > 0)
					{
						user.Recipes.RemoveAt(0);
					}
					for(int i=0; i<user.Ingots.Count(); i++)
					{
						user.Ingots[i].Quantity=0;
					}
					while(user.Heroes.Count > 0)
					{
						user.Heroes.RemoveAt(0);
					}
					user.Gold=0;

					db.SaveChanges();
			}

			LoadGame();
		}
	}
}