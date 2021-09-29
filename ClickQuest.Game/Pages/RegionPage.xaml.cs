using System.Windows;
using System.Windows.Controls;
using ClickQuest.Controls;
using ClickQuest.Data.GameData;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Places;
using ClickQuest.Player;

namespace ClickQuest.Pages
{
	public partial class RegionPage : Page
	{
		public Region Region { get; set; }

		public RegionPage(Region currentRegion)
		{
			InitializeComponent();

			Region = currentRegion;
			DataContext = Region;

			CreateMonsterButton();
		}

		public void CreateMonsterButton()
		{
			var button = new MonsterButton(this);
			RegionPanel.Children.Insert(1, button);
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			// Stop poison and aura ticks (so that the monster doesn't die when we're outside of RegionPage + no exception when Timer attempts to tick in MainMenu).
			CombatTimerController.StopCombatTimers();

			InterfaceController.ChangePage(GameData.Pages["Town"], "Town");

			// Invoke Artifacts with the "on-region-leave" effect.
			foreach (var equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
			{
				equippedArtifact.ArtifactFunctionality.OnRegionLeave();
			}
		}
	}
}