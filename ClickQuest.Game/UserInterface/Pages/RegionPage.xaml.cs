using System.Windows;
using System.Windows.Controls;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Places;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Controls;

namespace ClickQuest.Game.UserInterface.Pages
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
			MonsterButton button = new MonsterButton(this);
			RegionPanel.Children.Insert(1, button);
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			// Stop poison and aura ticks (so that the monster doesn't die when we're outside of RegionPage + no exception when Timer attempts to tick in MainMenu).
			CombatTimerController.StopCombatTimers();

			InterfaceController.ChangePage(GameAssets.Pages["Town"], "Town");

			// Invoke Artifacts with the "on-region-leave" effect.
			foreach (Artifact equippedArtifact in User.Instance.CurrentHero.EquippedArtifacts)
			{
				equippedArtifact.ArtifactFunctionality.OnRegionLeave();
			}
		}
	}
}