using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Your clicks leave a stack of 'Brittle' on the enemy, increasing your Critical Click Chance against them by 5%. Stacks up to 5 times.
// Additionally, clicks against Monsters (excluding Bosses) with 5 'Brittle' stacks and below 10% health instantly finish them.
public class IceGolemsHeart : ArtifactFunctionality
{
	private const double CritChanceIncreasePerStack = 0.05;
	private const int MaxStacks = 5;
	private const double ExecuteThreshold = 0.10;

	private Enemy _currentEnemy;
	private int _stackCount;

	public override void OnEnemyClick(Enemy clickedEnemy)
	{
		if (_currentEnemy != clickedEnemy)
		{
			_currentEnemy = clickedEnemy;
			User.Instance.CurrentHero.CritChance -= _stackCount * CritChanceIncreasePerStack;
			_stackCount = 0;
		}

		if (_stackCount < MaxStacks)
		{
			_stackCount++;
			User.Instance.CurrentHero.CritChance += CritChanceIncreasePerStack;
		}

		var isEnemyAMonster = _currentEnemy is Monster;
		var isEnemyInThreshold = _currentEnemy.CurrentHealth <= ExecuteThreshold * _currentEnemy.Health;

		if (isEnemyAMonster && isEnemyInThreshold && _stackCount == MaxStacks)
		{
			CombatHelper.DealDamageToEnemy(_currentEnemy, _currentEnemy.CurrentHealth, DamageType.Artifact);
		}
	}

	public override void OnKill()
	{
		User.Instance.CurrentHero.CritChance -= _stackCount * CritChanceIncreasePerStack;
		_stackCount = 0;
		_currentEnemy = null;
	}

	public override void OnRegionLeave()
	{
		User.Instance.CurrentHero.CritChance -= _stackCount * CritChanceIncreasePerStack;
		_stackCount = 0;
		_currentEnemy = null;
	}

	public IceGolemsHeart()
	{
		Name = "Ice Golem's Heart";
	}
}