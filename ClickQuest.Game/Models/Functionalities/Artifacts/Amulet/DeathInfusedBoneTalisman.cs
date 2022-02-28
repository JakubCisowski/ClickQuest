using System;
using System.Windows.Threading;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.UserInterface.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Your first attack against a monster places 'Decay' on them, dealing damage every 0.5 seconds for 5 seconds. 
// Damage dealt is equal to your poison damage. Deals double damage against bosses.
public class DeathInfusedBoneTalisman : ArtifactFunctionality
{
	private const int TicksNumber = 10;
	private const int Interval = 500;
	private const int DamageModifier = 2;

	private Enemy _affectedEnemy;
	private readonly DispatcherTimer _timer;
	private int _ticksCount;

	public override void OnEnemyClick(Enemy clickedEnemy)
	{
		if (_affectedEnemy != clickedEnemy)
		{
			_affectedEnemy = clickedEnemy;

			_timer.Start();
		}
		
		base.OnEnemyClick(clickedEnemy);
	}

	public override void OnKill()
	{
		_timer.Stop();
		_affectedEnemy = null;
		
		base.OnKill();
	}

	public override void OnRegionLeave()
	{
		_timer.Stop();
	}

	public DeathInfusedBoneTalisman()
	{
		Name = "Death-Infused Bone Talisman";

		_timer = new DispatcherTimer
		{
			Interval = new TimeSpan(0, 0, 0, 0, Interval)
		};
		_timer.Tick += DecayTimer_Tick;
	}

	private void DecayTimer_Tick(object source, EventArgs e)
	{
		_ticksCount++;

		var damage = User.Instance.CurrentHero.PoisonDamage;

		if (InterfaceHelper.CurrentEnemy is Boss)
		{
			damage *= DamageModifier;
		}

		CombatHelper.DealDamageToCurrentEnemy(damage, DamageType.Magic);

		if (_ticksCount == TicksNumber)
		{
			_ticksCount = 0;
			_timer.Stop();
		}
	}
}