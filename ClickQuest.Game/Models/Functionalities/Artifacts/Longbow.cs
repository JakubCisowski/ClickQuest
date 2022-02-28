namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Increases your Click Damage by 25, Critical Click Chance by 10% and Critical Click Damage by 20%.
public class Longbow : ArtifactFunctionality
{
	private const int ClickDamageIncrease = 25;
	private const double CritChanceIncrease = 0.10;
	private const double CritDamageIncrease = 0.20;
	
	public override void OnEquip()
	{
		User.Instance.CurrentHero.ClickDamage += ClickDamageIncrease;
		User.Instance.CurrentHero.CritChance += CritChanceIncrease;
		User.Instance.CurrentHero.CritDamage += CritDamageIncrease;
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.ClickDamage -= ClickDamageIncrease;
		User.Instance.CurrentHero.CritChance -= CritChanceIncrease;
		User.Instance.CurrentHero.CritDamage -= CritDamageIncrease;
	}

	public Longbow()
	{
		Name = "Longbow";
	}
}