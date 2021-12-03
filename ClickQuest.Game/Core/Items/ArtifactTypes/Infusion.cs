using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.UserInterface.Controls;
using System.Linq;
using System.Windows;

namespace ClickQuest.Game.Core.Items.ArtifactTypes
{
    public class Infusion : ArtifactTypeFunctionality
    {
        public override bool CanBeEquipped()
        {
            bool isAnArtifactEquipped = User.Instance.CurrentHero.EquippedArtifacts.Count > 0;
            bool isNoInfusionEquipped = User.Instance.CurrentHero.EquippedArtifacts.All(x => x.ArtifactType != ArtifactType.Infusion);

            if (isAnArtifactEquipped && isNoInfusionEquipped)
            {
                return true;
            }

            AlertBox.Show("This artifact cannot be equipped right now - it requires at least 1 other artifact to be equipped, and cannot be equipped with another Infusion.", MessageBoxButton.OK);
            return false;
        }

        public Infusion()
        {
            ArtifactType = ArtifactType.Infusion;
            Description = "Has to be equipped with at least 1 other Artifact, and cannot be equipped with another Infusion.";
        }
    }
}