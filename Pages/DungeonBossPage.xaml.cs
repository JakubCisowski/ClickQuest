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

		private DispatcherTimer _fightTimer;
		private readonly Random _rng = new Random();
		private DispatcherTimer _poisonTimer;
		private int _poisonTicks;

		public Boss Boss { get; set; }
		public int Duration { get; set; }

		public DungeonBossPage()
		{
			InitializeComponent();

			CombatController.SetupFightTimer();
			CombatController.SetupPoisonTimer();
		}

		public void StartBossFight(Boss boss)
		{
			BossButton.IsEnabled = true;
			TownButton.Visibility = Visibility.Hidden;
			
			BindFightInfoToInterface(boss);

			CombatController._bossFightTimer.Start();
		}

		private void BindFightInfoToInterface(Boss boss)
		{
			Boss = boss;
			DataContext = Boss;

			var binding = new Binding("Duration") {Source = this};
			TimeRemainingBlock.SetBinding(TextBlock.TextProperty, binding);

			var binding2 = new Binding("Description") {Source = GameData.Dungeons.FirstOrDefault(x => x.BossIds.Contains(boss.Id))};
			DungeonDescriptionBlock.SetBinding(TextBlock.TextProperty, binding2);
		}

		public void GrantVictoryBonuses()
		{
			GrantBossReward();

			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Dungeon]++;

			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.DungeonsCompleted, 1);
		}

		public void HandleInterfaceAfterBossDeath()
		{
			BossButton.IsEnabled = false;
			TownButton.Visibility = Visibility.Visible;
			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
		}

		private void GrantBossReward()
		{
			int damageDealtToBoss = Boss.Health - Boss.CurrentHealth;
			// [PRERELEASE]
			int experienceGained = 10;
			User.Instance.CurrentHero.Experience += experienceGained;

			// Grant boss loot.
			// 1. Check % threshold for reward loot frequencies ("5-" is for inverting 0 -> full hp, 5 -> boss died).
			int threshold = 5 - Boss.CurrentHealth / (Boss.Health / 5);
			// 2. Iterate through every possible loot.
			string lootText = "Experience gained: " + experienceGained + " \n" + "Loot: \n";

			foreach (var loot in Boss.BossLoot)
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

		private void BossButton_Click(object sender, RoutedEventArgs e)
		{
			CombatController.HandleUserClickOnEnemy();
			CombatController.HandleBossDeathIfDefeated();
			this.StatsFrame.Refresh();
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