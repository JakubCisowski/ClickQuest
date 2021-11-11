using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.UserInterface;

namespace ClickQuest.Game.Core.Artifacts
{
	// Requires three artifact slots to equip. While on a Region, you don't deal any damage.
	// Instead, each click you make on a Monster will instantly defeat them.
	public class Transcendence : ArtifactFunctionality
	{
		public override void OnDealingClickDamage(ref int clickDamage, DamageType clickDamageType)
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

		public override void OnEnemyClick(Enemy clickedEnemy)
		{
			if (clickedEnemy is Monster monster)
			{
				CombatController.DealDamageToEnemy(monster, monster.CurrentHealth, DamageType.Artifact);
			}
		}

		public Transcendence()
		{
			Name = "Transcendence";
			ArtifactSlotsRequired = 3;
		}
	}
}