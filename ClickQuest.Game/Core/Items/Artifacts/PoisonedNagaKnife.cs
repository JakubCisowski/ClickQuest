using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Extensions.Combat;

namespace ClickQuest.Game.Core.Items.Artifacts;

// Clicks against Monsters deal additional damage equal to 5% of their maximum health. Does not work against Bosses.
public class PoisonedNagaKnife : ArtifactFunctionality
{
	private const double DamageDealtPercentage = 0.05;

	public override void OnEnemyClick(Enemy clickedEnemy)
	{
		if (clickedEnemy is Monster monster)
		{
			CombatController.DealDamageToEnemy(clickedEnemy, (int)(monster.Health * DamageDealtPercentage), DamageType.Artifact);
		}
	}

	public PoisonedNagaKnife()
	{
		Name = "Poisoned Naga Knife";
	}
}