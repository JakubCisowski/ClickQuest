using System;
using System.Collections.Generic;
using ClickQuest.Game.Extensions.CombatManager;
using ClickQuest.Game.Extensions.InterfaceManager;
using ClickQuest.Game.Heroes.Buffs;
using ClickQuest.Game.Items;
using ClickQuest.Game.Player;
using static ClickQuest.Game.Extensions.RandomnessManager.RandomnessController;

namespace ClickQuest.Game.Enemies
{
	public class Boss : Enemy
	{
		public List<BossLootPattern> BossLoot { get; set; }

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
			copy.BossLoot = BossLoot;

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

			foreach (var loot in BossLoot)
			{
				int itemIntegerCount = (int)loot.Frequencies[threshold];

				double randomizedValue = RNG.Next(1, 10001) / 10000d;
				if (randomizedValue < loot.Frequencies[threshold] - itemIntegerCount)
				{
					// Grant loot after checking if it's not empty.
					if (loot.LootType == LootType.Blessing)
					{
						bool hasBlessingActive = User.Instance.CurrentHero.Blessing != null;

						if (hasBlessingActive)
						{
							bool doesUserWantToSwap = Blessing.AskUserAndSwapBlessing(loot.LootId);

							if (doesUserWantToSwap == false)
							{
								continue;
							}
						}
						else
						{
							Blessing.AddOrReplaceBlessing(loot.LootId);
						}
						
						continue;
					}
					else
					{
						itemIntegerCount++;
					}
				}

				loot.Item.AddItem(itemIntegerCount);
				lootText += "- " + $"{itemIntegerCount}x " + loot.Item.Name + " (" + loot.LootType + ")\n";
			}

			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();

			// Grant gold reward.
			int goldReward = 2137; // (change value later)
			User.Instance.Gold += goldReward;
			lootText += "- " + goldReward + " (gold)\n";

			// [PRERELEASE] Display exp and loot for testing purposes.
			InterfaceController.CurrentBossPage.TestRewardsBlock.Text = lootText;

			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Dungeon]++;

			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.DungeonsCompleted, 1);
		}
	}
}