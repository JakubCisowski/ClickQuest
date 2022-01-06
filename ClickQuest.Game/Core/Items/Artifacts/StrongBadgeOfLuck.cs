using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.Core.Items.Artifacts;

// Increases your Click Damage by 20, and your Critical Click Chance by 10%.
public class StrongBadgeOfLuck : ArtifactFunctionality
{
	private const int ClickDamageIncrease = 20;
	private const double CritChanceIncrease = 0.10;

	public override void OnEquip()
	{
		User.Instance.CurrentHero.ClickDamage += ClickDamageIncrease;
		User.Instance.CurrentHero.CritChance += CritChanceIncrease;
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.ClickDamage -= ClickDamageIncrease;
		User.Instance.CurrentHero.CritChance -= CritChanceIncrease;
	}

	public StrongBadgeOfLuck()
	{
		Name = "Strong Badge of Luck";
	}
}