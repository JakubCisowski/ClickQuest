using System.Linq;
using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities.ArtifactTypes;

public class MeleeWeapon : ArtifactTypeFunctionality
{
	private const int ClickDamageIncrease = 10;
	private const double CritChanceIncrease = 0.10;

	private bool _statsIncreased;

	public override void OnEquip()
	{
		var hasTwoMeleeWeapons = User.Instance.CurrentHero.EquippedArtifacts.Count(x => x.ArtifactType == ArtifactType.MeleeWeapon) == 2;

		if (hasTwoMeleeWeapons)
		{
			User.Instance.CurrentHero.ClickDamage += ClickDamageIncrease;
			User.Instance.CurrentHero.CritChance += CritChanceIncrease;
			_statsIncreased = true;
		}
	}

	public override void OnUnequip()
	{
		if (_statsIncreased)
		{
			_statsIncreased = false;

			User.Instance.CurrentHero.ClickDamage -= ClickDamageIncrease;
			User.Instance.CurrentHero.CritChance -= CritChanceIncrease;
		}
	}

	public MeleeWeapon()
	{
		ArtifactType = ArtifactType.MeleeWeapon;
		Description = "If you have exactly two Melee Weapons equipped, gain 10 Click Damage and 10% Critical Click Chance.";
	}
}