namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Increases your Click Damage by 10, and your Poison Damage by 5.
public class PoisonedSword : ArtifactFunctionality
{
	private const int ClickDamageIncrease = 10;
	private const int PoisonDamageIncrease = 5;

	public override void OnEquip()
	{
		User.Instance.CurrentHero.ClickDamage += ClickDamageIncrease;
		User.Instance.CurrentHero.PoisonDamage += PoisonDamageIncrease;
		
		base.OnEquip();
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.ClickDamage -= ClickDamageIncrease;
		User.Instance.CurrentHero.PoisonDamage -= PoisonDamageIncrease;
		
		base.OnUnequip();
	}

	public PoisonedSword()
	{
		Name = "Poisoned Sword";
	}
}