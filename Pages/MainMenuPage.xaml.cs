using ClickQuest.Account;
using ClickQuest.Heroes;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class MainMenuPage : Page
	{

		public MainMenuPage()
		{
			InitializeComponent();
		}

		private void CreateHeroButton_Click(object sender, RoutedEventArgs e)
		{
			var hero = new Hero(HeroClass.Slayer, "TestHeroName");
			User.Heroes.Add(hero);
			User.CurrentHero = hero;

			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(new TownPage());
		}
	}
}
