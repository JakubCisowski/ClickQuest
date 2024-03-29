﻿namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Gain 50% Critical Click Chance. Excess critical chance is converted to Critical Click Damage (unlimited).
public class ScytheOfTheWorldEater : ArtifactFunctionality
{
	private const double CritChanceIncrease = 0.50;

	private double _critDamageIncreased;

	public override void OnEquip()
	{
		User.Instance.CurrentHero.CritChance += CritChanceIncrease;

		if (User.Instance.CurrentHero.CritChance > 1.0)
		{
			_critDamageIncreased = User.Instance.CurrentHero.CritChance - 1.0;
			User.Instance.CurrentHero.CritDamage += _critDamageIncreased;
		}
		
		base.OnEquip();
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.CritChance -= CritChanceIncrease;

		if (_critDamageIncreased > 0)
		{
			User.Instance.CurrentHero.CritDamage -= _critDamageIncreased;
			_critDamageIncreased = 0;
		}
		
		base.OnUnequip();
	}

	public ScytheOfTheWorldEater()
	{
		Name = "Scythe of the World Eater";
	}
}