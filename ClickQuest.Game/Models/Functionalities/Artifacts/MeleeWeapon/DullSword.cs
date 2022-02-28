namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Increases your Click Damage by 3.
public class DullSword : ArtifactFunctionality
{
	private const int DamageIncrease = 3;

	public override void OnEquip()
	{
		User.Instance.CurrentHero.ClickDamage += DamageIncrease;
		
		base.OnEquip();
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.ClickDamage -= DamageIncrease;
		
		base.OnUnequip();
	}

	public DullSword()
	{
		Name = "Dull Sword";
	}
}