using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Adventures;
using ClickQuest.Data.GameData;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Heroes.Buffs;
using ClickQuest.Items;
using ClickQuest.Player;
using static ClickQuest.Extensions.RandomnessManager.RandomnessController;

namespace ClickQuest.Heroes
{
	public class Hero : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private const int MAX_LEVEL = 100;
		public const double AURA_SPEED_PER_LEVEL = 0.01;
		public const double AURA_SPEED_BASE = 1;

		public int Id { get; set; }

		public int Experience { get; set;}
		
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
		public Specialization Specialization { get; set; }
		
		[JsonIgnore]
		public TimeSpan TimePlayed { get; set; }
		public string TimePlayedString { get; set; }
		
		public double AuraDamage { get; set; }
		public double AuraAttackSpeed { get; set; }


		[JsonIgnore]
		public string ThisHeroClass
		{
			get
			{
				return HeroClass.ToString();
			}
		}

		[JsonIgnore]
		public string ThisHeroRace
		{
			get
			{
				return HeroRace.ToString();
			}
		}

		[JsonIgnore]
		public string CritChanceText
		{
			get
			{
				string critChanceText = (Math.Clamp(CritChance, 0, 1) * 100).ToString("0.##");
				critChanceText += "%";
				return critChanceText;
			}
		}

		[JsonIgnore]
		public string CritDamageText
		{
			get
			{
				string critDamageText = Math.Floor(CritDamage * 100).ToString() + "%";
				return critDamageText;
			}
		}

		[JsonIgnore]
		public int LevelDamageBonus
		{
			get
			{
				return ClickDamagePerLevel * Level;
			}
		}

		[JsonIgnore]
		public int LevelDamageBonusTotal
		{
			get
			{
				return ClickDamagePerLevel * Level + 2;
			}
		}

		[JsonIgnore]
		public double LevelCritBonus
		{
			get
			{
				return Math.Round(CritChancePerLevel * Level * 100, 2);
			}
		}

		[JsonIgnore]
		public double LevelCritBonusTotal
		{
			get
			{
				return Math.Round(CritChancePerLevel * Level * 100 + 25, 2);
			}
		}

		[JsonIgnore]
		public int LevelPoisonBonus
		{
			get
			{
				return PoisonDamagePerLevel * Level;
			}
		}

		[JsonIgnore]
		public int LevelPoisonBonusTotal
		{
			get
			{
				return PoisonDamagePerLevel * Level + 1;
			}
		}

		[JsonIgnore]
		public string AuraDamageText
		{
			get
			{
				string auraDamageText = (AuraDamage * 100).ToString("0.##");
				auraDamageText += "%";
				return auraDamageText;
			}
		}

		[JsonIgnore]
		public string AuraDpsText
		{
			get
			{
				string auraDps = (Math.Round(AuraDamage * AuraAttackSpeed, 4) * 100).ToString("0.##");
				auraDps += "%";
				return auraDps;
			}
		}

		[JsonIgnore]
		public double LevelAuraBonus
		{
			get
			{
				return AURA_SPEED_PER_LEVEL * Level;
			}
		}


		public Hero(HeroClass heroClass, HeroRace heroRace, string heroName)
		{
			Materials = new List<Material>();
			Recipes = new List<Recipe>();
			Artifacts = new List<Artifact>();
			EquippedArtifacts = new List<Artifact>();
			Quests = new List<Quest>();

			Specialization = new Specialization();

			HeroClass = heroClass;
			HeroRace = heroRace;
			Experience = 0;
			Level = 0;
			Name = heroName;
			ClickDamagePerLevel = 1;
			AuraDamage = 0.1;
			CritDamage = 2.0;
			AuraAttackSpeed = AURA_SPEED_BASE;

			Id = User.Instance.Heroes.Select(x => x.Id).OrderByDescending(y => y).FirstOrDefault() + 1;

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
			ExperienceToNextLvl = Heroes.Experience.CalculateXpToNextLvl(this);
			ExperienceToNextLvlTotal = Experience + ExperienceToNextLvl;
			ExperienceProgress = Heroes.Experience.CalculateXpProgress(this);
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
			if (Level < MAX_LEVEL)
			{
				// Class specific bonuses and hero stats panel update.
				switch (HeroClass)
				{
					case HeroClass.Slayer:
						ClickDamage += ClickDamagePerLevel;
						CritChance += CritChancePerLevel;
						AuraAttackSpeed += AURA_SPEED_PER_LEVEL;

						break;

					case HeroClass.Venom:
						ClickDamage += ClickDamagePerLevel;
						PoisonDamage += PoisonDamagePerLevel;
						AuraAttackSpeed += AURA_SPEED_PER_LEVEL;

						break;
				}
			}
		}

		public void SetClassSpecificValues()
		{
			switch (HeroClass)
			{
				case HeroClass.Slayer:
					ClickDamage = 2;
					CritChance = 0.25;
					CritChancePerLevel = 0.004;
					PoisonDamage = 0;
					PoisonDamagePerLevel = 0;
					break;

				case HeroClass.Venom:
					ClickDamage = 2;
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
			User.Instance.CurrentHero.Blessing?.DisableBuff();
			User.Instance.CurrentHero.Blessing = null;
		}

		public void ResumeQuest()
		{
			Quests.FirstOrDefault(x => x.EndDate != default)?.StartQuest();
		}

		public void LoadQuests()
		{
			// Clone hero's quests using those from Database - rewards are not stored in Entity.
			for (int i = 0; i < Quests.Count; i++)
			{
				var heroQuest = Quests[i];
				var databaseQuest = GameData.Quests.FirstOrDefault(x => x.Id == heroQuest.Id);

				Quests[i] = databaseQuest.CopyQuest();

				// CopyQuest sets EndDate from GameData, so we need to get EndDate from Entity instead.
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
			foreach (var equippedArtifact in EquippedArtifacts)
			{
				equippedArtifact.ArtifactFunctionality.OnUnequip();
			}
		}

		public (int Damage, DamageType DamageType) CalculateBaseAndCritClickDamage()
		{
			int damage = ClickDamage;
			var damageType = DamageType.Normal;

			// Calculate crit (max 100%).
			double randomizedValue = RNG.Next(1, 101) / 100d;
			if (randomizedValue <= CritChance)
			{
				damage = (int) (damage * CritDamage);
				damageType = DamageType.Critical;

				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.CritsAmount, 1);
			}

			return (damage, damageType);
		}

		public void GainExperience(int value, bool isTriggeredFromOnExperienceGained = false)
		{
			int experienceGained = value;

			if (ExperienceToNextLvl != 0)
			{
				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.ExperienceGained, experienceGained);
			}

			Experience += value;
			Heroes.Experience.CheckIfLeveledUpAndGrantBonuses(this);
			ExperienceToNextLvl = Heroes.Experience.CalculateXpToNextLvl(this);
			ExperienceToNextLvlTotal = Experience + ExperienceToNextLvl;
			ExperienceProgress = Heroes.Experience.CalculateXpProgress(this);

			if (!isTriggeredFromOnExperienceGained)
			{
				foreach (var artifact in User.Instance.CurrentHero.EquippedArtifacts)
				{
					artifact.ArtifactFunctionality.OnExperienceGained(experienceGained);
				}
			}
		}
	}
}