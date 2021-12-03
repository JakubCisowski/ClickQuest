using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.UserInterface.Pages;
using System.Linq;

namespace ClickQuest.Game.Extensions.Items
{
    public static class ArtifactSetsController
    {
        public static void SwitchArtifactSet(int targetSetId)
        {
            while (User.Instance.CurrentHero.EquippedArtifacts.Count > 0)
            {
                int currentIndex = User.Instance.CurrentHero.EquippedArtifacts.Count - 1;

                Artifact artifactRemoved = User.Instance.CurrentHero.EquippedArtifacts[currentIndex];

                User.Instance.CurrentHero.EquippedArtifacts.Remove(artifactRemoved);
                artifactRemoved.ArtifactFunctionality.OnUnequip();
            }

            ArtifactSet? newSet = User.Instance.CurrentHero.ArtifactSets.FirstOrDefault(x => x.Id == targetSetId);

            foreach (int artifactId in newSet.ArtifactIds)
            {
                Artifact? newArtifact = GameAssets.Artifacts.FirstOrDefault(x => x.Id == artifactId);
                User.Instance.CurrentHero.EquippedArtifacts.Add(newArtifact);
                newArtifact.ArtifactFunctionality.OnEquip();
            }

            User.Instance.CurrentHero.CurrentArtifactSetId = targetSetId;
        }

        public static bool CanArtifactSetsBeChanged()
        {
            if (User.Instance.CurrentHero.Quests.Any(x => x.EndDate != default))
            {
                return false;
            }

            if (GameAssets.CurrentPage is RegionPage or DungeonBossPage)
            {
                return false;
            }

            return true;
        }
    }
}