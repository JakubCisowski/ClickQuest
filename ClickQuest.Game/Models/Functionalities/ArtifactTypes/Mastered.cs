using System;
using System.Linq;
using System.Windows.Threading;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.UserInterface.Helpers;

namespace ClickQuest.Game.Models.Functionalities.ArtifactTypes;

// On kill, chance to cause an explosion that instantly kills the next Monster. Only works on Regions. Chance is equal to 10% per Mastered artifact equipped.
public class Mastered : ArtifactTypeFunctionality
{
	private const double ExplosionChancePerArtifact = 0.10;
	
	private bool _canTrigger;
	private DispatcherTimer _timer;
	
	public override void OnKill()
	{
		if (_canTrigger)
		{
			var masteredArtifactsEquipped = User.Instance.CurrentHero.EquippedArtifacts.Count(x => x.ArtifactType == ArtifactType.Mastered);
			var randomNumber = RandomnessHelper.Rng.NextDouble();

			var chanceToTrigger = masteredArtifactsEquipped * ExplosionChancePerArtifact;

			if (randomNumber <= chanceToTrigger)
			{
				_canTrigger = false;
				_timer.Start();
			}
		}
	}

	public Mastered()
	{
		ArtifactType = ArtifactType.Mastered;
		Description = "On kill, chance to cause an explosion that instantly kills the next Monster. Only works on Regions. Chance is equal to <BOLD>10%</BOLD> per Mastered artifact equipped.";
		_canTrigger = true;

		_timer = new DispatcherTimer()
		{
			Interval = TimeSpan.FromMilliseconds(200)
		};
		_timer.Tick += ExplosionTimer_Tick;
	}
	
	private void ExplosionTimer_Tick(object source, EventArgs e)
	{
		CombatHelper.DealDamageToCurrentEnemy(InterfaceHelper.CurrentEnemy.CurrentHealth, DamageType.Magic);
		
		_timer.Stop();
		_canTrigger = true;
	}
}