using System;
using System.Windows.Threading;
using ClickQuest.Enemies;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	// If you deal 90% or more of an Enemy's health with a single click, gain double experience, 1 Gold and a 20 increase to Click Damage for 10 seconds.
	// The damage increase bonus does not stack.
	public class Naginata : ArtifactFunctionality
	{
		private const double HealthThreshold = 0.90;
		private const int GoldGained = 1;
		private const double ExperienceModifier = 2.0;
		private const int ClickDamageIncrease = 20;
		private const int ClickDamageDuration = 10;

		private DispatcherTimer _timer;

		public override void OnDealingClickDamage(ref int clickDamage)
		{
			if (InterfaceController.CurrentEnemy is Monster monster)
			{
				bool isDamageInThreshold = clickDamage >= HealthThreshold * monster.Health;

				if (isDamageInThreshold)
				{
					User.Instance.Gold += GoldGained;
					
					// todo: gain experience

					User.Instance.CurrentHero.ClickDamage += ClickDamageIncrease;
					_timer.Start();
				}
			}
		}

		public Naginata()
		{
			Name = "Naginata";
			_timer = new DispatcherTimer() {Interval = new TimeSpan(0, 0, ClickDamageDuration)};
			_timer.Tick += DamageIncreaseTimer_Tick;
		}
		
		private void DamageIncreaseTimer_Tick(object source, EventArgs e)
		{
			_timer.Stop();
			User.Instance.CurrentHero.ClickDamage -= ClickDamageIncrease;
		}
	}
}