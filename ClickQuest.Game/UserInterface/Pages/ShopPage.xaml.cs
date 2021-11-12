using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClickQuest.Game.Extensions.Gameplay;

namespace ClickQuest.Game.UserInterface.Pages
{
	public partial class ShopPage : Page
	{
		public const double SellingRatio = 0.65;

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
			var listOfPatterns = GameAssets.ShopOffer.Take(User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Trading]);

			foreach (var pattern in listOfPatterns)
			{
				result.Add(GameAssets.Recipes.FirstOrDefault(x => x.Id == pattern.Id));
			}

			return result;
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameAssets.Pages["Town"], "Town");
		}

		private void SellButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var item = b.CommandParameter as Item;

			int itemSellValue = 0;

			if (item.Rarity == Rarity.Mythic)
			{
				var result = AlertBox.Show($"Are you sure you want to sell {item.Name}?", MessageBoxButton.YesNo);

				if (result == MessageBoxResult.No)
				{
					return;
				}
			}

			if (item is Material)
			{
				// The selling ratio is only applied for materials.
				itemSellValue = (int)Math.Ceiling(item.Value * (SellingRatio + Specialization.SpecTradingRatioIncreasePerBuffValue * User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Trading]));
				GameController.UpdateSpecializationAmountAndUI(SpecializationType.Trading);
			}
			else
			{
				itemSellValue = item.Value;
			}

			(Application.Current.MainWindow as GameWindow).CreateFloatingTextUtility($"+{itemSellValue}", (SolidColorBrush)this.FindResource("BrushGold"), FloatingTextController.GoldPositionPoint);

			item.RemoveItem();
			User.Instance.Gold += itemSellValue;
			
			UpdateShop();
		}

		private void BuyButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var recipe = b.CommandParameter as Recipe;

			if (User.Instance.Gold >= recipe.Value)
			{
				var result = AlertBox.Show($"Are you sure you want to buy {recipe.Name} for {recipe.Value} gold?", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.No)
				{
					return;
				}

				(Application.Current.MainWindow as GameWindow).CreateFloatingTextUtility($"-{recipe.Value}", (SolidColorBrush)this.FindResource("BrushGold"), FloatingTextController.GoldPositionPoint);

				recipe.AddItem();
				User.Instance.Gold -= recipe.Value;

				GameController.UpdateSpecializationAmountAndUI(SpecializationType.Trading);

				UpdateShop();
			}
			else
			{
				AlertBox.Show($"You do not have enough gold to buy this item.\nIt costs {recipe.Value} gold.\nYou can get more gold by completing quests and selling loot from monsters and bosses.", MessageBoxButton.OK);
			}
		}
	}
}