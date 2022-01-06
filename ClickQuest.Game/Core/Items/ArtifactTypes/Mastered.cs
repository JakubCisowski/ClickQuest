using ClickQuest.Game.Core.Items.Types;

namespace ClickQuest.Game.Core.Items.ArtifactTypes;

public class Mastered : ArtifactTypeFunctionality
{
	public override void OnKill()
	{
		base.OnKill();
	}

	public Mastered()
	{
		ArtifactType = ArtifactType.Mastered;
		Description = "";
	}
}