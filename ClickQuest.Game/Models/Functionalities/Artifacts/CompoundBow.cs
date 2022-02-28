using System.Linq;
using ClickQuest.Game.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Increaes your Critical Click Damage by 20%. Upon consuming Ammunition, you have a 30% chance to recover it.
public class CompoundBow : ArtifactFunctionality
{
	private const double CritDamageIncrease = 0.20;
	private const double ChanceToRecoverAmmunitionOnConsume = 0.30;

	public override void OnEquip()
	{
		User.Instance.CurrentHero.CritDamage += CritDamageIncrease;
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.CritDamage -= CritDamageIncrease;
	}

	public override void OnConsumed(Artifact artifactConsumed)
	{
		// TODO: rework this to be less cringe
		// Check if Golden Bow is equipped, to prevent re-gaining double ammunition (essentially farming it, which is toxic)
		var goldenBow = User.Instance.CurrentHero.EquippedArtifacts.FirstOrDefault(x => x.Name == "Golden Bow");
		if (goldenBow is not null)
		{
			return;
		}
		
		var randomizedNumber = RandomnessHelper.Rng.Next(1, 10001);
		
		if (randomizedNumber <= ChanceToRecoverAmmunitionOnConsume * 10000)
		{
			artifactConsumed.AddItem(1);
		}
	}

	public CompoundBow()
	{
		Name = "Compound Bow";
	}
}