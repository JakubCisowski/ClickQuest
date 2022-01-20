using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities.Affixes;

public class CanOnlyBeCrit : AffixFunctionality
{
	public override void OnDealingClickDamage(ref int clickDamage, DamageType clickDamageType)
	{
		if (clickDamageType != DamageType.Critical)
		{
			clickDamage = 0;
		}
	}

	public CanOnlyBeCrit()
	{
		Affix = Affix.CanOnlyBeCrit;
	}
}