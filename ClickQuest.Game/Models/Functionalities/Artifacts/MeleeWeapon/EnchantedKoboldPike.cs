﻿namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Increases your Poison Damage by 10, and your Critical Click Damage by 20%.
public class EnchantedKoboldPike : ArtifactFunctionality
{
	private const int PoisonDamageIncrease = 10;
	private const double CritDamageIncrease = 0.20;

	public override void OnEquip()
	{
		User.Instance.CurrentHero.PoisonDamage += PoisonDamageIncrease;
		User.Instance.CurrentHero.CritDamage += CritDamageIncrease;
		
		base.OnEquip();
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.PoisonDamage -= PoisonDamageIncrease;
		User.Instance.CurrentHero.CritDamage -= CritDamageIncrease;
		
		base.OnUnequip();
	}

	public EnchantedKoboldPike()
	{
		Name = "Enchanted Kobold Pike";
	}
}