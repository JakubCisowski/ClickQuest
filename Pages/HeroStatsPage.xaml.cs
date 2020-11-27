using ClickQuest.Heroes;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class HeroStatsPage : Page
	{
		private Hero _hero;

		public HeroStatsPage()
		{
			InitializeComponent();

			_hero = Account.User.Instance.CurrentHero;
			this.DataContext = _hero;
		}

		private void ShowEquipmentButton_Click(object sender, RoutedEventArgs e)
		{
            EquipmentWindow.Instance.Show();
        }
	}
}