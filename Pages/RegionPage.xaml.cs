using System.Windows.Controls;
using System.Windows;
using ClickQuest.Places;

namespace ClickQuest.Pages
{
    public partial class RegionPage : Page
    {
        public RegionPage(Region currentRegion)
        {
            this.DataContext = currentRegion;
            InitializeComponent();
        }

        private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(new TownPage());
		}

        /*private void ExpButton_Click(object sender, RoutedEventArgs e)
		{
			_hero.Experience++;
		}*/
    }
}