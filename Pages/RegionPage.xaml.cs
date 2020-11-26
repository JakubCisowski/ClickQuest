using System.Windows.Controls;
using System.Windows;
using ClickQuest.Places;

namespace ClickQuest.Pages
{
    public partial class RegionPage : Page
    {
        private Region _region;

        public RegionPage(Region currentRegion)
        {
            InitializeComponent();

            _region = currentRegion;
            this.DataContext = _region;
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