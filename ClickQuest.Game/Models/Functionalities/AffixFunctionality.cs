using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities;

public class AffixFunctionality
{
	public Affix Affix { get; set; }
	
	// Used to modify or trigger an effect based on the amount of click damage dealt (regardless if critical or not).
	public virtual void OnDealingClickDamage(ref int clickDamage, DamageType clickDamageType)
	{
	}

	// Used to modify or trigger an effect based on the amount of poison dealt.
	public virtual void OnDealingPoisonDamage(ref int poisonDamage)
	{
	}
	
	// Used to modify or trigger an effect based on the amount of magic damage dealt.
	public virtual void OnDealingMagicDamage(ref int magicDamage)
	{
	}
}