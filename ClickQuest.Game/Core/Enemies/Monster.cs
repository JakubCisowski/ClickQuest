using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Items.Patterns;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Pages;
using MaterialDesignThemes.Wpf;
using static ClickQuest.Game.Extensions.Randomness.RandomnessController;

namespace ClickQuest.Game.Core.Enemies
{
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
			Monster copy = new Monster
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
				CombatTimerController.StopPoisonTimer();

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
				foreach (Artifact equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
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

			Item selectedLoot = RandomizeMonsterLoot();

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
						LootQueueController.AddToQueue(material.Name, material.Rarity, PackIconKind.Cog);
						break;

					case Recipe recipe:
						LootQueueController.AddToQueue(recipe.FullName, recipe.Rarity, PackIconKind.ScriptText);
						break;

					case Artifact artifact:
						LootQueueController.AddToQueue(artifact.Name, artifact.Rarity, PackIconKind.DiamondStone);
						break;
				}
			}

			CheckForDungeonKeyDrop();

			((GameAssets.CurrentPage as RegionPage).StatsFrame.Content as HeroStatsPage).RefreshAllDynamicStatsAndToolTips();
			CombatTimerController.UpdateAuraAttackSpeed();
		}

		public Item RandomizeMonsterLoot()
		{
			var frequencyList = MonsterLootPatterns.Select(x => x.Frequency).ToList();
			double randomizedValue = Rng.Next(1, 10001) / 10000d;

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
}