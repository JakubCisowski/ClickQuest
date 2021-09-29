using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ClickQuest.Game.Data.GameData;
using ClickQuest.Game.Enemies;
using ClickQuest.Game.Extensions.CombatManager;
using ClickQuest.Game.Extensions.InterfaceManager;

namespace ClickQuest.Game.Pages
{
	public partial class DungeonBossPage : Page, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Boss Boss { get; set; }
		public int Duration { get; set; }

		public DungeonBossPage()
		{
			InitializeComponent();
		}

		public void StartBossFight(Boss boss)
		{
			CombatTimerController.SetupFightTimer();
			CombatTimerController.SetupPoisonTimer();

			BossButton.IsEnabled = true;
			TownButton.Visibility = Visibility.Hidden;

			BindFightInfoToInterface(boss);

			CombatTimerController.BossFightTimer.Start();
		}

		private void BindFightInfoToInterface(Boss boss)
		{
			Boss = boss;
			DataContext = Boss;

			var binding = new Binding("Duration")
			{
				Source = this
			};
			TimeRemainingBlock.SetBinding(TextBlock.TextProperty, binding);

			var binding2 = new Binding("Description")
			{
				Source = GameData.Dungeons.FirstOrDefault(x => x.BossIds.Contains(boss.Id))
			};
			DungeonDescriptionBlock.SetBinding(TextBlock.TextProperty, binding2);
		}

		public void HandleInterfaceAfterBossDeath()
		{
			BossButton.IsEnabled = false;
			TownButton.Visibility = Visibility.Visible;
			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
		}

		private void BossButton_Click(object sender, RoutedEventArgs e)
		{
			CombatController.HandleUserClickOnEnemy();
			Boss.HandleEnemyDeathIfDefeated();
			StatsFrame.Refresh();
		}

		public void CreateFloatingTextPathAndStartAnimations(int damage, DamageType damageType)
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