using System;
using static ClickQuest.Game.Helpers.RandomnessHelper;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Your poison damage can now critically strike. The probability is calculated using your Critical Click Chance, and the damage using your Critical Click Damage.
public class WillOfTheAncients : ArtifactFunctionality
{
	public override void OnDealingPoisonDamage(ref int poisonDamage)
	{
		var critChance = User.Instance.CurrentHero.CritChance;
		var critDamage = User.Instance.CurrentHero.CritDamage;
		var randomizedValue = Rng.Next(1, 101) / 100d;

		if (randomizedValue <= critChance)
		{
			poisonDamage = (int)Math.Ceiling(poisonDamage * critDamage);
		}
	}

	public WillOfTheAncients()
	{
		Name = "Will of the Ancients";
	}
}