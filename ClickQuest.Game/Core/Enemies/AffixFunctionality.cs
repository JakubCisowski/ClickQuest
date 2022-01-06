namespace ClickQuest.Game.Core.Enemies;

public class AffixFunctionality
{
	public Affix Affix { get; set; }

	// Use to increase poison damage dealt (eg. by a percentage).
	public virtual void OnDealingPoisonDamage(ref int poisonDamage)
	{
	}
}