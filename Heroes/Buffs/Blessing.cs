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
using ClickQuest.Extensions.InterfaceManager;
using Microsoft.EntityFrameworkCore;
using ClickQuest.Items;
using ClickQuest.Interfaces;

namespace ClickQuest.Heroes.Buffs
{
	public enum BlessingType
	{
		ClickDamage = 0, CritChance, PoisonDamage, AuraDamage, AuraSpeed
	}

	[Owned]
	public partial class Blessing : INotifyPropertyChanged, IIdentifiable
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
		private bool _achievementBonusGranted;
		private DispatcherTimer _timer;

		#endregion Private Fields

		#region Properties

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

		public bool AchievementBonusGranted
		{
			get
			{
				return _achievementBonusGranted;
			}
			set
			{
				_achievementBonusGranted = value;
				OnPropertyChanged();
			}
		}

		#endregion Properties
		
		public Blessing()
		{

		}

		public Blessing CopyBlessing()
		{
			Blessing copy = new Blessing();

			copy.Id = Id;
			copy.Name = Name;
			copy.Type = Type;
			copy.Rarity = Rarity;
			copy.Duration = Duration;
			copy.Description = Description;
			copy.Buff = Buff;
			copy.Value = Value;
			copy.AchievementBonusGranted = false;

			return copy;
		}

		public void CheckAndAddAchievementProgress()
		{
			if (!AchievementBonusGranted)
			{
				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.BlessingsUsed, 1);
				AchievementBonusGranted = true;
			}
		}

		public void EnableBuff()
		{
			// Increase hero stat.
			switch (Type)
			{
				case BlessingType.ClickDamage:
					User.Instance.CurrentHero.ClickDamage += Buff;
					break;
			}

			InitializeAndStartTimer();
			UpdateDurationText();
			CheckAndAddAchievementProgress();
			InterfaceController.RefreshStatPanels();
		}

		public void DisableBuff()
		{
			_timer?.Stop();

			// Reduce hero stat to its original value.
			switch (Type)
			{
				case BlessingType.ClickDamage:
					User.Instance.CurrentHero.ClickDamage -= Buff;
					break;
			}

			// Reset DurationText.
			DurationText = "";

			InterfaceController.RefreshStatPanels();
		}

		private void InitializeAndStartTimer()
		{
			_timer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 1)
			};
			_timer.Tick += Timer_Tick;
			_timer.Start();
		}

		private void UpdateDurationText()
		{
			DurationText = $"{Name}\n{Duration / 60}m {Duration % 60}s";
		}

		private void Timer_Tick(object source, EventArgs e)
		{
			Duration--;
			UpdateDurationText();

			// If blessing is finished.
			if (Duration <= 0)
			{
				User.Instance.CurrentHero.RemoveBlessing();
			}
		}

		
	}
}