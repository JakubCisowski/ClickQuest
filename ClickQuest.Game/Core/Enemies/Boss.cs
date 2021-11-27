using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Items.Patterns;
using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.Gameplay;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Windows;
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
			get
			{
				return _currentHealth;
			}
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
			var copy = new Boss();

			copy.Id = Id;
			copy.Name = Name;
			copy.Health = Health;
			copy.CurrentHealth = Health;
			copy.Description = Description;
			copy.CurrentHealthProgress = CurrentHealthProgress;
			copy.BossLootPatterns = BossLootPatterns;
			copy.AffixFunctionalities = AffixFunctionalities;
			copy.Affixes = Affixes;

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
				foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
				{
					equippedArtifact.ArtifactFunctionality.OnKill();
				}
				
				InterfaceController.CurrentBossPage.HandleInterfaceAfterBossDeath();
			}
		}

		public override void GrantVictoryBonuses()
		{
			int damageDealtToBoss = Health - CurrentHealth;
			// [PRERELEASE]
			int experienceGained = 10;
			User.Instance.CurrentHero.GainExperience(experienceGained);

			// Grant boss loot.
			// 1. Check % threshold for reward loot frequencies ("5-" is for inverting 0 -> full hp, 5 -> boss died).
			int threshold = 5 - (int)Math.Ceiling(CurrentHealth / (Health / 5.0));
			// 2. Iterate through every possible loot.

			var lootQueueEntries = new List<LootQueueEntry>();

			foreach (var loot in BossLootPatterns)
			{
				int itemIntegerCount = (int)loot.Frequencies[threshold];

				double randomizedValue = RNG.Next(1, 10001) / 10000d;
				if (randomizedValue < loot.Frequencies[threshold] - itemIntegerCount)
				{
					itemIntegerCount++;
				}

				// Grant loot after checking if it's not empty.
				if (loot.BossLootType == RewardType.Blessing)
				{
					bool isBlessingAccepted = Blessing.AskUserAndSwapBlessing(loot.BossLootId);

					if (!GameAssets.BestiaryEntries.Any(x=>x.EntryType == BestiaryEntryType.BossLoot && x.LootType == RewardType.Blessing && x.Id==loot.BossLootId))
					{
						GameAssets.BestiaryEntries.Add(new BestiaryEntry() { Id = loot.BossLootId, LootType = RewardType.Blessing, EntryType = BestiaryEntryType.BossLoot });
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

					if (!GameAssets.BestiaryEntries.Any(x=>x.EntryType == BestiaryEntryType.BossLoot && x.LootType == loot.BossLootType && x.Id==loot.BossLootId))
					{
						GameAssets.BestiaryEntries.Add(new BestiaryEntry() { Id = loot.BossLootId, LootType = loot.BossLootType, EntryType = BestiaryEntryType.BossLoot });
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

			GameController.UpdateSpecializationAmountAndUI(SpecializationType.Dungeon);

			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.DungeonsCompleted, 1);
		}
	}
}