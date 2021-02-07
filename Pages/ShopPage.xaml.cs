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
            ItemsListViewSell.ItemsSource = User.Instance.Materials.Cast<Item>().Concat(User.Instance.Recipes.Cast<Item>());

            // Calculate shop offer size according to specialization bonus (base bonus: 5).
            ItemsListViewBuy.ItemsSource = Database.ShopOffer.Take(Account.User.Instance.Specialization.SpecBuyingBuff);

            ItemsListViewSell.Items.Refresh();
            ItemsListViewBuy.Items.Refresh();
        }

        #region Events

        private void TownButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Town"]);
            (Window.GetWindow(this) as GameWindow).LocationInfo = "Town";
        }

        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var item = b.CommandParameter as Item;

            Account.User.Instance.RemoveItem(item);
            Account.User.Instance.Gold += item.Value;

            EquipmentWindow.Instance.UpdateEquipment();
            UpdateShop();
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var recipe = b.CommandParameter as Recipe;

            // Check if user has enough gold.
            if (Account.User.Instance.Gold >= recipe.Value)
            {
                Account.User.Instance.AddItem(recipe);
                Account.User.Instance.Gold -= recipe.Value;

                EquipmentWindow.Instance.UpdateEquipment();
                UpdateShop();

                // Increase Specialization Buying amount.
                Account.User.Instance.Specialization.SpecBuyingAmount++;
            }
            else
            {
                // Error
            }
        }

        #endregion
    }
}