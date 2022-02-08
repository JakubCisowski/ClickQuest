using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models.Interfaces;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Helpers;
using ClickQuest.Game.UserInterface.Helpers.ToolTips;

namespace ClickQuest.Game.Models;

public enum BlessingType { ClickDamage = 0, CritChance, CritDamage, PoisonDamage, AuraDamage, AuraSpeed }

public class Blessing : INotifyPropertyChanged, IIdentifiable
{
	public event PropertyChangedEventHandler PropertyChanged;
	private DispatcherTimer _timer;

	public string DurationStatsPanelText { get; set; }
	
	public string DurationPriestPageText => Duration % 60 == 0 ? $"{Duration / 60} min" : $"{Duration / 60} min {Duration % 60} sec"  ;

	public int Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string Lore { get; set; }
	public Rarity Rarity { get; set; }
	public int Value { get; set; }
	public BlessingType Type { get; set; }
	public int Duration { get; set; }
	public int Buff { get; set; }
	public bool AchievementBonusGranted { get; set; }

	public string TypeString => Type.ToString();

	public string RarityString => Rarity.ToString();

	public bool IsFinished => Duration <= 0;

	public ToolTip ToolTip => ItemToolTipHelper.GenerateBlessingToolTip(this);

	public Blessing CopyBlessing()
	{
		var copy = new Blessing
		{
			Id = Id,
			Name = Name,
			Type = Type,
			Rarity = Rarity,
			Duration = Duration,
			Description = Description,
			Lore = Lore,
			Buff = Buff,
			Value = Value,
			AchievementBonusGranted = false
		};

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
				User.Instance.CurrentHero.CritChance += 0.01d * Buff;
				break;

			case BlessingType.PoisonDamage:
				User.Instance.CurrentHero.PoisonDamage += Buff;
				break;

			case BlessingType.AuraDamage:
				User.Instance.CurrentHero.AuraDamage += 0.01d * Buff;
				break;

			case BlessingType.AuraSpeed:
				User.Instance.CurrentHero.AuraAttackSpeed += 0.01d * Buff;
				break;
		}

		InitializeAndStartTimer();
		UpdateDurationText();
		CheckAndAddAchievementProgress();
		InterfaceHelper.RefreshBlessingInterfaceOnCurrentPage(Type);
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
				User.Instance.CurrentHero.CritChance -= 0.01d * Buff;
				break;

			case BlessingType.PoisonDamage:
				User.Instance.CurrentHero.PoisonDamage -= Buff;
				break;

			case BlessingType.AuraDamage:
				User.Instance.CurrentHero.AuraDamage -= 0.01d * Buff;
				break;

			case BlessingType.AuraSpeed:
				User.Instance.CurrentHero.AuraAttackSpeed -= 0.01d * Buff;
				break;
		}

		DurationStatsPanelText = "";

		InterfaceHelper.RefreshBlessingInterfaceOnCurrentPage(Type);
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
		DurationStatsPanelText = $"{Name}\n{Duration / 60}m {Duration % 60}s";
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
		var blessingBlueprint = GameAssets.Blessings.FirstOrDefault(x => x.Id == newBlessingId);

		var result = MessageBoxResult.OK;

		if (User.Instance.CurrentHero.Blessing != null)
		{
			result = AlertBox.Show($"Do you want to swap current blessing to {blessingBlueprint?.Name}?\n{blessingBlueprint.Description}");
		}

		if (result == MessageBoxResult.Yes)
		{
			AddOrReplaceBlessing(newBlessingId);
			return true;
		}

		return false;
	}

	public static void AddOrReplaceBlessing(int newBlessingId)
	{
		var blessingBlueprint = GameAssets.Blessings.FirstOrDefault(x => x.Id == newBlessingId);

		User.Instance.CurrentHero.RemoveBlessing();

		var newBlessing = blessingBlueprint?.CopyBlessing();
		newBlessing.Duration += User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Blessing];
		Specializations.UpdateSpecializationAmountAndUi(SpecializationType.Blessing);

		User.Instance.CurrentHero.Blessing = newBlessing;
		newBlessing.EnableBuff();

		InterfaceHelper.RefreshBlessingInterfaceOnCurrentPage(newBlessing.Type);
	}
}