using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Heroes;
using ClickQuest.Places;
using ClickQuest.Player;

namespace ClickQuest.Pages
{
	public partial class TownPage : Page
	{
		private readonly Hero _hero;

		public TownPage()
		{
			InitializeComponent();

			_hero = User.Instance.CurrentHero;
			DataContext = _hero;

			GenerateRegionButtons();
		}

		private void GenerateRegionButtons()
		{
			for (int i = 0; i < GameData.Regions.Count; i++)
			{
				var region = GameData.Regions[i];

				var regionButton = new Button {Name = "Region" + region.Id, Width = 150, Height = 50, Tag = region};

				var regionBlock = new TextBlock {Text = region.Name, FontSize = 20};

				regionButton.Content = regionBlock;

				regionButton.Click += RegionButton_Click;

				RegionsPanel.Children.Insert(i + 1, regionButton);
			}
		}

		private void RegionButton_Click(object sender, RoutedEventArgs e)
		{
			var selectedRegion = (sender as Button).Tag as Region;
			string regionName = selectedRegion.Name;
			var selectedRegionPage = GameData.Pages[regionName] as RegionPage;

			bool canHeroEnterThisRegion = User.Instance.CurrentHero.Level >= selectedRegion.LevelRequirement;
			if (canHeroEnterThisRegion)
			{
				InterfaceController.RefreshStatsAndEquipmentPanelsOnPage(selectedRegionPage);

				// Start AuraTimer if no quest is active.
				if (User.Instance.CurrentHero.Quests.All(x => x.EndDate == default))
				{
					var monsterButton = selectedRegionPage.RegionPanel.Children.OfType<MonsterButton>().FirstOrDefault();
					monsterButton.StartAuraTimer();
				}

				InterfaceController.ChangePage(selectedRegionPage, $"{regionName}");
			}
			else
			{
				AlertBox.Show($"To enter this location you need to be {selectedRegion.LevelRequirement} lvl.\nGain experience by completing quests, and defeating monsters in previous regions!", MessageBoxButton.OK);
			}
		}

		private void ShopButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameData.Pages["Shop"], "Shop");
			(GameData.Pages["Shop"] as ShopPage).UpdateShop();
		}

		private void MainMenuButton_Click(object sender, RoutedEventArgs e)
		{
			(GameData.Pages["MainMenu"] as MainMenuPage).UpdateSelectOrDeleteHeroButtons();

			// Pause all quest timers (so that quest doesn't finish while current hero is not selected).
			User.Instance.CurrentHero.Quests.FirstOrDefault(x => x.EndDate == default)?.PauseTimer();

			User.Instance.CurrentHero.PauseBlessing();
			User.Instance.CurrentHero.UpdateTimePlayed();
			User.Instance.CurrentHero = null;

			InterfaceController.ChangePage(GameData.Pages["MainMenu"], "");
		}

		private void QuestMenuButton_Click(object sender, RoutedEventArgs e)
		{
			(GameData.Pages["QuestMenu"] as QuestMenuPage).LoadPage();
			InterfaceController.ChangePage(GameData.Pages["QuestMenu"], "Quests");
		}

		private void BlacksmithButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameData.Pages["Blacksmith"], "Blacksmith");
			(GameData.Pages["Blacksmith"] as BlacksmithPage).UpdateBlacksmithItems();
		}

		private void PriestButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameData.Pages["Priest"], "Priest");
			(GameData.Pages["Priest"] as PriestPage).UpdatePriest();
		}

		private void DungeonSelectButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameData.Pages["DungeonSelect"], "Selecting dungeon group");
		}
	}
}