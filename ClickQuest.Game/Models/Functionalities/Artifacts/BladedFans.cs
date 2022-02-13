using System;
using System.Windows.Threading;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.UserInterface.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// For the first 5 seconds in combat with an Enemy, you cannot deal damage to them, and instead accumulate damage.
// After 5 seconds, explode dealing damage to the enemy. Damage dealt is equal to 200% of the accumulated damage.
public class BladedFans : ArtifactFunctionality
{
	private const int LockoutDuration = 5;
	private const int DamageModifier = 2;

	private Enemy _currentEnemy;
	private int _damageStored;
	private readonly DispatcherTimer _timer;

	public override void OnDealingDamage(ref int baseDamage)
	{
		if (_currentEnemy != InterfaceHelper.CurrentEnemy)
		{
			_currentEnemy = InterfaceHelper.CurrentEnemy;
			_timer.Start();
		}

		if (_timer.IsEnabled)
		{
			_damageStored += baseDamage;
			baseDamage = 0;
		}
	}

	public override void OnDealingMagicDamage(ref int magicDamage)
	{
		if (_currentEnemy != InterfaceHelper.CurrentEnemy)
		{
			_currentEnemy = InterfaceHelper.CurrentEnemy;
			_timer.Start();
		}

		if (_timer.IsEnabled)
		{
			_damageStored += magicDamage;
			magicDamage = 0;
		}
	}

	public override void OnRegionLeave()
	{
		_timer.Stop();
		_currentEnemy = null;
	}

	public BladedFans()
	{
		Name = "Bladed Fans";
		_timer = new DispatcherTimer
		{
			Interval = new TimeSpan(0, 0, LockoutDuration)
		};
		_timer.Tick += ExplosionTimer_Tick;
	}

	private void ExplosionTimer_Tick(object source, EventArgs e)
	{
		_timer.Stop();
		CombatHelper.DealDamageToCurrentEnemy(_damageStored * DamageModifier, DamageType.Magic);
		_damageStored = 0;
	}
}