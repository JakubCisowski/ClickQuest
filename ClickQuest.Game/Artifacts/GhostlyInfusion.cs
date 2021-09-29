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
		private const double DamageModifier = 0.1;

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

		public override void OnDealingClickDamage(ref int clickDamage, DamageType clickDamageType)
		{
			if (_isNextClickEmpowered)
			{
				_isNextClickEmpowered = false;

				if (clickDamageType == DamageType.Normal)
				{
					int criticalDamageDealt = (int) (clickDamage * User.Instance.CurrentHero.CritDamage);

					CombatController.DealDamageToEnemy(criticalDamageDealt, DamageType.Critical);
					CombatController.DealDamageToEnemy((int) (criticalDamageDealt * DamageModifier), DamageType.Artifact);
					clickDamage = 0;
				}
				else if (clickDamageType == DamageType.Critical)
				{
					CombatController.DealDamageToEnemy((int) (clickDamage * DamageModifier), DamageType.Artifact);
				}
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