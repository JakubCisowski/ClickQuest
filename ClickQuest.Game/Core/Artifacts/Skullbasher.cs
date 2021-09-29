using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Extensions.Combat;

namespace ClickQuest.Game.Core.Artifacts
{
	// Every other click you make deals an additional 10 (?) damage.
	public class Skullbasher : ArtifactFunctionality
	{
		private const int DamageDealt = 10;

		private bool _isNextClickEmpowered;

		public override void OnEnemyClick()
		{
			if (!_isNextClickEmpowered)
			{
				_isNextClickEmpowered = true;
				return;
			}

			_isNextClickEmpowered = false;

			CombatController.DealDamageToEnemy(DamageDealt, DamageType.Artifact);
		}

		public Skullbasher()
		{
			Name = "Skullbasher";
		}
	}
}