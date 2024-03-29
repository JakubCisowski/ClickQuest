using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ClickQuest.Game.DataTypes.Collections;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Helpers;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.Game.UserInterface.Pages;

public partial class MainMenuPage : Page
{
	public MainMenuPage()
	{
		InitializeComponent();
		UpdateCreateHeroButton();
		UpdateSelectOrDeleteHeroButtons();
	}

	public void UpdateSelectOrDeleteHeroButtons()
	{
		ClearSelectOrDeletePanels();

		for (var i = 0; i < User.Instance.Heroes.Count; i++)
		{
			SelectOrDeleteHeroButtonsGrid.Children.Add(GenerateSelectOrDeletePanel(i));
		}
	}

	private void ClearSelectOrDeletePanels()
	{
		var selectOrDeletePanels = SelectOrDeleteHeroButtonsGrid.Children.OfType<StackPanel>();

		for (var i = 0; i < selectOrDeletePanels.Count(); i++)
		{
			SelectOrDeleteHeroButtonsGrid.Children.Remove(selectOrDeletePanels.ElementAt(i--));
		}
	}

	private StackPanel GenerateSelectOrDeletePanel(int heroPosition)
	{
		var hero = User.Instance.Heroes[heroPosition];
		var selectHeroButton = GenerateSelectHeroButton(hero);
		var deleteHeroButton = GenerateDeleteHeroButton(hero);

		var panel = new StackPanel
		{
			Orientation = Orientation.Horizontal,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center
		};

		panel.Children.Add(selectHeroButton);
		panel.Children.Add(deleteHeroButton);

		OrganizeSelectOrDeleteGrid(panel, heroPosition);

		return panel;
	}

	private Button GenerateDeleteHeroButton(Hero hero)
	{
		var toolTip = new ToolTip
		{
			Style = (Style)FindResource("ToolTipSimple")
		};

		var toolTipBlock = new TextBlock
		{
			Style = (Style)FindResource("ToolTipTextBlockBase"),
			Text = "This will permanently delete the hero and all of their items\nCurrencies (gold, ingots, dungeon keys) will not be reset"
		};

		toolTip.Content = toolTipBlock;

		var deleteHeroButton = new Button
		{
			Name = "DeleteHero" + hero.Id,
			Width = 50,
			Height = 50,
			Margin = new Thickness(5),
			Background = (SolidColorBrush)FindResource("BrushGray5"),
			Tag = hero
		};

		deleteHeroButton.ToolTip = toolTip;

		var deleteHeroIcon = new PackIcon
		{
			Width = 30,
			Height = 30,
			Kind = PackIconKind.DeleteForever,
			Foreground = (SolidColorBrush)FindResource("BrushBlack")
		};

		deleteHeroButton.Content = deleteHeroIcon;

		var deleteHeroButtonStyle = FindResource("ButtonStyleDanger") as Style;
		deleteHeroButton.Style = deleteHeroButtonStyle;

		deleteHeroButton.Click += DeleteHeroButton_Click;

		return deleteHeroButton;
	}

	private static void OrganizeSelectOrDeleteGrid(StackPanel panel, int heroPosition)
	{
		if (User.Instance.Heroes.Count < 4)
		{
			Grid.SetRow(panel, heroPosition + 1);
			Grid.SetColumnSpan(panel, 2);
		}
		else
		{
			if (heroPosition / 3 == 0)
			{
				panel.HorizontalAlignment = HorizontalAlignment.Right;
				panel.Margin = new Thickness(0, 0, 20, 0);
			}
			else
			{
				panel.HorizontalAlignment = HorizontalAlignment.Left;
				panel.Margin = new Thickness(20, 0, 0, 0);
			}

			Grid.SetRow(panel, heroPosition % 3 + 1);
			Grid.SetColumn(panel, heroPosition / 3);
		}
	}

