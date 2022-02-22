using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.DataTypes.Structs;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models.Functionalities.Artifacts;
using ClickQuest.Game.UserInterface.Helpers;
using ClickQuest.Game.UserInterface.Pages;
using ClickQuest.Game.UserInterface.Windows;
using MaterialDesignThemes.Wpf;
using static ClickQuest.Game.Helpers.RandomnessHelper;

namespace ClickQuest.Game.Models;

public class Monster : Enemy
{
	public List<MonsterLootPattern> MonsterLootPatterns { get; set; }
	
	[JsonIgnore]
	public int SpawnRegionId { get; set; }

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
			MonsterLootPatterns = MonsterLootPatterns,
			SpawnRegionId = SpawnRegionId
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
		var experienceGained = ExperienceHelper.CalculateMonsterXpReward(this);
		User.Instance.CurrentHero.GainExperience(experienceGained);

		var selectedLoot = RandomizeMonsterLoot();

		if (selectedLoot != null)
		{
			(selectedLoot as Artifact)?.CreateMythicTag(Name);

			// AddItem starts loot animation.
			selectedLoot.AddItem(displayFloatingText: true);

			// Mark the corresponding Pattern as discovered.
			if (!GameAssets.BestiaryEntries.Any(x => x.EntryType == BestiaryEntryType.MonsterLoot && x.Id == selectedLoot.Id))
			{
				GameAssets.BestiaryEntries.Add(new BestiaryEntry
				{
					Id = selectedLoot.Id,
					RelatedEnemyId = this.Id,
					EntryType = BestiaryEntryType.MonsterLoot
				});
			}
		}

		CheckForDungeonKeyDrop();

		((GameAssets.CurrentPage as RegionPage).StatsFrame.Content as HeroStatsPage).RefreshAllDynamicStatsAndToolTips();
		CombatTimersHelper.UpdateAuraAttackSpeed();
	}

	public Item RandomizeMonsterLoot()
	{
		var frequencyList = MonsterLootPatterns.Select(x => x.Frequency).ToList();

		// Special case - if Cloak of the Woodlander is equipped, and player is currently in Woodland Cascades, increase rare item chances.
		var currentFrameContent = (Application.Current.MainWindow as GameWindow)?.CurrentFrame.Content;
		var isCurrentFrameContentARegion = currentFrameContent is RegionPage;
		
		if (isCurrentFrameContentARegion)
		{
			var isCurrentRegionWoodlandCascades = (currentFrameContent as RegionPage).Region.Name == "Woodland Cascades";
			var isCloakOfTheWoodlanderEquipped = User.Instance.CurrentHero.EquippedArtifacts.FirstOrDefault(x => x.ArtifactFunctionality is CloakOfTheWoodlander) != null;

			if (isCurrentRegionWoodlandCascades && isCloakOfTheWoodlanderEquipped)
			{
				frequencyList = CloakOfTheWoodlander.ModifyWoodlandCascadesLootFrequencies(frequencyList);
			}
		}
		
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