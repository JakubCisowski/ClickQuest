using System.Linq;
using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.Core.Items.ArtifactTypes
{
	public class Utility : ArtifactTypeFunctionality
	{
		private const double ExperienceModifierPerArtifact = 0.10;
		
		public override void OnExperienceGained(int experienceGained)
		{
			int amountOfUtilityArtifacts = User.Instance.CurrentHero.EquippedArtifacts.Count(x => x.ArtifactType == ArtifactType.Utility);

			if (amountOfUtilityArtifacts != 0)
			{
				int bonusExperience = (int) (amountOfUtilityArtifacts * experienceGained * ExperienceModifierPerArtifact);
				
				User.Instance.CurrentHero.GainExperience(bonusExperience, true);
			}
		}

		public Utility()
		{
			ArtifactType = ArtifactType.Utility;
			Description = "Each equipped Utility Artifact increases the amount of Experience you gain by 10%.";
		}
	}
}