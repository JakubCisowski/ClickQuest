using System.Windows;
using System.Windows.Controls;
using ClickQuest.Account;
using ClickQuest.Data;
using ClickQuest.Items;
using System.Linq;

namespace ClickQuest.Pages
{
    public partial class ShopPage : Page
    {
        public ShopPage()
        {
            InitializeComponent();
            UpdateShop();
        }

        private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Town"]);
        }

        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            var b=sender as Button;
            var material = b.CommandParameter as Material;
            material.Quantity--;

            EquipmentWindow.Instance.UpdateEquipment();
            UpdateShop();
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
        }

        public void UpdateShop()
        {
            ItemsListViewSell.ItemsSource = User.Instance.Items.Where(x=>x is Material).ToList();
            ItemsListViewBuy.ItemsSource = Database.Recipes;

            ItemsListViewSell.Items.Refresh();
            ItemsListViewBuy.Items.Refresh();
        }
    }
}