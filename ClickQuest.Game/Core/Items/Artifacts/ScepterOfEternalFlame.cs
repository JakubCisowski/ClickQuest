using System;
using System.Windows.Threading;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Extensions.Combat;

namespace ClickQuest.Game.Core.Items.Artifacts
{
	// On click, applies burning for 2 seconds that deals a total of 80 (?) damage.
	public class ScepterOfEternalFlame : ArtifactFunctionality
	{
		private const int BurningDamage = 80;
		private const double BurningDuration = 2;
		private const int TicksNumber = 10;

		private int _ticksCount;

		private readonly DispatcherTimer _timer;

		public override void OnEnemyClick(Enemy clickedEnemy)
		{
			_timer.Start();
			_ticksCount = 0;
		}

		public override void OnKill()
		{
			_timer.Stop();
		}

		public override void OnRegionLeave()
		{
			_timer.Stop();
		}

		public ScepterOfEternalFlame()
		{
			Name = "Scepter of Eternal Flame";
			_timer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 0, (int) (BurningDuration / TicksNumber * 1000d))
			};
			_timer.Tick += BurningTimer_Tick;
		}

		private void BurningTimer_Tick(object source, EventArgs e)
		{
			_ticksCount++;

			const int damage = BurningDamage / TicksNumber;

			CombatController.DealDamageToCurrentEnemy(damage, DamageType.Artifact);

			if (_ticksCount == TicksNumber)
			{
				_ticksCount = 0;
				_timer.Stop();
			}
		}
	}
}