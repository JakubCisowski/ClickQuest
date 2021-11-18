using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.Extensions.UserInterface.ToolTips;
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
				var buyRuns = new List<Run>();
				buyRuns.Add(new Run("Are you sure you want to buy "));
				buyRuns.Add(new Run($"{blessingBlueprint.Name}"){FontFamily = (FontFamily)this.FindResource("FontRegularDemiBold")});
				buyRuns.Add(new Run(" for "));
				buyRuns.Add(new Run($"{blessingBlueprint.Value} gold"){Foreground = (SolidColorBrush)this.FindResource("BrushGold"), FontFamily = (FontFamily)this.FindResource("FontRegularDemiBold")});
				buyRuns.Add(new Run("?"));
				
				var result = AlertBox.Show(buyRuns, MessageBoxButton.YesNo);

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
				var notEnoughGoldRuns = new List<Run>();
				notEnoughGoldRuns.Add(new Run("You do not have enough gold to buy this blessing.\nIt costs "));
				notEnoughGoldRuns.Add(new Run($"{blessingBlueprint.Value} gold"){Foreground = (SolidColorBrush)this.FindResource("BrushGold"), FontFamily = (FontFamily)this.FindResource("FontRegularDemiBold")});
				notEnoughGoldRuns.Add(new Run(".\nYou can get more gold by completing quests and selling loot from monsters and bosses."));
				
				AlertBox.Show(notEnoughGoldRuns, MessageBoxButton.OK);
			}
		}

		private void BuyButton_OnInitialized(object sender, EventArgs e)
		{
			var button = sender as Button;

			if (button?.ToolTip == null)
			{
				var toolTip = new ToolTip()
				{
					Style = (Style)this.FindResource("ToolTipSimple")
				};

				GeneralToolTipController.SetToolTipDelayAndDuration(button);

				var toolTipBlock = new TextBlock()
				{
					Style = (Style)this.FindResource("ToolTipTextBlockBase")
				};

				toolTipBlock.Inlines.Add(new Run("Buy for "));
				toolTipBlock.Inlines.Add(new Run($"{(button.CommandParameter as Blessing).Value} gold"){FontFamily=(FontFamily)this.FindResource("FontRegularDemiBold"),Foreground=(SolidColorBrush)this.FindResource("BrushGold")});

				toolTip.Content = toolTipBlock;

				button.ToolTip = toolTip;
			}
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameAssets.Pages["Town"], "Town");
		}
	}
}