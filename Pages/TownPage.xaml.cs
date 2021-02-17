using ClickQuest.Account;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Heroes;
using ClickQuest.Items;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class TownPage : Page
	{
		private Hero _hero;

		public TownPage()
		{
			InitializeComponent();

			_hero = Account.User.Instance.CurrentHero;
			this.DataContext = _hero;

			GenerateRegionButtons();
		}

		private void GenerateRegionButtons()
		{
			// Create a button for each region.
			for (int i = 0; i < Data.Database.Regions.Count; i++)
			{
				var button = new Button()
				{
					Name = "Region" + Data.Database.Regions[i].Id.ToString(),
					Width = 150,
					Height = 50,
				};

				var block = new TextBlock()
				{
					Text = Data.Database.Regions[i].Name,
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
			string regionName = Data.Database.Regions.FirstOrDefault(x => x.Id == regionId).Name;

			// Check if the current hero can enter this location (level requirement).
			if (User.Instance.CurrentHero.Level >= Data.Database.Regions.FirstOrDefault(x => x.Id == regionId).LevelRequirement)
			{
				(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages[regionName]);
				(Database.Pages[regionName] as RegionPage).EquipmentFrame.Refresh();
				(Window.GetWindow(this) as GameWindow).LocationInfo = $"{regionName}";
			}
			// Else display a warning.
			else
			{
				AlertBox.Show($"To enter this location you need to be {Data.Database.Regions.FirstOrDefault(x => x.Id == regionId).LevelRequirement} lvl.\nGain experience by completing quests, and defeating monsters in previous regions!", MessageBoxButton.OK);
			}
		}

		private void ShopButton_Click(object sender, RoutedEventArgs e)
		{
			// Enter Shop page.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Shop"]);
			(Database.Pages["Shop"] as ShopPage).EquipmentFrame.Refresh();
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Shop";
			(Database.Pages["Shop"] as ShopPage).UpdateShop();
		}

		private void MainMenuButton_Click(object sender, RoutedEventArgs e)
		{
			(Data.Database.Pages["MainMenu"] as MainMenuPage).GenerateHeroButtons();

			// Pause all blessings.
			Blessing.PauseBlessings();

			// Pause all quest timers (so that quest doesn't finish while current hero is not selected).
			Account.User.Instance.CurrentHero.Quests.FirstOrDefault(x => x.EndDate == default(DateTime))?.PauseTimer();

			(Application.Current.MainWindow as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["MainMenu"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "";
		}

		private void QuestMenuButton_Click(object sender, RoutedEventArgs e)
		{
			// Enter Quests page.
			(Data.Database.Pages["QuestMenu"] as QuestMenuPage).LoadPage();
			(Application.Current.MainWindow as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["QuestMenu"]);
			(Database.Pages["QuestMenu"] as QuestMenuPage).EquipmentFrame.Refresh();
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Quests";
		}

		private void BlacksmithButton_Click(object sender, RoutedEventArgs e)
		{
			// Enter Blacksmith page.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Blacksmith"]);
			(Database.Pages["Blacksmith"] as BlacksmithPage).EquipmentFrame.Refresh();
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Blacksmith";
			(Database.Pages["Blacksmith"] as BlacksmithPage).UpdateBlacksmith();
		}

		private void PriestButton_Click(object sender, RoutedEventArgs e)
		{
			// Enter Priest page.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Priest"]);
			(Database.Pages["Priest"] as PriestPage).EquipmentFrame.Refresh();
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Priest";
			(Database.Pages["Priest"] as PriestPage).UpdatePriest();
		}

		private void DungeonSelectButton_Click(object sender, RoutedEventArgs e)
		{
			// Enter DungeonSelect page.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["DungeonSelect"]);
			(Database.Pages["DungeonSelect"] as DungeonSelectPage).EquipmentFrame.Refresh();
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Selecting dungeon group";
		}

		#endregion Events
	}
}