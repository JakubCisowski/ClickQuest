using ClickQuest.Player;
using ClickQuest.Heroes;
using ClickQuest.Items;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using ClickQuest.Data;
using ClickQuest.Pages;
using ClickQuest.Heroes.Buffs;

namespace ClickQuest.Entity
{
	public static class EntityOperations
	{
		public static void SaveGame()
		{
			// Serialize Achievements.
			User.Instance.Achievements.SerializeAchievements();

			// Setialize Specializations.
			foreach (var hero in User.Instance.Heroes)
			{
				hero.Specialization.SerializeSpecializationAmounts();
			}

			using (var db = new UserContext())
			{
				// Make sure we get the right user by Id
				var user = db.Users.FirstOrDefault(x => x.Id == User.Instance.Id);

				user.Gold = User.Instance.Gold;
				user.Heroes = User.Instance.Heroes;
				user.Ingots = User.Instance.Ingots;
				user.DungeonKeys = User.Instance.DungeonKeys;
				user.Achievements = User.Instance.Achievements;

				// Save every equipment, quests, blessings and specializations for each hero.
				foreach (var heroFromDb in user.Heroes)
				{
					var heroFromApp = User.Instance.Heroes.FirstOrDefault(x => x.Id == heroFromDb.Id);

					heroFromDb.Materials = heroFromApp.Materials;
					heroFromDb.Recipes = heroFromApp.Recipes;
					heroFromDb.Artifacts = heroFromApp.Artifacts;
					heroFromDb.Specialization = heroFromApp.Specialization;
					heroFromDb.Quests = heroFromApp.Quests;
					heroFromDb.Blessing = heroFromApp.Blessing;
					heroFromDb.TimePlayed = heroFromApp.TimePlayed;
				}

				db.Users.Update(user);
				db.SaveChanges();
			}
		}

		public static void LoadGame()
		{
			using (var db = new UserContext())
			{
				// Load user. Include all collections in it.
				var user = db.Users.Include(x => x.Heroes).ThenInclude(x => x.Materials).
					Include(x => x.Heroes).ThenInclude(x => x.Recipes).
					Include(x => x.Heroes).ThenInclude(x => x.Artifacts).
					Include(x => x.Heroes).ThenInclude(x => x.Quests).
					Include(x => x.Heroes).
					Include(x => x.Ingots).Include(x => x.DungeonKeys).FirstOrDefault();
				User.Instance = user;

				// Update MaterialIds dictionary and RequirementsDescription for each recipe.
				foreach (var hero in User.Instance.Heroes)
				{
					foreach (var recipe in hero.Recipes)
					{
						recipe.UpdateRequirementsDescription();
					}

					foreach (var quest in hero.Quests)
					{
						quest.UpdateAllRewardsDescription();
					}
				}
			}

			// Deserialize achievements.
			User.Instance.Achievements.DeserializeAchievements();

			// Desetialize Specializations.
			foreach (var hero in User.Instance.Heroes)
			{
				hero.Specialization.DeserializeSpecializationAmounts();
				hero.Specialization.UpdateSpecialization();
			}
		}

		public static void RemoveItem(Item item)
		{
			using (var db = new UserContext())
			{
				var user = db.Users.Include(x => x.Heroes).ThenInclude(x => x.Materials).
					Include(x => x.Heroes).ThenInclude(x => x.Recipes).
					Include(x => x.Heroes).ThenInclude(x => x.Artifacts).FirstOrDefault();

				// Get the right hero to remove from.
				var currentHero = user.Heroes.FirstOrDefault(x => x.Id == User.Instance.CurrentHero.Id);

				// Remove the item from the right collection.
				if (item is Material)
				{
					var material = currentHero.Materials.FirstOrDefault(x => x.Id == item.Id);
					if (material != null)
					{
						currentHero.Materials.Remove(material);
					}
				}
				else if (item is Recipe)
				{
					var recipe = currentHero.Recipes.FirstOrDefault(x => x.Id == item.Id);
					if (recipe != null)
					{
						currentHero.Recipes.Remove(recipe);
					}
				}
				else if (item is Artifact)
				{
					var artifact = currentHero.Artifacts.FirstOrDefault(x => x.Id == item.Id);
					if (artifact != null)
					{
						currentHero.Artifacts.Remove(artifact);
					}
				}

				db.SaveChanges();
			}
		}

