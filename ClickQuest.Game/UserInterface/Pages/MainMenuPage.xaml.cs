using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Gameplay;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.Extensions.UserInterface.ToolTips;
using ClickQuest.Game.UserInterface.Controls;
using MaterialDesignThemes.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ClickQuest.Game.UserInterface.Pages
{
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

			for (int i = 0; i < User.Instance.Heroes.Count; i++)
			{
				SelectOrDeleteHeroButtonsGrid.Children.Add(GenerateSelectOrDeletePanel(i));
			}
		}

		private void ClearSelectOrDeletePanels()
		{
			var selectOrDeletePanels = SelectOrDeleteHeroButtonsGrid.Children.OfType<StackPanel>();

			for (int i = 0; i < selectOrDeletePanels.Count(); i++)
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
			var toolTip = new ToolTip()
			{
				Style= (Style)FindResource("ToolTipSimple")
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

			GeneralToolTipController.SetToolTipDelayAndDuration(deleteHeroButton);
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

		private void OrganizeSelectOrDeleteGrid(StackPanel panel, int heroPosition)
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
				Background = (SolidColorBrush)this.FindResource("BrushGray1")
			};

			var selectHeroButtonBlock = new TextBlock
			{
				TextAlignment = TextAlignment.Center
			};

			var heroNameText = new Run(hero.Name) { FontFamily = (FontFamily)this.FindResource("FontFancy") };
			var heroLevelText = new Run($"\n{hero.Level} lvl | ");
			var heroClassText = new Run($"{hero.HeroClass}");
			var separator = new Run(" | ");
			var heroTotalTimePlayedText = new Run($"{Math.Floor(hero.TimePlayed.TotalHours)}h {hero.TimePlayed.Minutes}m") { FontFamily = (FontFamily)this.FindResource("FontRegularLightItalic") };

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
			if (User.Instance.Heroes.Count == User.HERO_LIMIT)
			{
				var disabledInfoBlock = new TextBlock
				{
					TextAlignment = TextAlignment.Center
				};

				var disabledText = new Run("Can't create new hero\nMax heroes reached!") { FontFamily = (FontFamily)this.FindResource("FontRegularLightItalic") };
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
			if (User.Instance.Heroes.Count < User.HERO_LIMIT)
			{
				InterfaceController.ChangePage(GameAssets.Pages["HeroCreation"], "Hero Creation");
			}
		}

		private void SelectHeroButton_Click(object sender, RoutedEventArgs e)
		{
			var selectedHero = (sender as Button).Tag as Hero;
			User.Instance.CurrentHero = selectedHero;

			selectedHero.LoadQuests();
			selectedHero.ResumeBlessing();
			selectedHero.ReequipArtifacts();
			selectedHero.SessionStartDate = DateTime.Now;

			selectedHero.RefreshHeroExperience();
			GameAssets.RefreshPages();

			InterfaceController.ChangePage(GameAssets.Pages["Town"], "Town");

			selectedHero.ResumeQuest();
		}

		private void DeleteHeroButton_Click(object sender, RoutedEventArgs e)
		{
			var hero = (sender as Button).Tag as Hero;

			var result = AlertBox.Show($"Press Yes to delete {hero.Name}.", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
			{
				User.Instance.Heroes.Remove(hero);

				UpdateSelectOrDeleteHeroButtons();
				UpdateCreateHeroButton();
			}
		}

		private void ResetProgressButton_Click(object sender, RoutedEventArgs e)
		{
			var result = AlertBox.Show("This action will delete all heroes, along with their equipment, as well as all currencies and achievements. Are you sure?", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes)
			{
				GameController.ResetAllProgress();
				UpdateSelectOrDeleteHeroButtons();
				UpdateCreateHeroButton();
			}
		}

		private void CreditsBlock_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			AlertBox.Show("Nic", MessageBoxButton.OK);
		}
	}
}