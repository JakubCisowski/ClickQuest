using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ClickQuest.Data.GameData;
using ClickQuest.Enemies;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Heroes;
using ClickQuest.Heroes.Buffs;
using ClickQuest.Player;

namespace ClickQuest.Pages
{
	public partial class DungeonBossPage : Page, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private Boss _boss;
		private DispatcherTimer _fightTimer;
		private readonly Random _rng = new Random();
		private DispatcherTimer _poisonTimer;
		private int _poisonTicks;

		public int Duration { get; set; }

		public DungeonBossPage()
		{
			InitializeComponent();

			SetupFightTimer();
			SetupPoisonTimer();
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

		private void StartPoisonTimer()
		{
			if (User.Instance.CurrentHero.PoisonDamage > 0)
			{
				_poisonTicks = 0;
				_poisonTimer.Start();
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
			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
		}

		private void GrantBossReward()
		{
			int damageDealtToBoss = _boss.Health - _boss.CurrentHealth;
			// [PRERELEASE]
			int experienceGained = 10;
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

			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();

			// Grant gold reward.
			int goldReward = 2137; // (change value later)
			User.Instance.Gold += goldReward;
			lootText += "- " + goldReward + " (gold)\n";

			// [PRERELEASE] Display exp and loot for testing purposes.
			TestRewardsBlock.Text = lootText;
		}

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

				CreateFloatingTextPathAndStartAnimations(poisonDamage, DamageType.Poison);

				_poisonTicks++;

				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.PoisonTicksAmount, 1);

				HandleBossDeathIfDefeated();
			}
		}

		private void BossButton_Click(object sender, RoutedEventArgs e)
		{
			StartPoisonTimer();

			var damageBaseAndCritInfo = User.Instance.CurrentHero.CalculateClickDamage();
			var damageOnHit = User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Clicking];
			var damageTotal = damageBaseAndCritInfo.Damage + damageOnHit;
			_boss.CurrentHealth -= damageTotal;

			var damageType = damageBaseAndCritInfo.IsCritical ? DamageType.Critical : DamageType.Normal;
			CreateFloatingTextPathAndStartAnimations(damageBaseAndCritInfo.Damage, damageType);

			CreateFloatingTextPathAndStartAnimations(damageOnHit, DamageType.OnHit);

			HandleBossDeathIfDefeated();

			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Clicking]++;
		}

		private void CreateFloatingTextPathAndStartAnimations(int damage, DamageType damageType)
		{
			if (damage == 0)
			{
				return;
			}

			int animationDuration = 1;
			int maximumPositionOffset = 50;
			var mousePosition = Mouse.GetPosition(DamageTextCanvas);
			
			var panel = FloatingTextController.CreateFloatingTextPanel(damage, damageType);
			
			var randomizedPositions = FloatingTextController.RandomizeFloatingTextPathPosition(mousePosition, DamageTextCanvas.ActualWidth, DamageTextCanvas.ActualHeight, maximumPositionOffset);

			Canvas.SetLeft(panel, randomizedPositions.X);
			Canvas.SetTop(panel, randomizedPositions.Y);

			DamageTextCanvas.Children.Add(panel);

			var textOpacityAnimation = FloatingTextController.CreateTextOpacityAnimation(animationDuration);
			textOpacityAnimation.Completed += FloatingTextAnimation_Completed;
			panel.BeginAnimation(OpacityProperty, textOpacityAnimation);

			var transform = new ScaleTransform(1, 1);
			panel.LayoutTransform = transform;
			var animationX = new DoubleAnimation(1, 0.5, new Duration(TimeSpan.FromSeconds(animationDuration)));
			transform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
			var animationY = new DoubleAnimation(1, 0.5, new Duration(TimeSpan.FromSeconds(animationDuration)));
			transform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameData.Pages["Town"], "Town");

			// [PRERELEASE]
			TestRewardsBlock.Text = "";
		}

		private void FloatingTextAnimation_Completed(object sender, EventArgs e)
		{
			// Remove invisible paths.
			DamageTextCanvas.Children.Remove(DamageTextCanvas.Children.OfType<Border>().FirstOrDefault(x => x.Opacity == 0));
		}
	}
}