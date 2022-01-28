using System;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;

namespace ClickQuest.Game.Models.Functionalities.ArtifactTypes;

// For each Mighty Weapon you have equipped, deal additional on-hit damage.
// The damage is equal to 5% of your Click Damage multiplied by your Critical Click Damage.
public class MightyWeapon : ArtifactTypeFunctionality
{
	private const double ClickDamagePercentageAsBonusDamage = 0.05;

	public override void OnEnemyClick(Enemy clickedEnemy)
	{
		var bonusDamageDealt = (int)Math.Ceiling(User.Instance.CurrentHero.ClickDamage * ClickDamagePercentageAsBonusDamage * User.Instance.CurrentHero.CritDamage);

		CombatHelper.DealDamageToEnemy(clickedEnemy, bonusDamageDealt, DamageType.Magic);
	}

	public MightyWeapon()
	{
		ArtifactType = ArtifactType.MightyWeapon;
		Description = "For each Mighty Weapon you have equipped, deal additional on-hit damage. The damage is equal to 5% of your Click Damage multiplied by your Critical Click Damage.";
	}
}