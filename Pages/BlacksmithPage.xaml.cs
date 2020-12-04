using System.Windows;
using System.Linq;
using System.Windows.Controls;
using ClickQuest.Data;
using ClickQuest.Account;
using ClickQuest.Items;

namespace ClickQuest.Pages
{
    public partial class BlacksmithPage : Page
    {
        public BlacksmithPage()
        {
            InitializeComponent();
        }

        public void UpdateBlacksmith()
        {
            ItemsListViewMelt.ItemsSource = User.Instance.Materials;

			ItemsListViewMelt.Items.Refresh();
        }
        
        #region Events

        private void MeltButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var material = b.CommandParameter as Material;

            // Remove X items (for now only 1)
			Account.User.Instance.RemoveItem(material);

            // Add an ingot of that rarity
            var ingot = Account.User.Instance.Ingots.Where(x => x.Rarity == material.Rarity).FirstOrDefault();
            ingot.Quantity++;

            EquipmentWindow.Instance.UpdateEquipment();
			UpdateBlacksmith();
		}

        private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Town"]);
		}

        #endregion
    }
}