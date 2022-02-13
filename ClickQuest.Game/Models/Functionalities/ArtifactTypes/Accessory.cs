using System;
using System.Linq;
using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities.ArtifactTypes;

// If you have exactly two Accessories and an Amulet equipped, increase your Click Damage, Poison Damage, Critical Click Chance and Critical Click Damage by 20%.
public class Accessory : ArtifactTypeFunctionality
{
	private const double StatModifier = 0.20;

	// These need to be static, otherwise equipping the artifact and unequipping the others will not decrease the stats.
	public static int ClickDamageIncreased;
	public static int PoisonDamageIncreased;
	public static double CritChanceIncreased;
	public static double CritDamageIncreased;
	public static bool StatsIncreased;

	public override void OnEquip()
	{
		var hasTwoAccessories = User.Instance.CurrentHero.EquippedArtifacts.Count(x => x.ArtifactType == ArtifactType.Accessory) == 2;
		var hasAmulet = User.Instance.CurrentHero.EquippedArtifacts.Count(x => x.ArtifactType == ArtifactType.Amulet) == 1;

		if (hasTwoAccessories && hasAmulet)
		{
			ClickDamageIncreased = (int)Math.Ceiling(User.Instance.CurrentHero.ClickDamage * StatModifier);
			PoisonDamageIncreased = (int)Math.Ceiling(User.Instance.CurrentHero.PoisonDamage * StatModifier);
			CritChanceIncreased = 0.2;
			CritDamageIncreased = 0.2;

			User.Instance.CurrentHero.ClickDamage += ClickDamageIncreased;
			User.Instance.CurrentHero.PoisonDamage += PoisonDamageIncreased;
			User.Instance.CurrentHero.CritChance += CritChanceIncreased;
			User.Instance.CurrentHero.CritDamage += CritDamageIncreased;

			StatsIncreased = true;
		}
	}

	public override void OnUnequip()
	{
		if (StatsIncreased)
		{
			StatsIncreased = false;

			User.Instance.CurrentHero.ClickDamage -= ClickDamageIncreased;
			User.Instance.CurrentHero.PoisonDamage -= PoisonDamageIncreased;
			User.Instance.CurrentHero.CritChance -= CritChanceIncreased;
			User.Instance.CurrentHero.CritDamage -= CritDamageIncreased;

			ClickDamageIncreased = PoisonDamageIncreased = 0;
			CritChanceIncreased = 0.0;
			CritDamageIncreased = 0.0;
		}
	}

	public Accessory()
	{
		ArtifactType = ArtifactType.Accessory;
		Description = "If you have exactly two Accessories and an Amulet equipped, increase your <NORMAL>Click Damage</NORMAL>, <POISON>Poison Damage</POISON>, <CRITICAL>Critical Click Chance</CRITICAL> and <CRITICAL>Critical Click Damage</CRITICAL> by <BOLD>20%</BOLD>.";
	}
}