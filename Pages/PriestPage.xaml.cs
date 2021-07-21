using ClickQuest.Player;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Entity;
using ClickQuest.Items;
using ClickQuest.Heroes.Buffs;
using ClickQuest.Heroes;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using ClickQuest.Extensions.InterfaceManager;

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
			InterfaceController.ChangePage(Data.GameData.Pages["Town"], "Town");
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
				var result = AlertBox.Show($"Are you sure you want to buy {blessingBlueprint.Name} for {blessingBlueprint.Value} gold?", MessageBoxButton.YesNo);

				if (result == MessageBoxResult.Cancel)
				{
					return;
				}

				bool hasBlessingActive = User.Instance.CurrentHero.Blessing != null;

				if(hasBlessingActive)
				{
					bool doesUserWantToSwap = Blessing.AskUserAndSwapBlessing(blessingBlueprint.Id);

					if (doesUserWantToSwap == false)
					{
						return;
					}
				}
				else
				{
					Blessing.AddOrReplaceBlessing(blessingBlueprint.Id);
				}

				User.Instance.Gold -= blessingBlueprint.Value;

				UpdatePriest();
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