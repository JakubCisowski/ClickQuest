using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Items.Patterns;
using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.Gameplay;
using ClickQuest.Game.Extensions.UserInterface;
using MaterialDesignThemes.Wpf;
using static ClickQuest.Game.Extensions.Randomness.RandomnessController;

namespace ClickQuest.Game.Core.Enemies
{
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
			Boss copy = new Boss
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
				CombatTimerController.StopPoisonTimer();
				CombatTimerController.BossFightTimer.Stop();

				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.BossesDefeated, 1);

				GrantVictoryBonuses();

				// Invoke Artifacts with the "on-death" effect.
				foreach (Artifact equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
				{
					equippedArtifact.ArtifactFunctionality.OnKill();
				}

				InterfaceController.CurrentBossPage.HandleInterfaceAfterBossDeath();
			}
		}

		public override void GrantVictoryBonuses()
		{
			// [PRERELEASE]
			const int experienceGained = 10;
			User.Instance.CurrentHero.GainExperience(experienceGained);

			// Grant boss loot.
			// 1. Check % threshold for reward loot frequencies ("5-" is for inverting 0 -> full hp, 5 -> boss died).
			int threshold = 5 - (int) Math.Ceiling(CurrentHealth / (Health / 5.0));
			// 2. Iterate through every possible loot.

			var lootQueueEntries = new List<LootQueueEntry>();

			foreach (BossLootPattern loot in BossLootPatterns)
			{
				var itemIntegerCount = (int) loot.Frequencies[threshold];

				double randomizedValue = Rng.Next(1, 10001) / 10000d;
				if (randomizedValue < loot.Frequencies[threshold] - itemIntegerCount)
				{
					itemIntegerCount++;
				}

				// Grant loot after checking if it's not empty.
				if (loot.BossLootType == RewardType.Blessing)
				{
					bool isBlessingAccepted = Blessing.AskUserAndSwapBlessing(loot.BossLootId);

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
						Blessing? blessing = GameAssets.Blessings.FirstOrDefault(x => x.Id == loot.BossLootId);

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

			LootQueueController.AddToQueue(lootQueueEntries);

			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();

			GameController.UpdateSpecializationAmountAndUi(SpecializationType.Dungeon);

			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.DungeonsCompleted, 1);
		}
	}
}