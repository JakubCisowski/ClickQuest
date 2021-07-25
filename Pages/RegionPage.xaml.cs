using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Places;

namespace ClickQuest.Pages
{
	public partial class RegionPage : Page
	{
		public RegionPage(Region currentRegion)
		{
			InitializeComponent();

			Region = currentRegion;
			DataContext = Region;

			CreateMonsterButton();
		}

		public Region Region { get; set; }

		public void CreateMonsterButton()
		{
			var button = new MonsterButton(this);
			RegionPanel.Children.Insert(1, button);
		}

		#region Events

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			// Stop poison and aura ticks (so that the monster doesn't die when we're outside of RegionPage + no exception when Timer attempts to tick in MainMenu).
			RegionPanel.Children.OfType<MonsterButton>().FirstOrDefault().StopCombatTimers();

			InterfaceController.ChangePage(GameData.Pages["Town"], "Town");
		}

		#endregion
	}
}