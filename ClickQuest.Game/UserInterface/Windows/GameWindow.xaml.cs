using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Data;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.Gameplay;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Pages;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ClickQuest.Game.UserInterface.Windows
{
	public partial class GameWindow : Window, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private string _locationInfo;

		public string LocationInfo
		{
			get
			{
				return _locationInfo;
			}
			set
			{
				_locationInfo = value;
				SwitchLocationInfoBorderVisibility();
			}
		}

		public GameWindow()
		{
			InitializeComponent();

			DataContext = this;

			(GameAssets.Pages["MainMenu"] as MainMenuPage).UpdateCreateHeroButton();
			(GameAssets.Pages["MainMenu"] as MainMenuPage).UpdateSelectOrDeleteHeroButtons();

			InterfaceController.ChangePage(GameAssets.Pages["MainMenu"], "");
		}

		public void CreateFloatingTextCombat(int damage, DamageType damageType)
		{
			if (damage == 0)
			{
				return;
			}

			int animationDuration = 1;
			int maximumPositionOffset = 50;
			var mousePosition = Mouse.GetPosition(FloatingTextAnimationCanvas);

			var panel = FloatingTextController.CreateFloatingTextCombatBorder(damage, damageType);

			var randomizedPositions = FloatingTextController.RandomizeFloatingTextPathPosition(mousePosition, FloatingTextAnimationCanvas.ActualWidth, FloatingTextAnimationCanvas.ActualHeight, maximumPositionOffset);

			Canvas.SetLeft(panel, randomizedPositions.X);
			Canvas.SetTop(panel, randomizedPositions.Y);

			FloatingTextAnimationCanvas.Children.Add(panel);

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

		public void CreateFloatingTextUtility(string value, SolidColorBrush textBrush, Point position)
		{
			if (string.IsNullOrEmpty(value))
			{
				return;
			}

			int animationDuration = 1;

			var panel = FloatingTextController.CreateFloatingTextUtilityBorder(value, textBrush);

			Canvas.SetLeft(panel, position.X);
			Canvas.SetTop(panel, position.Y);

			FloatingTextAnimationCanvas.Children.Add(panel);

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

		private void FloatingTextAnimation_Completed(object sender, EventArgs e)
		{
			// Remove invisible paths.
			FloatingTextAnimationCanvas.Children.Remove(FloatingTextAnimationCanvas.Children.OfType<Border>().FirstOrDefault(x => x.Opacity == 0));
		}

		private void SwitchLocationInfoBorderVisibility()
		{
			LocationInfoBorder.Visibility = LocationInfo == "" ? Visibility.Hidden : Visibility.Visible;
		}

		private void DragableTop_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				DragMove();
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			User.Instance.Achievements.TotalTimePlayed += DateTime.Now - User.SessionStartDate;

			GameController.OnHeroExit();

			UserDataLoader.Save();

			base.OnClosing(e);
		}

		private void AchievementsButton_Click(object sender, RoutedEventArgs e)
		{
			AchievementsWindow.Instance.Show();
		}

		private void ExitButton_Click(object sender, RoutedEventArgs e)
		{
			var result = AlertBox.Show("Are you sure you want to quit?\nAll progress will be saved.", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes)
			{
				Application.Current.Shutdown();
			}
		}
	}
}