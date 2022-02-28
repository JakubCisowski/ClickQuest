using System;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Increases all Magic Damage dealt by 10%. Additionally, consuming Ammunition deals 20 Magic Damage to the current Enemy.
public class ElementalBow : ArtifactFunctionality
{
	private const double MagicDamageModifier = 1.1;
	private const int DamageDealtUponConsuming = 20;

	public override void OnDealingMagicDamage(ref int magicDamage)
	{
		magicDamage = (int)Math.Ceiling(magicDamage * MagicDamageModifier );
	}

	public override void OnConsumed(Artifact artifactConsumed)
	{
		CombatHelper.DealDamageToCurrentEnemy(DamageDealtUponConsuming, DamageType.Magic);
	}

	public ElementalBow()
	{
		Name = "Elemental Bow";
	}
}