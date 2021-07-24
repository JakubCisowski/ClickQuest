using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Places;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class RegionPage : Page
	{
		private Region _region;

		public Region Region
		{
			get
			{
				return _region;
			}
			set
			{
				_region = value;
			}
		}

		public RegionPage(Region currentRegion)
		{
			InitializeComponent();

			_region = currentRegion;
			this.DataContext = _region;

			CreateMonsterButton();
		}

		public void CreateMonsterButton()
		{
			// Create a new MonsterButton control.
			var button = new MonsterButton(this);
			this.RegionPanel.Children.Insert(1, button);
		}

		#region Events

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			// Stop poison and aura ticks (so that the monster doesn't die when we're outside of RegionPage; and so that there's no exception when Timer attempts to tick in MainMenu).
			foreach (var ctrl in RegionPanel.Children)
			{
				if (ctrl is MonsterButton m)
				{
					m.StopCombatTimers();
					// Break - there should never be more than one MonsterButton.
					break;
				}
			}

			// Go back to Town.
			InterfaceController.ChangePage(Data.GameData.Pages["Town"], "Town");
		}

		#endregion
	}
}