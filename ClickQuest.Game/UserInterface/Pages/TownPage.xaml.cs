using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes;
using ClickQuest.Game.Core.Places;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.Gameplay;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Controls;

namespace ClickQuest.Game.UserInterface.Pages;

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
		for (var i = 0; i < GameAssets.Regions.Count; i++)
		{
			var region = GameAssets.Regions[i];

			var regionButton = new Button
			{
				Name = "Region" + region.Id,
				Width = 200,
				Height = 70,
				Tag = region,
				Background = (SolidColorBrush)FindResource("BrushGray1")
			};

			var regionBlock = new TextBlock
			{
				FontSize = 20,
				TextAlignment = TextAlignment.Center
			};

			var regionNameRun = new Run(region.Name)
			{
				FontSize = 20
			};

			if (regionNameRun.Text.Length > 20)
			{
				regionNameRun.FontSize = 28 - regionNameRun.Text.Length / 2;
			}

			regionBlock.Inlines.Add(regionNameRun);

			regionBlock.Inlines.Add(new Run($"\nLevel: {region.LevelRequirement}")
			{
				FontFamily = (FontFamily)FindResource("FontRegularLightItalic")
			});

			regionButton.Content = regionBlock;

			regionButton.Click += RegionButton_Click;

			if (User.Instance.CurrentHero?.Level < region.LevelRequirement)
			{
				regionButton.Style = FindResource("ButtonStyleDisabled") as Style;
				regionButton.Background = (SolidColorBrush)FindResource("BrushGray5");
				regionButton.IsEnabled = false;
			}

			if (i <= GameAssets.Regions.Count / 2)
			{
				RegionsPanelLeft.Children.Insert(i, regionButton);
			}
			else
			{
				RegionsPanelRight.Children.Insert(i - GameAssets.Regions.Count / 2 - 1, regionButton);
			}
		}
	}

	private static void RegionButton_Click(object sender, RoutedEventArgs e)
	{
		var selectedRegion = (sender as Button).Tag as Region;
		var regionName = selectedRegion.Name;
		var selectedRegionPage = GameAssets.Pages[regionName] as RegionPage;

		var canHeroEnterThisRegion = User.Instance.CurrentHero.Level >= selectedRegion.LevelRequirement;
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

			// [PRERELEASE]
			foreach (var key in User.Instance.DungeonKeys)
			{
				key.AddItem(100);
			}

			foreach (var ingot in User.Instance.Ingots)
			{
				ingot.AddItem(100);
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