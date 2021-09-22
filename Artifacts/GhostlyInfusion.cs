using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	// Has to be equipped with at least one other artifact. 
	// After killing a monster, your next click is guaranteed to critically hit, and will deal 10% more damage.
	public class GhostlyInfusion : ArtifactFunctionality
	{
		private const double DamageModifier = 1.1;
		
		private bool _isNextClickEmpowered = false;
		
		public override void OnEquip()
		{
			if (User.Instance.CurrentHero.EquippedArtifacts.Count < 0)
			{
				// todo: Do not equip this artifact.
			}
		}

		public override void OnEnemyClick()
		{
			if (_isNextClickEmpowered)
			{
				_isNextClickEmpowered = false;

				int damageDealt = (int) (User.Instance.CurrentHero.ClickDamage * DamageModifier * User.Instance.CurrentHero.CritDamage);
				
				CombatController.DealDamageToEnemy(damageDealt, DamageType.Critical);
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