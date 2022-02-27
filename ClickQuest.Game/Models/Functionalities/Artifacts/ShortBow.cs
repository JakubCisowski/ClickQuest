namespace ClickQuest.Game.Models.Functionalities.Artifacts;

public class ShortBow : ArtifactFunctionality
{
	private const int ClickDamageIncrease = 5;
	
	public override void OnEquip()
	{
		User.Instance.CurrentHero.ClickDamage += ClickDamageIncrease;
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.ClickDamage -= ClickDamageIncrease;
	}

	public ShortBow()
	{
		Name = "Short Bow";
	}
}