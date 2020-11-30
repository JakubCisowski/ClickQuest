using ClickQuest.Account;
using ClickQuest.Heroes;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System;

namespace ClickQuest.Pages
{
	public partial class MainMenuPage : Page
    {
        public MainMenuPage()
        {
            InitializeComponent();
            GenerateHeroButtons();
        }

        private void CreateHeroButton_Click(object sender, RoutedEventArgs e)
        {
            // Add a random number to hero name to distinguish them
            var rng = new Random();

            var hero = new Hero(HeroClass.Slayer, "TestHeroName" + rng.Next(1,1000));
            User.Instance.Heroes.Add(hero);
            User.Instance.CurrentHero = hero;

            Data.Database.LoadPages();

            (Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["Town"]);
        }

        private void GenerateHeroButtons()
        {
            HeroesPanel.Children.Clear();

            for (int i = 0; i < Account.User.Instance.Heroes.Count; i++)
            {
                var button = new Button()
                {
                    Name = "Hero" + Account.User.Instance.Heroes[i].Id,
                    Content = Account.User.Instance.Heroes[i].Name + ", " + Account.User.Instance.Heroes[i].ThisHeroClass.ToString() + " [" + Account.User.Instance.Heroes[i].Level + " lvl]",
                    Width = 250,
                    Height = 50
                };

                  var button2 = new Button()
                {
                    Name = "DeleteHero" + Account.User.Instance.Heroes[i].Id,
                    Content = "Delete " + Account.User.Instance.Heroes[i].Name + ", " + Account.User.Instance.Heroes[i].ThisHeroClass.ToString() + " [" + Account.User.Instance.Heroes[i].Level + " lvl]",
                    Width = 250,
                    Height = 50
                };


                button.Click += SelectHeroButton_Click;

                button2.Click += DeleteHeroButton_Click;

                HeroesPanel.Children.Add(button);
                HeroesPanel.Children.Add(button2);
            }
        }

        private void SelectHeroButton_Click(object sender, RoutedEventArgs e)
        {
            var id = int.Parse((sender as Button).Name.Substring(4));
            var hero = Account.User.Instance.Heroes.Where(x => x.Id == id).FirstOrDefault();
            Account.User.Instance.CurrentHero = hero;

            Account.User.Instance.Gold = hero.Gold;

            Data.Database.LoadPages();
            (Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["Town"]);
        }

        private void DeleteHeroButton_Click(object sender, RoutedEventArgs e)
        {
            var id = int.Parse((sender as Button).Name.Substring(10));
            var hero = Account.User.Instance.Heroes.Where(x => x.Id == id).FirstOrDefault();
            Account.User.Instance.Heroes.Remove(hero);

            GenerateHeroButtons();
        }
    }
}