		public static void RemoveHero(Hero heroToRemove)
		{
			using (var db = new UserContext())
			{
				var user = db.Users.Include(x => x.Heroes).ThenInclude(x => x.Materials)
					.Include(x => x.Heroes).ThenInclude(x => x.Recipes)
					.Include(x => x.Heroes).ThenInclude(x => x.Artifacts)
					.Include(x => x.Heroes).ThenInclude(x => x.Quests)
					.Include(x => x.Heroes)
					.FirstOrDefault();

				var hero = user.Heroes.FirstOrDefault(x => x.Id == heroToRemove.Id);
				if (hero != null)
				{
					// Remove collections.
					while (hero.Materials.Count > 0)
					{
						hero.Materials.RemoveAt(0);
					}
					while (hero.Artifacts.Count > 0)
					{
						hero.Artifacts.RemoveAt(0);
					}
					while (hero.Recipes.Count > 0)
					{
						hero.Recipes.RemoveAt(0);
					}
					while (hero.Quests.Count > 0)
					{
						hero.Quests.RemoveAt(0);
					}

					// Remove hero.
					db.Entry(hero).State = EntityState.Deleted;
					user.Heroes.Remove(hero);
				}

				db.SaveChanges();
			}
		}

		public static void RemoveQuests()
		{
			using (var db = new UserContext())
			{
				var user = db.Users.Include(x => x.Heroes).ThenInclude(x => x.Quests).FirstOrDefault();

				var hero = user.Heroes.FirstOrDefault(x => x.Id == Player.User.Instance.CurrentHero.Id);

				if (hero != null)
				{
					while (hero.Quests.Count > 0)
					{
						hero.Quests.RemoveAt(0);
					}

					db.SaveChanges();
				}
			}
		}

		public static void ResetProgress()
		{
			using (var db = new UserContext())
			{
				var user = db.Users.Include(x => x.Heroes).ThenInclude(x => x.Materials)
					.Include(x => x.Heroes).ThenInclude(x => x.Recipes)
					.Include(x => x.Heroes).ThenInclude(x => x.Artifacts)
					.Include(x => x.Heroes).ThenInclude(x => x.Quests)
					.Include(x => x.Heroes)
					.Include(x => x.Ingots).Include(x => x.DungeonKeys)
					.FirstOrDefault();

				// Delete all items and heroes (except for ingots - only set their quantity to 0).

				for (int i = 0; i < user.Ingots.Count; i++)
				{
					user.Ingots[i].Quantity = 0;
				}
				for (int i = 0; i < user.DungeonKeys.Count; i++)
				{
					user.DungeonKeys[i].Quantity = 0;
				}
				foreach (var hero in user.Heroes)
				{
					while (hero.Materials.Count > 0)
					{
						hero.Materials.RemoveAt(0);
					}
					while (hero.Artifacts.Count > 0)
					{
						hero.Artifacts.RemoveAt(0);
					}
					while (hero.Recipes.Count > 0)
					{
						hero.Recipes.RemoveAt(0);
					}
					while (hero.Quests.Count > 0)
					{
						hero.Quests.RemoveAt(0);
					}

					// Reset Specializations.
					hero.Specialization.SpecializationAmounts = new Extensions.CollectionsManager.ObservableDictionary<SpecializationType, int>();
					Extensions.CollectionsManager.CollectionInitializer.InitializeDictionary<SpecializationType, int>(hero.Specialization.SpecializationAmounts);

				}
				while (user.Heroes.Count > 0)
				{
					db.Entry(user.Heroes[0]).State = EntityState.Deleted;
					user.Heroes.RemoveAt(0);
				}
				user.Gold = 0;

				// Reset Achievements.
				var achievements = user.Achievements;
				achievements.NumericAchievementCollection = new Extensions.CollectionsManager.ObservableDictionary<NumericAchievementType, long>();
				Extensions.CollectionsManager.CollectionInitializer.InitializeDictionary<NumericAchievementType, long>(achievements.NumericAchievementCollection);
				achievements.TotalTimePlayed = default;

				db.SaveChanges();
			}

			// Load the empty collections.
			LoadGame();
		}

		public static void CreateAndSeedDatabase()
		{
			using (var db = new UserContext())
			{
				// Ensure the database exists - if not, create it.
				db.Database.EnsureCreated();

				var user = db.Users.Include(x => x.Ingots).Include(x => x.DungeonKeys).FirstOrDefault();

				// The seeding has to be done here, because otherwise we would need to set up relationships manually (using foreign keys, etc.)

				// If there are no dungeon keys in the database, add them (seed).
				if (user.DungeonKeys.Count == 0)
				{
					foreach (var dungeonKey in GameData.DungeonKeys)
					{
						user.DungeonKeys.Add(dungeonKey.CopyItem(0));
					}
				}

				// If there are no ingots in the database, add them (seed).
				if (user.Ingots.Count == 0)
				{
					foreach (var ingot in GameData.Ingots)
					{
						user.Ingots.Add(ingot.CopyItem(0));
					}
				}

			db.Users.Update(user);
				db.SaveChanges();
			}
		}
	}
}