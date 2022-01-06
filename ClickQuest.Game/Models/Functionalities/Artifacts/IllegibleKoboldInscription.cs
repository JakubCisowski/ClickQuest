using System;
using System.Windows.Threading;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// If you have any Blessing equipped, gain 15 Click Damage and 10% Aura Speed.
public class IllegibleKoboldInscription : ArtifactFunctionality
{
	private const int ClickDamageIncrease = 15;
	private const double AuraSpeedIncrease = 0.10;
	private readonly DispatcherTimer _timer;

	public override void OnEquip()
	{
		if (User.Instance.CurrentHero.Blessing is not null)
		{
			_timer.Interval = TimeSpan.FromSeconds(User.Instance.CurrentHero.Blessing.Duration);
			_timer.Start();

			User.Instance.CurrentHero.ClickDamage += ClickDamageIncrease;
			User.Instance.CurrentHero.AuraAttackSpeed += AuraSpeedIncrease;
		}
	}

	public override void OnBlessingStarted(Blessing blessing)
	{
		if (_timer.IsEnabled)
		{
			_timer.Stop();
			User.Instance.CurrentHero.ClickDamage -= ClickDamageIncrease;
			User.Instance.CurrentHero.AuraAttackSpeed -= AuraSpeedIncrease;
		}

		_timer.Interval = TimeSpan.FromSeconds(blessing.Duration);
		_timer.Start();

		User.Instance.CurrentHero.ClickDamage += ClickDamageIncrease;
		User.Instance.CurrentHero.AuraAttackSpeed += AuraSpeedIncrease;
	}

	public IllegibleKoboldInscription()
	{
		Name = "Illegible Kobold Inscription";
		_timer = new DispatcherTimer();
		_timer.Tick += BonusStatsTimer_Tick;
	}

	private void BonusStatsTimer_Tick(object source, EventArgs e)
	{
		_timer.Stop();

		User.Instance.CurrentHero.ClickDamage -= ClickDamageIncrease;
		User.Instance.CurrentHero.AuraAttackSpeed -= AuraSpeedIncrease;
	}
}