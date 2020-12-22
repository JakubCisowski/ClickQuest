using ClickQuest.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class MainMenuPage : Page
	{
		public MainMenuPage()
		{
			InitializeComponent();
			GenerateHeroButtons();
		}

		public void GenerateHeroButtons()
		{
			HeroesPanel.Children.Clear();

			// For each hero:
			for (int i = 0; i < Account.User.Instance.Heroes.Count; i++)
			{
				// Generate 'select hero' button.
				var selectHeroButton = new Button()
				{
					Name = "Hero" + Account.User.Instance.Heroes[i].Id,
					Content = Account.User.Instance.Heroes[i].Name + ", " + Account.User.Instance.Heroes[i].ThisHeroClass.ToString() + " [" + Account.User.Instance.Heroes[i].Level + " lvl]",
					Width = 250,
					Height = 50,
					Margin = new Thickness(5)
				};

				// Generate 'delete hero' button.
				var deleteHeroButton = new Button()
				{
					Name = "DeleteHero" + Account.User.Instance.Heroes[i].Id,
					Content = "Delete " + Account.User.Instance.Heroes[i].Name + ", " + Account.User.Instance.Heroes[i].ThisHeroClass.ToString() + " [" + Account.User.Instance.Heroes[i].Level + " lvl]",
					Width = 150,
					Height = 50,
					Margin = new Thickness(5)
				};

				// Assign click events to them.
				selectHeroButton.Click += SelectHeroButton_Click;
				deleteHeroButton.Click += DeleteHeroButton_Click;

				// Add them to the  heroes panel in main menu page.
				HeroesPanel.Children.Add(selectHeroButton);
				HeroesPanel.Children.Add(deleteHeroButton);
			}
		}

		#region Events

		private void CreateHeroButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["HeroCreation"]);
		}

		private void SelectHeroButton_Click(object sender, RoutedEventArgs e)
		{
			// Select this hero as current hero and navigate to town.
			var id = int.Parse((sender as Button).Name.Substring(4));
			var hero = Account.User.Instance.Heroes.Where(x => x.Id == id).FirstOrDefault();
			Account.User.Instance.CurrentHero = hero;
			// Refresh hero stats panel info.
			hero.ExperienceToNextLvl = Heroes.Experience.CalculateXpToNextLvl(hero);

			Data.Database.RefreshPages();
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["Town"]);
		}

		private void DeleteHeroButton_Click(object sender, RoutedEventArgs e)
		{
			var id = int.Parse((sender as Button).Name.Substring(10));
			var hero = Account.User.Instance.Heroes.Where(x => x.Id == id).FirstOrDefault();

			// Remove the hero from User and Database.
			Account.User.Instance.Heroes.Remove(hero);
			Entity.EntityOperations.RemoveHero(hero);

			GenerateHeroButtons();
		}

		private void ResetProgressButton_Click(object sender, RoutedEventArgs e)
		{
			// Reset all progress - only account isn't removed.
			Entity.EntityOperations.ResetProgress();
			GenerateHeroButtons();
		}

		#endregion Events
	}
}