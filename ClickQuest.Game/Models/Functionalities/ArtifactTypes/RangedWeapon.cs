using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities.ArtifactTypes;

public class RangedWeapon : ArtifactTypeFunctionality
{
	public RangedWeapon()
	{
		ArtifactType = ArtifactType.RangedWeapon;
		Description = "Requires <BOLD>Ammunition</BOLD> to use. On top of its normal effect, has an additional effect based on the Ammunition used.";
	}
}