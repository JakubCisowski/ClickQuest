using ClickQuest.Game.Enemies;
using ClickQuest.Game.Extensions.CombatManager;
using ClickQuest.Game.Extensions.InterfaceManager;
using ClickQuest.Game.Items;

namespace ClickQuest.Game.Artifacts
{
	// Every third Click you make against the same enemy deals bonus damage based on their maximum health.
	// Against Monsters, deals 6% of their health. Against Bosses, deals 1% of their health.
	public class VialOfPlague : ArtifactFunctionality
	{
		private const int ClickThreshold = 3;
		private const double DamageAgainstMonsters = 0.06;
		private const double DamageAgainstBosses = 0.01;

		private int _clickCount;
		private Enemy _currentEnemy;
		
		public override void OnEnemyClick()
		{
			if (_currentEnemy != InterfaceController.CurrentEnemy)
			{
				_currentEnemy = InterfaceController.CurrentEnemy;
				_clickCount = 0;
			}

			_clickCount++;

			if (_clickCount == ClickThreshold)
			{
				int damageDealt = _currentEnemy is Monster ? (int) (_currentEnemy.Health * DamageAgainstMonsters) : (int) (_currentEnemy.Health * DamageAgainstBosses);
				CombatController.DealDamageToEnemy(damageDealt, DamageType.Artifact);

				_clickCount = 0;
			}
		}

		public VialOfPlague()
		{
			Name = "Vial of Plague";
		}
	}
}