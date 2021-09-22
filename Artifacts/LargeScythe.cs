﻿using System;
using System.Linq;
using System.Windows;
using ClickQuest.Controls;
using ClickQuest.Enemies;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Interfaces;
using ClickQuest.Items;
using ClickQuest.Pages;

namespace ClickQuest.Artifacts
{
	// All damage dealt by you is increased by 15% against Monsters (excluding Bosses).
	public class LargeScythe : ArtifactFunctionality
	{
		private const double DamageIncreasePercent = 0.15;

		public override void OnDealingDamage(int baseDamage)
		{
			if (InterfaceController.CurrentEnemy is Monster)
			{
				int damageDealt = (int) DamageIncreasePercent * baseDamage;
			
				CombatController.DealDamageToEnemy(damageDealt, DamageType.Artifact);
			}
		}

		public LargeScythe()
		{
			Name = "Large Scythe";
		}
	}
}