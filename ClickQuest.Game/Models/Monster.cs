using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.DataTypes.Structs;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.UserInterface.Helpers;
using ClickQuest.Game.UserInterface.Pages;
using MaterialDesignThemes.Wpf;
using static ClickQuest.Game.Helpers.RandomnessHelper;

namespace ClickQuest.Game.Models;

public class Monster : Enemy
{
	public List<MonsterLootPattern> MonsterLootPatterns { get; set; }

	[JsonIgnore]
	public override int CurrentHealth
	{
		get => _currentHealth;
		set
		{
			// value - new current health
			if (value == Health)
			{
				_currentHealth = value;
			}
			else if (value <= 0)
			{
				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.TotalDamageDealt, _currentHealth);
				_currentHealth = 0;
				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.MonstersDefeated, 1);
			}
			else
			{
				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.TotalDamageDealt, _currentHealth - value);
				_currentHealth = value;
			}

			CurrentHealthProgress = CalculateCurrentHealthProgress();
		}
	}

	public override Monster CopyEnemy()
	{
		var copy = new Monster
		{
			Id = Id,
			Name = Name,
			Health = Health,
			CurrentHealth = Health,
			Description = Description,
			CurrentHealthProgress = CurrentHealthProgress,
			MonsterLootPatterns = MonsterLootPatterns
		};

		return copy;
	}

	public override void HandleEnemyDeathIfDefeated()
	{
		if (CurrentHealth <= 0)
		{
			CombatTimersHelper.StopPoisonTimer();

			// Mark the Monster as discovered.
			if (!GameAssets.BestiaryEntries.Any(x => x.EntryType == BestiaryEntryType.Monster && x.Id == Id))
			{
				GameAssets.BestiaryEntries.Add(new BestiaryEntry
				{
					Id = Id,
					EntryType = BestiaryEntryType.Monster
				});
			}

			GrantVictoryBonuses();

			// Invoke Artifacts with the "on-death" effect.
			foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
			{
				equippedArtifact.ArtifactFunctionality.OnKill();
			}

			InterfaceHelper.CurrentMonsterButton?.SpawnMonster();
		}
	}

	public override void GrantVictoryBonuses()
	{
		var experienceGained = ExperienceHelper.CalculateMonsterXpReward(Health);
		User.Instance.CurrentHero.GainExperience(experienceGained);

		var selectedLoot = RandomizeMonsterLoot();

		if (selectedLoot != null)
		{
			(selectedLoot as Artifact)?.CreateMythicTag(Name);
			selectedLoot.AddItem();

			// Mark the corresponding Pattern as discovered.
			if (!GameAssets.BestiaryEntries.Any(x => x.EntryType == BestiaryEntryType.MonsterLoot && x.Id == selectedLoot.Id))
			{
				GameAssets.BestiaryEntries.Add(new BestiaryEntry
				{
					Id = selectedLoot.Id,
					EntryType = BestiaryEntryType.MonsterLoot
				});
			}

			switch (selectedLoot)
			{
				case Material material:
					LootQueueHelper.AddToQueue(material.Name, material.Rarity, PackIconKind.Cog);
					break;

				case Recipe recipe:
					LootQueueHelper.AddToQueue(recipe.FullName, recipe.Rarity, PackIconKind.ScriptText);
					break;

				case Artifact artifact:
					LootQueueHelper.AddToQueue(artifact.Name, artifact.Rarity, PackIconKind.DiamondStone);
					break;
			}
		}

		CheckForDungeonKeyDrop();

		((GameAssets.CurrentPage as RegionPage).StatsFrame.Content as HeroStatsPage).RefreshAllDynamicStatsAndToolTips();
		CombatTimersHelper.UpdateAuraAttackSpeed();
	}

	public Item RandomizeMonsterLoot()
	{
		var frequencyList = MonsterLootPatterns.Select(x => x.Frequency).ToList();
		var randomizedValue = Rng.Next(1, 10001) / 10000d;

		var i = 0;

		while (i < frequencyList.Count)
		{
			if (randomizedValue < frequencyList[i])
			{
				return MonsterLootPatterns[i].Item;
			}

			randomizedValue -= frequencyList[i];
			i++;
		}

		return null;
	}
}