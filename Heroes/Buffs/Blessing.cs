using System;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Threading;
using ClickQuest.Controls;
using ClickQuest.Data.GameData;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Interfaces;
using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Heroes.Buffs
{
	public enum BlessingType { ClickDamage = 0, CritChance, CritDamage, PoisonDamage, AuraDamage, AuraSpeed }

	public class Blessing : INotifyPropertyChanged, IIdentifiable
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private DispatcherTimer _timer;

		public string DurationText { get; set; }

		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public Rarity Rarity { get; set; }
		public int Value { get; set; }
		public BlessingType Type { get; set; }
		public int Duration { get; set; }
		public int Buff { get; set; }
		public bool AchievementBonusGranted { get; set; }

		[JsonIgnore]
		public string TypeString
		{
			get
			{
				return Type.ToString();
			}
		}

		[JsonIgnore]
		public string RarityString
		{
			get
			{
				return Rarity.ToString();
			}
		}

		[JsonIgnore]
		public bool IsFinished
		{
			get
			{
				return Duration <= 0;
			}
		}

		public Blessing CopyBlessing()
		{
			var copy = new Blessing();

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
			// Trigger on-blessing artifacts.
			foreach (var artifact in User.Instance.CurrentHero.EquippedArtifacts)
			{
				artifact.ArtifactFunctionality.OnBlessingStarted(this);
			}

			// Increase hero stat.
			switch (Type)
			{
				case BlessingType.ClickDamage:
					User.Instance.CurrentHero.ClickDamage += Buff;
					break;

				case BlessingType.CritDamage:
					User.Instance.CurrentHero.CritDamage += 0.01d * Buff;
					break;
				
				case BlessingType.CritChance:
					break;
				
				case BlessingType.PoisonDamage:
					break;
				
				case BlessingType.AuraDamage:
					User.Instance.CurrentHero.AuraDamage += 0.01d * Buff;
					break;
				
				case BlessingType.AuraSpeed:
					break;
			}

			InitializeAndStartTimer();
			UpdateDurationText();
			CheckAndAddAchievementProgress();
			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
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

				case BlessingType.CritDamage:
					User.Instance.CurrentHero.CritDamage -= 0.01d * Buff;
					break;
				
				case BlessingType.CritChance:
					break;
				
				case BlessingType.PoisonDamage:
					break;
				
				case BlessingType.AuraDamage:
					User.Instance.CurrentHero.AuraDamage -= 0.01d * Buff;
					break;
				
				case BlessingType.AuraSpeed:
					break;
			}

			DurationText = "";

			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
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

			if (IsFinished)
			{
				User.Instance.CurrentHero.RemoveBlessing();
			}
		}

		public static bool AskUserAndSwapBlessing(int newBlessingId)
		{
			var blessingBlueprint = GameData.Blessings.FirstOrDefault(x => x.Id == newBlessingId);

			var result = MessageBoxResult.OK;

			if (User.Instance.CurrentHero.Blessing != null)
			{
				result = AlertBox.Show($"Do you want to swap current blessing to {blessingBlueprint.Name}?\n{blessingBlueprint.Description}");
			}

			if (result == MessageBoxResult.OK)
			{
				AddOrReplaceBlessing(newBlessingId);
				return true;
			}

			return false;
		}

		public static void AddOrReplaceBlessing(int newBlessingId)
		{
			var blessingBlueprint = GameData.Blessings.FirstOrDefault(x => x.Id == newBlessingId);

			User.Instance.CurrentHero.RemoveBlessing();

			var newBlessing = blessingBlueprint.CopyBlessing();
			newBlessing.Duration += User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Blessing];
			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Blessing]++;

			User.Instance.CurrentHero.Blessing = newBlessing;
			newBlessing.EnableBuff();

			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
		}
	}
}