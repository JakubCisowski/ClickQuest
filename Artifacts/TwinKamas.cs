using System;
using System.Windows.Threading;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Items;

namespace ClickQuest.Artifacts
{
	// Counts as two artifacts, therefore requires two free slots to use. After clicking on a monster, strike again after a 1s delay.
	// The second attack can critically hit, as well as apply on-hit effects (such as poison).
	public class TwinKamas : ArtifactFunctionality
	{
		private const int StrikeDelay = 1;

		private readonly DispatcherTimer _timer;

		public override void OnEnemyClick()
		{
			_timer.Start();
		}

		public TwinKamas()
		{
			Name = "Twin Kamas";
			ArtifactSlotsRequired = 2;
			_timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, StrikeDelay)};
			_timer.Tick += SecondStrikeTimer_Tick;
		}

		private void SecondStrikeTimer_Tick(object source, EventArgs e)
		{
			// Instructions in this order should not trigger this artifact's effect again.
			CombatController.HandleUserClickOnEnemy();
			_timer.Stop();
		}
	}
}