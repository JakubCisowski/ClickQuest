using System;
using System.Collections.Generic;
using System.Windows.Threading;
using ClickQuest.Items;

namespace ClickQuest.Artifacts
{
	// Killing a Monster grants a 10% increase to all non-artifact damage dealt for 5 seconds. Stacks up to 5 times. Each stack has its own duration.
	public class BloodstoneStaff : ArtifactFunctionality
	{
		private const double DamageModifier = 1.1;
		private const int MaxStacks = 5;

		private List<DateTime> _stackApplicationTimes = new List<DateTime>();
		private DispatcherTimer _timer;
		
		public override void OnKill()
		{
			if (_stackApplicationTimes.Count < MaxStacks)
			{
				// todo: w tym momencie damage będzie multiplicative (1.1 * 1.1 * 1.1) - czy tak powinno być?
			}
			else
			{
				
			}
		}
		
		public BloodstoneStaff()
		{
			Name = "Bloodstone Staff";
			_timer = new DispatcherTimer() {Interval = new TimeSpan(0, 0, 1)};
			_timer.Tick += EmpowermentTimer_Tick;
		}
		
		private void EmpowermentTimer_Tick(object source, EventArgs e)
		{
			
		}
	}
}