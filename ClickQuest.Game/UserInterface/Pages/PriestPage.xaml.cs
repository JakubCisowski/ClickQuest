using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ClickQuest.Game.Models;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Helpers;
using ClickQuest.Game.UserInterface.Windows;

namespace ClickQuest.Game.UserInterface.Pages;

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

		if (ItemsListViewBuy.Items.Count > InterfaceHelper.VendorItemsNeededToShowScrollBar)
		{
			ScrollViewer.SetVerticalScrollBarVisibility(ItemsListViewBuy, ScrollBarVisibility.Visible);
		}
		else
		{
			ScrollViewer.SetVerticalScrollBarVisibility(ItemsListViewBuy, ScrollBarVisibility.Disabled);
		}
	}

	public static List<Blessing> GetPriestOfferAsBlessings()
	{
		var result = new List<Blessing>();
		var listOfPatterns = GameAssets.PriestOffer;

		foreach (var pattern in listOfPatterns)
		{
			result.Add(GameAssets.Blessings.FirstOrDefault(x => x.Id == pattern.VendorItemId));
		}

		return result;
	}

	private void BuyButton_Click(object sender, RoutedEventArgs e)
	{
		var b = sender as Button;
		var blessingBlueprint = b.CommandParameter as Blessing;

		if (User.Instance.Gold >= blessingBlueprint.Value)
		{
			var buyRuns = new List<Run>
			{
				new Run("Are you sure you want to buy "),
				new Run($"{blessingBlueprint.Name}")
				{
					FontFamily = (FontFamily)FindResource("FontRegularDemiBold")
				},
				new Run(" for "),
				new Run($"{blessingBlueprint.Value} gold")
				{
					Foreground = (SolidColorBrush)FindResource("BrushGold"),
					FontFamily = (FontFamily)FindResource("FontRegularDemiBold")
				},
				new Run("?")
			};

			var result = AlertBox.Show(buyRuns);

			if (result == MessageBoxResult.No)
			{
				return;
			}

			var hasBlessingActive = User.Instance.CurrentHero.Blessing != null;

			if (hasBlessingActive)
			{
				var doesUserWantToSwap = Blessing.AskUserAndSwapBlessing(blessingBlueprint.Id);

				if (doesUserWantToSwap == false)
				{
					return;
				}
			}
			else
			{
				Blessing.AddOrReplaceBlessing(blessingBlueprint.Id);
			}

			(Application.Current.MainWindow as GameWindow).CreateFloatingTextUtility($"-{blessingBlueprint.Value}", (SolidColorBrush)FindResource("BrushGold"), FloatingTextHelper.GoldPositionPoint);

			User.Instance.Gold -= blessingBlueprint.Value;

			UpdatePriest();
		}
		else
		{
			var notEnoughGoldRuns = new List<Run>
			{
				new Run("You do not have enough gold to buy this blessing.\nIt costs "),
				new Run($"{blessingBlueprint.Value} gold")
				{
					Foreground = (SolidColorBrush)FindResource("BrushGold"),
					FontFamily = (FontFamily)FindResource("FontRegularDemiBold")
				},
				new Run(".\nYou can get more gold by completing quests and selling loot from monsters and bosses.")
			};

			AlertBox.Show(notEnoughGoldRuns, MessageBoxButton.OK);
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
			toolTipBlock.Inlines.Add(new Run($"{(button.CommandParameter as Blessing).Value} gold")
			{
				FontFamily = (FontFamily)FindResource("FontRegularDemiBold"),
				Foreground = (SolidColorBrush)FindResource("BrushGold")
			});

			toolTip.Content = toolTipBlock;

			button.ToolTip = toolTip;
		}
	}

	private void TownButton_Click(object sender, RoutedEventArgs e)
	{
		InterfaceHelper.ChangePage(GameAssets.Pages["Town"], "Town");
	}
}