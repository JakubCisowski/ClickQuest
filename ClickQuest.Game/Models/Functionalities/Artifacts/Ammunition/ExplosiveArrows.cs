using System;
using System.Linq;
using System.Windows.Threading;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models.Interfaces;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

using UserInterface.Helpers;

// Can be consumed by clicking. Applies burning that deals 50 damage every second until the Enemy dies.
// Additional clicks against burning Enemies will not consume additional Ammunition.
public class ExplosiveArrows : ArtifactFunctionality, IConsumable
{
	private const int DamageDealtPerTick = 50;
	private const int TicksIntervalInSeconds = 1;
	private Enemy _enemy;

	private readonly DispatcherTimer _timer;

	public override void OnEnemyClick(Enemy clickedEnemy)
	{
		var rangedWeaponArtifact = User.Instance.CurrentHero.EquippedArtifacts.FirstOrDefault(x => x.ArtifactType == ArtifactType.RangedWeapon);

		if (rangedWeaponArtifact is null)
		{
			return;
		}

		var ammunitionArtifact = User.Instance.CurrentHero.EquippedArtifacts.FirstOrDefault(x => x.ArtifactFunctionality == this);

		if (ammunitionArtifact is null)
		{
			return;
		}

		Consume(ammunitionArtifact);

		_enemy = InterfaceHelper.CurrentEnemy;
	}

	public override void OnKill()
	{
		_timer.Stop();
		
		base.OnKill();
	}

	public void Consume(Artifact ammunitionArtifact)
	{
		if (!_timer.IsEnabled)
		{
			ammunitionArtifact.RemoveItem(1);
			_timer.Start();
		}

		foreach (var artifact in User.Instance.CurrentHero.EquippedArtifacts)
		{
			artifact.ArtifactFunctionality.OnConsumed(ammunitionArtifact);
		}
	}

	public ExplosiveArrows()
	{
		Name = "Explosive Arrows";
		ArtifactSlotsRequired = 0;
		
		_timer = new DispatcherTimer()
		{
			Interval = new TimeSpan(0, 0, 0, TicksIntervalInSeconds)
		};
		_timer.Tick += BurningTimer_Tick;
	}
	
	private void BurningTimer_Tick(object source, EventArgs e)
	{
		if (InterfaceHelper.CurrentEnemy != _enemy || (_enemy is Boss && CombatTimersHelper.BossFightDuration <= 0))
		{
			_timer.Stop();
			return;
		}
		
		CombatHelper.DealDamageToCurrentEnemy(DamageDealtPerTick, DamageType.Magic);
	}
}