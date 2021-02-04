using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Places;
using System.Windows;
using System.Windows.Controls;

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

			CreateMonsterButton();
		}

		public void CreateMonsterButton()
		{
			var button = new MonsterButton(_region, this);
			this.RegionPanel.Children.Insert(1, button);
		}

		#region Events

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Town"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Town";
		}

		#endregion
	}
}