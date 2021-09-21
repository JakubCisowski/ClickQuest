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
				CombatTimerController.StartPoisonTimer();

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
				CombatTimerController.StopPoisonTimer();

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
				CombatTimerController.StopPoisonTimer();
				CombatTimerController.BossFightTimer.Stop();

				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.BossesDefeated, 1);

				GetCurrentBossPage().GrantVictoryBonuses();
				GetCurrentBossPage().HandleInterfaceAfterBossDeath();
			}
		}
		
		
	}
}