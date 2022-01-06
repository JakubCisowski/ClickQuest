using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.Core.Items.Artifacts;

// Increases your Click Damage by 10, and your Poison Damage by 5.
public class PoisonedSword : ArtifactFunctionality
{
	private const int ClickDamageIncrease = 10;
	private const int PoisonDamageIncrease = 5;

	public override void OnEquip()
	{
		User.Instance.CurrentHero.ClickDamage += ClickDamageIncrease;
		User.Instance.CurrentHero.PoisonDamage += PoisonDamageIncrease;
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.ClickDamage -= ClickDamageIncrease;
		User.Instance.CurrentHero.PoisonDamage -= PoisonDamageIncrease;
	}

	public PoisonedSword()
	{
		Name = "Poisoned Sword";
	}
}