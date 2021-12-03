using System;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;

namespace ClickQuest.Game.Core.Items.ArtifactTypes
{
	public class MightyWeapon : ArtifactTypeFunctionality
	{
		private const double ClickDamagePercentageAsBonusDamage = 0.05;
		
		public override void OnEnemyClick(Enemy clickedEnemy)
		{
			int bonusDamageDealt = (int) Math.Ceiling(User.Instance.CurrentHero.ClickDamage * ClickDamagePercentageAsBonusDamage * User.Instance.CurrentHero.CritDamage);
			
			CombatController.DealDamageToEnemy(clickedEnemy, bonusDamageDealt, DamageType.Artifact);
		}

		public MightyWeapon()
		{
			ArtifactType = ArtifactType.MightyWeapon;
			Description = "For each Mighty Weapon you have equipped, deal additional on-hit damage. The damage is equal to 5% of your Click Damage multiplied by your Critical Click Damage.";
		}
	}
}