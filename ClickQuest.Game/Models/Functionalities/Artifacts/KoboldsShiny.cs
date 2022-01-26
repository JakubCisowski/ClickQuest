using System;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Increases all damage dealt by 25%, but reduces your Aura Damage by 50%.
public class KoboldsShiny : ArtifactFunctionality
{
	private const double DamageIncreaseModifier = 0.25;
	private const double AuraDamageReductionModifier = 0.50;

	public override void OnDealingDamage(ref int damage)
	{
		damage += (int)Math.Ceiling(damage * DamageIncreaseModifier);
	}

	public override void OnDealingAuraDamage(ref int auraDamage)
	{
		auraDamage = (int)(auraDamage * AuraDamageReductionModifier);
	}

	public KoboldsShiny()
	{
		Name = "Kobold's Shiny";
	}
}