using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using ClickQuest.Game.Core.Heroes;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.UserInterface.Controls.Styles.Themes;

namespace ClickQuest.Game.Core.Player;

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
		set => _instance = value;
	}

	[JsonIgnore]
	public Hero CurrentHero { get; set; }

	public int LastHeroId { get; set; }

	private int _gold;
	public const int HeroLimit = 6;
	public List<Hero> Heroes { get; set; }
	public List<Ingot> Ingots { get; set; }
	public List<DungeonKey> DungeonKeys { get; set; }
	public static DateTime SessionStartDate { get; set; }
	public Achievements Achievements { get; set; }

	public ColorTheme Theme { get; set; }

	public int Gold
	{
		get => _gold;
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

	public User()
	{
		Heroes = new List<Hero>();
		Ingots = new List<Ingot>();
		DungeonKeys = new List<DungeonKey>();
		Achievements = new Achievements();
		Theme = ColorTheme.Blue;
	}
}