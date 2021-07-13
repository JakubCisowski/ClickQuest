using ClickQuest.Player;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Entity;
using ClickQuest.Items;
using ClickQuest.Heroes;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;

namespace ClickQuest.Pages
{
	public partial class PriestPage : Page
	{
		public PriestPage()
		{
			InitializeComponent();

			UpdatePriest();
		}

		public void UpdatePriest()
		{
			ItemsListViewBuy.ItemsSource = GetPriestOfferAsBlessings();
			ItemsListViewBuy.Items.Refresh();
		}

		public List<Blessing> GetPriestOfferAsBlessings()
		{
			var result = new List<Blessing>();
			var listOfIds = GameData.PriestOffer;

			foreach (var id in listOfIds)
			{
				result.Add(GameData.Blessings.FirstOrDefault(x => x.Id == id));
			}

			return result;
		}

		#region Events
		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			// Go back to Town.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(GameData.Pages["Town"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Town";
			(GameData.Pages["Town"] as TownPage).EquipmentFrame.Refresh();
			(GameData.Pages["Town"] as TownPage).StatsFrame.Refresh();
		}

		private void BuyButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var blessingBlueprint = b.CommandParameter as Blessing;

			// Check if user has enough gold.
			if (User.Instance.Gold >= blessingBlueprint.Value)
			{
				// Show buy alert.
				var result = AlertBox.Show($"Are you sure you want to buy {blessingBlueprint.Name} for {blessingBlueprint.Value} gold?\nThis action will remove all your current blessings.", MessageBoxButton.YesNo);

				// If user clicked cancel on buy alert - return.
				if (result == MessageBoxResult.Cancel)
				{
					return;
				}

				// Remove gold, start blessing
				User.Instance.Gold -= blessingBlueprint.Value;

				// Increase Blessing Specialization amount.
				User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Blessing] ++;

				// Cancel current blessings.
				foreach (var bless in User.Instance.CurrentHero.Blessings)
				{
					bless.DisableBuff();
					EntityOperations.RemoveBlessing(bless);
				}
				// And remove them.
				User.Instance.CurrentHero.Blessings.Clear();

				// Create a new Blessing.
				var blessing = blessingBlueprint.CopyBlessing();

				// Increase his duration based on Blessing Specialization buff.
				blessing.Duration += User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Blessing];

				User.Instance.CurrentHero.Blessings.Add(blessing);
				blessing.EnableBuff();

				UpdatePriest();

				// Update StatsPage.
				this.StatsFrame.Refresh();
			}
			else
			{
				// Display an error.
				AlertBox.Show($"You do not have enough gold to buy this blessing.\nIt costs {blessingBlueprint.Value} gold.\nYou can get more gold by completing quests and selling loot from monsters and bosses.", MessageBoxButton.OK);
			}
		}
		#endregion
	}
}