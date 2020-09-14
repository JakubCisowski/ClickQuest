using ClickQuest.Heroes;
using System.Windows;

namespace ClickQuest
{
	public partial class MainWindow : Window
	{
		private Hero _hero;

		public MainWindow()
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
