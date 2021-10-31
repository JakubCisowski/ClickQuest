using System;
using System.Windows.Threading;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Extensions.UserInterface;

namespace ClickQuest.Game.Core.Artifacts
{
	// All damage dealt is increased by 50% for the first few seconds in combat.
	// Against Monsters, the duration is 2 seconds; against Bosses - 5 seconds. The duration is refreshed each time you kill a Monster.
	public class GladiatorsTrident : ArtifactFunctionality
	{
		private const int DurationAgainstMonsters = 2;
		private const int DurationAgainstBosses = 5;
		private const double DamageModifier = 0.50;

		private Enemy _currentEnemy;
		private readonly DispatcherTimer _timer;

		public override void OnDealingDamage(ref int baseDamage)
		{
			var newEnemy = InterfaceController.CurrentEnemy;

			if (newEnemy != _currentEnemy)
			{
				_currentEnemy = newEnemy;
				_timer.Stop();

				_timer.Interval = new TimeSpan(0, 0, newEnemy is Monster ? DurationAgainstMonsters : DurationAgainstBosses);
				_timer.Start();
			}

			if (_timer.IsEnabled)
			{
				int bonusDamage = (int) (baseDamage * DamageModifier);
				baseDamage += bonusDamage;
			}
		}

		public override void OnRegionLeave()
		{
			_timer.Stop();
		}

		public GladiatorsTrident()
		{
			Name = "Gladiator's Trident";
			_timer = new DispatcherTimer();
			_timer.Tick += DamageIncreaseTimer_Tick;
		}

		private void DamageIncreaseTimer_Tick(object source, EventArgs e)
		{
			_timer.Stop();
		}
	}
}