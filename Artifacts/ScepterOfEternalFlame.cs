using System;
using System.Windows.Threading;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Items;

namespace ClickQuest.Artifacts
{
	// On click, applies burning for 2 seconds that deals a total of 80 (?) damage.
	public class ScepterOfEternalFlame : ArtifactFunctionality
	{
		private const int BurningDamage = 80;
		private const int BurningDuration = 2;
		private const int TicksNumber = 10;

		private int _ticksCount;

		private readonly DispatcherTimer _timer;

		public override void OnEnemyClick()
		{
			_timer.Start();
		}

		public ScepterOfEternalFlame()
		{
			Name = "Scepter of Eternal Flame";
			_timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, BurningDuration / TicksNumber * 100)};
			_timer.Tick += BurningTimer_Tick;
		}

		private void BurningTimer_Tick(object source, EventArgs e)
		{
			_ticksCount++;

			int damage = BurningDamage / TicksNumber;

			CombatController.DealDamageToEnemy(damage, DamageType.Artifact);

			if (_ticksCount == TicksNumber)
			{
				_ticksCount = 0;
				_timer.Stop();
			}
		}
	}
}