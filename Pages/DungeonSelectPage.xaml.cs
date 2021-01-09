using System.Windows;
using System.Windows.Controls;
using ClickQuest.Data;

namespace ClickQuest.Pages
{
	public partial class DungeonSelectPage : Page
	{
        public DungeonSelectPage()
        {
            InitializeComponent();
        }

        private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Town"]);
		}
    }
}