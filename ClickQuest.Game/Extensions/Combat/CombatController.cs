using System.Linq;
using System.Windows;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Gameplay;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Windows;

namespace ClickQuest.Game.Extensions.Combat
{
	public static class CombatController
	{
		public static void HandleUserClickOnEnemy()
		{
			bool isNoQuestActive = User.Instance.CurrentHero.Quests.All(x => x.EndDate == default);

			if (isNoQuestActive)
			{
				Enemy clickedEnemy = InterfaceController.CurrentEnemy;

				CombatTimerController.StartPoisonTimer();

				var damageBaseAndCritInfo = User.Instance.CurrentHero.CalculateBaseAndCritClickDamage();
				int damageOnHit = User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Clicking];
				GameController.UpdateSpecializationAmountAndUi(SpecializationType.Clicking);

				// Passing clickedEnemy ensures that even if we kill the enemy with dealt damage (eg. click damage),
				// other effects (on-hit and artifacts) will be dealt to that dead enemy instead of being carried over.
				// However, stacking artifacts and other similar bonuses will still work properly.

				DealDamageToEnemy(clickedEnemy, damageBaseAndCritInfo.Damage, damageBaseAndCritInfo.DamageType);

				DealDamageToEnemy(clickedEnemy, damageOnHit, DamageType.OnHit);

				// Invoke Artifacts with the "on-enemy-click" effect.
				foreach (Artifact equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
				{
					equippedArtifact.ArtifactFunctionality.OnEnemyClick(clickedEnemy);
				}
			}
			else
			{
				AlertBox.Show("Your hero is busy completing quest!\nCheck back when it's finished.", MessageBoxButton.OK);
			}
		}

		public static void DealDamageToCurrentEnemy(int damage, DamageType damageType = DamageType.Normal)
		{
			DealDamageToEnemy(InterfaceController.CurrentEnemy, damage, damageType);
		}

		public static void DealDamageToEnemy(Enemy enemy, int damage, DamageType damageType = DamageType.Normal)
		{
			// Make sure enemy is still alive before dealing damage.
			if (enemy?.CurrentHealth <= 0)
			{
				return;
			}

			// Trigger on dealing (any) damage artifact event.
			if (damageType != DamageType.Artifact)
			{
				foreach (Artifact equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
				{
					equippedArtifact.ArtifactFunctionality.OnDealingDamage(ref damage);
				}
			}

			// Trigger damage type specific artifact events.
			foreach (Artifact equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
			{
				switch (damageType)
				{
					case DamageType.Normal or DamageType.Critical:
						equippedArtifact.ArtifactFunctionality.OnDealingClickDamage(ref damage, damageType);
						break;

					case DamageType.Poison:
						equippedArtifact.ArtifactFunctionality.OnDealingPoisonDamage(ref damage);
						break;

					case DamageType.Aura:
						equippedArtifact.ArtifactFunctionality.OnDealingAuraDamage(ref damage);
						break;
				}
			}

			// Trigger boss affix-related events.
			if (enemy is Boss boss)
			{
				foreach (AffixFunctionality affix in boss.AffixFunctionalities)
				{
					switch (damageType)
					{
						case DamageType.Poison:
							affix.OnDealingPoisonDamage(ref damage);
							break;
					}
				}
			}

			enemy.CurrentHealth -= damage;

			// Invoke floating text method.
			(Application.Current.MainWindow as GameWindow).CreateFloatingTextCombat(damage, damageType);

			enemy.HandleEnemyDeathIfDefeated();
		}
	}
}