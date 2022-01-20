using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities.Affixes;

public class ImmuneToArtifactDamage : AffixFunctionality
{
	public override void OnDealingArtifactDamage(ref int artifactDamage)
	{
		artifactDamage = 0;
	}

	public ImmuneToArtifactDamage()
	{
		Affix = Affix.ImmuneToArtifactDamage;
	}
}