using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.UserInterface.Helpers;
using ClickQuest.Game.UserInterface.Windows;
using static ClickQuest.Game.Helpers.RandomnessHelper;

namespace ClickQuest.Game.Models;

public class Hero : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;

	private const int MaxLevel = 100;
	public const double AuraSpeedPerLevel = 0.004;
	public const double AuraSpeedBase = 1;
	public const double AuraDamageBase = 0.084;

	public int Id { get; set; }

	public int Experience { get; set; }

	public int ExperienceToNextLvl { get; set; }

	public int ExperienceToNextLvlTotal { get; set; }

	public int ExperienceProgress { get; set; }

	public DateTime SessionStartDate { get; set; }

	public int ClickDamagePerLevel { get; set; }
	public double CritChancePerLevel { get; set; }
	public int PoisonDamagePerLevel { get; set; }
	public string Name { get; set; }
	public HeroRace HeroRace { get; set; }
	public HeroClass HeroClass { get; set; }
	public int ClickDamage { get; set; }
	public double CritChance { get; set; }
	public double CritDamage { get; set; }
	public int PoisonDamage { get; set; }
	public int Level { get; set; }
	public List<Material> Materials { get; set; }
	public List<Recipe> Recipes { get; set; }
	public List<Artifact> Artifacts { get; set; }
	public List<Artifact> EquippedArtifacts { get; set; }
	public List<Quest> Quests { get; set; }
	public Blessing Blessing { get; set; }
	public Specializations Specializations { get; set; }
	public List<ArtifactSet> ArtifactSets { get; set; }
	public int CurrentArtifactSetId { get; set; }

	[JsonIgnore]
	public TimeSpan TimePlayed { get; set; }

	public string TimePlayedString { get; set; }

	public double AuraDamage { get; set; }
	public double AuraAttackSpeed { get; set; }

	public string ThisHeroClass => HeroClass.ToString();

	public string ThisHeroRace => HeroRace.ToString();

	public string CritChanceText
	{
		get
		{
			var critChanceText = (Math.Clamp(CritChance, 0, 1) * 100).ToString("0.##");
			critChanceText += "%";
			return critChanceText;
		}
	}

	public string CritDamageText
	{
		get
		{
			var critDamageText = Math.Floor(CritDamage * 100).ToString();
			critDamageText += "%";
			return critDamageText;
		}
	}

	public int ClickDamageBase => 10;

	public int LevelDamageBonus => ClickDamagePerLevel * Level;

	public int LevelDamageBonusTotal => ClickDamagePerLevel * Level + ClickDamageBase;

	public double LevelCritBonus => Math.Round(CritChancePerLevel * Level * 100, 2);

	public double LevelCritBonusTotal => Math.Round(CritChancePerLevel * Level * 100 + 25, 2);

	public int LevelPoisonBonus => PoisonDamagePerLevel * Level;

	public int LevelPoisonBonusTotal => PoisonDamagePerLevel * Level + 1;

	public string AuraDamageText
	{
		get
		{
			var auraDamageText = (AuraDamage * 100).ToString("0.##");
			auraDamageText += "%";
			return auraDamageText;
		}
	}

	public string AuraSpeedText => AuraAttackSpeed.ToString("0.###");

	public string AuraDpsText
	{
		get
		{
			var auraDps = (Math.Ceiling(AuraDamage * AuraAttackSpeed * 10000) / 100).ToString("0.##");
			auraDps += "%";
			return auraDps;
		}
	}

	public double LevelAuraBonus => AuraSpeedPerLevel * Level;

	public string LevelAuraBonusText => LevelAuraBonus.ToString("0.###");

	public double LevelAuraBonusTotal => AuraSpeedBase + AuraSpeedPerLevel * Level;

	public string LevelAuraBonusTotalText => LevelAuraBonusTotal.ToString("0.###");

	public Hero(HeroClass heroClass, HeroRace heroRace, string heroName)
	{
		Materials = new List<Material>();
		Recipes = new List<Recipe>();
		Artifacts = new List<Artifact>();
		EquippedArtifacts = new List<Artifact>();
		Quests = new List<Quest>();

		Specializations = new Specializations();

		ArtifactSets = new List<ArtifactSet>
		{
			new ArtifactSet
			{
				Id = 0,
				Name = "Default set"
			}
		};

		HeroClass = heroClass;
		HeroRace = heroRace;
		Experience = 0;
		Level = 0;
		Name = heroName;
		ClickDamagePerLevel = 1;
		AuraDamage = 0.084;
		CritDamage = 2.0;
		AuraAttackSpeed = AuraSpeedBase;

		Id = ++User.Instance.LastHeroId;

		SetClassSpecificValues();
		RefreshHeroExperience();
	}

	public Hero()
	{
		RefreshHeroExperience();
	}

	public void RefreshHeroExperience()
	{
		// Updates hero experience to make sure panels are updated at startup.
		ExperienceToNextLvl = ExperienceHelper.CalculateXpToNextLvl(this);
		ExperienceToNextLvlTotal = Experience + ExperienceToNextLvl;
		ExperienceProgress = ExperienceHelper.CalculateXpProgress(this);
	}

	public void UpdateTimePlayed()
	{
		if (SessionStartDate != default)
		{
			TimePlayed += DateTime.Now - SessionStartDate;
			SessionStartDate = default;
		}
	}

	public void GrantLevelUpBonuses()
	{
		if (Level < MaxLevel)
		{
			// Class specific bonuses and hero stats panel update.
			switch (HeroClass)
			{
				case HeroClass.Slayer:
					ClickDamage += ClickDamagePerLevel;
					CritChance += CritChancePerLevel;
					AuraAttackSpeed += AuraSpeedPerLevel;

					break;

				case HeroClass.Venom:
					ClickDamage += ClickDamagePerLevel;
					PoisonDamage += PoisonDamagePerLevel;
					AuraAttackSpeed += AuraSpeedPerLevel;

					break;
			}
		}
	}

	public void SetClassSpecificValues()
	{
		switch (HeroClass)
		{
			case HeroClass.Slayer:
				ClickDamage = 10;
				CritChance = 0.25;
				CritChancePerLevel = 0.004;
				PoisonDamage = 0;
				PoisonDamagePerLevel = 0;
				break;

			case HeroClass.Venom:
				ClickDamage = 10;
				CritChance = 0;
				CritChancePerLevel = 0;
				PoisonDamage = 1;
				PoisonDamagePerLevel = 2;
				break;
		}
	}

	public void ResumeBlessing()
	{
		// Resume blessings (if there are any left) - used when user selects a hero.
		Blessing?.EnableBuff();
	}

	public void PauseBlessing()
	{
		// Pause current blessings - used when user exits the game or returns to main menu page.
		Blessing?.DisableBuff();
	}

	public void RemoveBlessing()
	{
		if (User.Instance.CurrentHero.Blessing is not null)
		{
			User.Instance.CurrentHero.Blessing.DisableBuff();
			InterfaceHelper.RefreshBlessingInterfaceOnCurrentPage(User.Instance.CurrentHero.Blessing.Type);
			User.Instance.CurrentHero.Blessing = null;
		}
	}

	public void ResumeQuest()
	{
		Quests.FirstOrDefault(x => x.EndDate != default)?.StartQuest();
	}

	public void LoadQuests()
	{
		// Clone hero's quests using those from Database - rewards are not stored in Entity.
		for (var i = 0; i < Quests.Count; i++)
		{
			var heroQuest = Quests[i];
			var databaseQuest = GameAssets.Quests.FirstOrDefault(x => x.Id == heroQuest.Id);

			Quests[i] = databaseQuest.CopyQuest();

			// CopyQuest sets EndDate from GameAssets, so we need to get EndDate from Entity instead.
			Quests[i].EndDate = heroQuest.EndDate;
		}
	}

	public void ReequipArtifacts()
	{
		foreach (var equippedArtifact in EquippedArtifacts)
		{
			equippedArtifact.ArtifactFunctionality.OnEquip();
		}
	}

	public void UnequipArtfacts()
	{
		// Trigger OnUnequip, OnRegionLeave and similar artifact effects.
		// This is done so that stacking effects are not retained permanently.
		foreach (var equippedArtifact in EquippedArtifacts)
		{
			equippedArtifact.ArtifactFunctionality.OnUnequip();
			equippedArtifact.ArtifactFunctionality.OnRegionLeave();
		}
	}

	public (int Damage, DamageType DamageType) CalculateBaseAndCritClickDamage()
	{
		var damage = ClickDamage;
		var damageType = DamageType.Normal;

		// Calculate crit (max 100%).
		var randomizedValue = Rng.Next(1, 101) / 100d;
		if (randomizedValue <= CritChance)
		{
			damage = (int)(damage * CritDamage);
			damageType = DamageType.Critical;

			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.CritsAmount, 1);
		}

		return (damage, damageType);
	}

	public void GainExperience(int experienceGained)
	{
		foreach (var artifact in User.Instance.CurrentHero.EquippedArtifacts)
		{
			artifact.ArtifactFunctionality.OnExperienceGained(ref experienceGained);
		}

		if (ExperienceToNextLvl != 0)
		{
			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.ExperienceGained, experienceGained);
		}

		(Application.Current.MainWindow as GameWindow).CreateFloatingTextUtility($"+{experienceGained}", (SolidColorBrush)Application.Current.FindResource("BrushExperienceRelated"), FloatingTextHelper.ExperiencePositionPoint);

		Experience += experienceGained;
		ExperienceHelper.CheckIfLeveledUpAndGrantBonuses(this);
		ExperienceToNextLvl = ExperienceHelper.CalculateXpToNextLvl(this);
		ExperienceToNextLvlTotal = Experience + ExperienceToNextLvl;
		ExperienceProgress = ExperienceHelper.CalculateXpProgress(this);
	}
}