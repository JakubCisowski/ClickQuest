﻿using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Extensions.UserInterface;

namespace ClickQuest.Game.Core.Items.Artifacts
{
	// All non-artifact damage dealt by you is increased by 10% against Bosses.
	public class SilverGreatsword : ArtifactFunctionality
	{
		private const double DamageIncreasePercent = 0.10;

		public override void OnDealingDamage(ref int baseDamage)
		{
			if (InterfaceController.CurrentEnemy is Boss)
			{
				var bonusDamage = (int) (DamageIncreasePercent * baseDamage);

				baseDamage += bonusDamage;
			}
		}

		public SilverGreatsword()
		{
			Name = "Silver Greatsword";
		}
	}
}