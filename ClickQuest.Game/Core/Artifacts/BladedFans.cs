using System;
using System.Windows.Threading;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.UserInterface;

namespace ClickQuest.Game.Core.Artifacts
{
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
			if (_currentEnemy != InterfaceController.CurrentEnemy)
			{
				_currentEnemy = InterfaceController.CurrentEnemy;
				_timer.Start();
			}

			if (_timer.IsEnabled)
			{
				_damageStored += baseDamage;
				baseDamage = 0;
			}
		}

		public override void OnRegionLeave()
		{
			_timer.Stop();
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
			CombatController.DealDamageToCurrentEnemy(_damageStored * DamageModifier, DamageType.Artifact);
		}
	}
}