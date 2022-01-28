using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.DataTypes.Structs;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models.Functionalities;
using ClickQuest.Game.UserInterface.Helpers;
using MaterialDesignThemes.Wpf;
using static ClickQuest.Game.Helpers.RandomnessHelper;

namespace ClickQuest.Game.Models;

public class Boss : Enemy
{
	public List<BossLootPattern> BossLootPatterns { get; set; }

	[JsonIgnore]
	public List<AffixFunctionality> AffixFunctionalities { get; set; }

	public List<Affix> Affixes { get; set; }

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
				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.BossesDefeated, 1);
			}
			else
			{
				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.TotalDamageDealt, _currentHealth - value);
				_currentHealth = value;
			}

			CurrentHealthProgress = CalculateCurrentHealthProgress();
		}
	}

	public override Boss CopyEnemy()
	{
		var copy = new Boss
		{
			Id = Id,
			Name = Name,
			Health = Health,
			CurrentHealth = Health,
			Description = Description,
			CurrentHealthProgress = CurrentHealthProgress,
			BossLootPatterns = BossLootPatterns,
			AffixFunctionalities = AffixFunctionalities,
			Affixes = Affixes
		};

		return copy;
	}

	public override void HandleEnemyDeathIfDefeated()
	{
		if (CurrentHealth <= 0)
		{
			CombatTimersHelper.StopPoisonTimer();
			CombatTimersHelper.BossFightTimer.Stop();

			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.BossesDefeated, 1);

			GrantVictoryBonuses();

			// Invoke Artifacts with the "on-death" effect.
			foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
			{
				equippedArtifact.ArtifactFunctionality.OnKill();
			}

			InterfaceHelper.CurrentBossPage.HandleInterfaceAfterBossDeath();
		}
	}

	public override void GrantVictoryBonuses()
	{
		// Grant boss rewards.
		// 1. Check % threshold for reward exp and loot frequencies ("5-" is for inverting 0 -> full hp, 5 -> boss died).
		var threshold = 5 - (int)Math.Ceiling(CurrentHealth / (Health / 5.0));

		// 2. Grant experience
		var experienceReward = ExperienceHelper.CalculateBossXpReward(Health, threshold);
		User.Instance.CurrentHero.GainExperience(experienceReward);

		// 3. Iterate through every possible loot.

		var lootQueueEntries = new List<LootQueueEntry>();

		foreach (var loot in BossLootPatterns)
		{
			var itemIntegerCount = (int)loot.Frequencies[threshold];

			var randomizedValue = Rng.Next(1, 10001) / 10000d;
			if (randomizedValue < loot.Frequencies[threshold] - itemIntegerCount)
			{
				itemIntegerCount++;
			}

			// Grant loot after checking if it's not empty.
			if (loot.BossLootType == RewardType.Blessing)
			{
				var isBlessingAccepted = Blessing.AskUserAndSwapBlessing(loot.BossLootId);

				if (!GameAssets.BestiaryEntries.Any(x => x.EntryType == BestiaryEntryType.BossLoot && x.LootType == RewardType.Blessing && x.Id == loot.BossLootId))
				{
					GameAssets.BestiaryEntries.Add(new BestiaryEntry
					{
						Id = loot.BossLootId,
						LootType = RewardType.Blessing,
						EntryType = BestiaryEntryType.BossLoot
					});
				}

				if (isBlessingAccepted)
				{
					// Start blessing animation.
					var blessing = GameAssets.Blessings.FirstOrDefault(x => x.Id == loot.BossLootId);

					lootQueueEntries.Add(new LootQueueEntry(blessing.Name, blessing.Rarity, PackIconKind.BookCross, 1));
				}

				continue;
			}

			(loot.Item as Artifact)?.CreateMythicTag(Name);

			if (itemIntegerCount > 0)
			{
				loot.Item.AddItem(itemIntegerCount);

				if (!GameAssets.BestiaryEntries.Any(x => x.EntryType == BestiaryEntryType.BossLoot && x.LootType == loot.BossLootType && x.Id == loot.BossLootId))
				{
					GameAssets.BestiaryEntries.Add(new BestiaryEntry
					{
						Id = loot.BossLootId,
						LootType = loot.BossLootType,
						EntryType = BestiaryEntryType.BossLoot
					});
				}

				// Start loot animation.
				switch (loot.Item)
				{
					case Material material:
						lootQueueEntries.Add(new LootQueueEntry(material.Name, material.Rarity, PackIconKind.Cog, itemIntegerCount));
						break;

					case Recipe recipe:
						lootQueueEntries.Add(new LootQueueEntry(recipe.FullName, recipe.Rarity, PackIconKind.ScriptText, itemIntegerCount));
						break;

					case Artifact artifact:
						lootQueueEntries.Add(new LootQueueEntry(artifact.Name, artifact.Rarity, PackIconKind.DiamondStone, itemIntegerCount));
						break;
				}
			}
		}

		LootQueueHelper.AddToQueue(lootQueueEntries);

		InterfaceHelper.RefreshStatsAndEquipmentPanelsOnCurrentPage();

		Specializations.UpdateSpecializationAmountAndUi(SpecializationType.Dungeon);

		User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.DungeonsCompleted, 1);
	}
}