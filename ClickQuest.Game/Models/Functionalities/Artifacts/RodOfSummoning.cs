using System;
using System.Windows.Threading;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// On click, summon a Familiar that will repeatedly click on the Enemy every second for damage equal to 70% of your Click Damage,
// for 5 seconds or until the Enemy dies. If the Familiar kills the Enemy, this damage is increased by 1% (up to a maximum of 100%)
// until you leave the current Region.
public class RodOfSummoning : ArtifactFunctionality
{
	private const double BaseFamiliarClickDamageModifier = 0.70;
	private const double FamiliarClickDamageModifierPerStack = 0.01;
	private const int MaxStacks = 30;
	private const int FamiliarDuration = 5;

	private int _stacksCount;
	private int _familiarAttacksCount;
	private readonly DispatcherTimer _timer;

	public override void OnEnemyClick(Enemy clickedEnemy)
	{
		_familiarAttacksCount = 0;
		_timer.Start();
	}

	public override void OnRegionLeave()
	{
		_timer.Stop();
		_stacksCount = 0;
	}

	public override void OnKill()
	{
		// If the Familiar is attacking (_timer is enabled).
		if (_stacksCount < MaxStacks && _timer.IsEnabled)
		{
			_stacksCount++;
		}

		_timer.Stop();
	}

	public RodOfSummoning()
	{
		Name = "Rod of Summoning";
		_timer = new DispatcherTimer
		{
			Interval = TimeSpan.FromSeconds(1)
		};
		_timer.Tick += FamiliarTimer_Tick;
	}

	private void FamiliarTimer_Tick(object sender, EventArgs e)
	{
		var totalFamiliarDamageModifier = BaseFamiliarClickDamageModifier + FamiliarClickDamageModifierPerStack * _stacksCount;
		var damage = (int)Math.Ceiling(User.Instance.CurrentHero.ClickDamage * totalFamiliarDamageModifier);
		CombatHelper.DealDamageToCurrentEnemy(damage, DamageType.Magic);

		_familiarAttacksCount++;

		if (_familiarAttacksCount == FamiliarDuration)
		{
			_timer.Stop();
		}
	}
}