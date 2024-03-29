﻿namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Gain 20% Critical Click Damage.
public class MeatCleaver : ArtifactFunctionality
{
	private const double CritDamageIncrease = 0.20;

	public override void OnEquip()
	{
		User.Instance.CurrentHero.CritDamage += CritDamageIncrease;
		
		base.OnEquip();
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.CritDamage -= CritDamageIncrease;
		
		base.OnUnequip();
	}

	public MeatCleaver()
	{
		Name = "Meat Cleaver";
	}
}