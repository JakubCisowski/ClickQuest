using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using ClickQuest.Data;
using ClickQuest.Enemies;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Heroes;
using ClickQuest.Heroes.Buffs;
using ClickQuest.Player;

namespace ClickQuest.Pages
{
	public partial class DungeonBossPage : Page, INotifyPropertyChanged
	{
		public DungeonBossPage()
		{
			InitializeComponent();

			SetupFightTimer();
			SetupPoisonTimer();
		}

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

		private void SetupFightTimer()
		{
			_fightTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 1)};
			_fightTimer.Tick += FightTimer_Tick;
		}

		private void SetupPoisonTimer()
		{
			int poisonIntervalMs = 500;

			_poisonTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, poisonIntervalMs)};
			_poisonTimer.Tick += PoisonTimer_Tick;
			_poisonTicks = 0;
		}

		public void StartBossFight(Boss boss)
		{
			BossButton.IsEnabled = true;
			TownButton.Visibility = Visibility.Hidden;

			// SpecDungeonBuff's base value is 30 - the fight's duration will always be 30s or more.
			Duration = User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Dungeon];

			BindFightInfoToInterface(boss);

			_fightTimer.Start();
		}

		private void BindFightInfoToInterface(Boss boss)
		{
			_boss = boss;
			DataContext = _boss;

			var binding = new Binding("Duration") {Source = this};
			TimeRemainingBlock.SetBinding(TextBlock.TextProperty, binding);

			var binding2 = new Binding("Description") {Source = GameData.Dungeons.FirstOrDefault(x => x.BossIds.Contains(boss.Id))};
			DungeonDescriptionBlock.SetBinding(TextBlock.TextProperty, binding2);
		}

		private void HandleBossDeathIfDefeated()
		{
			if (_boss.CurrentHealth <= 0)
			{
				StopPoisonTimer();
				_fightTimer.Stop();

				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.BossesDefeated, 1);

				GrantVictoryBonuses();
				HandleInterfaceAfterBossDeath();
			}
		}

		private void StopPoisonTimer()
		{
			_poisonTimer.Stop();
			_poisonTicks = 0;
		}

		public void GrantVictoryBonuses()
		{
			GrantBossReward();

			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Dungeon]++;

			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.DungeonsCompleted, 1);
		}

		private void HandleInterfaceAfterBossDeath()
		{
			BossButton.IsEnabled = false;
			TownButton.Visibility = Visibility.Visible;
			InterfaceController.RefreshStatsAndEquipmentPanelsOnPage(GameData.Pages["DungeonBoss"]);
		}

		private void GrantBossReward()
		{
			int damageDealtToBoss = _boss.Health - _boss.CurrentHealth;
			int experienceGained = Experience.CalculateMonsterXpReward(damageDealtToBoss);
			User.Instance.CurrentHero.Experience += experienceGained;

			// Grant boss loot.
			// 1. Check % threshold for reward loot frequencies ("5-" is for inverting 0 -> full hp, 5 -> boss died).
			int threshold = 5 - _boss.CurrentHealth / (_boss.Health / 5);
			// 2. Iterate through every possible loot.
			string lootText = "Experience gained: " + experienceGained + " \n" + "Loot: \n";

			foreach (var loot in _boss.BossLoot)
			{
				double randomizedValue = _rng.Next(1, 10001) / 10000d;
				if (randomizedValue < loot.Frequencies[threshold])
				{
					// Grant loot after checking if it's not empty.
					if (loot.Item.Id != 0)
					{
						loot.Item.AddItem();
						lootText += "- " + loot.Item.Name + " (" + loot.ItemType + ")\n";
					}
				}
			}

			InterfaceController.RefreshStatsAndEquipmentPanelsOnPage(GameData.Pages["DungeonBoss"]);

			// Grant gold reward.
			int goldReward = 2137; // (change value later)
			User.Instance.Gold += goldReward;
			lootText += "- " + goldReward + " (gold)\n";

			// [PRERELEASE] Display exp and loot for testing purposes.
			TestRewardsBlock.Text = lootText;
		}

		private void StartPoisonTimer()
		{
			if (User.Instance.CurrentHero.PoisonDamage > 0)
			{
				_poisonTicks = 0;
				_poisonTimer.Start();
			}
		}

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
		private readonly Random _rng = new Random();
		private DispatcherTimer _poisonTimer;
		private int _poisonTicks;

		#endregion

		#region Events

		private void FightTimer_Tick(object source, EventArgs e)
		{
			Duration--;

			// Check if time is up.
			if (Duration <= 0)
			{
				StopPoisonTimer();

				_fightTimer.Stop();

				GrantVictoryBonuses();
				HandleInterfaceAfterBossDeath();
			}
		}

		private void PoisonTimer_Tick(object source, EventArgs e)
		{
			int poisonTicksMax = 5;

			if (_poisonTicks >= poisonTicksMax)
			{
				_poisonTimer.Stop();
			}
			else
			{
				int poisonDamage = User.Instance.CurrentHero.PoisonDamage;
				_boss.CurrentHealth -= poisonDamage;

				_poisonTicks++;

				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.PoisonTicksAmount, 1);

				HandleBossDeathIfDefeated();
			}
		}

		private void BossButton_Click(object sender, RoutedEventArgs e)
		{
			StartPoisonTimer();

			int damage = User.Instance.CurrentHero.CalculateClickDamage();
			_boss.CurrentHealth -= damage;

			HandleBossDeathIfDefeated();

			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Clicking]++;
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameData.Pages["Town"], "Town");

			// [PRERELEASE]
			TestRewardsBlock.Text = "";
		}

		#endregion
	}
}