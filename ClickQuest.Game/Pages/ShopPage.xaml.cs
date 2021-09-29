using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.Game.Controls;
using ClickQuest.Game.Data.GameData;
using ClickQuest.Game.Extensions.InterfaceManager;
using ClickQuest.Game.Heroes.Buffs;
using ClickQuest.Game.Items;
using ClickQuest.Game.Player;

namespace ClickQuest.Game.Pages
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

			if (User.Instance.CurrentHero != null)
			{
				ItemsListViewBuy.ItemsSource = GetShopOfferAsRecipes();
			}

			ItemsListViewSellMaterials.Items.Refresh();
			ItemsListViewSellRecipes.Items.Refresh();
			ItemsListViewBuy.Items.Refresh();
		}

		public List<Recipe> GetShopOfferAsRecipes()
		{
			var result = new List<Recipe>();
			var listOfIds = GameData.ShopOffer.Take(User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Buying]);

			foreach (int id in listOfIds)
			{
				result.Add(GameData.Recipes.FirstOrDefault(x => x.Id == id));
			}

			return result;
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameData.Pages["Town"], "Town");
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

			if (User.Instance.Gold >= recipe.Value)
			{
				var result = AlertBox.Show($"Are you sure you want to buy {recipe.Name} for {recipe.Value} gold?");
				if (result == MessageBoxResult.Cancel)
				{
					return;
				}

				recipe.AddItem();
				User.Instance.Gold -= recipe.Value;

				User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Buying]++;

				InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
				UpdateShop();
			}
			else
			{
				AlertBox.Show($"You do not have enough gold to buy this item.\nIt costs {recipe.Value} gold.\nYou can get more gold by completing quests and selling loot from monsters and bosses.", MessageBoxButton.OK);
			}
		}
	}
}