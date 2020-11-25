using ClickQuest.Heroes;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class MainPage : Page
	{
		private Hero _hero;

		public MainPage()
		{
            InitializeComponent();

            _hero = new Hero()
			{
				Name = "asdf",
				Experience = 0
			};

			this.DataContext = _hero;
		}

		private void ExpButton_Click(object sender, RoutedEventArgs e)
		{
			_hero.Experience++;
		}
	}
}
