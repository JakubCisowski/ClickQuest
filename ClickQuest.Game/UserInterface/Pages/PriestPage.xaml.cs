using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Windows;

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
			var listOfPatterns = GameAssets.PriestOffer;

			foreach (var pattern in listOfPatterns)
			{
				result.Add(GameAssets.Blessings.FirstOrDefault(x => x.Id == pattern.Id));
			}

			return result;
		}

		private void BuyButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var blessingBlueprint = b.CommandParameter as Blessing;

			if (User.Instance.Gold >= blessingBlueprint.Value)
			{
				var result = AlertBox.Show($"Are you sure you want to buy {blessingBlueprint.Name} for {blessingBlueprint.Value} gold?", MessageBoxButton.YesNo);

				if (result == MessageBoxResult.No)
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

				(Application.Current.MainWindow as GameWindow).CreateFloatingTextUtility($"-{blessingBlueprint.Value}", (SolidColorBrush)this.FindResource("BrushGold"), FloatingTextController.GoldPositionPoint);

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