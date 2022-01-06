using ClickQuest.Game.Extensions.Randomness;

namespace ClickQuest.Game.Core.Items.Artifacts;

// Each monster you kill has a 5% chance to drop a Leather Satchel. The item drops independently of all other items,
// and is not used in crafting, so can be safely sold.
public class LuckyKoboldShovel : ArtifactFunctionality
{
	private const double ChanceToGetItemOnKill = 0.05;

	public override void OnKill()
	{
		var randomizedNumber = RandomnessController.Rng.Next(1, 10001);

		if (randomizedNumber <= ChanceToGetItemOnKill * 10000)
		{
			// TODO
			var specialMaterial = new Material();
			specialMaterial.AddItem();
		}
	}

	public LuckyKoboldShovel()
	{
		Name = "Lucky Kobold Shovel";
	}
}