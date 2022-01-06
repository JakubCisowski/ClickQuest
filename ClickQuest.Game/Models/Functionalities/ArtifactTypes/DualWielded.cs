using System.Windows;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.UserInterface.Controls;

namespace ClickQuest.Game.Models.Functionalities.ArtifactTypes;

public class DualWielded : ArtifactTypeFunctionality
{
	private const int ArtifactSlotsRequired = 2;

	public override bool CanBeEquipped()
	{
		var hasFreeSlots = 3 - User.Instance.CurrentHero.EquippedArtifacts.Count > 1;

		if (hasFreeSlots)
		{
			return true;
		}

		AlertBox.Show("This artifact cannot be equipped right now - it requires two free Artifact slots.", MessageBoxButton.OK);
		return false;
	}

	public DualWielded()
	{
		ArtifactType = ArtifactType.DualWielded;
		Description = "Requires two free slots to equip. If equipped with an Artifact of type Infusion, double its effectiveness.";
	}
}