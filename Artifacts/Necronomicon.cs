using System;
using System.Windows.Threading;
using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	// After killing a monster, your poison damage is increased by 10 for 5 seconds. This effect does not stack.
	public class Necronomicon : ArtifactFunctionality
	{
		private const int PoisonDamageIncrease = 10;
		private const int StackDuration = 5;

		private DispatcherTimer _timer;
		
		public override void OnKill()
		{
			User.Instance.CurrentHero.PoisonDamage += PoisonDamageIncrease;
			// todo: co jeżeli zabijemy moba kiedy to trwa? czy to się odświeży?
			_timer.Start();
		}

		public Necronomicon()
		{
			Name = "Necronomicon";
			_timer = new DispatcherTimer() {Interval = new TimeSpan(0, 0, StackDuration)};
			_timer.Tick += PoisonDamageIncreaseTimer_Tick;
		}
		
		private void PoisonDamageIncreaseTimer_Tick(object source, EventArgs e)
		{
			_timer.Stop();
			User.Instance.CurrentHero.PoisonDamage -= PoisonDamageIncrease;
		}
	}
}