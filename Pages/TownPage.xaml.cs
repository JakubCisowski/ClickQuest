using ClickQuest.Heroes;
using ClickQuest.Places;
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

		private void Test1_Click(object sender, RoutedEventArgs e)
		{
			//(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(new RegionPage(new Region(){Name="Test1"}));
		}

		private void Test2_Click(object sender, RoutedEventArgs e)
		{
			//(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(new RegionPage(new Region(){Name="Test2"}));
		}
	}
}
