using System;
using System.Collections.Generic;
using System.Linq;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Extensions.Combat;

namespace ClickQuest.Game.Core.Items.Artifacts
{
	// Clicking on an enemy 3 times within 0.5 (?) seconds deals a bonus 20 (?) damage.
	public class LightningBolt : ArtifactFunctionality
	{
		private const int TimeFrame = 500;
		private const int ClicksRequired = 3;
		private const int DamageDealt = 20;

		private readonly List<DateTime> _clickTimes;

		public override void OnEnemyClick(Enemy clickedEnemy)
		{
			_clickTimes.Add(DateTime.Now);

			if (_clickTimes.Count == ClicksRequired)
			{
				var requiredTimeSpan = TimeSpan.FromMilliseconds(TimeFrame);
				var actualTimeSpan = _clickTimes.Last() - _clickTimes.First();

				if (actualTimeSpan <= requiredTimeSpan)
				{
					CombatController.DealDamageToEnemy(clickedEnemy, DamageDealt, DamageType.Artifact);
				}

				_clickTimes.Clear();
			}
		}

		public override void OnRegionLeave()
		{
			_clickTimes.Clear();
		}

		public LightningBolt()
		{
			Name = "Lightning Bolt";
			_clickTimes = new List<DateTime>();
		}
	}
}