using ClickQuest.Account;
using ClickQuest.Heroes;
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
            GenerateRegionButtons();
        }

        private void CreateHeroButton_Click(object sender, RoutedEventArgs e)
        {
            Entity.EntityOperations.ClearHeroes();
            var hero = new Hero(HeroClass.Slayer, "TestHeroName");
            User.Instance.Heroes.Add(hero);
            User.Instance.CurrentHero = hero;

            Data.Database.LoadPages();

            (Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["Town"]);
        }

        private void GenerateRegionButtons()
        {
            for (int i = 0; i < Account.User.Instance.Heroes.Count; i++)
            {
                var button = new Button()
                {
                    Name = "Hero" + Account.User.Instance.Heroes[i].Id,
                    Content = Account.User.Instance.Heroes[i].Name + ", " + Account.User.Instance.Heroes[i].ThisHeroClass.ToString() + " [" + Account.User.Instance.Heroes[i].Level + " lvl]",
                    Width = 50,
                    Height = 50
                };

                button.Click += SelectHeroButton_Click;

                HeroesPanel.Children.Add(button);
            }
        }

        private void SelectHeroButton_Click(object sender, RoutedEventArgs e)
        {
            var id = int.Parse((sender as Button).Name.Substring(4));
            var hero = Account.User.Instance.Heroes.Where(x => x.Id == id).FirstOrDefault();
            Account.User.Instance.CurrentHero = hero;

            Data.Database.LoadPages();
            (Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["Town"]);
        }
    }
}