using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using ClickQuest.Player;

namespace ClickQuest.Data.UserData
{
	public static class UserDataLoader
	{
		public static string UserDataPath;

		static UserDataLoader()
		{
			// [PRERELEASE]
			UserDataPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, @"Data\", @"UserData\", "User.json");
		}

		public static void Load()
		{
			if (!File.Exists(UserDataPath))
			{
				SeedIngots();
				SeedDungeonKeys();
				return;
			}

			string json = File.ReadAllText(UserDataPath);
			var user = JsonSerializer.Deserialize<User>(json);

			User.Instance = user;

			if (User.Instance.Ingots.Count == 0 || User.Instance.DungeonKeys.Count == 0)
			{
				SeedIngots();
				SeedDungeonKeys();

				Save();
			}

			PostLoad();
		}

		public static void PostLoad()
		{
			// Re-arrange references for all artifacts on all heroes using GameData.
			foreach (var hero in User.Instance.Heroes)
			{
				hero.Artifacts.ForEach(x => x.ArtifactFunctionality = GameData.GameData.Artifacts.FirstOrDefault(y => y.Name == x.Name)?.ArtifactFunctionality);
			}

			// Re-arrange references between hero's Artifacts and EquippedArtifacts.
			foreach (var hero in User.Instance.Heroes)
			{
				var newEquippedArtifacts = hero.Artifacts.Where(x => hero.EquippedArtifacts.Select(y => y.Name).Contains(x.Name)).ToList();
				hero.EquippedArtifacts = newEquippedArtifacts;
			}
			
			// Reload Achievements.
			User.Instance.Achievements.DeserializeAchievements();
			
			// Reload TimePlayed for all heroes.
			User.Instance.Heroes.ForEach(x=>x.TimePlayed = TimeSpan.Parse(x.TimePlayedString));
		}

		public static void Save()
		{
			// Convert Achievements to a serializable string.
			User.Instance.Achievements.SerializeAchievements();
			
			// Convert TimePlayed on all heroes to string.
			User.Instance.Heroes.ForEach(x=>x.TimePlayedString = x.TimePlayed.ToString());

			string json = JsonSerializer.Serialize(User.Instance);
			File.WriteAllText(UserDataPath, json);
		}

		public static void SeedIngots()
		{
			if (User.Instance.Ingots.Count == 0)
			{
				foreach (var ingot in GameData.GameData.Ingots)
				{
					User.Instance.Ingots.Add(ingot.CopyItem(0));
				}
			}
		}

		public static void SeedDungeonKeys()
		{
			if (User.Instance.DungeonKeys.Count == 0)
			{
				foreach (var dungeonKey in GameData.GameData.DungeonKeys)
				{
					User.Instance.DungeonKeys.Add(dungeonKey.CopyItem(0));
				}
			}
		}
	}
}