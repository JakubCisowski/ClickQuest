using ClickQuest.Player;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Heroes;
using ClickQuest.Items;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.Extensions.InterfaceManager;

namespace ClickQuest.Pages
{
	public partial class TownPage : Page
	{
		private Hero _hero;

		public TownPage()
		{
			InitializeComponent();

			_hero = User.Instance.CurrentHero;
			this.DataContext = _hero;

			GenerateRegionButtons();
		}

		private void GenerateRegionButtons()
		{
			// Create a button for each region.
			for (int i = 0; i < GameData.Regions.Count; i++)
			{
				var button = new Button()
				{
					Name = "Region" + GameData.Regions[i].Id.ToString(),
					Width = 150,
					Height = 50,
				};

				var block = new TextBlock()
				{
					Text = GameData.Regions[i].Name,
					FontSize = 20
				};

				button.Content = block;

				button.Click += RegionButton_Click;

				RegionsPanel.Children.Insert(i + 1, button);
			}
		}

		#region Events

		private void RegionButton_Click(object sender, RoutedEventArgs e)
		{
			// Enter the chosen Region page.
			var regionId = int.Parse((sender as Button).Name.Substring(6));
			string regionName = GameData.Regions.FirstOrDefault(x => x.Id == regionId).Name;

			// Check if the current hero can enter this location (level requirement).
			if (User.Instance.CurrentHero.Level >= GameData.Regions.FirstOrDefault(x => x.Id == regionId).LevelRequirement)
			{
				var selectedRegionPage = GameData.Pages[regionName] as RegionPage;
				(GameData.Pages[regionName] as RegionPage).StatsFrame.Refresh();
				(GameData.Pages[regionName] as RegionPage).EquipmentFrame.Refresh();

				// Start AuraTimer if no quest is active.
				if (User.Instance.CurrentHero.Quests.All(x => x.EndDate == default(DateTime)))
				{
					var monsterButton = selectedRegionPage.RegionPanel.Children.OfType<MonsterButton>().FirstOrDefault();
					monsterButton.StartAuraTimer();
				}
				
				InterfaceController.ChangePage(selectedRegionPage, $"{regionName}");
			}
			// Else display a warning.
			else
			{
				AlertBox.Show($"To enter this location you need to be {GameData.Regions.FirstOrDefault(x => x.Id == regionId).LevelRequirement} lvl.\nGain experience by completing quests, and defeating monsters in previous regions!", MessageBoxButton.OK);
			}
		}

		private void ShopButton_Click(object sender, RoutedEventArgs e)
		{
			// Enter Shop page.
			InterfaceController.ChangePage(Data.GameData.Pages["Shop"], "Shop");
			(GameData.Pages["Shop"] as ShopPage).UpdateShop();
		}

		private void MainMenuButton_Click(object sender, RoutedEventArgs e)
		{
			(GameData.Pages["MainMenu"] as MainMenuPage).UpdateSelectOrDeleteHeroButtons();

			// Pause all blessings.
			User.Instance.CurrentHero.PauseBlessing();

			// Pause all quest timers (so that quest doesn't finish while current hero is not selected).
			User.Instance.CurrentHero.Quests.FirstOrDefault(x => x.EndDate == default(DateTime))?.PauseTimer();

			// Calculate time played on that hero.
			User.Instance.CurrentHero.UpdateTimePlayed();

			// Set current hero to null.
			User.Instance.CurrentHero = null;

			InterfaceController.ChangePage(Data.GameData.Pages["MainMenu"], "");
		}

		private void QuestMenuButton_Click(object sender, RoutedEventArgs e)
		{
			// Enter Quests page.
			(GameData.Pages["QuestMenu"] as QuestMenuPage).LoadPage();
			InterfaceController.ChangePage(Data.GameData.Pages["QuestMenu"], "Quests");
		}

		private void BlacksmithButton_Click(object sender, RoutedEventArgs e)
		{
			// Enter Blacksmith page.
			InterfaceController.ChangePage(Data.GameData.Pages["Blacksmith"], "Blacksmith");
			(GameData.Pages["Blacksmith"] as BlacksmithPage).UpdateBlacksmithItems();
		}

		private void PriestButton_Click(object sender, RoutedEventArgs e)
		{
			// Enter Priest page.
			InterfaceController.ChangePage(Data.GameData.Pages["Priest"], "Priest");
			(GameData.Pages["Priest"] as PriestPage).UpdatePriest();
		}

		private void DungeonSelectButton_Click(object sender, RoutedEventArgs e)
		{
			// Enter DungeonSelect page.
			InterfaceController.ChangePage(Data.GameData.Pages["DungeonSelect"], "Selecting dungeon group");
		}

		#endregion Events
	}
}