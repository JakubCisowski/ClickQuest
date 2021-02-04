using ClickQuest.Account;
using ClickQuest.Data;
using ClickQuest.Heroes;
using ClickQuest.Items;
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
					Content = Data.Database.Regions[i].Name,
					Width = 80,
					Height = 50
				};

				button.Click += RegionButton_Click;

				RegionsPanel.Children.Insert(i, button);
			}
		}

		#region Events

		private void RegionButton_Click(object sender, RoutedEventArgs e)
		{
			var regionId = int.Parse((sender as Button).Name.Substring(6));
			string regionName = Data.Database.Regions.FirstOrDefault(x => x.Id == regionId).Name;

			// Check if the current hero can enter this location (level requirement).
			if (User.Instance.CurrentHero.Level >= Data.Database.Regions.FirstOrDefault(x => x.Id == regionId).LevelRequirement)
			{
				(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages[regionName]);
				(Window.GetWindow(this) as GameWindow).LocationInfo = $"{regionName}";
			}
			// Else display a warning.
		}

		private void ShopButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Shop"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Shop";
			(Database.Pages["Shop"] as ShopPage).UpdateShop();
		}

		private void MainMenuButton_Click(object sender, RoutedEventArgs e)
		{
			(Data.Database.Pages["MainMenu"] as MainMenuPage).GenerateHeroButtons();

			// Pause all blessings.
			Blessing.PauseBlessings();

			(Application.Current.MainWindow as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["MainMenu"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "";
		}

		private void QuestMenuButton_Click(object sender, RoutedEventArgs e)
		{
			(Data.Database.Pages["QuestMenu"] as QuestMenuPage).LoadPage();
			(Application.Current.MainWindow as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["QuestMenu"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Quests";
		}

		private void BlacksmithButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Blacksmith"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Blacksmith";
			(Database.Pages["Blacksmith"] as BlacksmithPage).UpdateBlacksmith();
		}
		private void PriestButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Priest"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Priest";
			(Database.Pages["Priest"] as PriestPage).UpdatePriest();
		}
		private void DungeonSelectButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["DungeonSelect"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Selecting dungeon group";
		}

		#endregion
	}
}
