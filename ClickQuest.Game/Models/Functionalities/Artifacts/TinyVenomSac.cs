namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Increases your Poison Damage by 1.
public class TinyVenomSac : ArtifactFunctionality
{
	private const int PoisonDamageIncrease = 1;

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

	public TinyVenomSac()
	{
		Name = "Tiny Venom Sac";
	}
}