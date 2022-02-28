using System;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Increases your Aura Damage by 25%, but reduces your Click Damage by 25%.
public class BatWingBackpack : ArtifactFunctionality
{
	private const double DamageModifier = 0.25;

	private double _auraDamageIncreased;
	private int _clickDamageDecreased;

	public override void OnEquip()
	{
		_auraDamageIncreased = User.Instance.CurrentHero.AuraDamage * DamageModifier;
		_clickDamageDecreased = (int)Math.Ceiling(User.Instance.CurrentHero.ClickDamage * DamageModifier);

		User.Instance.CurrentHero.AuraDamage += _auraDamageIncreased;
		User.Instance.CurrentHero.ClickDamage -= _clickDamageDecreased;
		
		base.OnEquip();
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.AuraDamage -= _auraDamageIncreased;
		User.Instance.CurrentHero.ClickDamage += _clickDamageDecreased;

		_auraDamageIncreased = 0;
		_clickDamageDecreased = 0;
		
		base.OnUnequip();
	}

	public BatWingBackpack()
	{
		Name = "Bat Wing Backpack";
	}
}