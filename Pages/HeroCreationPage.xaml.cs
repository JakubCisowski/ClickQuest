using ClickQuest.Account;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Heroes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class HeroCreationPage : Page
	{
		public HeroCreationPage()
		{
			InitializeComponent();

			// Populate ComboBox with hero classes and pre-select the first element.
			HeroClassBox.ItemsSource = Enum.GetValues(typeof(HeroClass)).Cast<HeroClass>().Skip(1);
		}

		public void CreateHeroButton_Click(object sender, RoutedEventArgs e)
		{
			// Check if hero name is chosen.
			if (!string.IsNullOrEmpty(HeroNameBox.Text))
			{
				// Validate name - it can contain only letters, digits or spaces (not at the front or back) and can be up to 15 characters long.
				bool isValid = HeroNameBox.Text.Trim().All(x => Char.IsLetterOrDigit(x) || x == ' ') && HeroNameBox.Text.Trim().Length <= 15;

				if (!isValid)
				{
					// Display an error.
					AlertBox.Show($"Hero name can contain up to 15 characters.\nValid characters: A-Z, a-z, 0-9, space.", MessageBoxButton.OK);
					return;
				}

				// Create hero based on user inputs and select it.
				var hero = new Hero((HeroClass)Enum.Parse(typeof(HeroClass), HeroClassBox.SelectedValue.ToString()), HeroNameBox.Text);
				User.Instance.Heroes.Add(hero);

				// Reload the database.
				Entity.EntityOperations.SaveGame();
				Entity.EntityOperations.LoadGame();

				User.Instance.CurrentHero = hero;

				// Refresh bindings.
				Data.Database.RefreshPages();

				// Go to Town.
				(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["Town"]);
				(Database.Pages["Town"] as TownPage).EquipmentFrame.Refresh();
			}
		}

		public void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			// Go back to Menu.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["MainMenu"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "";
		}
	}
}
