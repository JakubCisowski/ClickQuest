using System.Collections.Generic;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Heroes.Buffs;
using ClickQuest.Items;
using ClickQuest.Player;
using static ClickQuest.Extensions.RandomnessManager.RandomnessController;

namespace ClickQuest.Enemies
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

		public override bool HandleEnemyDeathIfDefeated()
		{
			if (CurrentHealth <= 0)
			{
				CombatTimerController.StopPoisonTimer();
				CombatTimerController.BossFightTimer.Stop();

				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.BossesDefeated, 1);

				GrantVictoryBonuses();
				InterfaceController.CurrentBossPage.HandleInterfaceAfterBossDeath();

				return true;
			}

			return false;
		}

		public override void GrantVictoryBonuses()
		{
			int damageDealtToBoss = Health - CurrentHealth;
			// [PRERELEASE]
			int experienceGained = 10;
			User.Instance.CurrentHero.GainExperience(experienceGained);

			// Grant boss loot.
			// 1. Check % threshold for reward loot frequencies ("5-" is for inverting 0 -> full hp, 5 -> boss died).
			int threshold = 5 - CurrentHealth / (Health / 5);
			// 2. Iterate through every possible loot.
			string lootText = "Experience gained: " + experienceGained + " \n" + "Loot: \n";

			foreach (var loot in BossLoot)
			{
				double randomizedValue = RNG.Next(1, 10001) / 10000d;
				if (randomizedValue < loot.Frequencies[threshold])
				{
					// Grant loot after checking if it's not empty.
					if (loot.Item.Id != 0)
					{
						loot.Item.AddItem();
						lootText += "- " + loot.Item.Name + " (" + loot.ItemType + ")\n";
					}
				}
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