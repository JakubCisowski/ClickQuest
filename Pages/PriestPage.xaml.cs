using ClickQuest.Account;
using ClickQuest.Data;
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
		}

		private void BuyButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var blessingBlueprint = b.CommandParameter as Blessing;

			// Remove gold, start blessing
			if (User.Instance.Gold >= blessingBlueprint.Value)
			{
				User.Instance.Gold -= blessingBlueprint.Value;

				// Increase Blessing Specialization amount.
				Account.User.Instance.Specialization.SpecBlessingAmount++;

				// Create a new Blessing.
				var blessing = new Blessing(blessingBlueprint);
				// Increase his duration based on Blessing Specialization buff.
				blessing.Duration += Account.User.Instance.Specialization.SpecBlessingBuff;
				User.Instance.Blessings.Add(blessing);
				blessing.ChangeBuffStatus(true);
			}

			UpdatePriest();
		}
		#endregion
	}
}