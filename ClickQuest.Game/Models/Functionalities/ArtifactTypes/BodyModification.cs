using System.Linq;
using System.Windows;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.UserInterface.Controls;

namespace ClickQuest.Game.Models.Functionalities.ArtifactTypes;

public class BodyModification : ArtifactTypeFunctionality
{
	public override bool CanBeEquipped()
	{
		var isNoOtherBodyModificationEquipped = User.Instance.CurrentHero.EquippedArtifacts.Count(x => x.ArtifactType == ArtifactType.BodyModification) == 0;

		if (isNoOtherBodyModificationEquipped)
		{
			return true;
		}

		AlertBox.Show("This artifact cannot be equipped right now - only one Body Modification can be equipped at a time.", MessageBoxButton.OK);
		return false;
	}

	public BodyModification()
	{
		ArtifactType = ArtifactType.BodyModification;
		Description = "You can only equip one Artifact of this type.";
	}
}