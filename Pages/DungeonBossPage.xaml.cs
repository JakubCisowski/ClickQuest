using ClickQuest.Player;
using ClickQuest.Data;
using ClickQuest.Enemies;
using ClickQuest.Heroes;
using ClickQuest.Heroes.Buffs;
using ClickQuest.Windows;
using System;
using System.ComponentModel;
using System.Linq;
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
		private Boss _boss;
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
			_fightTimer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 1)
			};
			_fightTimer.Tick += FightTimer_Tick;

			// Setup poison Timer to tick every 0.5s.
			_poisonTimer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 0, 500)
			};
			_poisonTimer.Tick += PoisonTimer_Tick;
		}

		public void StartBossFight(Boss boss)
		{
			BossButton.IsEnabled = true;

			// SpecDungeonBuff's base value is 30 - the fight's duration will always be 30s or more.
			Duration = User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Dungeon];

			// Select the boss, and bind it to interface.
			_boss = boss;
			this.DataContext = _boss;

			// Bind the remaining Duration to interface.
			var binding = new Binding("Duration")
			{
				Source = this
			};
			TimeRemainingBlock.SetBinding(TextBlock.TextProperty, binding);

			// Bind Dungeon's description
			var binding2 = new Binding("Description")
			{
				Source = GameData.Dungeons.FirstOrDefault(x=>x.BossIds.Contains(boss.Id))
			};
			DungeonDescriptionBlock.SetBinding(TextBlock.TextProperty, binding2);

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

				// Increase achievement amount.
				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.BossesDefeated, 1);

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
			// 2. Iterate through every possible loot.
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
						loot.Item.AddItem();
						lootText += "- " + loot.Item.Name + " (" + loot.ItemType + ")\n";
						(GameData.Pages["DungeonBoss"] as DungeonBossPage).EquipmentFrame.Refresh();
					}
				}
			}

			// Grant gold reward.
			User.Instance.Gold += 2137; // (change value later)
			lootText += "- " + "2137" + " (gold)\n";

			// Display exp and loot for testing purposes.
			this.TestRewardsBlock.Text = lootText;

			// Increase boss killing specialization amount (it will update buffs in setter).
			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Dungeon]++;

			// Increase achievement amount.
			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.DungeonsCompleted, 1);

			// Disable clicking on BossButton.
			BossButton.IsEnabled = false;

			// Make TownButton visible.
			this.TownButton.Visibility = Visibility.Visible;

			// Refresh stats frame (for specialization update).
			(GameData.Pages["DungeonBoss"] as DungeonBossPage).StatsFrame.Refresh();
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
				int poisonDamage = Player.User.Instance.CurrentHero.PoisonDamage;
				_poisonTicks++;

				// Increase achievement amount.
				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.PoisonTicksAmount, 1);

				_boss.CurrentHealth -= poisonDamage;
				// Check if boss is dead now.
				CheckIfBossDied();
			}
		}

		private void BossButton_Click(object sender, RoutedEventArgs e)
		{
			// Start PoisonTimer only if hero has any PoisonDamage.
			if (User.Instance.CurrentHero.PoisonDamage>0)
			{
				// Reset poison ticks.
				_poisonTicks = 0;
				_poisonTimer.Start();
			}

			// Calculate damage dealt to boss.
			int damage = User.Instance.CurrentHero.ClickDamage;
			// Calculate crit.
			double num = _rng.Next(1, 101) / 100d;
			if (num <= User.Instance.CurrentHero.CritChance)
			{
				damage *= 2;

				// Increase achievement amount.
				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.CritsAmount, 1);
			}
			// Apply specialization clicking buff.
			damage += User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Clicking];

			// Deal damage to boss.
			_boss.CurrentHealth -= damage;

			// Increase Clicking specialization.
			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Clicking]++;

			// Check if boss is dead now.
			CheckIfBossDied();
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(GameData.Pages["Town"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Town";
			(GameData.Pages["Town"] as TownPage).EquipmentFrame.Refresh();
			(GameData.Pages["Town"] as TownPage).StatsFrame.Refresh();

			TestRewardsBlock.Text = "";
		}

		#endregion
	}
}