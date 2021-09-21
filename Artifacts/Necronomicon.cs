using System;
using System.Windows.Threading;
using ClickQuest.Items;

namespace ClickQuest.Artifacts
{
	// After killing a monster, your poison damage is increased by 10 for 3 seconds. This effect does not stack.
	public class Necronomicon : ArtifactFunctionality
	{
		private const int MaxStacks = 3;
		private const int DamageIncreasePerStack = 10;
		private const int StackDuration = 10;

		private int _stacks;
		private DispatcherTimer _timer;
		
		public override void OnKill()
		{
			base.OnKill();
		}

		public Necronomicon()
		{
			Name = "Necronomicon";
			_timer = new DispatcherTimer() {Interval = new TimeSpan(0, 0, StackDuration)};
		}
	}
}