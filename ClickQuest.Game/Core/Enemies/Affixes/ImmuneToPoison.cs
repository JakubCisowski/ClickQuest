namespace ClickQuest.Game.Core.Enemies.Affixes;

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