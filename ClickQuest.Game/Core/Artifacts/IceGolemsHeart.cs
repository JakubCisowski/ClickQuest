using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.Interface;

namespace ClickQuest.Game.Core.Artifacts
{
	// Your clicks leave a stack of 'Brittle' on the enemy, increasing your Critical Click Chance against them by 5%. Stacks up to 5 times.
	// Additionally, clicks against Monsters (excluding Bosses) with 5 'Brittle' stacks and below 10% health instantly finish them.
	public class IceGolemsHeart : ArtifactFunctionality
	{
		private const double CritChanceIncreasePerStack = 0.05;
		private const int MaxStacks = 5;
		private const double ExecuteThreshold = 0.10;

		private Enemy _currentEnemy;
		private int _stackCount;

		public override void OnEnemyClick()
		{
			if (_currentEnemy != InterfaceController.CurrentEnemy)
			{
				_currentEnemy = InterfaceController.CurrentEnemy;
				User.Instance.CurrentHero.CritChance -= _stackCount * CritChanceIncreasePerStack;
				_stackCount = 0;
			}
			else
			{
				if (_stackCount < MaxStacks)
				{
					User.Instance.CurrentHero.CritChance += CritChanceIncreasePerStack;
					_stackCount++;
				}

				bool isEnemyAMonster = _currentEnemy is Monster;
				bool isEnemyInThreshold = _currentEnemy.CurrentHealth <= ExecuteThreshold * _currentEnemy.Health;

				if (isEnemyAMonster && isEnemyInThreshold && _stackCount == MaxStacks)
				{
					CombatController.DealDamageToEnemy(_currentEnemy.CurrentHealth, DamageType.Artifact);
				}
			}
		}

		public IceGolemsHeart()
		{
			Name = "Ice Golem's Heart";
		}
	}
}