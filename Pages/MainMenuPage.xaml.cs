using ClickQuest.Data;
using ClickQuest.Items;
using ClickQuest.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

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
			if (Account.User.Instance.Heroes.Count == 6)
			{
				var block = new TextBlock();
				block.TextAlignment = TextAlignment.Center;

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
			for (int i = 0; i < Account.User.Instance.Heroes.Count; i++)
			{
				var hero = Account.User.Instance.Heroes[i];
				// Create stack panel with both select and delete hero buttons.
				var panel = new StackPanel();
				panel.Orientation = Orientation.Horizontal;
				panel.HorizontalAlignment = HorizontalAlignment.Center;
				panel.VerticalAlignment = VerticalAlignment.Center;

				// Generate 'select hero' button.
				var selectHeroButton = new Button()
				{
					Name = "Hero" + hero.Id,
					Width = 250,
					Height = 50,
					Margin = new Thickness(5)
				};

				// Generate 'select hero' button content.
				var block = new TextBlock();
				block.TextAlignment = TextAlignment.Center;

				var bold = new Bold(new Run(hero.Name));
				block.Inlines.Add(bold);

				var normal = new Run($"\n{hero.Level} lvl | {hero.HeroClass}");
				block.Inlines.Add(normal);

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
				icon.Width=30;
				icon.Height=30;
				icon.Kind=PackIconKind.DeleteForever;
				icon.Foreground=new SolidColorBrush(Colors.Gray);
				deleteHeroButton.Content=icon;

				// Set deleteHeroButton's style to danger
				var style = this.FindResource("ButtonStyleDanger") as Style;
				deleteHeroButton.Style=style;

				// Assign click events to them.
				selectHeroButton.Click += SelectHeroButton_Click;
				deleteHeroButton.Click += DeleteHeroButton_Click;

				panel.Children.Add(selectHeroButton);
				panel.Children.Add(deleteHeroButton);

				// Add them to the button grid in main menu page.
				// If there is only one hero, center its buttons.
				if (Account.User.Instance.Heroes.Count < 4)
				{
					ButtonsGrid.Children.Add(panel);
					Grid.SetRow(panel, i + 1);
					Grid.SetColumnSpan(panel, 2);
				}
				// Else, create two columns.
				else
				{
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
			if (Account.User.Instance.Heroes.Count < 6)
			{
				(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["HeroCreation"]);
			}
		}

		private void SelectHeroButton_Click(object sender, RoutedEventArgs e)
		{
			// Select this hero as current hero and navigate to town.
			var id = int.Parse((sender as Button).Name.Substring(4));
			var hero = Account.User.Instance.Heroes.Where(x => x.Id == id).FirstOrDefault();
			Account.User.Instance.CurrentHero = hero;
			// Refresh hero stats panel info.
			hero.ExperienceToNextLvl = Heroes.Experience.CalculateXpToNextLvl(hero);

			// Resume quests for the selected hero.
			Account.User.Instance.CurrentHero.Quests.FirstOrDefault(x => x.EndDate != default(DateTime))?.StartQuest();

			// Resume blessings on this account.
			Blessing.ResumeBlessings();

			Data.Database.RefreshPages();
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["Town"]);
		}

		private void DeleteHeroButton_Click(object sender, RoutedEventArgs e)
		{
			var id = int.Parse((sender as Button).Name.Substring(10));
			var hero = Account.User.Instance.Heroes.Where(x => x.Id == id).FirstOrDefault();

			var result = AlertBox.Show($"Press OK to delete {hero.Name}.");

			if (result == MessageBoxResult.OK)
			{
				// Remove the hero from User and Database.
				Account.User.Instance.Heroes.Remove(hero);
				Entity.EntityOperations.RemoveHero(hero);

				GenerateHeroButtons();
			}
		}

		private void ResetProgressButton_Click(object sender, RoutedEventArgs e)
		{
			// Reset all progress - only account isn't removed.
			Entity.EntityOperations.ResetProgress();
			GenerateHeroButtons();
		}

		#endregion Events
	}
}