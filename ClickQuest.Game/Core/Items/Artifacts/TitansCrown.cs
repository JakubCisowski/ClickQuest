using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.Core.Items.Artifacts
{
	// All experience gained is increased by 30%.
	public class TitansCrown : ArtifactFunctionality
	{
		private const double BonusExperienceMultiplier = 0.30;

		public override void OnExperienceGained(int experienceGained)
		{
			int bonusExperience = (int) (experienceGained * BonusExperienceMultiplier);
			User.Instance.CurrentHero.GainExperience(bonusExperience, true);
		}

		public TitansCrown()
		{
			Name = "Titan's Crown";
		}
	}
}