using ClickQuest.Account;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace ClickQuest.Items
{
	public enum BlessingType
	{
		ClickDamage = 0
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
				string str = Rarity.ToString() + ' ';

				for (int i = 0; i < (int)Rarity; i++)
				{
					str += "✩";
				}

				return str;
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

						foreach (var hero in User.Instance.Heroes)
						{
							hero.ClickDamage += Buff;
						}

						_timer = new DispatcherTimer();
						_timer.Interval = new TimeSpan(0, 0, 1);
						_timer.Tick += Timer_Tick;
						_timer.Start();

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

						foreach (var hero in User.Instance.Heroes)
						{
							hero.ClickDamage -= Buff;
						}

						break;
				}
			}
		}

		private void Timer_Tick(object source, EventArgs e)
		{
			Duration--;

			if (Duration <= 0)
			{
				// End the blessing.
				ChangeBuffStatus(false);

				// Remove it from User and Database (if it's saved there).
				User.Instance.Blessings.Remove(this);
				Entity.EntityOperations.RemoveBlessing(this);
			}
		}

		public static void ResumeBlessings()
		{
			// Resume blessings (if there are any left) - used when user selects a hero.
			foreach (var blessing in User.Instance.Blessings)
			{
				blessing.ChangeBuffStatus(true);
			}
		}

		public static void PauseBlessings()
		{
			// Pause current blessings and save them to the database - used when user exits the game or returns to main menu page.
			for (int i = 0; i < User.Instance.Blessings.Count; i++)
			{
				User.Instance.Blessings[0].ChangeBuffStatus(false);
			}
		}
	}
}