namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// All experience gained is increased by 30%.
public class TitansCrown : ArtifactFunctionality
{
	private const double BonusExperienceMultiplier = 0.30;

	public override void OnExperienceGained(ref int experienceGained)
	{
		var bonusExperience = (int)(experienceGained * BonusExperienceMultiplier);
		experienceGained += bonusExperience;
	}

	public TitansCrown()
	{
		Name = "Titan's Crown";
	}
}