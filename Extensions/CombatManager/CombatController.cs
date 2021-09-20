using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using ClickQuest.Controls;
using ClickQuest.Data.GameData;
using ClickQuest.Enemies;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Heroes.Buffs;
using ClickQuest.Pages;
using ClickQuest.Places;
using ClickQuest.Player;

namespace ClickQuest.Extensions.CombatManager
{
	public static class CombatController
	{
		private static DispatcherTimer _poisonTimer;
		private static DispatcherTimer _auraTimer;
		public static DispatcherTimer _bossFightTimer;
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
				var bossPage = GetCurrentBossPage();
				if (bossPage is not null)
				{
					GetCurrentBossPage().Duration = _bossFightDuration;
				}
			}
		}
		
		public static int AuraTickDamage
        {
        	get
        	{
        		return (int) Math.Ceiling(User.Instance.CurrentHero.AuraDamage * GetCurrentEnemy().Health);
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

		public static Enemy GetCurrentEnemy()
		{
			object currentFrameContent = (Application.Current.MainWindow as GameWindow)?.CurrentFrame.Content;
			bool isCurrentFrameContentARegion = currentFrameContent is RegionPage;

			if (isCurrentFrameContentARegion)
			{
				return (currentFrameContent as RegionPage).RegionPanel.Children.OfType<MonsterButton>().FirstOrDefault()?.Monster;
			}

			bool isCurrentFrameContentADungeon = currentFrameContent is DungeonBossPage;

			if (isCurrentFrameContentADungeon)
			{
				return (currentFrameContent as DungeonBossPage).Boss;
			}

			return null;
		}
		
		public static MonsterButton GetCurrentMonsterButton()
		{
			bool isCurrentFrameContentARegion = GameData.CurrentPage is RegionPage;

			if (isCurrentFrameContentARegion)
			{
				return (GameData.CurrentPage as RegionPage).RegionPanel.Children.OfType<MonsterButton>().FirstOrDefault();
			}

			return null;
		}

		public static DungeonBossPage GetCurrentBossPage()
		{
			bool isCurrentFrameContentARegion = GameData.CurrentPage is DungeonBossPage;

			if (isCurrentFrameContentARegion)
			{
				return GameData.CurrentPage as DungeonBossPage;
			}

			return null;
		}

		public static void HandleUserClickOnEnemy()
		{
			bool isNoQuestActive = User.Instance.CurrentHero.Quests.All(x => x.EndDate == default);

			if (isNoQuestActive)
			{
				StartPoisonTimer();

				var damageBaseAndCritInfo = User.Instance.CurrentHero.CalculateClickDamage();
				var damageOnHit = User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Clicking];

				var damageType = damageBaseAndCritInfo.IsCritical ? DamageType.Critical : DamageType.Normal;
				
				DealDamageToMonster(damageBaseAndCritInfo.Damage, damageType);
				DealDamageToMonster(damageOnHit, DamageType.OnHit);
				
				// Invoke Artifacts with the "on-click" effect.
				foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
				{
					equippedArtifact.ArtifactFunctionality.OnEnemyClick();
				}

				User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Clicking]++;
			}
			else
			{
				AlertBox.Show("Your hero is busy completing quest!\nCheck back when it's finished.", MessageBoxButton.OK);
			}
		}
		
		public static void DealDamageToMonster(int damage, DamageType damageType = DamageType.Normal)
		{
			// Deals damage and invokes Artifacts with the "on-damage" effect.
			GetCurrentEnemy().CurrentHealth -= damage;

			// CreateFloatingTextPathAndStartAnimations(damage, damageType);

			foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
			{
				equippedArtifact.ArtifactFunctionality.OnDealingDamage(damage);
			}
		}
		
		public static void HandleMonsterDeathIfDefeated()
		{
			if (GetCurrentEnemy().CurrentHealth <= 0)
			{
				StopPoisonTimer();

				GetCurrentMonsterButton().GrantVictoryBonuses();
				
				// Invoke Artifacts with the "on-death" effect.
				foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
				{
					equippedArtifact.ArtifactFunctionality.OnKill();
				}

				
			}
		}
		
		public static void HandleBossDeathIfDefeated()
		{
			if (GetCurrentEnemy().CurrentHealth <= 0)
			{
				StopPoisonTimer();
				_bossFightTimer.Stop();

				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.BossesDefeated, 1);

				GetCurrentBossPage().GrantVictoryBonuses();
				GetCurrentBossPage().HandleInterfaceAfterBossDeath();
			}
		}
		
		private static void PoisonTimer_Tick(object source, EventArgs e)
		{
			int poisonTicksMax = 5;

			if (_poisonTicks >= poisonTicksMax)
			{
				_poisonTimer.Stop();
			}
			else
			{
				int poisonDamage = User.Instance.CurrentHero.PoisonDamage;
				GetCurrentEnemy().CurrentHealth -= poisonDamage;

				// CreateFloatingTextPathAndStartAnimations(poisonDamage, DamageType.Poison);

				_poisonTicks++;

				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.PoisonTicksAmount, 1);

				// todo: rework this shit
				if (GetCurrentEnemy() is Monster)
				{
					HandleMonsterDeathIfDefeated();
				}
				else if (GetCurrentEnemy() is Boss)
				{
					HandleBossDeathIfDefeated();
				}

			}
		}
		
		private static void AuraTimer_Tick(object source, EventArgs e)
		{
			if (User.Instance.CurrentHero != null)
			{
				GetCurrentEnemy().CurrentHealth -= AuraTickDamage;

				// CreateFloatingTextPathAndStartAnimations(AuraTickDamage, DamageType.Aura);
				
				// todo: rework this shit
				HandleMonsterDeathIfDefeated();
			}
		}
		
		private static void BossFightTimerTick(object source, EventArgs e)
		{
			BossFightDuration--;

			// Check if time is up.
			if (BossFightDuration <= 0)
			{
				StopPoisonTimer();

				_bossFightTimer.Stop();

				GetCurrentBossPage().GrantVictoryBonuses();
				GetCurrentBossPage().HandleInterfaceAfterBossDeath();
			}
		}
		
		public static void SetupAuraTimer()
		{
			_auraTimer = new DispatcherTimer();
			_auraTimer.Tick += AuraTimer_Tick;
		}

		public static void SetupPoisonTimer()
		{
			int poisonIntervalMs = 500;
			_poisonTimer = new DispatcherTimer();
			_poisonTimer.Interval = new TimeSpan(0, 0, 0, 0, poisonIntervalMs);
			_poisonTimer.Tick += PoisonTimer_Tick;
			_poisonTicks = 0;
		}
		
		public static void SetupFightTimer()
		{
			if (User.Instance.CurrentHero != null)
			{
				_bossFightTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 1)};
				_bossFightTimer.Tick += BossFightTimerTick;

				// SpecDungeonBuff's base value is 30 - the fight's duration will always be 30s or more.
				BossFightDuration = User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Dungeon];
			}
		}

		public static void StartAuraTimer()
		{
			if (User.Instance.CurrentHero != null)
			{
				// ex.: 1.50 aura attack speed = 1.5 aura ticks per second
				_auraTimer.Interval = TimeSpan.FromSeconds(1d / User.Instance.CurrentHero.AuraAttackSpeed);

				_auraTimer.Start();
			}
		}

		public static void StartPoisonTimer()
		{
			if (User.Instance.CurrentHero.PoisonDamage > 0)
			{
				_poisonTicks = 0;
				_poisonTimer.Start();
			}
		}

		public static void StopCombatTimers()
		{
			StopPoisonTimer();
			_auraTimer.Stop();
		}

		public static void StopPoisonTimer()
		{
			_poisonTimer.Stop();
			_poisonTicks = 0;
		}
		
		public static void UpdateAuraAttackSpeed()
		{
			_auraTimer.Stop();
			_auraTimer.Interval = TimeSpan.FromSeconds(AuraTickInterval);
			_auraTimer.Start();
		}
	}
}