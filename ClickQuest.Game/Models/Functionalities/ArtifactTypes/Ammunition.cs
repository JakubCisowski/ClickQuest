using System.Linq;
using System.Windows;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.UserInterface.Controls;

namespace ClickQuest.Game.Models.Functionalities.ArtifactTypes;

// Does not take up an Artifact slot. On its own, Ammunition-type Artifacts do not have an effect,
// but can be consumed if a Ranged Weapon is equipped. Only one Ammunition can be equipped at a time.
public class Ammunition : ArtifactTypeFunctionality
{
	public override bool CanBeEquipped()
	{
		var isAnyAmmunitionEquipped = User.Instance.CurrentHero.EquippedArtifacts.Any(x => x.ArtifactType == ArtifactType.Ammunition);

		if (isAnyAmmunitionEquipped)
		{
			AlertBox.Show("This artifact cannot be equipped right now - it cannot be equipped with another Ammunition-type Artifact.", MessageBoxButton.OK);
			return false;
		}

		return true;
	}

	public Ammunition()
	{
		ArtifactType = ArtifactType.Ammunition;
		Description = "Does not take up an Artifact slot. On its own, Ammunition-type Artifacts do not have an effect, but can be consumed if a <BOLD>Ranged Weapon</BOLD> is equipped. Only one Ammunition can be equipped at a time.";
	}
}