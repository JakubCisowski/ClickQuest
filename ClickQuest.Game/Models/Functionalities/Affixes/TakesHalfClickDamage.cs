using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities.Affixes;

public class TakesHalfClickDamage : AffixFunctionality
{
	public override void OnDealingClickDamage(ref int clickDamage, DamageType clickDamageType)
	{
		clickDamage /= 2;
	}

	public TakesHalfClickDamage()
	{
		Affix = Affix.TakesHalfClickDamage;
	}
}