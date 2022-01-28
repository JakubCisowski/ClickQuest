using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Your poison damage is converted to Magic Damage and increased by 50%.
// Additionally, all Magic damage dealt is increased by 30%.
public class TomeOfDestructiveMagic : ArtifactFunctionality
{
	private const double DamageConversionRatio = 1.5;
	private const double MagicDamageIncreaseRatio = 1.3;
	
	public override void OnDealingPoisonDamage(ref int poisonDamage)
	{
		int convertedDamage = (int)(poisonDamage * DamageConversionRatio);
		poisonDamage = 0;
		
		CombatHelper.DealDamageToCurrentEnemy(convertedDamage, DamageType.Magic);
	}

	public override void OnDealingMagicDamage(ref int magicDamage)
	{
		magicDamage = (int)(magicDamage * MagicDamageIncreaseRatio);
	}

	public TomeOfDestructiveMagic()
	{
		Name = "Tome of Destructive Magic";
	}
}