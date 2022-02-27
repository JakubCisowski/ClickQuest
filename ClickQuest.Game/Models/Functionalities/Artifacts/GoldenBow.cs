namespace ClickQuest.Game.Models.Functionalities.Artifacts;

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