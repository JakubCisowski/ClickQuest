using ClickQuest.Player;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Entity;
using ClickQuest.Items;
using MaterialDesignThemes.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ClickQuest.Pages
{
	public partial class MainMenuPage : Page
	{
		public MainMenuPage()
		{
			InitializeComponent();
			GenerateHeroButtons();
		}

		public void GenerateHeroButtons()
		{
			// If max hero limit is reached, disable create hero button.
			if (User.Instance.Heroes.Count == 6)
			{
				var block = new TextBlock
				{
					TextAlignment = TextAlignment.Center
				};

				var italic = new Italic(new Run("Can't create new hero\nMax heroes reached!"));
				block.Inlines.Add(italic);

				CreateHeroButton.Content = block;
				CreateHeroButton.Style = this.FindResource("ButtonStyleDisabled") as Style;
			}
			else
			{
				CreateHeroButton.Content = "Create a new hero!";
				CreateHeroButton.Style = this.FindResource("ButtonStyleGeneral") as Style;
			}

			// Remove all stackpanels from the grid.
			for (int i = 0; i < ButtonsGrid.Children.Count; i++)
			{
				if (ButtonsGrid.Children[i] is StackPanel stack)
				{
					ButtonsGrid.Children.Remove(stack);
					i--;
				}
			}

			// For each hero:
			for (int i = 0; i < User.Instance.Heroes.Count; i++)
			{
				var hero = User.Instance.Heroes[i];
				// Create stack panel with both select and delete hero buttons.
				var panel = new StackPanel
				{
					Orientation = Orientation.Horizontal,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				};

				// Generate 'select hero' button.
				var selectHeroButton = new Button()
				{
					Name = "Hero" + hero.Id,
					Width = 250,
					Height = 50,
					Margin = new Thickness(5)
				};

				// Generate 'select hero' button content.
				var block = new TextBlock
				{
					TextAlignment = TextAlignment.Center
				};

				var bold = new Bold(new Run(hero.Name));
				block.Inlines.Add(bold);

				var normal = new Run($"\n{hero.Level} lvl | {hero.HeroClass} | ");
				block.Inlines.Add(normal);

				// Add time played.
				block.Inlines.Add(new Italic(new Run($"{Math.Floor(hero.TimePlayed.TotalHours)}h {hero.TimePlayed.Minutes}m")));

				selectHeroButton.Content = block;

				// Generate 'delete hero' button.
				var deleteHeroButton = new Button()
				{
					Name = "DeleteHero" + hero.Id,
					Width = 50,
					Height = 50,
					Margin = new Thickness(5)
				};

				var icon = new PackIcon();
				icon.Width = 30;
				icon.Height = 30;
				icon.Kind = PackIconKind.DeleteForever;
				icon.Foreground = new SolidColorBrush(Colors.Gray);
				deleteHeroButton.Content = icon;

				// Set deleteHeroButton's style to danger
				var style = this.FindResource("ButtonStyleDanger") as Style;
				deleteHeroButton.Style = style;

				// Assign click events to them.
				selectHeroButton.Click += SelectHeroButton_Click;
				deleteHeroButton.Click += DeleteHeroButton_Click;

				panel.Children.Add(selectHeroButton);
				panel.Children.Add(deleteHeroButton);

				// Add them to the button grid in main menu page.
				// If there is only one hero, center its buttons.
				if (User.Instance.Heroes.Count < 4)
				{
					ButtonsGrid.Children.Add(panel);
					Grid.SetRow(panel, i + 1);
					Grid.SetColumnSpan(panel, 2);
				}
				// Else, create two columns.
				else
				{
					if (i / 3 == 0)
					{
						panel.HorizontalAlignment = HorizontalAlignment.Right;
						panel.Margin = new Thickness(0, 0, 20, 0);
					}
					else
					{
						panel.HorizontalAlignment = HorizontalAlignment.Left;
						panel.Margin = new Thickness(20, 0, 0, 0);
					}

					ButtonsGrid.Children.Add(panel);
					Grid.SetRow(panel, (i % 3) + 1);
					Grid.SetColumn(panel, i / 3);
				}
			}
		}

		#region Events

		private void CreateHeroButton_Click(object sender, RoutedEventArgs e)
		{
			// If the limit is not reached, move to hero creation page.
			if (User.Instance.Heroes.Count < 6)
			{
				(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(GameData.Pages["HeroCreation"]);
				(Window.GetWindow(this) as GameWindow).LocationInfo = "Hero creation";
			}
		}

		private void SelectHeroButton_Click(object sender, RoutedEventArgs e)
		{
			// Select this hero as current hero and navigate to town.
			var id = int.Parse((sender as Button).Name.Substring(4));
			var hero = User.Instance.Heroes.Where(x => x.Id == id).FirstOrDefault();
			User.Instance.CurrentHero = hero;
			// Refresh hero stats panel info.
			hero.RefreshHeroExperience();

			// Clone hero's quests using those from Database.
			foreach (var heroQuest in hero.Quests)
			{
				var databaseQuest = GameData.Quests.FirstOrDefault(x => x.Id == heroQuest.Id);
				heroQuest.CopyQuest(databaseQuest);
			}

			// Resume quests for the selected hero.
			User.Instance.CurrentHero.Quests.FirstOrDefault(x => x.EndDate != default(DateTime))?.StartQuest();

			// Resume blessings for dis hero (it resumes only for current hero).
			Blessing.ResumeBlessings();

			// Set hero session start date.
			hero.SessionStartDate = DateTime.Now;

			// Refresh pages, move to town and change location text.
			GameData.RefreshPages();
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Data.GameData.Pages["Town"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Town";
			(GameData.Pages["Town"] as TownPage).StatsFrame.Refresh();
			(GameData.Pages["Town"] as TownPage).EquipmentFrame.Refresh();
		}

		private void DeleteHeroButton_Click(object sender, RoutedEventArgs e)
		{
			var id = int.Parse((sender as Button).Name.Substring(10));
			var hero = User.Instance.Heroes.Where(x => x.Id == id).FirstOrDefault();

			var result = AlertBox.Show($"Press OK to delete {hero.Name}.");

			if (result == MessageBoxResult.OK)
			{
				// Remove the hero from User and Database.
				User.Instance.Heroes.Remove(hero);
				EntityOperations.RemoveHero(hero);

				GenerateHeroButtons();
			}
		}

		private void ResetProgressButton_Click(object sender, RoutedEventArgs e)
		{
			// Reset all progress - only Player isn't removed.
			EntityOperations.ResetProgress();
			GenerateHeroButtons();
		}

		#endregion Events
	}
}