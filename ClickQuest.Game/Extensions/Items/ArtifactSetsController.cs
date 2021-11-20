using System.Linq;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Data;
using ClickQuest.Game.Extensions.Collections;
using ClickQuest.Game.Extensions.UserInterface;

namespace ClickQuest.Game.Extensions.Items
{
	public static class ArtifactSetsController
	{
		public static void SwitchArtifactSet(int targetSetId)
		{

			while (User.Instance.CurrentHero.EquippedArtifacts.Count > 0)
			{
				var currentIndex = User.Instance.CurrentHero.EquippedArtifacts.Count - 1;
				User.Instance.CurrentHero.EquippedArtifacts.RemoveAt(currentIndex);
				User.Instance.CurrentHero.EquippedArtifacts[currentIndex].ArtifactFunctionality.OnUnequip();
			}

			var newSet = User.Instance.CurrentHero.ArtifactSets.FirstOrDefault(x => x.Id == targetSetId);

			foreach (var artifactId in newSet.ArtifactIds)
			{
				var newArtifact = GameAssets.Artifacts.FirstOrDefault(x=>x.Id==artifactId);
				User.Instance.CurrentHero.EquippedArtifacts.Add(newArtifact);
				newArtifact.ArtifactFunctionality.OnEquip();
			}

			User.Instance.CurrentHero.CurrentArtifactSetId = targetSetId;
		}
	}
}