using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using ClickQuest.Game.Extensions.Combat;

namespace ClickQuest.Game.Core.Items.Artifacts
{
	// Killing a Monster grants a 10% increase to all non-artifact damage dealt for 5 seconds. Stacks additively up to 5 times. Each stack has its own duration.
	public class BloodstoneStaff : ArtifactFunctionality
	{
		private const double DamageModifierPerStack = 0.1;
		private const int MaxStacks = 5;
		private const int StackDuration = 5;

		private readonly List<DateTime> _stackList = new List<DateTime>();
		private readonly DispatcherTimer _timer;

		public override void OnKill()
		{
			if (_stackList.Count >= MaxStacks)
			{
				// Replace the oldest stack.
				_stackList.RemoveAt(0);
			}

			_stackList.Add(DateTime.Now);

			_timer.Start();
		}

		public override void OnDealingClickDamage(ref int clickDamage, DamageType clickDamageType)
		{
			var bonusDamage = (int) (clickDamage * _stackList.Count * DamageModifierPerStack);

			clickDamage += bonusDamage;
		}

		public override void OnDealingPoisonDamage(ref int poisonDamage)
		{
			var bonusDamage = (int) (poisonDamage * _stackList.Count * DamageModifierPerStack);

			poisonDamage += bonusDamage;
		}

		public override void OnDealingAuraDamage(ref int auraDamage)
		{
			var bonusDamage = (int) (auraDamage * _stackList.Count * DamageModifierPerStack);

			auraDamage += bonusDamage;
		}

		public BloodstoneStaff()
		{
			Name = "Bloodstone Staff";
			_timer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 0, 500)
			}; // todo: The timer might have to tick faster.
			_timer.Tick += EmpowermentTimer_Tick;
		}

		private void EmpowermentTimer_Tick(object source, EventArgs e)
		{
			if (_stackList.Count > 0)
			{
				DateTime oldestStack = _stackList.ElementAt(0);

				// If the stack has been active for at least StackDuration seconds, remove it.
				if (DateTime.Now - oldestStack >= TimeSpan.FromSeconds(StackDuration))
				{
					_stackList.RemoveAt(0);
				}
			}

			if (_stackList.Count == 0)
			{
				_timer.Stop();
			}
		}
	}
}