	private Button GenerateSelectHeroButton(Hero hero)
	{
		var selectHeroButton = new Button
		{
			Name = "Hero" + hero.Id,
			Width = 250,
			Height = 50,
			Margin = new Thickness(5),
			Tag = hero,
			Background = (SolidColorBrush)FindResource("BrushGray1")
		};

		var selectHeroButtonBlock = new TextBlock
		{
			TextAlignment = TextAlignment.Center
		};

		var heroNameText = new Run(hero.Name)
		{
			FontFamily = (FontFamily)FindResource("FontFancy")
		};
		var heroLevelText = new Run($"\n{hero.Level} lvl | ");
		var heroClassText = new Run($"{hero.HeroClass}");
		var separator = new Run(" | ");
		var heroTotalTimePlayedText = new Run($"{Math.Floor(hero.TimePlayed.TotalHours)}h {hero.TimePlayed.Minutes}m")
		{
			FontFamily = (FontFamily)FindResource("FontRegularLightItalic")
		};

		heroNameText.FontSize = 20;
		switch (hero.HeroClass)
		{
			case HeroClass.Slayer:
				heroClassText.Foreground = (SolidColorBrush)FindResource("BrushSlayerRelated");
				break;

			case HeroClass.Venom:
				heroClassText.Foreground = (SolidColorBrush)FindResource("BrushVenomRelated");
				break;
		}

		selectHeroButtonBlock.Inlines.Add(heroNameText);
		selectHeroButtonBlock.Inlines.Add(heroLevelText);
		selectHeroButtonBlock.Inlines.Add(heroClassText);
		selectHeroButtonBlock.Inlines.Add(separator);
		selectHeroButtonBlock.Inlines.Add(heroTotalTimePlayedText);

		selectHeroButton.Content = selectHeroButtonBlock;

		selectHeroButton.Click += SelectHeroButton_Click;

		return selectHeroButton;
	}

	public void UpdateCreateHeroButton()
	{
		if (User.Instance.Heroes.Count == User.HeroLimit)
		{
			var disabledInfoBlock = new TextBlock
			{
				TextAlignment = TextAlignment.Center
			};

			var disabledText = new Run("Can't create new hero\nMax heroes reached!")
			{
				FontFamily = (FontFamily)FindResource("FontRegularLightItalic")
			};
			disabledInfoBlock.Inlines.Add(disabledText);

			CreateHeroButton.Content = disabledInfoBlock;
			CreateHeroButton.Style = FindResource("ButtonStyleDisabled") as Style;
		}
		else
		{
			CreateHeroButton.Content = "Create a new hero!";
			CreateHeroButton.Style = FindResource("ButtonStyleGeneral") as Style;
		}
	}

	private void CreateHeroButton_Click(object sender, RoutedEventArgs e)
	{
		if (User.Instance.Heroes.Count < User.HeroLimit)
		{
			InterfaceHelper.ChangePage(GameAssets.Pages["HeroCreation"], "Hero Creation");
		}
	}

	private static void SelectHeroButton_Click(object sender, RoutedEventArgs e)
	{
		var selectedHero = (sender as Button)?.Tag as Hero;
		User.Instance.CurrentHero = selectedHero;

		if (selectedHero != null)
		{
			selectedHero.LoadQuests();
			selectedHero.ResumeBlessing();
			selectedHero.ReequipArtifacts();
			selectedHero.Specializations.UpdateSpecialization();
			selectedHero.SessionStartDate = DateTime.Now;

			selectedHero.RefreshHeroExperience();
			GameAssets.RefreshPages();

			InterfaceHelper.ChangePage(GameAssets.Pages["Town"], "Town");

			selectedHero.ResumeQuest();
		}
	}

	private void DeleteHeroButton_Click(object sender, RoutedEventArgs e)
	{
		var hero = (sender as Button)?.Tag as Hero;

		var result = AlertBox.Show($"Press Yes to delete {hero.Name}.");
		if (result == MessageBoxResult.Yes)
		{
			User.Instance.Heroes.Remove(hero);

			UpdateSelectOrDeleteHeroButtons();
			UpdateCreateHeroButton();
		}
	}

	private void ResetProgressButton_Click(object sender, RoutedEventArgs e)
	{
		var result = AlertBox.Show("This action will delete all heroes, along with their equipment, as well as all currencies and achievements. Are you sure?");

		if (result == MessageBoxResult.Yes)
		{
			User.Instance.LastHeroId = 0;
			User.Instance.Heroes.Clear();
			User.Instance.Gold = 0;

			User.Instance.DungeonKeys.Clear();
			UserDataHelper.SeedDungeonKeys();

			User.Instance.Ingots.Clear();
			UserDataHelper.SeedIngots();

			User.Instance.Achievements.TotalTimePlayed = default;
			User.Instance.Achievements.NumericAchievementCollection = new ObservableDictionary<NumericAchievementType, long>();
			CollectionsHelper.InitializeDictionary(User.Instance.Achievements.NumericAchievementCollection);

			UpdateSelectOrDeleteHeroButtons();
			UpdateCreateHeroButton();
		}
	}

	private void CreditsBlock_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
	{
		AlertBox.Show("Nic", MessageBoxButton.OK);
	}
}