using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities.ArtifactTypes;

// Enables Ammunition use if equipped. On top of its normal effect, has an additional effect based on the Ammunition used.
public class RangedWeapon : ArtifactTypeFunctionality
{
	public RangedWeapon()
	{
		ArtifactType = ArtifactType.RangedWeapon;
		Description = "Enables <BOLD>Ammunition</BOLD> use if equipped. On top of its normal effect, has an additional effect based on the Ammunition used.";
	}
}