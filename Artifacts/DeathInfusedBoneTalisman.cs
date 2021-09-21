using System;
using System.Windows.Threading;
using ClickQuest.Enemies;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	// Your first attack against a monster places 'Decay' on them, dealing damage every 0.5 seconds for 2.5 seconds. 
	// Damage dealt is equal to your poison damage. Deals double damage against bosses.
	public class DeathInfusedBoneTalisman : ArtifactFunctionality
	{
		private const int TicksNumber = 5;
		private const int Interval = 500;
		private const int DamageModifier = 2;
		
		private Enemy _affectedEnemy;
		private DispatcherTimer _timer;
		private int _ticksCount = 0;
		
		public override void OnEnemyClick()
		{
			// Todo: include bosses
			var monsterButton = InterfaceController.CurrentMonsterButton;

			if (monsterButton.Monster != _affectedEnemy)
			{
				_affectedEnemy = monsterButton.Monster;
				
				_timer.Start();
			}
		}

		public DeathInfusedBoneTalisman()
		{
			Name = "Death-Infused Bone Talisman";
			
			_timer = new DispatcherTimer()
			{
				Interval = new TimeSpan(0,0,0,0,Interval)
			};
			_timer.Tick += DecayTimer_Tick;
		}

		private void DecayTimer_Tick(object source, EventArgs e)
		{
			_ticksCount++;

			int damage = User.Instance.CurrentHero.PoisonDamage * DamageModifier;
			
			CombatController.DealDamageToMonster(damage, DamageType.Artifact);

			if (_ticksCount == TicksNumber)
			{
				_ticksCount = 0;
				_timer.Stop();
			}
		}
	}
}