using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Your poison damage is converted to Artifact Damage and increased by 50%.
// Additionally, all Artifact damage dealt is increased by 30%.
public class TomeOfDestructiveMagic : ArtifactFunctionality
{
	private const double DamageConversionRatio = 1.5;
	private const double ArtifactDamageIncreaseRatio = 1.3;
	
	public override void OnDealingPoisonDamage(ref int poisonDamage)
	{
		int convertedDamage = (int)(poisonDamage * DamageConversionRatio);
		poisonDamage = 0;
		
		CombatHelper.DealDamageToCurrentEnemy(convertedDamage, DamageType.Artifact);
	}

	public override void OnDealingArtifactDamage(ref int artifactDamage)
	{
		artifactDamage = (int)(artifactDamage * ArtifactDamageIncreaseRatio);
	}

	public TomeOfDestructiveMagic()
	{
		Name = "Tome of Destructive Magic";
	}
}