using ClickQuest.Enemies;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	// Requires three artifact slots to equip. Your click, poison and aura damage become zero.
	// Instead, each click you make on a Monster will give you loot and instantly defeat them. Does not work on Bosses.
	public class Transcendence : ArtifactFunctionality
	{
		public override void OnDealingClickDamage(ref int clickDamage)
		{
			if (InterfaceController.CurrentEnemy is Monster)
			{
				clickDamage = 0;
			}
		}

		public override void OnDealingPoisonDamage(ref int poisonDamage)
		{
			if (InterfaceController.CurrentEnemy is Monster monster)
			{
				poisonDamage = 0;
			}
		}

		public override void OnDealingAuraDamage(ref int auraDamage)
		{
			if (InterfaceController.CurrentEnemy is Monster monster)
			{
				auraDamage = 0;
			}
		}

		public override void OnEnemyClick()
		{
			if (InterfaceController.CurrentEnemy is Monster monster)
			{
				CombatController.DealDamageToEnemy(monster.CurrentHealth, DamageType.Artifact);
			}
		}

		public Transcendence()
		{
			Name = "Transcendence";
			ArtifactSlotsRequired = 3;
		}
	}
}