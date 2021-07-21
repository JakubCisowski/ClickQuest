using ClickQuest.Player;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Items;
using ClickQuest.Heroes;
using ClickQuest.Heroes.Buffs;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using ClickQuest.Extensions.InterfaceManager;

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
			ItemsListViewSellMaterials.ItemsSource = User.Instance.CurrentHero?.Materials;
			ItemsListViewSellRecipes.ItemsSource = User.Instance.CurrentHero?.Recipes;

			// Calculate shop offer size according to specialization bonus (base bonus: 5).
			if (User.Instance.CurrentHero != null)
			{
				ItemsListViewBuy.ItemsSource = GetShopOfferAsRecipes();
			}

			ItemsListViewSellMaterials.Items.Refresh();
			ItemsListViewSellRecipes.Items.Refresh();
			ItemsListViewBuy.Items.Refresh();
		}

		#region Events

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			// Go back to Town.
			(GameData.Pages["Town"] as TownPage).EquipmentFrame.Refresh();
			(GameData.Pages["Town"] as TownPage).StatsFrame.Refresh();
			InterfaceController.ChangePage(Data.GameData.Pages["Town"], "Town");
		}

		private void SellButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var item = b.CommandParameter as Item;

			item.RemoveItem();
			User.Instance.Gold += item.Value;

			(GameData.Pages["Shop"] as ShopPage).EquipmentFrame.Refresh();
			UpdateShop();
		}

		private void BuyButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var recipe = b.CommandParameter as Recipe;

			// Check if user has enough gold.
			if (User.Instance.Gold >= recipe.Value)
			{
				// Show buy alert.
				var result = AlertBox.Show($"Are you sure you want to buy {recipe.Name} for {recipe.Value} gold?", MessageBoxButton.YesNo);

				// If user clicked cancel on buy alert - return.
				if (result == MessageBoxResult.Cancel)
				{
					return;
				}

				recipe.AddItem();
				User.Instance.Gold -= recipe.Value;

				(GameData.Pages["Shop"] as ShopPage).EquipmentFrame.Refresh();
				
				// Refresh stats frame (for specialization update).
				(GameData.Pages["Shop"] as ShopPage).StatsFrame.Refresh();
				UpdateShop();

				// Increase Specialization Buying amount.
				User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Buying]++;
			}
			else
			{
				// Display an error.
				AlertBox.Show($"You do not have enough gold to buy this item.\nIt costs {recipe.Value} gold.\nYou can get more gold by completing quests and selling loot from monsters and bosses.", MessageBoxButton.OK);
			}
		}

		#endregion Events

		public List<Recipe> GetShopOfferAsRecipes()
		{
			var result = new List<Recipe>();
			var listOfIds = GameData.ShopOffer.Take(User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Buying]);

			foreach (var id in listOfIds)
			{
				result.Add(GameData.Recipes.FirstOrDefault(x => x.Id == id));
			}

			return result;
		}
	}
}