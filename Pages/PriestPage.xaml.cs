using ClickQuest.Player;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Entity;
using ClickQuest.Items;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
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
			ItemsListViewBuy.ItemsSource = Database.PriestOffer;
			ItemsListViewBuy.Items.Refresh();
		}

		#region Events
		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			// Go back to Town.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Town"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Town";
			(Database.Pages["Town"] as TownPage).EquipmentFrame.Refresh();
			(Database.Pages["Town"] as TownPage).StatsFrame.Refresh();
		}

		private void BuyButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var blessingBlueprint = b.CommandParameter as Blessing;

			// Check if user has enough gold.
			if (User.Instance.Gold >= blessingBlueprint.Value)
			{
				// Show buy alert.
				var result = AlertBox.Show($"Are you sure you want to buy {blessingBlueprint.Name} for {blessingBlueprint.Value} gold?\nThis action will remove all your current blessings.", MessageBoxButton.YesNo);

				// If user clicked cancel on buy alert - return.
				if (result == MessageBoxResult.Cancel)
				{
					return;
				}

				// Remove gold, start blessing
				User.Instance.Gold -= blessingBlueprint.Value;

				// Increase Blessing Specialization amount.
				User.Instance.CurrentHero.Specialization.SpecBlessingAmount++;

				// Cancel current blessings.
				foreach (var bless in User.Instance.CurrentHero.Blessings)
				{
					bless.ChangeBuffStatus(false);
					EntityOperations.RemoveBlessing(bless);
				}
				// And remove them.
				User.Instance.CurrentHero.Blessings.Clear();

				// Create a new Blessing.
				var blessing = new Blessing(blessingBlueprint);

				// Increase his duration based on Blessing Specialization buff.
				blessing.Duration += User.Instance.CurrentHero.Specialization.SpecBlessingBuff;

				User.Instance.CurrentHero.Blessings.Add(blessing);
				blessing.ChangeBuffStatus(true);

				UpdatePriest();

				// Update StatsPage.
				this.StatsFrame.Refresh();
			}
			else
			{
				// Display an error.
				AlertBox.Show($"You do not have enough gold to buy this blessing.\nIt costs {blessingBlueprint.Value} gold.\nYou can get more gold by completing quests and selling loot from monsters and bosses.", MessageBoxButton.OK);
			}
		}
		#endregion
	}
}