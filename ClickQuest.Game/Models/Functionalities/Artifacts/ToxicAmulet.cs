namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Your poison damage is increased by 20%.
public class ToxicAmulet : ArtifactFunctionality
{
	private const double PoisonDamageModifier = 1.2;

	public override void OnDealingPoisonDamage(ref int poisonDamage)
	{
		var bonusPoisonDamage = (int)(poisonDamage * PoisonDamageModifier);

		poisonDamage += bonusPoisonDamage;
	}

	public ToxicAmulet()
	{
		Name = "Toxic Amulet";
	}
}