using ClickQuest.Account;
using ClickQuest.Data;
using ClickQuest.Items;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
			ItemsListViewCraft.ItemsSource = User.Instance.Recipes;

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

		private void CraftButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var recipe = b.CommandParameter as Recipe;

			// Check if user has required materials to craft.
			foreach (var pair in recipe.MaterialIds)
			{
				var material = User.Instance.Materials.FirstOrDefault(x=>x.Id==pair.Key);
				if (! (material!=null && material.Quantity>=pair.Value))
				{
					// Error - no materials.
					return;
				}
			}
			
			
			// If he has, remove them.
			foreach (var pair in recipe.MaterialIds)
			{
				var material = User.Instance.Materials.FirstOrDefault(x=>x.Id==pair.Key);
				if (material!=null && material.Quantity>=pair.Value)
				{
					for(int i=0; i<pair.Value; i++)
					{
						User.Instance.RemoveItem(material);
					}
				}
			}

			// Add artifact to equipment.
			var artifact = Data.Database.Artifacts.FirstOrDefault(x=>x.Id == recipe.ArtifactId);
			User.Instance.AddItem(artifact);

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