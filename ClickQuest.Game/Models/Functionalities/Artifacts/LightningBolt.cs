using System;
using System.Collections.Generic;
using System.Linq;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Clicking on an enemy 3 times within 0.5 seconds deals a bonus 300 damage.
public class LightningBolt : ArtifactFunctionality
{
	private const int TimeFrame = 500;
	private const int ClicksRequired = 3;
	private const int DamageDealt = 300;

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
				CombatHelper.DealDamageToEnemy(clickedEnemy, DamageDealt, DamageType.Magic);
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