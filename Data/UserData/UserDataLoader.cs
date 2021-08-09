using System;
using System.IO;
using System.Text.Json;
using ClickQuest.Player;
using ClickQuest.Data;

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
			var json = File.ReadAllText(UserDataPath);
			var user = JsonSerializer.Deserialize<User>(json);

			User.Instance = user;

			if (User.Instance.Ingots.Count == 0 || User.Instance.DungeonKeys.Count == 0)
			{
				SeedIngots();
				SeedDungeonKeys();

				Save();
			}
		}

		public static void Save()
		{
			var json = JsonSerializer.Serialize(User.Instance);
			File.WriteAllText(UserDataPath, json);
		}

		private static void SeedIngots()
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