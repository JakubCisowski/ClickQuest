using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	public class TitansCrown : ArtifactFunctionality
	{
		private const double BonusExperienceMultiplier = 0.30;

		// todo: trigger this somewhere
		public override void OnExperienceGained(int experienceGained)
		{
			int bonusExperience = (int) (experienceGained * BonusExperienceMultiplier);
			User.Instance.CurrentHero.Experience += bonusExperience;
		}

		public TitansCrown()
		{
			Name = "Titan's Crown";
		}
	}
}