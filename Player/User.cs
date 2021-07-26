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
		public event PropertyChangedEventHandler PropertyChanged;

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
			}
		}

		private int _gold;
		public const int HERO_LIMIT = 6;

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public List<Hero> Heroes{ get; set; }

		public List<Ingot> Ingots{ get; set; }

		public List<DungeonKey> DungeonKeys{ get; set; }

		[NotMapped]
		public Hero CurrentHero{ get; set; }

		public static DateTime SessionStartDate { get; set; }

		public int Gold
		{
			get
			{
				return _gold;
			}
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
				
			}
		}

		public Achievements Achievements{ get; set; }

		public User()
		{
			Heroes = new List<Hero>();
			Ingots = new List<Ingot>();
			DungeonKeys = new List<DungeonKey>();
			Achievements = new Achievements();
		}
	}
}