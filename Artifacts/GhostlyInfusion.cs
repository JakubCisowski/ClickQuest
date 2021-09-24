using System.Windows;
using ClickQuest.Controls;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	// Has to be equipped with at least one other artifact. 
	// After killing a monster, your next click is guaranteed to critically hit, and will deal 10% more damage.
	public class GhostlyInfusion : ArtifactFunctionality
	{
		private const double DamageModifier = 1.1;

		private bool _isNextClickEmpowered;

		public override bool CanBeEquipped()
		{
			bool canBeEquippedBase = base.CanBeEquipped();

			if (!canBeEquippedBase)
			{
				return false;
			}
			
			bool isAnArtifactEquipped = User.Instance.CurrentHero.EquippedArtifacts.Count > 0;

			if (isAnArtifactEquipped)
			{
				return true;
			}

			AlertBox.Show("This artifact cannot be equipped right now - it requires at least 1 other artifact to be equipped.", MessageBoxButton.OK);
			return false;
		}

		public override void OnEnemyClick()
		{
			if (_isNextClickEmpowered)
			{
				_isNextClickEmpowered = false;

				// todo: póki co nie działa jak intended - zadaje zwiększony crit damage POZA zwykłym kliknięciem, czyli zadaje (click damage + crit damage * 1.1)
				// intended działanie:
				// jeśli atak by skrytował, to zadaje 1.1* dmg
				// jeśli atak by nie skrytował, to krytuje i zadaje 1.1* dmg
				// ma to być jeden atak
				int damageDealt = (int) (User.Instance.CurrentHero.ClickDamage * DamageModifier * User.Instance.CurrentHero.CritDamage);

				CombatController.DealDamageToEnemy(damageDealt, DamageType.Artifact);
			}
		}

		public override void OnKill()
		{
			_isNextClickEmpowered = true;
		}

		public GhostlyInfusion()
		{
			Name = "Ghostly Infusion";
		}
	}
}