using ClickQuest.Player;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Heroes;
using ClickQuest.Heroes.Buffs;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ControlzEx;
using System.Windows.Data;
using ClickQuest.Extensions.InterfaceManager;

namespace ClickQuest.Pages
{
	public partial class HeroCreationPage : Page
	{
		public HeroCreationPage()
		{
			InitializeComponent();

			// Populate ComboBox with hero classes and races and pre-select the first element.
			HeroClassBox.ItemsSource = Enum.GetValues(typeof(HeroClass)).Cast<HeroClass>().Skip(1);
			HeroRaceBox.ItemsSource = Enum.GetValues(typeof(HeroRace)).Cast<HeroRace>();
		}	

		private static void SeedSpecializations(Hero hero)
		{
			hero.Specialization = new Specialization();
		}

		public void CreateHeroButton_Click(object sender, RoutedEventArgs e)
		{
			// Check if hero name, race and class are chosen.
			if (!string.IsNullOrEmpty(HeroNameBox.Text) && HeroClassBox.SelectedItems.Count > 0 && HeroRaceBox.SelectedItems.Count > 0)
			{
				// Validate name - it can contain only letters, digits or spaces (not at the front or back) and can be up to 15 characters long.
				bool isValid = HeroNameBox.Text.Trim().All(x => char.IsLetterOrDigit(x) || x == ' ') && HeroNameBox.Text.Trim().Length <= 15;

				if (!isValid)
				{
					// Display an error.
					AlertBox.Show($"Hero name can contain up to 15 characters.\nValid characters: A-Z, a-z, 0-9, space.", MessageBoxButton.OK);
					return;
				}

				// Create hero based on user inputs and select it.
				var hero = new Hero((HeroClass)Enum.Parse(typeof(HeroClass), HeroClassBox.SelectedValue.ToString()), (HeroRace)Enum.Parse(typeof(HeroRace), HeroRaceBox.SelectedValue.ToString()), HeroNameBox.Text);
				User.Instance.Heroes.Add(hero);

				// Seed specializations for the new hero.
				SeedSpecializations(hero);

				// Reload the database.
				Entity.EntityOperations.SaveGame();
				Entity.EntityOperations.LoadGame();

				// Select current hero from entity database, because variable 'hero' doesn't represent the same hero that was loaded from entity in LoadGame().
				User.Instance.CurrentHero = User.Instance.Heroes.FirstOrDefault(x => x.Id == hero.Id);

				// Refresh Specialization buffs.
				User.Instance.CurrentHero.Specialization.UpdateBuffs();

				// Refresh bindings.
				GameData.RefreshPages();

				// Set hero session start date.
				hero.SessionStartDate = DateTime.Now;

				// Go to Town.
				InterfaceController.ChangePage(Data.GameData.Pages["Town"], "Town");
				(GameData.Pages["Town"] as TownPage).EquipmentFrame.Refresh();
				(GameData.Pages["Town"] as TownPage).StatsFrame.Refresh();
			}
		}

		public void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			// Go back to Menu.
			InterfaceController.ChangePage(Data.GameData.Pages["MainMenu"], "");
		}
	}
}
