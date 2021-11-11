using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.UserInterface;

namespace ClickQuest.Game.Core.Artifacts
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

		public override void OnEnemyClick(Enemy clickedEnemy)
		{
			if (_currentEnemy != clickedEnemy)
			{
				_currentEnemy = clickedEnemy;
				_clickCount = 0;
			}

			_clickCount++;

			if (_clickCount == ClickThreshold)
			{
				int damageDealt = _currentEnemy is Monster ? (int) (_currentEnemy.Health * DamageAgainstMonsters) : (int) (_currentEnemy.Health * DamageAgainstBosses);
				CombatController.DealDamageToEnemy(_currentEnemy, damageDealt, DamageType.Artifact);

				_clickCount = 0;
			}
		}

		public override void OnRegionLeave()
		{
			_clickCount = 0;
		}

		public VialOfPlague()
		{
			Name = "Vial of Plague";
		}
	}
}