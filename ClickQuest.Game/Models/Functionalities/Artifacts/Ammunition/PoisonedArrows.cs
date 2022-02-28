using System;
using System.Linq;
using System.Windows.Threading;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models.Interfaces;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Can be consumed by clicking. Applies poison that deals 50 damage every 0.5 seconds for 2.5 seconds.
// Additional clicks increase this duration.
public class PoisonedArrows : ArtifactFunctionality, IConsumable
{
	private const int DamageDealtPerTick = 50;
	private const double PoisonDuration = 2.5;
	private const int TicksNumber = 5;

	private int _ticksCount;
	private int _maxTicksCount;
	
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
	}

	public void Consume(Artifact ammunitionArtifact)
	{
		ammunitionArtifact.RemoveItem(1);

		if (_timer.IsEnabled)
		{
			_maxTicksCount += TicksNumber;
		}
		else
		{
			_maxTicksCount = TicksNumber;
			_timer.Start();
		}
		
		foreach (var artifact in User.Instance.CurrentHero.EquippedArtifacts)
		{
			artifact.ArtifactFunctionality.OnConsumed(ammunitionArtifact);
		}
	}

	public PoisonedArrows()
	{
		Name = "Poisoned Arrows";
		ArtifactSlotsRequired = 0;
		
		_timer = new DispatcherTimer()
		{
			Interval = new TimeSpan(0, 0, 0, 0, (int)(PoisonDuration / TicksNumber * 1000d))
		};
		_timer.Tick += PoisonTimer_Tick;
	}
	
	private void PoisonTimer_Tick(object source, EventArgs e)
	{
		_ticksCount++;
		
		CombatHelper.DealDamageToCurrentEnemy(DamageDealtPerTick, DamageType.Poison);

		if (_ticksCount >= _maxTicksCount)
		{
			_ticksCount = 0;
			_maxTicksCount = TicksNumber;
			_timer.Stop();
		}
	}
}