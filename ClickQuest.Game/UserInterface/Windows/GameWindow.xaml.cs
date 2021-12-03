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
            get => _locationInfo;
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

            const int animationDuration = 1;
            const int maximumPositionOffset = 50;
            Point mousePosition = Mouse.GetPosition(FloatingTextAnimationCanvas);

            Border panel = FloatingTextController.CreateFloatingTextCombatBorder(damage, damageType);

            (double X, double Y) randomizedPositions = FloatingTextController.RandomizeFloatingTextPathPosition(mousePosition, FloatingTextAnimationCanvas.ActualWidth, FloatingTextAnimationCanvas.ActualHeight, maximumPositionOffset);

            Canvas.SetLeft(panel, randomizedPositions.X);
            Canvas.SetTop(panel, randomizedPositions.Y);

            FloatingTextAnimationCanvas.Children.Add(panel);

            DoubleAnimation textOpacityAnimation = FloatingTextController.CreateTextOpacityAnimation(animationDuration);
            textOpacityAnimation.Completed += FloatingTextAnimation_Completed;
            panel.BeginAnimation(OpacityProperty, textOpacityAnimation);

            ScaleTransform transform = new ScaleTransform(1, 1);
            panel.LayoutTransform = transform;
            DoubleAnimation animationX = new DoubleAnimation(1, 0.5, new Duration(TimeSpan.FromSeconds(animationDuration)));
            transform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
            DoubleAnimation animationY = new DoubleAnimation(1, 0.5, new Duration(TimeSpan.FromSeconds(animationDuration)));
            transform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);
        }

        public void CreateFloatingTextUtility(string value, SolidColorBrush textBrush, Point position)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            const int animationDuration = 1;

            Border panel = FloatingTextController.CreateFloatingTextUtilityBorder(value, textBrush);

            Canvas.SetLeft(panel, position.X);
            Canvas.SetTop(panel, position.Y);

            FloatingTextAnimationCanvas.Children.Add(panel);

            DoubleAnimation textOpacityAnimation = FloatingTextController.CreateTextOpacityAnimation(animationDuration);
            textOpacityAnimation.Completed += FloatingTextAnimation_Completed;
            panel.BeginAnimation(OpacityProperty, textOpacityAnimation);

            ScaleTransform transform = new ScaleTransform(1, 1);
            panel.LayoutTransform = transform;
            DoubleAnimation animationX = new DoubleAnimation(1, 0.5, new Duration(TimeSpan.FromSeconds(animationDuration)));
            transform.BeginAnimation(ScaleTransform.ScaleXProperty, animationX);
            DoubleAnimation animationY = new DoubleAnimation(1, 0.5, new Duration(TimeSpan.FromSeconds(animationDuration)));
            transform.BeginAnimation(ScaleTransform.ScaleYProperty, animationY);
        }

        public void CreateFloatingTextLoot(Border lootBorder, int animationDelay = 0)
        {
            const int animationDuration = 5;

            // Start position is center of the screen (so center of the enemy as well).
            Point startPosition = FloatingTextController.EnemyCenterPoint;
            Point endPosition = FloatingTextController.LootEndPositionPoint;

            Canvas.SetLeft(lootBorder, startPosition.X);
            Canvas.SetTop(lootBorder, startPosition.Y);

            FloatingTextAnimationCanvas.Children.Add(lootBorder);

            // Add animationDelay to the duration, because otherwise all of the animations spawn (and disappear) at the same time.
            DoubleAnimation textOpacityAnimation = FloatingTextController.CreateTextOpacityAnimation(animationDuration + animationDelay);
            textOpacityAnimation.Completed += FloatingTextAnimation_Completed;
            lootBorder.BeginAnimation(OpacityProperty, textOpacityAnimation);

            double top = Canvas.GetTop(lootBorder);
            double left = Canvas.GetLeft(lootBorder);
            TranslateTransform transform = new TranslateTransform();
            lootBorder.RenderTransform = transform;

            DoubleAnimation animX = new DoubleAnimation(startPosition.X - left, endPosition.X - left - 100, TimeSpan.FromSeconds(animationDuration));
            DoubleAnimation animY = new DoubleAnimation(startPosition.Y - top, endPosition.Y - top - 100, TimeSpan.FromSeconds(animationDuration));

            // Delay the animations.
            animX.BeginTime = TimeSpan.FromSeconds(animationDelay);
            animY.BeginTime = TimeSpan.FromSeconds(animationDelay);

            transform.BeginAnimation(TranslateTransform.XProperty, animX);
            transform.BeginAnimation(TranslateTransform.YProperty, animY);
        }

        private void FloatingTextAnimation_Completed(object sender, EventArgs e)
        {
            // Remove invisible paths.
            FloatingTextAnimationCanvas.Children.Remove(FloatingTextAnimationCanvas.Children.OfType<Border>().FirstOrDefault(x => Math.Abs(x.Opacity - 0.5) < 0.001));
        }

        private void SwitchLocationInfoBorderVisibility()
        {
            LocationInfoBorder.Visibility = LocationInfo == "" ? Visibility.Hidden : Visibility.Visible;
        }

        private void DraggableTop_MouseDown(object sender, MouseButtonEventArgs e)
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

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            Page currentPage = GameAssets.CurrentPage;

            if (currentPage is RegionPage or DungeonBossPage)
            {
                AlertBox.Show("You cannot view this page while in combat!", MessageBoxButton.OK);
                return;
            }

            InterfaceController.ChangePage(new InfoPage(currentPage, LocationInfo), "Bestiary & Game Mechanics");
        }

        private void AchievementsButton_Click(object sender, RoutedEventArgs e)
        {
            AchievementsWindow.Show();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = AlertBox.Show("Are you sure you want to quit?\nAll progress will be saved.");

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow.Instance.Show();
        }
    }
}