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
			CombatHelper.DealDamageToEnemy(clickedEnemy, (int)(monster.Health * DamageDealtPercentage), DamageType.Artifact);
		}
	}

	public PoisonedNagaKnife()
	{
		Name = "Poisoned Naga Knife";
	}
}