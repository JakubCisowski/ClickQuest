using System.Windows;
using System.Windows.Controls;
using ClickQuest.Game.Controls;
using ClickQuest.Game.Data.GameData;
using ClickQuest.Game.Extensions.CombatManager;
using ClickQuest.Game.Extensions.InterfaceManager;
using ClickQuest.Game.Places;
using ClickQuest.Game.Player;

namespace ClickQuest.Game.Pages
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