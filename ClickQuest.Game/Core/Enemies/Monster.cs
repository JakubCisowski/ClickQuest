using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Items.Patterns;
using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Collections;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Pages;
using ClickQuest.Game.UserInterface.Windows;
using static ClickQuest.Game.Extensions.Randomness.RandomnessController;

namespace ClickQuest.Game.Core.Enemies
{
	public class Monster : Enemy
	{
		public List<MonsterLootPattern> MonsterLootPatterns { get; set; }

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
			copy.MonsterLootPatterns = MonsterLootPatterns;

			return copy;
		}

		public override void HandleEnemyDeathIfDefeated()
		{
			if (CurrentHealth <= 0)
			{
				CombatTimerController.StopPoisonTimer();

				// Mark the Monster as discovered.
				if (!GameAssets.BestiaryEntries.Any(x=>x.EntryType == BestiaryEntryType.Monster && x.Id==this.Id))
				{
					GameAssets.BestiaryEntries.Add(new BestiaryEntry() { Id = this.Id, EntryType = BestiaryEntryType.Monster });
				}

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

			var selectedLoot = RandomizeMonsterLoot();

			if (selectedLoot != null)
			{
				(selectedLoot as Artifact)?.CreateMythicTag(Name);
				selectedLoot.AddItem();

				// Mark the corresponding Pattern as discovered.
				if (!GameAssets.BestiaryEntries.Any(x=>x.EntryType == BestiaryEntryType.MonsterLoot && x.Id==selectedLoot.Id))
				{
					GameAssets.BestiaryEntries.Add(new BestiaryEntry() { Id = selectedLoot.Id, EntryType = BestiaryEntryType.MonsterLoot });
				}
			}

			// [PRERELEASE] Display exp and loot for testing purposes.
			// (GameAssets.CurrentPage as RegionPage).TestRewardsBlock.Text = "Loot: " + selectedLoot?.Name + ", Base Exp: " + experienceGained;
			(Application.Current.MainWindow as GameWindow).CreateFloatingTextLoot(selectedLoot);

			CheckForDungeonKeyDrop();

			((GameAssets.CurrentPage as RegionPage).StatsFrame.Content as HeroStatsPage).RefreshAllDynamicStatsAndToolTips();
			CombatTimerController.UpdateAuraAttackSpeed();
		}

		public Item RandomizeMonsterLoot()
		{
			var frequencyList = MonsterLootPatterns.Select(x => x.Frequency).ToList();
			double randomizedValue = RNG.Next(1, 10001) / 10000d;

			int i = 0;

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
}