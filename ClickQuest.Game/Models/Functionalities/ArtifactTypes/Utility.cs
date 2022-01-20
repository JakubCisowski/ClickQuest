using System.Linq;
using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities.ArtifactTypes;

// Each equipped Utility Artifact increases the amount of Experience you gain by 10%.
public class Utility : ArtifactTypeFunctionality
{
	private const double ExperienceModifierPerArtifact = 0.10;

	public override void OnExperienceGained(int experienceGained)
	{
		var amountOfUtilityArtifacts = User.Instance.CurrentHero.EquippedArtifacts.Count(x => x.ArtifactType == ArtifactType.Utility);

		if (amountOfUtilityArtifacts != 0)
		{
			var bonusExperience = (int)(amountOfUtilityArtifacts * experienceGained * ExperienceModifierPerArtifact);

			User.Instance.CurrentHero.GainExperience(bonusExperience, true);
		}
	}

	public Utility()
	{
		ArtifactType = ArtifactType.Utility;
		Description = "Each equipped Utility Artifact increases the amount of Experience you gain by 10%.";
	}
}