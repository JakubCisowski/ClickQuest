using ClickQuest.Account;
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
			ItemsListViewSell.ItemsSource = User.Instance.Materials;
			ItemsListViewBuy.ItemsSource = Database.ShopOffer.Where(x => !Account.User.Instance.Recipes.Contains(x));

			ItemsListViewSell.Items.Refresh();
			ItemsListViewBuy.Items.Refresh();
		}

		#region Events

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Town"]);
		}

		private void SellButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var material = b.CommandParameter as Material;

			Account.User.Instance.RemoveItem(material);
			Account.User.Instance.Gold += material.Value;

			EquipmentWindow.Instance.UpdateEquipment();
			UpdateShop();
		}

		private void BuyButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var recipe = b.CommandParameter as Recipe;

			Account.User.Instance.AddItem(recipe);

			EquipmentWindow.Instance.UpdateEquipment();
			UpdateShop();
		}

		#endregion
	}
}