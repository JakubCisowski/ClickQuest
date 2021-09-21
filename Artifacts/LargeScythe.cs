using System;
using System.Linq;
using System.Windows;
using ClickQuest.Controls;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Interfaces;
using ClickQuest.Items;
using ClickQuest.Pages;

namespace ClickQuest.Artifacts
{
	// Your clicks deal 15% increased damages against Monsters (excluding Bosses).
	public class LargeScythe : ArtifactFunctionality
	{
		private const double DamageIncreasePercent = 0.15;

		public override void OnDealingDamage(int baseDamage)
		{
			CombatController.DealDamageToMonster((int)(DamageIncreasePercent * baseDamage), DamageType.Artifact);
		}

		public LargeScythe()
		{
			Name = "Large Scythe";
		}
	}
}