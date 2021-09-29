using ClickQuest.Game.Enemies;
using ClickQuest.Game.Extensions.CombatManager;
using ClickQuest.Game.Extensions.InterfaceManager;
using ClickQuest.Game.Items;

namespace ClickQuest.Game.Artifacts
{
	// Clicks against Monsters deal additional damage equal to 5% of their maximum health. Does not work against Bosses.
	public class PoisonedNagaKnife : ArtifactFunctionality
	{
		private const double DamageDealtPercentage = 0.05;

		public override void OnEnemyClick()
		{
			if (InterfaceController.CurrentEnemy is Monster monster)
			{
				CombatController.DealDamageToEnemy((int) (monster.Health * DamageDealtPercentage), DamageType.Artifact);
			}
		}

		public PoisonedNagaKnife()
		{
			Name = "Poisoned Naga Knife";
		}
	}
}