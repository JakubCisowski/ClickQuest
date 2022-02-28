namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Increases your Click Damage by 20, and your Critical Click Chance by 10%.
public class StrongBadgeOfLuck : ArtifactFunctionality
{
	private const int ClickDamageIncrease = 20;
	private const double CritChanceIncrease = 0.10;

	public override void OnEquip()
	{
		User.Instance.CurrentHero.ClickDamage += ClickDamageIncrease;
		User.Instance.CurrentHero.CritChance += CritChanceIncrease;
		
		base.OnEquip();
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.ClickDamage -= ClickDamageIncrease;
		User.Instance.CurrentHero.CritChance -= CritChanceIncrease;
		
		base.OnUnequip();
	}

	public StrongBadgeOfLuck()
	{
		Name = "Strong Badge of Luck";
	}
}