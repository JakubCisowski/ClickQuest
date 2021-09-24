﻿using ClickQuest.Enemies;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Items;

namespace ClickQuest.Artifacts
{
	// Clicks against Monsters deal additional damage equal to 5% of their maximum health. Does not work against Bosses.
	public class PoisonedNagaKnife : ArtifactFunctionality
	{
		private const double DamageDealtPercentage = 0.05;
		
		public override void OnEnemyClick()
		{
			if (InterfaceController.CurrentEnemy is Monster monster)
			{
				CombatController.DealDamageToEnemy((int)(monster.Health * DamageDealtPercentage), DamageType.Artifact);
			}
		}

		public PoisonedNagaKnife()
		{
			Name = "Poisoned Naga Knife";
		}
	}
}