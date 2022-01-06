namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Increases your Critical Click Chance by 5%.
public class DullKnife : ArtifactFunctionality
{
	private const double CritChanceIncrease = 0.05;

	public override void OnEquip()
	{
		User.Instance.CurrentHero.CritChance += CritChanceIncrease;
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.CritChance -= CritChanceIncrease;
	}

	public DullKnife()
	{
		Name = "Dull Knife";
	}
}