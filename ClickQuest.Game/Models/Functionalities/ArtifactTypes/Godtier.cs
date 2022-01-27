using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities.ArtifactTypes;

// The sheer power of the gods overwhelms you - you cannot tell what is happening around you anymore.
public class Godtier : ArtifactTypeFunctionality
{
	public Godtier()
	{
		ArtifactType = ArtifactType.Godtier;
		Description = "The sheer power of the gods overwhelms you - you cannot tell what is happening around you anymore.";
	}
}