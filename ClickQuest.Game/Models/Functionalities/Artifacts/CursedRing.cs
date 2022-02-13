using System;
using System.Windows.Threading;
using ClickQuest.Game.UserInterface.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// After 3 seconds in combat with an Enemy, curse them, increasing all damage they take by 20%.
public class CursedRing : ArtifactFunctionality
{
	private const int TimeToCurseEnemy = 3;
	private const double DamageIncreaseModifier = 0.20;

	private bool _isCurseActive;
	private Enemy _currentEnemy;
	private readonly DispatcherTimer _timer;

	public override void OnRegionEnter()
	{
		_isCurseActive = false;
		_currentEnemy = InterfaceHelper.CurrentEnemy;
		_timer.Start();
	}

	public override void OnKill()
	{
		_timer.Stop();
		_isCurseActive = false;

		_currentEnemy = InterfaceHelper.CurrentEnemy;
		_timer.Start();
		
		base.OnKill();
	}

	public override void OnDealingDamage(ref int damage)
	{
		if (_isCurseActive)
		{
			damage += (int)Math.Ceiling(damage * DamageIncreaseModifier);
		}
	}

	public CursedRing()
	{
		Name = "Cursed Ring";
		_timer = new DispatcherTimer
		{
			Interval = TimeSpan.FromSeconds(TimeToCurseEnemy)
		};
		_timer.Tick += CurseTimer_Tick;
	}

	private void CurseTimer_Tick(object sender, EventArgs e)
	{
		if (_currentEnemy.Equals(InterfaceHelper.CurrentEnemy))
		{
			_isCurseActive = true;
			_timer.Stop();
		}
	}
}