
using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using ClickQuest.Enemies;
using ClickQuest.Heroes.Buffs;
using ClickQuest.Pages;
using ClickQuest.Player;

namespace ClickQuest.Extensions.CombatManager
{
	public static class CombatTimerController
	{
		public static DispatcherTimer PoisonTimer;
		public static DispatcherTimer AuraTimer;
		public static DispatcherTimer BossFightTimer;
		private static int _poisonTicks;
		private static int _bossFightDuration;

		public static int BossFightDuration
		{
			get
			{
				return _bossFightDuration;
			}
			set
			{
				_bossFightDuration = value;
				
				// todo: refactor this shit
				var bossPage = CombatController.GetCurrentBossPage();
				if (bossPage is not null)
				{
					CombatController.GetCurrentBossPage().Duration = _bossFightDuration;
				}
			}
		}
		
		public static int AuraTickDamage
        {
        	get
        	{
        		return (int) Math.Ceiling(User.Instance.CurrentHero.AuraDamage * CombatController.GetCurrentEnemy().Health);
        	}
        }

        public static double AuraTickInterval
        {
        	get
        	{
        		return 1d / User.Instance.CurrentHero.AuraAttackSpeed;
        	}
        }

		public static void StartAuraTimerOnCurrentRegion()
		{
			bool isCurrentHeroSelected = User.Instance.CurrentHero != null;

			if (!isCurrentHeroSelected)
			{
				return;
			}

			bool isNoQuestActive = User.Instance.CurrentHero.Quests.All(x => x.EndDate == default);
			object currentFrameContent = (Application.Current.MainWindow as GameWindow)?.CurrentFrame.Content;
			bool isCurrentFrameContentARegion = currentFrameContent is RegionPage;

			if (isNoQuestActive && isCurrentFrameContentARegion)
			{
				StartAuraTimer();
			}
		}

		private static void PoisonTimer_Tick(object source, EventArgs e)
		{
			int poisonTicksMax = 5;

			if (_poisonTicks >= poisonTicksMax)
			{
				PoisonTimer.Stop();
			}
			else
			{
				int poisonDamage = User.Instance.CurrentHero.PoisonDamage;
				CombatController.GetCurrentEnemy().CurrentHealth -= poisonDamage;

				// CreateFloatingTextPathAndStartAnimations(poisonDamage, DamageType.Poison);

				_poisonTicks++;

				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.PoisonTicksAmount, 1);

				// todo: rework this shit
				if (CombatController.GetCurrentEnemy() is Monster)
				{
					CombatController.HandleMonsterDeathIfDefeated();
				}
				else if (CombatController.GetCurrentEnemy() is Boss)
				{
					CombatController.HandleBossDeathIfDefeated();
				}

			}
		}
		
		private static void AuraTimer_Tick(object source, EventArgs e)
		{
			if (User.Instance.CurrentHero != null)
			{
				CombatController.GetCurrentEnemy().CurrentHealth -= AuraTickDamage;

				// CreateFloatingTextPathAndStartAnimations(AuraTickDamage, DamageType.Aura);
				
				// todo: rework this shit
				CombatController.HandleMonsterDeathIfDefeated();
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

				CombatController.GetCurrentBossPage().GrantVictoryBonuses();
				CombatController.GetCurrentBossPage().HandleInterfaceAfterBossDeath();
			}
		}
		
		public static void SetupAuraTimer()
		{
			AuraTimer = new DispatcherTimer();
			AuraTimer.Tick += AuraTimer_Tick;
		}

		public static void SetupPoisonTimer()
		{
			int poisonIntervalMs = 500;
			PoisonTimer = new DispatcherTimer();
			PoisonTimer.Interval = new TimeSpan(0, 0, 0, 0, poisonIntervalMs);
			PoisonTimer.Tick += PoisonTimer_Tick;
			_poisonTicks = 0;
		}
		
		public static void SetupFightTimer()
		{
			if (User.Instance.CurrentHero != null)
			{
				BossFightTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 1)};
				BossFightTimer.Tick += BossFightTimerTick;

				// SpecDungeonBuff's base value is 30 - the fight's duration will always be 30s or more.
				BossFightDuration = User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Dungeon];
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
}