using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities.Affixes;

public class ImmuneToPoison : AffixFunctionality
{
	public override void OnDealingPoisonDamage(ref int poisonDamage)
	{
		poisonDamage = 0;
	}

	public ImmuneToPoison()
	{
		Affix = Affix.ImmuneToPoison;
	}
}