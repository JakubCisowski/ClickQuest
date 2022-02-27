using ClickQuest.Game.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

public class CompoundBow : ArtifactFunctionality
{
	private const double ChanceToRecoverAmmunitionOnConsume = 0.30;
	
	public override void OnConsumed(Artifact artifactConsumed)
	{
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