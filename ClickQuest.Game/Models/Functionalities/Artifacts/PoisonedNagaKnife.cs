using System;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Clicks against Monsters deal additional damage equal to 5% of their maximum health. Does not work against Bosses.
public class PoisonedNagaKnife : ArtifactFunctionality
{
	private const double DamageDealtPercentage = 0.05;

	public override void OnEnemyClick(Enemy clickedEnemy)
	{
		if (clickedEnemy is Monster monster)
		{
			CombatHelper.DealDamageToEnemy(clickedEnemy, (int)Math.Ceiling(monster.Health * DamageDealtPercentage), DamageType.Magic);
		}
		
		base.OnEnemyClick(clickedEnemy);
	}

	public PoisonedNagaKnife()
	{
		Name = "Poisoned Naga Knife";
	}
}