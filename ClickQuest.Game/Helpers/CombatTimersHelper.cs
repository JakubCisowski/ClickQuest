using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Models;
using ClickQuest.Game.UserInterface.Helpers;
using ClickQuest.Game.UserInterface.Pages;
using ClickQuest.Game.UserInterface.Windows;

namespace ClickQuest.Game.Helpers;

public static class CombatTimersHelper
{
	public static DispatcherTimer PoisonTimer;
	public static DispatcherTimer AuraTimer;
	public static DispatcherTimer BossFightTimer;
	private static int _poisonTicks;
	private static int _bossFightDuration;

	public static int BossFightDuration
	{
		get => _bossFightDuration;
		set
		{
			_bossFightDuration = value;

			// todo: refactor this shit
			var bossPage = InterfaceHelper.CurrentBossPage;
			if (bossPage is not null)
			{
				InterfaceHelper.CurrentBossPage.Duration = _bossFightDuration;
			}
		}
	}

	public static int AuraTickDamage => (int)Math.Ceiling(User.Instance.CurrentHero.AuraDamage * InterfaceHelper.CurrentEnemy.Health);

	public static double AuraTickInterval => 1d / User.Instance.CurrentHero.AuraAttackSpeed;

	public static void StartAuraTimerOnCurrentRegion()
	{
		var isCurrentHeroSelected = User.Instance.CurrentHero != null;

		if (!isCurrentHeroSelected)
		{
			return;
		}

		var isNoQuestActive = User.Instance.CurrentHero.Quests.All(x => x.EndDate == default);
		var currentFrameContent = (Application.Current.MainWindow as GameWindow)?.CurrentFrame.Content;
		var isCurrentFrameContentARegion = currentFrameContent is RegionPage;

		if (isNoQuestActive && isCurrentFrameContentARegion)
		{
			StartAuraTimer();
		}
	}

	private static void PoisonTimer_Tick(object source, EventArgs e)
	{
		const int poisonTicksMax = 5;

		if (_poisonTicks >= poisonTicksMax)
		{
			PoisonTimer.Stop();
		}
		else
		{
			CombatHelper.DealDamageToCurrentEnemy(User.Instance.CurrentHero.PoisonDamage, DamageType.Poison);

			_poisonTicks++;

			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.PoisonTicksAmount, 1);
		}
	}

	private static void AuraTimer_Tick(object source, EventArgs e)
	{
		if (User.Instance.CurrentHero != null)
		{
			CombatHelper.DealDamageToCurrentEnemy(AuraTickDamage, DamageType.Aura);
		}
	}

	private static void BossFightTimerTick(object source, EventArgs e)
	{
		BossFightDuration--;

		// Check if time is up.
		if (BossFightDuration <= 0)
		{
			StopPoisonTimer();

			BossFightTimer.Stop();

			InterfaceHelper.CurrentEnemy.GrantVictoryBonuses();

			// Invoke Artifacts with the "on-death" effect.
			// We need to do this here because some artifacts should reset upon leaving combat (eg. Ice Golem's Heart).
			// Even if the Boss wasn't fully defeated.
			foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
			{
				equippedArtifact.ArtifactFunctionality.OnKill();
			}

			InterfaceHelper.CurrentBossPage.HandleInterfaceAfterBossDeath();
		}
	}

	public static void SetupAuraTimer()
	{
		AuraTimer = new DispatcherTimer();
		AuraTimer.Tick += AuraTimer_Tick;
	}

	public static void SetupPoisonTimer()
	{
		const int poisonIntervalMs = 500;
		PoisonTimer = new DispatcherTimer
		{
			Interval = new TimeSpan(0, 0, 0, 0, poisonIntervalMs)
		};
		PoisonTimer.Tick += PoisonTimer_Tick;
		_poisonTicks = 0;
	}

	public static void SetupFightTimer()
	{
		if (User.Instance.CurrentHero != null)
		{
			BossFightTimer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 1)
			};
			BossFightTimer.Tick += BossFightTimerTick;

			// SpecDungeonBuff's base value is 30 - the fight's duration will always be 30s or more.
			BossFightDuration = User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Dungeon];
		}
	}

	public static void StartAuraTimer()
	{
		if (User.Instance.CurrentHero != null)
		{
			// ex.: 1.50 aura attack speed = 1.5 aura ticks per second
			AuraTimer.Interval = TimeSpan.FromSeconds(1d / User.Instance.CurrentHero.AuraAttackSpeed);

			AuraTimer.Start();
		}
	}

	public static void StartPoisonTimer()
	{
		if (User.Instance.CurrentHero.PoisonDamage > 0)
		{
			_poisonTicks = 0;
			PoisonTimer.Start();
		}
	}

	public static void StopCombatTimers()
	{
		StopPoisonTimer();
		AuraTimer.Stop();
	}

	public static void StopPoisonTimer()
	{
		PoisonTimer.Stop();
		_poisonTicks = 0;
	}

	public static void UpdateAuraAttackSpeed()
	{
		AuraTimer.Stop();
		AuraTimer.Interval = TimeSpan.FromSeconds(AuraTickInterval);
		AuraTimer.Start();
	}
}