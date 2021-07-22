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

			HeroClassBox.ItemsSource = Enum.GetValues(typeof(HeroClass)).Cast<HeroClass>().Where(x=>x!=HeroClass.All);
			HeroRaceBox.ItemsSource = Enum.GetValues(typeof(HeroRace)).Cast<HeroRace>();
		}	
		
		public void CreateHeroButton_Click(object sender, RoutedEventArgs e)
		{
			bool isHeroClassSelected = HeroClassBox.SelectedItems.Count > 0;
			bool isHeroRaceSelected = HeroRaceBox.SelectedItems.Count > 0;

			if (isHeroClassSelected && isHeroRaceSelected)
			{
				bool isHeroNameAlphanumericWithSpaces = HeroNameBox.Text.Trim().All(x => char.IsLetterOrDigit(x) || x == ' ');
				bool isHeroNameLengthCorrect = HeroNameBox.Text.Trim().Length > 0 && HeroNameBox.Text.Trim().Length <= 15;

				if (!isHeroNameAlphanumericWithSpaces || !isHeroNameLengthCorrect)
				{
					AlertBox.Show($"Hero name can contain up to 15 characters.\nValid characters: A-Z, a-z, 0-9, space.", MessageBoxButton.OK);
					return;
				}

				CreateHero();
				
				GameData.RefreshPages();

				InterfaceController.ChangePage(Data.GameData.Pages["Town"], "Town");
				InterfaceController.RefreshStatsAndEquipmentPanelsOnPage(Data.GameData.Pages["Town"]);
			}
		}

		private void CreateHero()
		{
			var newHero = new Hero((HeroClass)Enum.Parse(typeof(HeroClass), HeroClassBox.SelectedValue.ToString()), (HeroRace)Enum.Parse(typeof(HeroRace), HeroRaceBox.SelectedValue.ToString()), HeroNameBox.Text);
			
			User.Instance.Heroes.Add(newHero);

			// Reload the database.
			Entity.EntityOperations.SaveGame();
			Entity.EntityOperations.LoadGame();

			// Select current hero from entity database, because variable 'hero' doesn't represent the same hero that was loaded from entity in LoadGame().
			User.Instance.CurrentHero = User.Instance.Heroes.FirstOrDefault(x => x.Id == newHero.Id);

			User.Instance.CurrentHero.Specialization.UpdateBuffs();

			newHero.SessionStartDate = DateTime.Now;
		}

		public void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(Data.GameData.Pages["MainMenu"], "");
		}
	}
}
