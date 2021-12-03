using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;

namespace ClickQuest.Game.Core.Items.Artifacts
{
	// Has to be equipped with at least one other artifact. 
	// After killing a monster, your next click is guaranteed to critically hit, and will deal 10% more damage.
	public class GhostlyInfusion : ArtifactFunctionality
	{
		private const double DamageModifier = 0.1;

		private bool _isNextClickEmpowered;

		public override void OnDealingClickDamage(ref int clickDamage, DamageType clickDamageType)
		{
			if (_isNextClickEmpowered)
			{
				_isNextClickEmpowered = false;

				if (clickDamageType == DamageType.Normal)
				{
					var criticalDamageDealt = (int) (clickDamage * User.Instance.CurrentHero.CritDamage);

					CombatController.DealDamageToCurrentEnemy(criticalDamageDealt, DamageType.Critical);
					CombatController.DealDamageToCurrentEnemy((int) (criticalDamageDealt * DamageModifier), DamageType.Artifact);

					clickDamage = 0;
				}
				else if (clickDamageType == DamageType.Critical)
				{
					CombatController.DealDamageToCurrentEnemy((int) (clickDamage * DamageModifier), DamageType.Artifact);
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