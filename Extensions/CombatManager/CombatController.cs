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

				var damageBaseAndCritInfo = User.Instance.CurrentHero.CalculateBaseAndCritClickDamage();
				var damageOnHit = User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Clicking];
				
				DealDamageToMonster(damageBaseAndCritInfo.Damage, damageBaseAndCritInfo.DamageType);
				DealDamageToMonster(damageOnHit, DamageType.OnHit);
				
				// Invoke Artifacts with the "on-enemy-click" effect.
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
			InterfaceController.CurrentEnemy.CurrentHealth -= damage;

			// Invoke floating text method.
			// e.g. CreateFloatingTextPathAndStartAnimations(damage, damageType);

			// Trigger on dealing (any) damage artifact event.
			if(damageType != DamageType.Artifact)
			{
				foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
				{
					equippedArtifact.ArtifactFunctionality.OnDealingDamage(damage);
				}
			}

			// Trigger damage type specific artifact events.
			foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
			{
				switch (damageType)
				{
					case DamageType.Poison:
						equippedArtifact.ArtifactFunctionality.OnDealingPoisonDamage(damage);
					break;
				}
			}
		}
	}
}