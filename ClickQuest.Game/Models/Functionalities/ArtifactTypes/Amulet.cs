using System;
using System.Linq;
using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities.ArtifactTypes;

// If you have exactly two Accessories and an Amulet equipped, increase your Click Damage, Poison Damage, Critical Click Chance and Critical Click Damage by 20%.
public class Amulet : ArtifactTypeFunctionality
{
	private const double StatModifier = 0.20;

	// We will use the fields from the Accessory class, to prevent equipping an Accessory and unequipping an Amulet from not decreasing stats.
	// private int _clickDamageIncreased;
	// private int _poisonDamageIncreased;
	// private double _critChanceIncreased;
	// private double _critDamageIncreased;
	// private bool _statsIncreased;

	public override void OnEquip()
	{
		var hasTwoAccessories = User.Instance.CurrentHero.EquippedArtifacts.Count(x => x.ArtifactType == ArtifactType.Accessory) == 2;
		var hasAmulet = User.Instance.CurrentHero.EquippedArtifacts.Count(x => x.ArtifactType == ArtifactType.Amulet) == 1;

		if (hasTwoAccessories && hasAmulet)
		{
			Accessory.ClickDamageIncreased = (int)Math.Ceiling(User.Instance.CurrentHero.ClickDamage * StatModifier);
			Accessory.PoisonDamageIncreased = (int)Math.Ceiling(User.Instance.CurrentHero.PoisonDamage * StatModifier);
			Accessory.CritChanceIncreased = 0.2;
			Accessory.CritDamageIncreased = 0.2;

			User.Instance.CurrentHero.ClickDamage += Accessory.ClickDamageIncreased;
			User.Instance.CurrentHero.PoisonDamage += Accessory.PoisonDamageIncreased;
			User.Instance.CurrentHero.CritChance += Accessory.CritChanceIncreased;
			User.Instance.CurrentHero.CritDamage += Accessory.CritDamageIncreased;

			Accessory.StatsIncreased = true;
		}
	}

	public override void OnUnequip()
	{
		if (Accessory.StatsIncreased)
		{
			Accessory.StatsIncreased = false;

			User.Instance.CurrentHero.ClickDamage -= Accessory.ClickDamageIncreased;
			User.Instance.CurrentHero.PoisonDamage -= Accessory.PoisonDamageIncreased;
			User.Instance.CurrentHero.CritChance -= Accessory.CritChanceIncreased;
			User.Instance.CurrentHero.CritDamage -= Accessory.CritDamageIncreased;

			Accessory.ClickDamageIncreased = Accessory.PoisonDamageIncreased = 0;
			Accessory.CritChanceIncreased = Accessory.CritDamageIncreased = 0.0;
		}
	}

	public Amulet()
	{
		ArtifactType = ArtifactType.Amulet;
		Description = "If you have exactly two Accessories and an Amulet equipped, increase your <NORMAL>Click Damage</NORMAL>, <POISON>Poison Damage</POISON>, <CRITICAL>Critical Click Chance</CRITICAL> and <CRITICAL>Critical Click Damage</CRITICAL> by <BOLD>20%</BOLD>.";
	}
}