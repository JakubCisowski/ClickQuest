using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities.Affixes;

public class ImmuneToMagicDamage : AffixFunctionality
{
	public override void OnDealingMagicDamage(ref int magicDamage)
	{
		magicDamage = 0;
	}

	public ImmuneToMagicDamage()
	{
		Affix = Affix.ImmuneToMagicDamage;
	}
}