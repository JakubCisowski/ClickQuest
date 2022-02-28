namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Your Ammunition is no longer consumed (but still grants its full effects).
public class GoldenBow : ArtifactFunctionality
{
	public override void OnConsumed(Artifact artifactConsumed)
	{
		artifactConsumed.AddItem(1);
	}

	public GoldenBow()
	{
		Name = "Golden Bow";
	}
}