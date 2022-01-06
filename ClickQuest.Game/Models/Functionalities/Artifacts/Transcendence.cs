using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.UserInterface.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Requires three artifact slots to equip. While on a Region, you don't deal any damage.
// Instead, each click you make on a Monster will instantly defeat them.
public class Transcendence : ArtifactFunctionality
{
	public override void OnDealingClickDamage(ref int clickDamage, DamageType clickDamageType)
	{
		if (InterfaceHelper.CurrentEnemy is Monster)
		{
			clickDamage = 0;
		}
	}

	public override void OnDealingPoisonDamage(ref int poisonDamage)
	{
		if (InterfaceHelper.CurrentEnemy is Monster)
		{
			poisonDamage = 0;
		}
	}

	public override void OnDealingAuraDamage(ref int auraDamage)
	{
		if (InterfaceHelper.CurrentEnemy is Monster)
		{
			auraDamage = 0;
		}
	}

	public override void OnEnemyClick(Enemy clickedEnemy)
	{
		if (clickedEnemy is Monster monster)
		{
			CombatHelper.DealDamageToEnemy(monster, monster.CurrentHealth, DamageType.Artifact);
		}
	}

	public Transcendence()
	{
		Name = "Transcendence";
		ArtifactSlotsRequired = 3;
	}
}