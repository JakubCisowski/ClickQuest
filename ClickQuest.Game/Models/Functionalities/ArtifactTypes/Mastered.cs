using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities.ArtifactTypes;

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