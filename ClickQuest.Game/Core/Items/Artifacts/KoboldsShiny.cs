using System;
using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.Core.Items.Artifacts;

// Increases all damage dealt by 25%, but reduces your Aura Damage by 50%.
public class KoboldsShiny : ArtifactFunctionality
{
	private const double DamageIncreaseModifier = 0.25;
	private const double AuraDamageReductionModifier = 0.50;

	public override void OnEquip()
	{
		User.Instance.CurrentHero.AuraDamage -= User.Instance.CurrentHero.AuraDamage * AuraDamageReductionModifier;
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.AuraDamage += User.Instance.CurrentHero.AuraDamage * AuraDamageReductionModifier;
	}

	public override void OnDealingDamage(ref int damage)
	{
		damage += (int)Math.Ceiling(damage * DamageIncreaseModifier);
	}

	public KoboldsShiny()
	{
		Name = "Kobold's Shiny";
	}
}