using ClickQuest.Player;
using ClickQuest.Data;
using ClickQuest.Windows;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace ClickQuest.Items
{
	public enum BlessingType
	{
		ClickDamage = 0, CritChance, PoisonDamage, AuraDamage, AuraSpeed
	}

	public partial class Blessing : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		#endregion INotifyPropertyChanged

		#region Private Fields

		private int _id;
		private string _name;
		private string _description;
		private Rarity _rarity;
		public BlessingType _type;
		private int _value;
		private int _duration;
		private string _durationText;
		private int _buff;
		private DispatcherTimer _timer;

		#endregion Private Fields

		#region Properties

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int DbKey { get; set; }

		public int Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
				OnPropertyChanged();
			}
		}

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

		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
				OnPropertyChanged();
			}
		}

		public Rarity Rarity
		{
			get
			{
				return _rarity;
			}
			set
			{
				_rarity = value;
				OnPropertyChanged();
			}
		}

		public int Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				OnPropertyChanged();
			}
		}

		public BlessingType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
				OnPropertyChanged();
			}
		}

		public int Duration
		{
			get
			{
				return _duration;
			}
			set
			{
				_duration = value;
				OnPropertyChanged();
			}
		}

		[NotMapped]
		public string DurationText
		{
			get
			{
				return _durationText;
			}
			set
			{
				_durationText = value;
				OnPropertyChanged();
			}
		}

		public int Buff
		{
			get
			{
				return _buff;
			}
			set
			{
				_buff = value;
				OnPropertyChanged();
			}
		}

		public string TypeString
		{
			get
			{
				return Type.ToString();
			}
		}

		public string RarityString
		{
			get
			{
				return Rarity.ToString();
			}
		}

		#endregion Properties

		// Copy constructor
		public Blessing(Blessing blessing)
		{
			Id = blessing.Id;
			Name = blessing.Name;
			Type = blessing.Type;
			Rarity = blessing.Rarity;
			Duration = blessing.Duration;
			Description = blessing.Description;
			Buff = blessing.Buff;
			Value = blessing.Value;
		}

		public Blessing(int id, string name, BlessingType type, Rarity rarity, int duration, string description, int buff, int value)
		{
			Id = id;
			Name = name;
			Type = type;
			Rarity = rarity;
			Duration = duration;
			Description = description;
			Buff = buff;
			Value = value;
		}

		public void ChangeBuffStatus(bool add)
		{
			// Assign buff to every hero.
			if (add)
			{
				switch (Type)
				{
					case BlessingType.ClickDamage:

						// Remove all blessings except the last one added.
						for (var i = 0; i < User.Instance.CurrentHero.Blessings.Count - 1; i++)
						{
							var bless = User.Instance.CurrentHero.Blessings[i];

							bless.ChangeBuffStatus(false);
							User.Instance.CurrentHero.Blessings.Remove(bless);
							Entity.EntityOperations.RemoveBlessing(bless);
						}

						// Increase achievement amount.
						if (Duration == Database.Blessings.FirstOrDefault(x=>x.Id==this.Id).Duration)
						{
							User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.BlessingsUsed]++;
							AchievementsWindow.Instance.UpdateAchievements();
						}
						
						// Assign buff.
						User.Instance.CurrentHero.ClickDamage += Buff;

						// Start timer.
						_timer = new DispatcherTimer
						{
							Interval = new TimeSpan(0, 0, 1)
						};
						_timer.Tick += Timer_Tick;
						_timer.Start();

						// Set duration text for hero stats panel.
						DurationText = $"{Name}\n{Duration / 60}m {Duration % 60}s";

						// Refresh all stats panel bindings.
						foreach (var page in Database.Pages.Skip(2))
						{
							dynamic p = page.Value;
							p.StatsFrame.Refresh();
						}

						break;
				}
			}
			// Cancel buff.
			else
			{
				switch (Type)
				{
					case BlessingType.ClickDamage:

						_timer.Stop();

						// Cancel buff.
						User.Instance.CurrentHero.ClickDamage -= Buff;

						break;
				}
			}
		}

		private void Timer_Tick(object source, EventArgs e)
		{
			Duration--;
			DurationText = $"{Name}\n{Duration / 60}m {Duration % 60}s";

			if (Duration <= 0)
			{
				// End the blessing.
				ChangeBuffStatus(false);

				// Remove it from User and Database (if it's saved there).
				User.Instance.CurrentHero.Blessings.Remove(this);
				Entity.EntityOperations.RemoveBlessing(this);

				// Reset DurationText.
				DurationText = "";

				// Refresh all stats panel bindings.
				foreach (var page in Database.Pages.Skip(2))
				{
					dynamic p = page.Value;
					p.StatsFrame.Refresh();
				}
			}
		}

		public static void ResumeBlessings()
		{
			// Resume blessings (if there are any left) - used when user selects a hero.
			foreach (var blessing in User.Instance.CurrentHero.Blessings)
			{
				blessing.ChangeBuffStatus(true);
			}
		}

		public static void PauseBlessings()
		{
			// Pause current blessings and save them to the database - used when user exits the game or returns to main menu page.
			if (User.Instance.CurrentHero != null)
			{
				for (int i = 0; i < User.Instance.CurrentHero.Blessings.Count; i++)
				{
					User.Instance.CurrentHero.Blessings[i].ChangeBuffStatus(false);
				}
			}
		}
	}
}