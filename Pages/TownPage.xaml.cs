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

			_hero = User.Instance.CurrentHero;
			this.DataContext = _hero;

			GenerateRegionButtons();
		}

		private void GenerateRegionButtons()
		{
			// Create a button for each region.
			for (int i = 0; i < Database.Regions.Count; i++)
			{
				var button = new Button()
				{
					Name = "Region" + Database.Regions[i].Id.ToString(),
					Width = 150,
					Height = 50,
				};

				var block = new TextBlock()
				{
					Text = Database.Regions[i].Name,
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
			string regionName = Database.Regions.FirstOrDefault(x => x.Id == regionId).Name;

			// Check if the current hero can enter this location (level requirement).
			if (User.Instance.CurrentHero.Level >= Database.Regions.FirstOrDefault(x => x.Id == regionId).LevelRequirement)
			{
				(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages[regionName]);
				(Database.Pages[regionName] as RegionPage).StatsFrame.Refresh();
				(Database.Pages[regionName] as RegionPage).EquipmentFrame.Refresh();
				// Start AuraTimer if no quest is active.
				if (User.Instance.CurrentHero.Quests.All(x => x.EndDate == default(DateTime)))
				{
					foreach (var ctrl in ((Database.Pages[regionName] as RegionPage).RegionPanel.Children))
					{
						if (ctrl is MonsterButton monsterButton)
						{
							monsterButton.StartAuraTimer();
							break;
						}
					}
				}
				(Window.GetWindow(this) as GameWindow).LocationInfo = $"{regionName}";
			}
			// Else display a warning.
			else
			{
				AlertBox.Show($"To enter this location you need to be {Database.Regions.FirstOrDefault(x => x.Id == regionId).LevelRequirement} lvl.\nGain experience by completing quests, and defeating monsters in previous regions!", MessageBoxButton.OK);
			}
		}

		private void ShopButton_Click(object sender, RoutedEventArgs e)
		{
			// Enter Shop page.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Shop"]);
			(Database.Pages["Shop"] as ShopPage).StatsFrame.Refresh();
			(Database.Pages["Shop"] as ShopPage).EquipmentFrame.Refresh();
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Shop";
			(Database.Pages["Shop"] as ShopPage).UpdateShop();
		}

		private void MainMenuButton_Click(object sender, RoutedEventArgs e)
		{
			(Database.Pages["MainMenu"] as MainMenuPage).GenerateHeroButtons();

			// Pause all blessings.
			Blessing.PauseBlessings();

			// Pause all quest timers (so that quest doesn't finish while current hero is not selected).
			User.Instance.CurrentHero.Quests.FirstOrDefault(x => x.EndDate == default(DateTime))?.PauseTimer();

			// Calculate time played on that hero.
			User.Instance.CurrentHero.UpdateTimePlayed();

			// Set current hero to null.
			User.Instance.CurrentHero = null;

			(Application.Current.MainWindow as GameWindow).CurrentFrame.Navigate(Database.Pages["MainMenu"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "";
		}

		private void QuestMenuButton_Click(object sender, RoutedEventArgs e)
		{
			// Enter Quests page.
			(Database.Pages["QuestMenu"] as QuestMenuPage).LoadPage();
			(Application.Current.MainWindow as GameWindow).CurrentFrame.Navigate(Database.Pages["QuestMenu"]);
			(Database.Pages["QuestMenu"] as QuestMenuPage).StatsFrame.Refresh();
			(Database.Pages["QuestMenu"] as QuestMenuPage).EquipmentFrame.Refresh();
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Quests";
		}

		private void BlacksmithButton_Click(object sender, RoutedEventArgs e)
		{
			// Enter Blacksmith page.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Blacksmith"]);
			(Database.Pages["Blacksmith"] as BlacksmithPage).StatsFrame.Refresh();
			(Database.Pages["Blacksmith"] as BlacksmithPage).EquipmentFrame.Refresh();
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Blacksmith";
			(Database.Pages["Blacksmith"] as BlacksmithPage).UpdateBlacksmith();
		}

		private void PriestButton_Click(object sender, RoutedEventArgs e)
		{
			// Enter Priest page.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Priest"]);
			(Database.Pages["Priest"] as PriestPage).StatsFrame.Refresh();
			(Database.Pages["Priest"] as PriestPage).EquipmentFrame.Refresh();
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Priest";
			(Database.Pages["Priest"] as PriestPage).UpdatePriest();
		}

		private void DungeonSelectButton_Click(object sender, RoutedEventArgs e)
		{
			// Enter DungeonSelect page.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["DungeonSelect"]);
			(Database.Pages["DungeonSelect"] as DungeonSelectPage).StatsFrame.Refresh();
			(Database.Pages["DungeonSelect"] as DungeonSelectPage).EquipmentFrame.Refresh();
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Selecting dungeon group";
		}

		#endregion Events
	}
}