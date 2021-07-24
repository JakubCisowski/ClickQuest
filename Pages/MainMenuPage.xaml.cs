using ClickQuest.Player;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Entity;
using ClickQuest.Items;
using ClickQuest.Extensions.InterfaceManager;
using MaterialDesignThemes.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ClickQuest.Heroes;

namespace ClickQuest.Pages
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
			var deleteHeroButton = new Button()
			{
				Name = "DeleteHero" + hero.Id,
				Width = 50,
				Height = 50,
				Margin = new Thickness(5),
				Tag = hero
			};

			var deleteHeroIcon = new PackIcon()
			{
				Width = 30,
				Height = 30,
				Kind = PackIconKind.DeleteForever,
				Foreground = new SolidColorBrush(Colors.Gray)
			};

			deleteHeroButton.Content = deleteHeroIcon;

			var deleteHeroButtonStyle = this.FindResource("ButtonStyleDanger") as Style;
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

				Grid.SetRow(panel, (heroPosition % 3) + 1);
				Grid.SetColumn(panel, heroPosition / 3);
			}
		}

		private void ClearSelectOrDeletePanels()
		{
			var selectOrDeletePanels = SelectOrDeleteHeroButtonsGrid.Children.OfType<StackPanel>();

			for (int i = 0; i < selectOrDeletePanels.Count();i++)
			{
				SelectOrDeleteHeroButtonsGrid.Children.Remove(selectOrDeletePanels.ElementAt(i--));
			}
		}

		private Button GenerateSelectHeroButton(Hero hero)
		{
			var selectHeroButton = new Button()
			{
				Name = "Hero" + hero.Id,
				Width = 250,
				Height = 50,
				Margin = new Thickness(5),
				Tag = hero
			};

			var selectHeroButtonBlock = new TextBlock
			{
				TextAlignment = TextAlignment.Center
			};

			var heroNameText = new Bold(new Run(hero.Name));
			var heroLevelAndClassText = new Run($"\n{hero.Level} lvl | {hero.HeroClass} | ");
			var heroTotalTimePlayedText = new Italic(new Run($"{Math.Floor(hero.TimePlayed.TotalHours)}h {hero.TimePlayed.Minutes}m"));

			selectHeroButtonBlock.Inlines.Add(heroNameText);
			selectHeroButtonBlock.Inlines.Add(heroLevelAndClassText);
			selectHeroButtonBlock.Inlines.Add(heroTotalTimePlayedText);

			selectHeroButton.Content = selectHeroButtonBlock;
			
			selectHeroButton.Click += SelectHeroButton_Click;

			return selectHeroButton;
		}
		
		private void UpdateCreateHeroButton()
		{
			if (User.Instance.Heroes.Count == User.HERO_LIMIT)
			{
				var disabledInfoBlock = new TextBlock
				{
					TextAlignment = TextAlignment.Center
				};

				var disabledText = new Italic(new Run("Can't create new hero\nMax heroes reached!"));
				disabledInfoBlock.Inlines.Add(disabledText);

				CreateHeroButton.Content = disabledInfoBlock;
				CreateHeroButton.Style = this.FindResource("ButtonStyleDisabled") as Style;
			}
			else
			{
				CreateHeroButton.Content = "Create a new hero!";
				CreateHeroButton.Style = this.FindResource("ButtonStyleGeneral") as Style;
			}
		}

		#region Events

		private void CreateHeroButton_Click(object sender, RoutedEventArgs e)
		{
			if (User.Instance.Heroes.Count < User.HERO_LIMIT)
			{
				InterfaceController.ChangePage(Data.GameData.Pages["HeroCreation"], "Hero Creation");
			}
		}

		private void SelectHeroButton_Click(object sender, RoutedEventArgs e)
		{
			var selectedHero = (sender as Button).Tag as Hero;
			User.Instance.CurrentHero = selectedHero;
			
			selectedHero.LoadQuests();
			selectedHero.ResumeQuest();
			selectedHero.ResumeBlessing();
			selectedHero.SessionStartDate = DateTime.Now;

			selectedHero.RefreshHeroExperience();
			GameData.RefreshPages();

			InterfaceController.ChangePage(Data.GameData.Pages["Town"], "Town");
			InterfaceController.RefreshStatsAndEquipmentPanelsOnPage(GameData.Pages["Town"]);
		}

		private void DeleteHeroButton_Click(object sender, RoutedEventArgs e)
		{
			var hero = (sender as Button).Tag as Hero;

			var result = AlertBox.Show($"Press OK to delete {hero.Name}.");
			if (result == MessageBoxResult.OK)
			{
				User.Instance.Heroes.Remove(hero);
				EntityOperations.RemoveHero(hero);

				UpdateSelectOrDeleteHeroButtons();
				UpdateCreateHeroButton();
			}
		}

		private void ResetProgressButton_Click(object sender, RoutedEventArgs e)
		{
			EntityOperations.ResetProgress();
			UpdateSelectOrDeleteHeroButtons();
		}

		#endregion Events
	}
}