using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Controls;

namespace ClickQuest.Game.UserInterface.Pages
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
			var listOfIds = GameAssets.PriestOffer;

			foreach (int id in listOfIds)
			{
				result.Add(GameAssets.Blessings.FirstOrDefault(x => x.Id == id));
			}

			return result;
		}

		private void BuyButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var blessingBlueprint = b.CommandParameter as Blessing;

			if (User.Instance.Gold >= blessingBlueprint.Value)
			{
				var result = AlertBox.Show($"Are you sure you want to buy {blessingBlueprint.Name} for {blessingBlueprint.Value} gold?");

				if (result == MessageBoxResult.Cancel)
				{
					return;
				}

				bool hasBlessingActive = User.Instance.CurrentHero.Blessing != null;

				if (hasBlessingActive)
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
				AlertBox.Show($"You do not have enough gold to buy this blessing.\nIt costs {blessingBlueprint.Value} gold.\nYou can get more gold by completing quests and selling loot from monsters and bosses.", MessageBoxButton.OK);
			}
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameAssets.Pages["Town"], "Town");
		}
	}
}