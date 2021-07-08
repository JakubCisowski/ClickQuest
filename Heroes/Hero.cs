using ClickQuest.Player;
using ClickQuest.Items;
using ClickQuest.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace ClickQuest.Heroes
{
	public class Hero : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		#endregion INotifyPropertyChanged

		#region Private Fields

		private string _name;
		private int _experience;
		private int _experienceToNextLvl;
		private int _experienceProgress;
		private int _experienceToNextLvlTotal;
		private int _level;
		private int _clickDamage;
		private double _critChance;
		private int _poisonDamage;
		private HeroRace _heroRace;
		private HeroClass _heroClass;
		private List<Material> _materials;
		private List<Recipe> _recipes;
		private List<Artifact> _artifacts;
		private List<Quest> _quests;
		private List<Blessing> _blessings;
		private Specialization _specialization;
		private double _auraDamage;
		private double _auraAttackSpeed;
		// Specialisations/Professions

		#endregion Private Fields

		#region Properties

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public int ClickDamagePerLevel { get; set; }
		public double CritChancePerLevel { get; set; }
		public int PoisonDamagePerLevel { get; set; }

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				OnPropertyChanged();
			}
		}

		public int Experience
		{
			get
			{
				return _experience;
			}
			set
			{
				_experience = value;
				Heroes.Experience.CheckIfLeveledUp(this);
				ExperienceToNextLvl = Heroes.Experience.CalculateXpToNextLvl(this);
				ExperienceToNextLvlTotal = Experience + ExperienceToNextLvl;
				ExperienceProgress = Heroes.Experience.CalculateXpProgress(this);
				OnPropertyChanged();
			}
		}

		[NotMapped]
		public int ExperienceToNextLvl
		{
			get
			{
				return _experienceToNextLvl;
			}
			set
			{
				_experienceToNextLvl = value;
				OnPropertyChanged();
			}
		}

		[NotMapped]
		public int ExperienceToNextLvlTotal
		{
			get
			{
				return _experienceToNextLvlTotal;
			}
			set
			{
				_experienceToNextLvlTotal = value;
				OnPropertyChanged();
			}
		}

		[NotMapped]
		public int ExperienceProgress
		{
			get
			{
				return _experienceProgress;
			}
			set
			{
				_experienceProgress = value;
				OnPropertyChanged();
			}
		}
		public HeroRace HeroRace
		{
			get
			{
				return _heroRace;
			}
			set
			{
				_heroRace = value;
				OnPropertyChanged();
			}
		}
		public HeroClass HeroClass
		{
			get
			{
				return _heroClass;
			}
			set
			{
				_heroClass = value;
				OnPropertyChanged();
			}
		}

		public int ClickDamage
		{
			get
			{
				return _clickDamage;
			}
			set
			{
				_clickDamage = value;
				OnPropertyChanged();
			}
		}

		public double CritChance
		{
			get
			{
				return _critChance;
			}
			set
			{
				_critChance = value;
				OnPropertyChanged();
			}
		}

		public int PoisonDamage
		{
			get
			{
				return _poisonDamage;
			}
			set
			{
				_poisonDamage = value;
				OnPropertyChanged();
			}
		}

		public int Level
		{
			get
			{
				return _level;
			}
			set
			{
				_level = value;
				OnPropertyChanged();
			}
		}

		public string ThisHeroClass
		{
			get
			{
				return _heroClass.ToString();
			}
		}
		public string ThisHeroRace
		{
			get
			{
				return _heroRace.ToString();
			}
		}

		public string CritChanceText
		{
			get
			{	
				var critChanceText = string.Format("{0:P1}", CritChance > 1 ? 1 : CritChance);
				return critChanceText[critChanceText.Length - 2] == '0' ? critChanceText.Remove(critChanceText.Length - 3, 2) : critChanceText;
			}
		}
		public List<Material> Materials
		{
			get
			{
				return _materials;
			}
			set
			{
				_materials = value;
				OnPropertyChanged();
			}
		}
		public List<Recipe> Recipes
		{
			get
			{
				return _recipes;
			}
			set
			{
				_recipes = value;
				OnPropertyChanged();
			}
		}
		public List<Artifact> Artifacts
		{
			get
			{
				return _artifacts;
			}
			set
			{
				_artifacts = value;
				OnPropertyChanged();
			}
		}

		public List<Quest> Quests
		{
			get
			{
				return _quests;
			}
			set
			{
				_quests = value;
				OnPropertyChanged();
			}
		}
		public List<Blessing> Blessings
		{
			get
			{
				return _blessings;
			}
			set
			{
				_blessings = value;
				OnPropertyChanged();
			}
		}
		public Specialization Specialization
		{
			get
			{
				return _specialization;
			}
			set
			{
				_specialization = value;
				OnPropertyChanged();
			}
		}
		public int LevelDamageBonus
		{
			get
			{
				return ClickDamagePerLevel * Level;
			}
		}
		public int LevelDamageBonusTotal
		{
			get
			{
				return ClickDamagePerLevel * Level + 2;
			}
		}
		public double LevelCritBonus
		{
			get
			{
				return CritChancePerLevel * Level * 100;
			}
		}
		public double LevelCritBonusTotal
		{
			get
			{
				return CritChancePerLevel * Level * 100 + 25;
			}
		}
		public int LevelPoisonBonus
		{
			get
			{
				return PoisonDamagePerLevel * Level;
			}
		}
		public int LevelPoisonBonusTotal
		{
			get
			{
				return PoisonDamagePerLevel * Level + 1;
			}
		}
		public TimeSpan TimePlayed{ get; set;}

		[NotMapped]
		public DateTime SessionStartDate{ get; set;}
		public double AuraDamage
		{
			get
			{
				return _auraDamage;
			}
			set
			{
				_auraDamage = value;
				OnPropertyChanged();
			}
		}
		public double AuraAttackSpeed
		{
			get
			{
				return _auraAttackSpeed;
			}
			set
			{
				_auraAttackSpeed = value;
				OnPropertyChanged();
			}
		}
		public string AuraDamageText
		{
			get
			{
				var auraDamageText = string.Format("{0:P1}", AuraDamage);
				return auraDamageText[auraDamageText.Length - 2] == '0' ? auraDamageText.Remove(auraDamageText.Length - 3, 2) : auraDamageText;
			}
		}
		public string AuraDps
		{
			get
			{
				var auraDps = string.Format("{0:P1}", Math.Round(_auraDamage * _auraAttackSpeed, 1));
				return auraDps[auraDps.Length - 2] == '0' ? auraDps.Remove(auraDps.Length - 3, 2) : auraDps;
			}
		}
		public double LevelAuraBonus
		{
			get
			{
				// AuraAttackSpeed per level * Level
				return 1 * Level;
			}
		}
		public double LevelAuraBonusTotal
		{
			get
			{
				// AuraAttackSpeed per level * Level + AuraBaseAttackSpeed
				return 1 * Level + 1;
			}
		}

		#endregion Properties

		public Hero(HeroClass heroClass, HeroRace heroRace, string heroName)
		{
			Materials = new List<Material>();
			Recipes = new List<Recipe>();
			Artifacts = new List<Artifact>();
			Quests = new List<Quest>();
			Blessings = new List<Blessing>();

			HeroClass = heroClass;
			HeroRace = heroRace;
			Experience = 0;
			Level = 0;
			Name = heroName;
			ClickDamagePerLevel = 1;
			AuraDamage = 0.1;
			AuraAttackSpeed = 1;

			// Set class specific values.
			switch (heroClass)
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

			UpdateHero();
		}

		public Hero()
		{
			UpdateHero();
		}

		public void UpdateHero()
		{
			// Updates hero experience to make sure panels are updated at startup.
			ExperienceToNextLvl = Heroes.Experience.CalculateXpToNextLvl(this);
			ExperienceToNextLvlTotal = Experience + ExperienceToNextLvl;
			ExperienceProgress = Heroes.Experience.CalculateXpProgress(this);
		}

		public void UpdateTimePlayed()
		{
			if (SessionStartDate!=default)
			{
				TimePlayed+= DateTime.Now - SessionStartDate;
				SessionStartDate=default(DateTime);
			}
		}

		public void GrantLevelUpBonuses()
		{
			if (Level == 100)
			{
				// // Set tooltips once and never set them again after lvl 100.
				// switch (_heroClass)
				// {
				// 	case HeroClass.Slayer:
				// 		break;

				// 	case HeroClass.Venom:
				// 		break;
				// }
			}
			else if (Level < 100)
			{
				// Class specific bonuses and hero stats panel update.
				switch (_heroClass)
				{
					case HeroClass.Slayer:
						ClickDamage += ClickDamagePerLevel;
						CritChance += CritChancePerLevel;
						
						break;

					case HeroClass.Venom:
						ClickDamage += ClickDamagePerLevel;
						PoisonDamage += PoisonDamagePerLevel;

						break;
				}
			}
		}
		public void AddItem(Item itemToAdd)
		{
			var type = itemToAdd.GetType();

			if (type == typeof(Recipe))
			{
				// Add to Recipes.

				foreach (var item in Recipes)
				{
					if (item.Id == itemToAdd.Id)
					{
						item.Quantity++;
						return;
					}
				}

				// If user doesn't have this item, clone and add it.
				var copy = new Recipe(itemToAdd);

				Recipes.Add(copy);
				copy.Quantity++;

				// Increase achievement amount.
				User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.RecipesGained]++;
				AchievementsWindow.Instance.UpdateAchievements();
			}
			else if (type == typeof(Artifact))
			{
				// Add to Artifacts.

				foreach (var item in Artifacts)
				{
					if (item.Id == itemToAdd.Id)
					{
						item.Quantity++;
						return;
					}
				}

				// If user doesn't have this item, add it.
				var copy = new Artifact(itemToAdd);

				Artifacts.Add(copy);
				copy.Quantity++;

				// Increase achievement amount.
				switch(itemToAdd.Rarity)
				{
					case Rarity.General:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.GeneralArtifactsGained]++;
						break;
					case Rarity.Fine:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.FineArtifactsGained]++; 
						break;
					case Rarity.Superior:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.SuperiorArtifactsGained]++;
						break;
					case Rarity.Exceptional:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.ExceptionalArtifactsGained]++;
						break;
					case Rarity.Mythic:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.MythicArtifactsGained]++;
						break;
					case Rarity.Masterwork:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.MasterworkArtifactsGained]++;
						break;
				}
				AchievementsWindow.Instance.UpdateAchievements();
			}
			else if (type == typeof(Material))
			{
				// Add to Materials.

				foreach (var item in Materials)
				{
					if (item.Id == itemToAdd.Id)
					{
						item.Quantity++;
						return;
					}
				}

				// If user doesn't have this item, add it.
				var copy = new Material(itemToAdd);

				Materials.Add(copy);
				copy.Quantity++;

				// Increase achievement amount.
				User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.MaterialsGained]++;
				AchievementsWindow.Instance.UpdateAchievements();
			}
		}

		public void RemoveItem(Item itemToRemove)
		{
			var type = itemToRemove.GetType();

			if (type == typeof(Recipe))
			{
				// Revmove from Recipes.

				foreach (var item in Recipes)
				{
					if (item.Id == itemToRemove.Id)
					{
						item.Quantity--;
						if (item.Quantity <= 0)
						{
							// Remove item from database.
							Entity.EntityOperations.RemoveItem(item);
						}
						return;
					}
				}
			}
			else if (type == typeof(Artifact))
			{
				// Revmove from Artifacts.

				foreach (var item in Artifacts)
				{
					if (item.Id == itemToRemove.Id)
					{
						item.Quantity--;
						if (item.Quantity <= 0)
						{
							// Remove item from database.
							Entity.EntityOperations.RemoveItem(item);
						}
						return;
					}
				}
			}
			else if (type == typeof(Material))
			{
				// Revmove from Materials.

				foreach (var item in Materials)
				{
					if (item.Id == itemToRemove.Id)
					{
						item.Quantity--;
						if (item.Quantity <= 0)
						{
							// Remove item from database.
							Entity.EntityOperations.RemoveItem(item);
						}
						return;
					}
				}
			}

			// If user doesn't have this item, don't do anything (check Item.Quantity).
		}
	}
}