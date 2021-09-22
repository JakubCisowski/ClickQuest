using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ClickQuest.Enemies;
using ClickQuest.Extensions.CollectionsManager;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Heroes;
using ClickQuest.Heroes.Buffs;
using ClickQuest.Items;
using ClickQuest.Pages;
using ClickQuest.Places;
using ClickQuest.Player;

namespace ClickQuest.Controls
{
	public partial class MonsterButton : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private readonly RegionPage _regionPage;

		public Monster Monster { get; set; }

		public Region Region
		{
			get
			{
				return _regionPage.Region;
			}
		}

		public MonsterButton(RegionPage regionPage)
		{
			InitializeComponent();

			_regionPage = regionPage;

			CombatTimerController.SetupPoisonTimer();
			CombatTimerController.SetupAuraTimer();
			SpawnMonster();
		}

		public void SpawnMonster()
		{
			var frequencyList = Region.Monsters.Select(x => x.Frequency).ToList();
			int position = CollectionsController.RandomizeFreqenciesListPosition(frequencyList);
			Monster = Region.Monsters[position].Monster.CopyEnemy();

			DataContext = Monster;

			CombatTimerController.StartAuraTimerOnCurrentRegion();
		}

		private void MonsterButton_Click(object sender, RoutedEventArgs e)
		{
			bool isNoQuestActive = User.Instance.CurrentHero.Quests.All(x => x.EndDate == default);

			if (isNoQuestActive)
			{
				CombatController.HandleUserClickOnEnemy();

				bool isMonsterDead = Monster.HandleEnemyDeathIfDefeated();
				if (isMonsterDead)
				{
					SpawnMonster();
				}

				_regionPage.StatsFrame.Refresh();
			}
			else
			{
				AlertBox.Show("Your hero is busy completing quest!\nCheck back when it's finished.", MessageBoxButton.OK);
			}
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

		private void FloatingTextAnimation_Completed(object sender, EventArgs e)
		{
			// Remove invisible paths.
			DamageTextCanvas.Children.Remove(DamageTextCanvas.Children.OfType<Border>().FirstOrDefault(x => x.Opacity == 0));
		}
	}
}