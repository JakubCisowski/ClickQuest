using System;
using System.Windows.Threading;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.UserInterface.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// If you would deal 90% or more of an Enemy's health with a single click, gain double experience, 2 Gold and a 50 increase to Click Damage for 10 seconds.
// The damage increase bonus does not stack.
public class Naginata : ArtifactFunctionality
{
	private const double HealthThreshold = 0.90;
	private const int GoldGained = 2;
	private const double ExperienceModifier = 2.0;
	private const int ClickDamageIncrease = 50;
	private const int ClickDamageDuration = 10;

	private readonly DispatcherTimer _timer;
	private bool _wasMonsterOneshot;

	public override void OnDealingClickDamage(ref int clickDamage, DamageType clickDamageType)
	{
		if (InterfaceHelper.CurrentEnemy is Monster monster)
		{
			var isDamageInThreshold = clickDamage >= HealthThreshold * monster.Health;

			if (isDamageInThreshold)
			{
				User.Instance.Gold += GoldGained;

				_wasMonsterOneshot = true;

				User.Instance.CurrentHero.ClickDamage += ClickDamageIncrease;
				_timer.Start();
			}
		}
	}

	public override void OnExperienceGained(ref int experienceGained)
	{
		if (_wasMonsterOneshot)
		{
			_wasMonsterOneshot = false;

			experienceGained = (int)Math.Ceiling(experienceGained * ExperienceModifier);
		}
		
		base.OnExperienceGained(ref experienceGained);
	}

	public Naginata()
	{
		Name = "Naginata";
		_timer = new DispatcherTimer
		{
			Interval = new TimeSpan(0, 0, ClickDamageDuration)
		};
		_timer.Tick += DamageIncreaseTimer_Tick;
	}

	private void DamageIncreaseTimer_Tick(object source, EventArgs e)
	{
		_timer.Stop();
		User.Instance.CurrentHero.ClickDamage -= ClickDamageIncrease;
	}
}