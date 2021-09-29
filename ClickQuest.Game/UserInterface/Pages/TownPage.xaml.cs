using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes;
using ClickQuest.Game.Core.Places;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.Game;
using ClickQuest.Game.Extensions.Interface;
using ClickQuest.Game.UserInterface.Controls;

namespace ClickQuest.Game.UserInterface.Pages
{
	public partial class TownPage : Page
	{
		private readonly Hero _hero;

		public TownPage()
		{
			InitializeComponent();

			_hero = User.Instance.CurrentHero;
			DataContext = _hero;

			GenerateRegionButtons();
		}

		private void GenerateRegionButtons()
		{
			for (int i = 0; i < GameAssets.Regions.Count; i++)
			{
				var region = GameAssets.Regions[i];

				var regionButton = new Button
				{
					Name = "Region" + region.Id,
					Width = 150,
					Height = 50,
					Tag = region
				};

				var regionBlock = new TextBlock
				{
					Text = region.Name,
					FontSize = 20
				};

				regionButton.Content = regionBlock;

				regionButton.Click += RegionButton_Click;

				RegionsPanel.Children.Insert(i + 1, regionButton);
			}
		}

		private void RegionButton_Click(object sender, RoutedEventArgs e)
		{
			var selectedRegion = (sender as Button).Tag as Region;
			string regionName = selectedRegion.Name;
			var selectedRegionPage = GameAssets.Pages[regionName] as RegionPage;

			bool canHeroEnterThisRegion = User.Instance.CurrentHero.Level >= selectedRegion.LevelRequirement;
			if (canHeroEnterThisRegion)
			{
				// Start AuraTimer if no quest is active.
				if (User.Instance.CurrentHero.Quests.All(x => x.EndDate == default))
				{
					CombatTimerController.StartAuraTimer();
				}

				InterfaceController.ChangePage(selectedRegionPage, $"{regionName}");

				// Invoke Artifacts with the "on-region-enter" effect.
				foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
				{
					equippedArtifact.ArtifactFunctionality.OnRegionEnter();
				}
			}
			else
			{
				AlertBox.Show($"To enter this location you need to be {selectedRegion.LevelRequirement} lvl.\nGain experience by completing quests, and defeating monsters in previous regions!", MessageBoxButton.OK);
			}
		}

		private void ShopButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameAssets.Pages["Shop"], "Shop");
			(GameAssets.Pages["Shop"] as ShopPage).UpdateShop();
		}

		private void MainMenuButton_Click(object sender, RoutedEventArgs e)
		{
			(GameAssets.Pages["MainMenu"] as MainMenuPage).UpdateSelectOrDeleteHeroButtons();

			// Pause all quest timers (so that quest doesn't finish while current hero is not selected).
			User.Instance.CurrentHero.Quests.FirstOrDefault(x => x.EndDate != default)?.PauseTimer();

			GameController.OnHeroExit();

			User.Instance.CurrentHero = null;

			InterfaceController.ChangePage(GameAssets.Pages["MainMenu"], "");
		}

		private void QuestMenuButton_Click(object sender, RoutedEventArgs e)
		{
			(GameAssets.Pages["QuestMenu"] as QuestMenuPage).LoadPage();
			InterfaceController.ChangePage(GameAssets.Pages["QuestMenu"], "Quests");
		}

		private void BlacksmithButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameAssets.Pages["Blacksmith"], "Blacksmith");
			(GameAssets.Pages["Blacksmith"] as BlacksmithPage).UpdateBlacksmithItems();
		}

		private void PriestButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameAssets.Pages["Priest"], "Priest");
			(GameAssets.Pages["Priest"] as PriestPage).UpdatePriest();
		}

		private void DungeonSelectButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameAssets.Pages["DungeonSelect"], "Selecting dungeon group");
		}
	}
}