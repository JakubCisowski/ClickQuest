using ClickQuest.Items;
using System.Collections.Generic;
using System.Linq;

namespace ClickQuest.Entity
{
	public static class EntityOperations
	{
		public static void SaveGame()
		{
			using (var db = new UserContext())
			{
				db.Heroes.RemoveRange(db.Heroes);
				db.Materials.RemoveRange(db.Materials);
				db.Recipes.RemoveRange(db.Recipes);
				db.Artifacts.RemoveRange(db.Artifacts);
				db.Ingots.RemoveRange(db.Ingots);

				db.Heroes.AddRange(Account.User.Instance.Heroes);

				db.Materials.AddRange(Account.User.Instance.Items.Where(x => x is Material).Cast<Material>());
				db.Recipes.AddRange(Account.User.Instance.Items.Where(x => x is Recipe).Cast<Recipe>());
				db.Artifacts.AddRange(Account.User.Instance.Items.Where(x => x is Artifact).Cast<Artifact>());
				db.Ingots.AddRange(Account.User.Instance.Ingots);

				db.SaveChanges();
			}
		}

		public static void LoadGame()
		{
			using (var db = new UserContext())
			{
				Account.User.Instance.Heroes = db.Heroes.ToList();

				var list = new List<Item>();

				list.AddRange(db.Materials.ToList<Item>());
				list.AddRange(db.Artifacts.ToList<Item>());

				var recipes = db.Recipes.ToList();
				foreach (var recipe in recipes)
				{
					recipe.MaterialIds = Data.Database.Recipes.FirstOrDefault(x => x.Id == recipe.Id).MaterialIds;
					recipe.UpdateRequirementsDescription();
				}

				list.AddRange(recipes.Cast<Item>());

				Account.User.Instance.Items = list;

				Account.User.Instance.Ingots = db.Ingots.ToList();
			}
		}

		public static void ClearHeroes()
		{
			using (var db = new UserContext())
			{
				db.Heroes.RemoveRange(db.Heroes);
				db.Materials.RemoveRange(db.Materials);
				db.Recipes.RemoveRange(db.Recipes);
				db.Artifacts.RemoveRange(db.Artifacts);
				db.Ingots.RemoveRange(db.Ingots);
				db.SaveChanges();
			}
		}
	}
}