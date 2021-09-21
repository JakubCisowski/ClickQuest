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
			InterfaceController.CurrentEnemy.CurrentHealth -= damage;

			// CreateFloatingTextPathAndStartAnimations(damage, damageType);

			foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
			{
				equippedArtifact.ArtifactFunctionality.OnDealingDamage(damage);
			}
		}
	}
}