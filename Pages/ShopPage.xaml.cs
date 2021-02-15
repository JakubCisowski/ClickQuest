using ClickQuest.Account;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Items;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class ShopPage : Page
	{
		public ShopPage()
		{
			InitializeComponent();
			UpdateShop();
		}

		public void UpdateShop()
		{
			ItemsListViewSell.ItemsSource = User.Instance.Materials.Cast<Item>().Concat(User.Instance.Recipes.Cast<Item>());

			// Calculate shop offer size according to specialization bonus (base bonus: 5).
			ItemsListViewBuy.ItemsSource = Database.ShopOffer.Take(Account.User.Instance.Specialization.SpecBuyingBuff);

			ItemsListViewSell.Items.Refresh();
			ItemsListViewBuy.Items.Refresh();
		}

		#region Events

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			// Go back to Town.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Town"]);
			(Database.Pages["Town"] as TownPage).EquipmentFrame.Refresh();
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Town";
		}

		private void SellButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var item = b.CommandParameter as Item;

			Account.User.Instance.RemoveItem(item);
			Account.User.Instance.Gold += item.Value;

			(Data.Database.Pages["Shop"] as ShopPage).EquipmentFrame.Refresh();
			UpdateShop();
		}

		private void BuyButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var recipe = b.CommandParameter as Recipe;

			// Check if user has enough gold.
			if (Account.User.Instance.Gold >= recipe.Value)
			{
				// Show buy alert.
				var result = AlertBox.Show($"Are you sure you want to buy {recipe.Name} for {recipe.Value} gold?", MessageBoxButton.YesNo);

				// If user clicked cancel on buy alert - return.
				if (result == MessageBoxResult.Cancel)
				{
					return;
				}
			
				Account.User.Instance.AddItem(recipe);
				Account.User.Instance.Gold -= recipe.Value;

				(Data.Database.Pages["Shop"] as ShopPage).EquipmentFrame.Refresh();
				UpdateShop();

				// Increase Specialization Buying amount.
				Account.User.Instance.Specialization.SpecBuyingAmount++;
			}
			else
			{
				// Display an error.
				AlertBox.Show($"You do not have enough gold to buy this item.\nIt costs {recipe.Value} gold.\nYou can get more gold by completing quests and selling loot from monsters and bosses.", MessageBoxButton.OK);
			}
		}

		#endregion
	}
}