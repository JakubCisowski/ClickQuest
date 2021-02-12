﻿using ClickQuest.Items;
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
		private HeroClass _heroClass;
		private string _critChanceText;
		private List<Quest> _quests;

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

		public string CritChanceText
		{
			get
			{
				return _critChanceText;
			}
			set
			{
				_critChanceText = value;
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

		#endregion Properties

		public Hero(HeroClass heroClass, string heroName)
		{
			Quests = new List<Quest>();
			HeroClass = heroClass;
			Experience = 0;
			Level = 0;
			Name = heroName;
			ClickDamagePerLevel = 1;

			// Set class specific values.
			switch (heroClass)
			{
				case HeroClass.Slayer:
					ClickDamage = 2;
					CritChance = 0.25;
					CritChancePerLevel = 0.004;
					PoisonDamage = 0;
					PoisonDamagePerLevel = 0;
					CritChanceText = String.Format("{0:P1}", CritChance);
					break;

				case HeroClass.Venom:
					ClickDamage = 2;
					CritChance = 0;
					CritChancePerLevel = 0;
					PoisonDamage = 1;
					PoisonDamagePerLevel = 2;
					CritChanceText = String.Format("{0:P1}", CritChance);
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

		public void GrantLevelUpBonuses()
		{
			if (Level == 100)
			{
				// Set tooltips once and never set them again after lvl 100.
				switch (_heroClass)
				{
					case HeroClass.Slayer:
						CritChanceText = String.Format("{0:P1}", CritChance);
						break;

					case HeroClass.Venom:
						CritChanceText = String.Format("{0:P1}", CritChance);
						break;
				}
			}
			else if (Level < 100)
			{
				// Class specific bonuses and hero stats panel update.
				switch (_heroClass)
				{
					case HeroClass.Slayer:
						ClickDamage += ClickDamagePerLevel;
						CritChance += CritChancePerLevel;
						CritChanceText = String.Format("{0:P1}", CritChance);
						break;

					case HeroClass.Venom:
						ClickDamage += ClickDamagePerLevel;
						PoisonDamage += PoisonDamagePerLevel;
						CritChanceText = String.Format("{0:P1}", CritChance);
						break;
				}
			}
		}
	}
}