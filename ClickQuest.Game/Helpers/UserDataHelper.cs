using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using ClickQuest.Game.DataTypes.Structs;
using ClickQuest.Game.Models;

namespace ClickQuest.Game.Helpers;

public static class UserDataHelper
{
	public static readonly string UserDataPath;
	public static readonly string BestiaryDataPath;

	static UserDataHelper()
	{
		UserDataPath = Path.Combine(Environment.CurrentDirectory, @"Data\", @"UserData\", "User.aes");
		BestiaryDataPath = Path.Combine(Environment.CurrentDirectory, @"Data\", @"UserData\", "Bestiary.aes");

		if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, @"Data\", @"UserData")))
		{
			Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, @"Data\", @"UserData"));
		}
	}

	public static void Load()
	{
		if (!File.Exists(UserDataPath))
		{
			SeedIngots();
			SeedDungeonKeys();
			return;
		}

		// Decrypt the file.
		var encryptedJson = File.ReadAllBytes(UserDataPath);

		var json = EncryptionHelper.DecryptJsonUsingAes(encryptedJson);

		if (!string.IsNullOrEmpty(json))
		{
			var user = JsonSerializer.Deserialize<User>(json, new JsonSerializerOptions());

			User.Instance = user;

			if (User.Instance.Ingots.Count == 0 || User.Instance.DungeonKeys.Count == 0)
			{
				SeedIngots();
				SeedDungeonKeys();

				Save();
			}

			LoadBestiary();

			PostLoad();
		}
	}

	public static void PostLoad()
	{
		// Re-arrange references for all artifacts on all heroes using GameAssets.
		foreach (var hero in User.Instance.Heroes)
		{
			hero.Artifacts.ForEach(x => x.ArtifactFunctionality = GameAssets.Artifacts.FirstOrDefault(y => y.Name == x.Name)?.ArtifactFunctionality);
			hero.Recipes.ForEach(x =>
			{
				x.UpdateDescription();
				x.UpdateRequirementsDescription();
			});
		}

		// Re-arrange references between hero's Artifacts and EquippedArtifacts.
		foreach (var hero in User.Instance.Heroes)
		{
			for (var i = 0; i < hero.EquippedArtifacts.Count; i++)
			{
				var newEquippedArtifact = hero.Artifacts.FirstOrDefault(x => x.Id == hero.EquippedArtifacts[i].Id);
				hero.EquippedArtifacts[i] = newEquippedArtifact;
			}
		}

		// Reload Achievements.
		User.Instance.Achievements.DeserializeAchievements();

		// Reload TimePlayed for all heroes.
		User.Instance.Heroes.ForEach(x => x.TimePlayed = TimeSpan.Parse(x.TimePlayedString));
	}

	public static void LoadBestiary()
	{
		if (!File.Exists(BestiaryDataPath))
		{
			var stream = File.Create(BestiaryDataPath);
			stream.Close();
		}

		var encryptedJson = File.ReadAllBytes(BestiaryDataPath);
		var bestiaryJson = EncryptionHelper.DecryptJsonUsingAes(encryptedJson);

		if (!string.IsNullOrEmpty(bestiaryJson))
		{
			GameAssets.BestiaryEntries = JsonSerializer.Deserialize<List<BestiaryEntry>>(bestiaryJson);
		}
	}

	public static string Save()
	{
		// Convert Achievements to a serializable string.
		User.Instance.Achievements.SerializeAchievements();

		// Convert TimePlayed on all heroes to string.
		User.Instance.Heroes.ForEach(x => x.TimePlayedString = x.TimePlayed.ToString());

		var json = JsonSerializer.Serialize(User.Instance, new JsonSerializerOptions
		{
			IgnoreReadOnlyProperties = true,
			WriteIndented = true,
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
		});

		// Encrypt the file.
		var encrypted = EncryptionHelper.EncryptJsonUsingAes(json);

		File.WriteAllBytes(UserDataPath, encrypted);

		SaveBestiary();

		return json;
	}

	public static void SaveBestiary()
	{
		var bestiaryJson = JsonSerializer.Serialize(GameAssets.BestiaryEntries, new JsonSerializerOptions
		{
			IgnoreReadOnlyProperties = true,
			WriteIndented = true,
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
		});

		var encrypted = EncryptionHelper.EncryptJsonUsingAes(bestiaryJson);
		File.WriteAllBytes(BestiaryDataPath, encrypted);
	}

	public static void SeedIngots()
	{
		if (User.Instance.Ingots.Count == 0)
		{
			foreach (var ingot in GameAssets.Ingots)
			{
				User.Instance.Ingots.Add(ingot.CopyItem(0));
			}
		}
	}

	public static void SeedDungeonKeys()
	{
		if (User.Instance.DungeonKeys.Count == 0)
		{
			foreach (var dungeonKey in GameAssets.DungeonKeys)
			{
				User.Instance.DungeonKeys.Add(dungeonKey.CopyItem(0));
			}
		}
	}
}