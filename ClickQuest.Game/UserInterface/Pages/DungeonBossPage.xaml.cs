using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.UserInterface;

namespace ClickQuest.Game.UserInterface.Pages
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
				Source = GameAssets.Dungeons.FirstOrDefault(x => x.BossIds.Contains(boss.Id))
			};
			DungeonDescriptionBlock.SetBinding(TextBlock.TextProperty, binding2);
		}

		public void HandleInterfaceAfterBossDeath()
		{
			BossButton.IsEnabled = false;
			TownButton.Visibility = Visibility.Visible;
			
			InterfaceController.RefreshCurrentEquipmentPanelTabOnCurrentPage();
			(StatsFrame.Content as HeroStatsPage).RefreshAllDynamicStatsAndToolTips();
		}

		private void BossButton_Click(object sender, RoutedEventArgs e)
		{
			CombatController.HandleUserClickOnEnemy();
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameAssets.Pages["Town"], "Town");

			// [PRERELEASE]
			TestRewardsBlock.Text = "";
		}
	}
}