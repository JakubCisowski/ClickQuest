﻿using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Extensions.UserInterface;

namespace ClickQuest.Game.Core.Items.Artifacts
{
	// All non-artifact damage dealt by you is increased by 15% against Monsters (excluding Bosses).
	public class LargeScythe : ArtifactFunctionality
	{
		private const double DamageIncreasePercent = 0.15;

		public override void OnDealingDamage(ref int baseDamage)
		{
			if (InterfaceController.CurrentEnemy is Monster)
			{
				int bonusDamage = (int) (DamageIncreasePercent * baseDamage);

				baseDamage += bonusDamage;
			}
		}

		public LargeScythe()
		{
			Name = "Large Scythe";
		}
	}
}