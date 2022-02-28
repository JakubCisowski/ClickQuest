namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Increases your Poison Damage by 5.
public class LargeStinger : ArtifactFunctionality
{
	private const int PoisonDamageIncrease = 5;

	public override void OnEquip()
	{
		User.Instance.CurrentHero.PoisonDamage += PoisonDamageIncrease;
		
		base.OnEquip();
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.PoisonDamage -= PoisonDamageIncrease;
		
		base.OnUnequip();
	}

	public LargeStinger()
	{
		Name = "Large Stinger";
	}
}