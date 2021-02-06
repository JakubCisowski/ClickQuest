using ClickQuest.Account;
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
				// Validate name - it can contain only letters and digits, and can be up to 15 characters long.
				bool isValid = HeroNameBox.Text.All(x => Char.IsLetterOrDigit(x)) && HeroNameBox.Text.Length <= 15;

				if (!isValid)
				{
					// Display an error.
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

				(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["Town"]);
			}
		}

		public void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["MainMenu"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "";
		}
	}
}
