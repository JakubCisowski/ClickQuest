﻿using System.Linq;
using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities.ArtifactTypes;

// If you have exactly two Accessories and an Amulet equipped, increase your Click Damage, Poison Damage, Critical Click Chance and Critical Click Damage by 20%.
public class Amulet : ArtifactTypeFunctionality
{
	private const double StatModifier = 0.20;

	private int _clickDamageIncreased;
	private int _poisonDamageIncreased;
	private double _critChanceIncreased;
	private double _critDamageIncreased;
	private bool _statsIncreased;

	public override void OnEquip()
	{
		var hasTwoAccessories = User.Instance.CurrentHero.EquippedArtifacts.Count(x => x.ArtifactType == ArtifactType.Accessory) == 2;
		var hasAmulet = User.Instance.CurrentHero.EquippedArtifacts.Count(x => x.ArtifactType == ArtifactType.Amulet) == 1;

		if (hasTwoAccessories && hasAmulet)
		{
			_clickDamageIncreased = (int)(User.Instance.CurrentHero.ClickDamage * StatModifier);
			_poisonDamageIncreased = (int)(User.Instance.CurrentHero.PoisonDamage * StatModifier);
			_critChanceIncreased = 0.2;
			_critDamageIncreased = 0.2;

			User.Instance.CurrentHero.ClickDamage += _clickDamageIncreased;
			User.Instance.CurrentHero.PoisonDamage += _poisonDamageIncreased;
			User.Instance.CurrentHero.CritChance += _critChanceIncreased;
			User.Instance.CurrentHero.CritDamage += _critDamageIncreased;

			_statsIncreased = true;
		}
	}

	public override void OnUnequip()
	{
		if (_statsIncreased)
		{
			_statsIncreased = false;

			User.Instance.CurrentHero.ClickDamage -= _clickDamageIncreased;
			User.Instance.CurrentHero.PoisonDamage -= _poisonDamageIncreased;
			User.Instance.CurrentHero.CritChance -= _critChanceIncreased;
			User.Instance.CurrentHero.CritDamage -= _critDamageIncreased;

			_clickDamageIncreased = _poisonDamageIncreased = 0;
			_critChanceIncreased = 0.0;
			_critDamageIncreased = 0.0;
		}
	}

	public Amulet()
	{
		ArtifactType = ArtifactType.Amulet;
		Description = "If you have exactly two Accessories and an Amulet equipped, increase your <NORMAL>Click Damage</NORMAL>, <POISON>Poison Damage</POISON>, <CRITICAL>Critical Click Chance</CRITICAL> and <CRITICAL>Critical Click Damage</CRITICAL> by <BOLD>20%</BOLD>.";
	}
}