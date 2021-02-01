using ClickQuest.Data;
using ClickQuest.Items;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            // Remove all stackpanels from the grid.
            for (int i=0;i<ButtonsGrid.Children.Count;i++)
            {
                if (ButtonsGrid.Children[i] is StackPanel stack)
                {
                    ButtonsGrid.Children.Remove(stack);
                }
            }

            // For each hero:
            for (int i = 0; i < Account.User.Instance.Heroes.Count; i++)
            {
                // Create stack panel with both select and delete hero buttons.
                var panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;
                panel.HorizontalAlignment=HorizontalAlignment.Center;
                panel.VerticalAlignment=VerticalAlignment.Center;

                // Generate 'select hero' button.
                var selectHeroButton = new Button()
                {
                    Name = "Hero" + Account.User.Instance.Heroes[i].Id,
                    Content = Account.User.Instance.Heroes[i].Name + ", " + Account.User.Instance.Heroes[i].ThisHeroClass.ToString() + " [" + Account.User.Instance.Heroes[i].Level + " lvl]",
                    Width = 250,
                    Height = 50,
                    Margin = new Thickness(5)
                };

                // Generate 'delete hero' button.
                var deleteHeroButton = new Button()
                {
                    Name = "DeleteHero" + Account.User.Instance.Heroes[i].Id,
                    Content = "Delete hero",
                    Width = 150,
                    Height = 50,
                    Margin = new Thickness(5)
                };

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
            (Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["HeroCreation"]);
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

            // Remove the hero from User and Database.
            Account.User.Instance.Heroes.Remove(hero);
            Entity.EntityOperations.RemoveHero(hero);

            GenerateHeroButtons();
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