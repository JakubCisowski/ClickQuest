using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Items.Patterns;
using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.Gameplay;
using ClickQuest.Game.Extensions.UserInterface;
using static ClickQuest.Game.Extensions.Randomness.RandomnessController;

namespace ClickQuest.Game.Core.Enemies
{
	public class Boss : Enemy
	{
		public List<BossLootPattern> BossLootPatterns { get; set; }

		[JsonIgnore]
		public List<AffixFunctionality> AffixFunctionalities { get; set; }

		public List<Affix> Affixes { get; set; }

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
			int threshold = 5 - (int)Math.Ceiling((double)CurrentHealth / (Health / 5));
			// 2. Iterate through every possible loot.
			string lootText = "Experience gained: " + experienceGained + " \n" + "Loot: \n";

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
					Blessing.AskUserAndSwapBlessing(loot.BossLootId);

					continue;
				}

				(loot.Item as Artifact)?.CreateMythicTag(Name);

				if (itemIntegerCount > 0)
				{
					loot.Item.AddItem(itemIntegerCount);
					lootText += "- " + $"{itemIntegerCount}x " + loot.Item.Name + " (" + loot.BossLootType + ")\n";
				}
			}

			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();

			// [PRERELEASE] Display exp and loot for testing purposes.
			InterfaceController.CurrentBossPage.TestRewardsBlock.Text = lootText;

			GameController.UpdateSpecializationAmountAndUI(SpecializationType.Dungeon);

			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.DungeonsCompleted, 1);
		}
	}
}