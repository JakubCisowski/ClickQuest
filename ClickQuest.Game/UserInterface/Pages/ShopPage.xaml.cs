using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Helpers;
using ClickQuest.Game.UserInterface.Windows;

namespace ClickQuest.Game.UserInterface.Pages;

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
		ItemsListViewSellMaterials.ItemsSource = User.Instance.CurrentHero?.Materials.ReorderItemsInList();
		ItemsListViewSellRecipes.ItemsSource = User.Instance.CurrentHero?.Recipes.ReorderItemsInList();

		if (User.Instance.CurrentHero != null)
		{
			ItemsListViewBuy.ItemsSource = GetShopOfferAsRecipes();
		}

		ItemsListViewSellMaterials.Items.Refresh();
		ItemsListViewSellRecipes.Items.Refresh();
		ItemsListViewBuy.Items.Refresh();

		RefreshScrollBarVisibilities();
	}

	public void RefreshScrollBarVisibilities()
	{
		if (ItemsListViewSellMaterials.Items.Count > InterfaceHelper.VendorItemsNeededToShowScrollBar)
		{
			ScrollViewer.SetVerticalScrollBarVisibility(ItemsListViewSellMaterials, ScrollBarVisibility.Visible);
		}
		else
		{
			ScrollViewer.SetVerticalScrollBarVisibility(ItemsListViewSellMaterials, ScrollBarVisibility.Disabled);
		}

		if (ItemsListViewSellRecipes.Items.Count > InterfaceHelper.VendorItemsNeededToShowScrollBar)
		{
			ScrollViewer.SetVerticalScrollBarVisibility(ItemsListViewSellRecipes, ScrollBarVisibility.Visible);
		}
		else
		{
			ScrollViewer.SetVerticalScrollBarVisibility(ItemsListViewSellRecipes, ScrollBarVisibility.Disabled);
		}

		if (ItemsListViewBuy.Items.Count > InterfaceHelper.VendorItemsNeededToShowScrollBar)
		{
			ScrollViewer.SetVerticalScrollBarVisibility(ItemsListViewBuy, ScrollBarVisibility.Visible);
		}
		else
		{
			ScrollViewer.SetVerticalScrollBarVisibility(ItemsListViewBuy, ScrollBarVisibility.Disabled);
		}
	}

	public static List<Recipe> GetShopOfferAsRecipes()
	{
		var listOfPatterns = GameAssets.ShopOffer.Take(User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Trading]);

		return listOfPatterns.Select(pattern => GameAssets.Recipes.FirstOrDefault(x => x.Id == pattern.Id)).ToList();
	}

	private void TownButton_Click(object sender, RoutedEventArgs e)
	{
		InterfaceHelper.ChangePage(GameAssets.Pages["Town"], "Town");
	}

	private void SellOneButton_Click(object sender, RoutedEventArgs e)
	{
		var b = sender as Button;
		var item = b.CommandParameter as Item;

		HandleItemSelling(item, 1);
	}

	private void HandleItemSelling(Item item, int amount)
	{
		var itemSellValue = 0;

		if (item.Rarity == Rarity.Mythic)
		{
			var sellItemRuns = new List<Run>
			{
				new Run("Are you sure you want to sell "),
				new Run($"{amount}x {item.Name}")
				{
					FontFamily = (FontFamily)FindResource("FontRegularDemiBold")
				},
				new Run("?")
			};

			var result = AlertBox.Show(sellItemRuns);

			if (result == MessageBoxResult.No)
			{
				return;
			}
		}

		if (item is Material)
		{
			// The selling ratio is only applied for materials.
			itemSellValue = (int)Math.Ceiling(item.Value * amount * (SellingRatio + Specializations.SpecTradingRatioIncreasePerBuffValue * User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Trading]));
			Specializations.UpdateSpecializationAmountAndUi(SpecializationType.Trading, amount);
		}
		else
		{
			itemSellValue = item.Value * amount;
		}

		(Application.Current.MainWindow as GameWindow).CreateFloatingTextUtility($"+{itemSellValue}", (SolidColorBrush)FindResource("BrushGold"), FloatingTextHelper.GoldPositionPoint);

		item.RemoveItem(amount);
		User.Instance.Gold += itemSellValue;

		UpdateShop();
	}

	private void BuyButton_Click(object sender, RoutedEventArgs e)
	{
		var b = sender as Button;
		var recipe = b.CommandParameter as Recipe;

		if (User.Instance.Gold >= recipe.Value)
		{
			var buyRecipeRuns = new List<Run>
			{
				new Run("Are you sure you want to buy "),
				new Run($"{recipe.Name}")
				{
					FontFamily = (FontFamily)FindResource("FontRegularDemiBold")
				},
				new Run(" for "),
				new Run($"{recipe.Value} gold")
				{
					Foreground = (SolidColorBrush)FindResource("BrushGold"),
					FontFamily = (FontFamily)FindResource("FontRegularDemiBold")
				},
				new Run("?")
			};

			var result = AlertBox.Show(buyRecipeRuns);
			if (result == MessageBoxResult.No)
			{
				return;
			}

			(Application.Current.MainWindow as GameWindow).CreateFloatingTextUtility($"-{recipe.Value}", (SolidColorBrush)FindResource("BrushGold"), FloatingTextHelper.GoldPositionPoint);

			recipe.AddItem();
			User.Instance.Gold -= recipe.Value;

			Specializations.UpdateSpecializationAmountAndUi(SpecializationType.Trading);

			UpdateShop();
		}
		else
		{
			var notEnoughGoldRuns = new List<Run>
			{
				new Run("You do not have enough gold to buy this item.\nIt costs "),
				new Run($"{recipe.Value} gold")
				{
					Foreground = (SolidColorBrush)FindResource("BrushGold"),
					FontFamily = (FontFamily)FindResource("FontRegularDemiBold")
				},
				new Run(".\nYou can get more gold by completing quests and selling loot from monsters and bosses.")
			};

			AlertBox.Show(notEnoughGoldRuns, MessageBoxButton.OK);
		}
	}

	private void SellButton_OnInitialized(object sender, EventArgs e)
	{
		var button = sender as Button;

		if (button?.ToolTip == null)
		{
			var itemSellValue = 0;

			if (button.CommandParameter is Material material)
			{
				itemSellValue = (int)Math.Ceiling(material.Value * (SellingRatio + Specializations.SpecTradingRatioIncreasePerBuffValue * User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Trading]));
			}
			else
			{
				itemSellValue = (button.CommandParameter as Item).Value;
			}

			var toolTip = new ToolTip
			{
				Style = (Style)FindResource("ToolTipSimple")
			};

			var toolTipBlock = new TextBlock
			{
				Style = (Style)FindResource("ToolTipTextBlockBase")
			};

			toolTipBlock.Inlines.Add(new Run("Sell for "));
			toolTipBlock.Inlines.Add(new Run($"{itemSellValue} gold")
			{
				FontFamily = (FontFamily)FindResource("FontRegularDemiBold"),
				Foreground = (SolidColorBrush)FindResource("BrushGold")
			});

			toolTip.Content = toolTipBlock;

			button.ToolTip = toolTip;
		}
	}

	private void BuyButton_OnInitialized(object sender, EventArgs e)
	{
		var button = sender as Button;

		if (button?.ToolTip == null)
		{
			var toolTip = new ToolTip
			{
				Style = (Style)FindResource("ToolTipSimple")
			};

			var toolTipBlock = new TextBlock
			{
				Style = (Style)FindResource("ToolTipTextBlockBase")
			};

			toolTipBlock.Inlines.Add(new Run("Buy for "));
			toolTipBlock.Inlines.Add(new Run($"{(button.CommandParameter as Item).Value} gold")
			{
				FontFamily = (FontFamily)FindResource("FontRegularDemiBold"),
				Foreground = (SolidColorBrush)FindResource("BrushGold")
			});

			toolTip.Content = toolTipBlock;

			button.ToolTip = toolTip;
		}
	}
}