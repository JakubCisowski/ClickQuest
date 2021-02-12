using ClickQuest.Account;
using ClickQuest.Data;
using ClickQuest.Enemies;
using ClickQuest.Heroes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace ClickQuest.Pages
{
	public partial class DungeonBossPage : Page, INotifyPropertyChanged
	{
		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		#endregion INotifyPropertyChanged

		#region Private Fields
		private int _duration;
		private Monster _boss;
		private DispatcherTimer _fightTimer;
		private Random _rng = new Random();
		private DispatcherTimer _poisonTimer;
		private int _poisonTicks;

		#endregion

		#region Properties

		public int Duration
		{
			get
			{
				return _duration;
			}
			set
			{
				_duration = value;
				OnPropertyChanged();
			}
		}

		#endregion

		public DungeonBossPage()
		{
			InitializeComponent();

			// Setup 30 second timer for boss fight
			_fightTimer = new DispatcherTimer();
			_fightTimer.Interval = new System.TimeSpan(0, 0, 0, 1);
			_fightTimer.Tick += FightTimer_Tick;

			// Setup poison Timer to tick every 0.5s.
			_poisonTimer = new DispatcherTimer();
			_poisonTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
			_poisonTimer.Tick += PoisonTimer_Tick;
		}

		public void StartBossFight(Monster boss)
		{
			BossButton.IsEnabled = true;

			// SpecDungeonBuff's base value is 30 - the fight's duration will always be 30s or more.
			Duration = Account.User.Instance.Specialization.SpecDungeonBuff;

			// Select the boss, and bind it to interface.
			_boss = boss;
			this.DataContext = _boss;

			// Bind the remaining Duration to interface.
			var binding = new Binding("Duration");
			binding.Source = this;
			TimeRemainingBlock.SetBinding(TextBlock.TextProperty, binding);

			// Start 30 second timer.
			_fightTimer.Start();
		}

		private void CheckIfBossDied()
		{
			// If monster died - grant rewards and spawn another one.
			if (_boss.CurrentHealth <= 0)
			{
				// Reset poison.
				_poisonTimer.Stop();
				_poisonTicks = 0;

				// Reset fight timer.
				_fightTimer.Stop();

				// Grant exp and loot.
				GrantVictoryBonuses();
			}
		}

		public void GrantVictoryBonuses()
		{
			// Grant experience based on damage dealt to boss.
			User.Instance.CurrentHero.Experience += Experience.CalculateMonsterXpReward(_boss.Health - _boss.CurrentHealth);

			// Grant boss loot.
			// 1. Check % threshold for reward loot frequencies ("5-" is for inverting 0 -> full hp, 5 -> boss died).
			int threshold = 5 - (_boss.CurrentHealth / (_boss.Health / 5));
			// 2. Iterate through every possible loot
			string lootText = "Experience gained: " + Experience.CalculateMonsterXpReward(_boss.Health - _boss.CurrentHealth) + " \n" +
							  "Loot: \n";
			foreach (var loot in _boss.BossLoot)
			{
				double num = _rng.Next(1, 10001) / 10000d;
				if (num < loot.Frequencies[threshold])
				{
					// Grant loot after checking if it's not empty.
					if (loot.Item.Id != 0)
					{
						Account.User.Instance.AddItem(loot.Item);
						lootText += "- " + loot.Item.Name + " (" + loot.ItemType + ")\n";
						EquipmentWindow.Instance.UpdateEquipment();
						(Data.Database.Pages["DungeonBoss"] as DungeonBossPage).EquipmentFrame.Refresh();
					}
				}
			}

			// Display exp and loot for testing purposes.
			this.TestRewardsBlock.Text = lootText;

			// Increase boss killing specialization amount (it will update buffs in setter).
			Account.User.Instance.Specialization.SpecDungeonAmount++;

			// Make TownButton visible.
			TownButton.Visibility = Visibility.Visible;
		}

		#region Events

		private void FightTimer_Tick(object source, EventArgs e)
		{
			// Every 1 second, reduce the remaining fight duration.
			Duration--;

			// Check if time is up.
			if (Duration <= 0)
			{
				// Make it unable to click the boss.
				BossButton.IsEnabled = false;

				// Reset poison.
				_poisonTimer.Stop();
				_poisonTicks = 0;

				// End the fight.
				_fightTimer.Stop();

				// Grant bonuses.
				GrantVictoryBonuses();
			}
		}

		private void PoisonTimer_Tick(object source, EventArgs e)
		{
			// When poison ends, stop poison timer.
			if (_poisonTicks >= 5)
			{
				_poisonTimer.Stop();
			}
			// Otherwise deal poison damage and check if monster died.
			else
			{
				int poisonDamage = Account.User.Instance.CurrentHero.PoisonDamage;
				_poisonTicks++;
				_boss.CurrentHealth -= poisonDamage;
				// Check if boss is dead now.
				CheckIfBossDied();
			}
		}

		private void BossButton_Click(object sender, RoutedEventArgs e)
		{
			// Reset poison ticks.
			_poisonTicks = 0;
			_poisonTimer.Start();

			// Calculate damage dealt to boss.
			int damage = Account.User.Instance.CurrentHero.ClickDamage;
			// Calculate crit.
			double num = _rng.Next(1, 101) / 100d;
			if (num <= Account.User.Instance.CurrentHero.CritChance)
			{
				damage *= 2;
			}
			// Deal damage to boss.
			_boss.CurrentHealth -= damage;

			// Check if boss is dead now.
			CheckIfBossDied();
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Town"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Town";
			(Database.Pages["Town"] as TownPage).EquipmentFrame.Refresh();

			TestRewardsBlock.Text = "";
		}

		#endregion
	}
}