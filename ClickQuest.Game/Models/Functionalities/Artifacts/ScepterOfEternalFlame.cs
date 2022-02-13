using System;
using System.Windows.Threading;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// On click, applies burning for 2 seconds that deals a total of 600 damage.
public class ScepterOfEternalFlame : ArtifactFunctionality
{
	private const int BurningDamage = 600;
	private const double BurningDuration = 2;
	private const int TicksNumber = 10;

	private int _ticksCount;

	private readonly DispatcherTimer _timer;

	public override void OnEnemyClick(Enemy clickedEnemy)
	{
		_timer.Start();
		_ticksCount = 0;
		
		base.OnEnemyClick(clickedEnemy);
	}

	public override void OnKill()
	{
		_timer.Stop();
		
		base.OnKill();
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
			Interval = new TimeSpan(0, 0, 0, 0, (int)(BurningDuration / TicksNumber * 1000d))
		};
		_timer.Tick += BurningTimer_Tick;
	}

	private void BurningTimer_Tick(object source, EventArgs e)
	{
		_ticksCount++;

		const int damage = BurningDamage / TicksNumber;

		CombatHelper.DealDamageToCurrentEnemy(damage, DamageType.Magic);

		if (_ticksCount == TicksNumber)
		{
			_ticksCount = 0;
			_timer.Stop();
		}
	}
}