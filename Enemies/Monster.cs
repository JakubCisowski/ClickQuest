using System.Collections.Generic;
using System.Linq;
using ClickQuest.Data.GameData;
using ClickQuest.Extensions.CollectionsManager;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Heroes;
using ClickQuest.Items;
using ClickQuest.Pages;
using ClickQuest.Player;

namespace ClickQuest.Enemies
{
	public class Monster : Enemy
	{
		public List<MonsterLootPattern> Loot { get; set; }

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
			var copy = new Monster();

			copy.Id = Id;
			copy.Name = Name;
			copy.Health = Health;
			copy.CurrentHealth = Health;
			copy.Description = Description;
			copy.CurrentHealthProgress = CurrentHealthProgress;
			copy.Loot = Loot;

			return copy;
		}

		public override void HandleEnemyDeathIfDefeated()
		{
			if (CurrentHealth <= 0)
			{
				CombatTimerController.StopPoisonTimer();

				GrantVictoryBonuses();

				// Invoke Artifacts with the "on-death" effect.
				foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
				{
					equippedArtifact.ArtifactFunctionality.OnKill();
				}

				InterfaceController.CurrentMonsterButton?.SpawnMonster();
			}
		}

		public override void GrantVictoryBonuses()
		{
			int experienceGained = Experience.CalculateMonsterXpReward(Health);
			User.Instance.CurrentHero.GainExperience(experienceGained);

			var frequencyList = Loot.Select(x => x.Frequency).ToList();
			int position = CollectionsController.RandomizeFreqenciesListPosition(frequencyList);
			var selectedLoot = Loot[position].Item;

			if (selectedLoot.Id != 0)
			{
				selectedLoot.AddItem();
			}

			// [PRERELEASE] Display exp and loot for testing purposes.
			(GameData.CurrentPage as RegionPage).TestRewardsBlock.Text = "Loot: " + selectedLoot.Name + ", Base Exp: " + experienceGained;

			CheckForDungeonKeyDrop();

			(GameData.CurrentPage as RegionPage).StatsFrame.Refresh();
			CombatTimerController.UpdateAuraAttackSpeed();
		}
	}
}