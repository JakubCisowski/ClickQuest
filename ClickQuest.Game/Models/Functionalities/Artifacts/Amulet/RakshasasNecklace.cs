﻿namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Increases Aura Attack Speed by 50% (additively).
public class RakshasasNecklace : ArtifactFunctionality
{
	private const double AuraAttackSpeedBonus = 0.5;

	public override void OnEquip()
	{
		User.Instance.CurrentHero.AuraAttackSpeed += AuraAttackSpeedBonus;
		
		base.OnEquip();
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.AuraAttackSpeed -= AuraAttackSpeedBonus;
		
		base.OnUnequip();
	}

	public RakshasasNecklace()
	{
		Name = "Rakshasa's Necklace";
	}
}