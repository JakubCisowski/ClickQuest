using ClickQuest.Account;
using ClickQuest.Data;
using ClickQuest.Items;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class QuestMenuPage : Page
	{
        public QuestMenuPage()
        {
            InitializeComponent();
        }

        private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Town"]);
		}
    }
}