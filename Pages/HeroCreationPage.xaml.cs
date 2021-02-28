using ClickQuest.Account;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Heroes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ControlzEx;
using System.Windows.Data;

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
			hero.Specialization = new Specialization()
			{
				SpecBuyingAmount = 0,
				SpecClickingAmount = 0,
				SpecBlessingAmount = 0,
				SpecCraftingAmount = 0,
				SpecQuestingAmount = 0,
				SpecMeltingAmount = 0,
				SpecDungeonAmount = 0,
				SpecBlessingThreshold = 1,
				SpecBlessingBuff = 0,
				SpecBuyingThreshold = 1,
				SpecBuyingBuff = 0,
				SpecQuestingThreshold = 1,
				SpecQuestingBuff = 0,
				SpecClickingThreshold = 1,
				SpecClickingBuff = 0,
				SpecCraftingThreshold = 1,
				SpecCraftingBuff = 0,
				SpecMeltingThreshold = 1,
				SpecMeltingBuff = 0,
				SpecDungeonThreshold = 1,
				SpecDungeonBuff = 0,
				SpecCraftingText = "Fine"
			};
		}

		public void CreateHeroButton_Click(object sender, RoutedEventArgs e)
		{
			// Check if hero name is chosen.
			if (!string.IsNullOrEmpty(HeroNameBox.Text))
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
				Database.RefreshPages();

				// Set hero session start date.
				hero.SessionStartDate = DateTime.Now;

				// Go to Town.
				(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["Town"]);
				(Database.Pages["Town"] as TownPage).EquipmentFrame.Refresh();
				(Database.Pages["Town"] as TownPage).StatsFrame.Refresh();
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
