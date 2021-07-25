using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using ClickQuest.Heroes;
using ClickQuest.Items;

namespace ClickQuest.Player
{
	public class User : INotifyPropertyChanged
	{
		public User()
		{
			Heroes = new List<Hero>();
			Ingots = new List<Ingot>();
			DungeonKeys = new List<DungeonKey>();
			Achievements = new Achievements();
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		#endregion INotifyPropertyChanged

		#region Singleton

		private static User _instance;

		public static User Instance
		{
			get
			{
				if (_instance is null)
				{
					_instance = new User();
				}

				return _instance;
			}
			set
			{
				_instance = value;
				_instance.OnPropertyChanged();
			}
		}

		#endregion Singleton

		#region Private Fields

		private List<Hero> _heroes;
		private Hero _currentHero;
		private List<Ingot> _ingots;
		private List<DungeonKey> _dungeonKeys;
		private int _gold;
		private Achievements _achievements;
		public const int HERO_LIMIT = 6;

		#endregion Private Fields

		#region Properties

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public List<Hero> Heroes
		{
			get { return _heroes; }
			set
			{
				_heroes = value;
				OnPropertyChanged();
			}
		}

		public List<Ingot> Ingots
		{
			get { return _ingots; }
			set
			{
				_ingots = value;
				OnPropertyChanged();
			}
		}

		public List<DungeonKey> DungeonKeys
		{
			get { return _dungeonKeys; }
			set
			{
				_dungeonKeys = value;
				OnPropertyChanged();
			}
		}

		[NotMapped]
		public Hero CurrentHero
		{
			get { return _currentHero; }
			set
			{
				_currentHero = value;
				OnPropertyChanged();
			}
		}

		public static DateTime SessionStartDate { get; set; }

		public int Gold
		{
			get { return _gold; }
			set
			{
				if (value - _gold > 0)
				{
					// Increase achievement amount.
					Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.GoldEarned, value - _gold);
				}
				else
				{
					// Increase achievement amount.
					Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.GoldSpent, _gold - value);
				}

				_gold = value;
				OnPropertyChanged();
			}
		}

		public Achievements Achievements
		{
			get { return _achievements; }
			set
			{
				_achievements = value;
				OnPropertyChanged();
			}
		}

		#endregion Properties
	}
}