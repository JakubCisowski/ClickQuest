using ClickQuest.Account;
using ClickQuest.Data;
using ClickQuest.Heroes;
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

			if(User.Instance.CurrentHero.Level>=Data.Database.Regions.FirstOrDefault(x => x.Id == regionId).LevelRequirement)
			{
				(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages[regionName]);
			}
		}

		private void ShopButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Shop"]);
			(Database.Pages["Shop"] as ShopPage).UpdateShop();
		}

		private void MainMenuButton_Click(object sender, RoutedEventArgs e)
		{
			(Data.Database.Pages["MainMenu"] as MainMenuPage).GenerateHeroButtons();
			(Application.Current.MainWindow as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["MainMenu"]);
		}

		private void BlacksmithButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Blacksmith"]);
			(Database.Pages["Blacksmith"] as BlacksmithPage).UpdateBlacksmith();
		}

		#endregion
	}
}
