using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using ClickQuest.Game.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// After clicking on an Enemy, strike again after a 1s delay.
// The second attack can critically hit, as well as apply on-hit effects (such as poison).
public class TwinKamas : ArtifactFunctionality
{
	private const int StrikeDelay = 1;

	private readonly DispatcherTimer _timer;

	private readonly List<DateTime> _clickTimes;

	public override void OnEnemyClick(Enemy clickedEnemy)
	{
		_clickTimes.Add(DateTime.Now);
		
		_timer.Start();
		
		base.OnEnemyClick(clickedEnemy);
	}

	public TwinKamas()
	{
		Name = "Twin Kamas";
		ArtifactSlotsRequired = 2;
		_timer = new DispatcherTimer
		{
			Interval = new TimeSpan(0, 0, 0, 0, 100)
		};
		_timer.Tick += SecondStrikeTimer_Tick;

		_clickTimes = new List<DateTime>();
	}

	private void SecondStrikeTimer_Tick(object source, EventArgs e)
	{
		while (true)
		{
			var dateTime = _clickTimes.FirstOrDefault();

			// If _clickTimes is empty, stop the timer.
			if (dateTime == default)
			{
				_timer.Stop();
				break;
			}

			// If at least (StrikeDelay) seconds has passed, click again. If not, wait (for next tick).
			if (DateTime.Now - dateTime >= TimeSpan.FromSeconds(StrikeDelay))
			{
				CombatHelper.HandleUserClickOnEnemy();
				_clickTimes.RemoveAt(0);
				
				// Remove the artificial click (should be the last one in the list).
				_clickTimes.RemoveAt(_clickTimes.Count - 1);
			}
			else
			{
				break;
			}
		}
	}
}