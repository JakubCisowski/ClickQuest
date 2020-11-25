using ClickQuest.Heroes;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class TownPage : Page
	{
		private Hero _hero;

		public TownPage()
		{
            InitializeComponent();

            _hero = new Hero(HeroClass.Slayer, "TestHeroName");

			this.DataContext = _hero;
		}

		private void ExpButton_Click(object sender, RoutedEventArgs e)
		{
			_hero.Experience++;
		}
	}
}
