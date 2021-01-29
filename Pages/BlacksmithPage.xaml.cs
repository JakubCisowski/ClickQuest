using ClickQuest.Account;
using ClickQuest.Data;
using ClickQuest.Items;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class BlacksmithPage : Page
	{
		private Random _rng = new Random();

		public BlacksmithPage()
		{
			InitializeComponent();
		}

		public void UpdateBlacksmith()
		{
			// Refresh list of items.
			ItemsListViewMeltMaterials.ItemsSource = User.Instance.Materials;
			ItemsListViewMeltArtifacts.ItemsSource = User.Instance.Artifacts;
			ItemsListViewCraft.ItemsSource = User.Instance.Recipes;

			ItemsListViewMeltMaterials.Items.Refresh();
			ItemsListViewMeltArtifacts.Items.Refresh();
			ItemsListViewCraft.Items.Refresh();
		}

		#region Events

		private void MeltButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;

			if (b.CommandParameter is Material material)
			{
				// A material is being melted.

				// Remove the material from inventory.
				Account.User.Instance.RemoveItem(material);

				// Add ingots of that rarity.
				var ingot = Account.User.Instance.Ingots.Where(x => x.Rarity == material.Rarity).FirstOrDefault();
				// Calculate ingot bonus based on Melting Specialization.
				var ingotAmount = 1;

				var meltingBuff = Account.User.Instance.Specialization.SpecMeltingBuff;
				while (meltingBuff >= 100)
				{
					ingotAmount++;
					meltingBuff -= 100;
				}
				if (meltingBuff > 0)
				{
					int num = _rng.Next(1, 101);
					if (num <= meltingBuff)
					{
						ingotAmount++;
					}
				}
				ingot.Quantity += ingotAmount;

				// Update both equipment and blacksmith page.
				EquipmentWindow.Instance.UpdateEquipment();
				UpdateBlacksmith();

				// Increase Specialization Melting amount.
				Account.User.Instance.Specialization.SpecMeltingAmount++;
			}
			else if (b.CommandParameter is Artifact artifact)
			{
				// An artifact is being melted.

				// Remove the artifact from inventory.
				Account.User.Instance.RemoveItem(artifact);

				// Add ingots of that rarity.
				var ingot = Account.User.Instance.Ingots.Where(x => x.Rarity == artifact.Rarity).FirstOrDefault();
				// Calculate ingot bonus based on Melting Specialization.
				var ingotAmount = 100;

				var meltingBuff = Account.User.Instance.Specialization.SpecMeltingBuff;
				while (meltingBuff >= 100)
				{
					ingotAmount += 100;
					meltingBuff -= 100;
				}
				if (meltingBuff > 0)
				{
					int num = _rng.Next(1, 101);
					if (num <= meltingBuff)
					{
						ingotAmount += 100;
					}
				}
				ingot.Quantity += ingotAmount;

				// Update both equipment and blacksmith page.
				EquipmentWindow.Instance.UpdateEquipment();
				UpdateBlacksmith();

				// Increase Specialization Melting amount.
				Account.User.Instance.Specialization.SpecMeltingAmount++;
			}
		}

		private void CraftMaterialButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var recipe = b.CommandParameter as Recipe;

			// Check if user meets Crafting Specialization rarity requirements.
			if (Account.User.Instance.Specialization.SpecCraftingBuff < (int)recipe.Rarity)
			{
				// Error - user doesn't meet requirements - stop this function.
				return;
			}

			// Check if user has required materials to craft.
			foreach (var pair in recipe.MaterialIds)
			{
				var material = User.Instance.Materials.FirstOrDefault(x => x.Id == pair.Key);
				if (!(material != null && material.Quantity >= pair.Value))
				{
					// Error - no materials - stop this function.
					return;
				}
			}

			// If he has, remove them.
			foreach (var pair in recipe.MaterialIds)
			{
				var material = User.Instance.Materials.FirstOrDefault(x => x.Id == pair.Key);
				if (material != null && material.Quantity >= pair.Value)
				{
					for (int i = 0; i < pair.Value; i++)
					{
						User.Instance.RemoveItem(material);
					}
				}
			}

			// Add artifact to equipment.
			var artifact = Data.Database.Artifacts.FirstOrDefault(x => x.Id == recipe.ArtifactId);
			User.Instance.AddItem(artifact);

			// Remove the recipe used.
			User.Instance.RemoveItem(recipe);

			EquipmentWindow.Instance.UpdateEquipment();
			UpdateBlacksmith();

			// Increase Specialization Crafting amount.
			Account.User.Instance.Specialization.SpecCraftingAmount++;
		}

		private void CraftIngotButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var recipe = b.CommandParameter as Recipe;

			// Check if user meets Crafting Specialization rarity requirements.
			if (Account.User.Instance.Specialization.SpecCraftingBuff < (int)recipe.Rarity)
			{
				// Error - user doesn't meet requirements - stop this function.
				return;
			}

			// Check if user has required ingots to craft.
			var ingotRarityNeeded = User.Instance.Ingots.FirstOrDefault(x => x.Rarity == recipe.Rarity);
			if (recipe.IngotsRequired <= ingotRarityNeeded.Quantity)
			{
				// If he has, remove them.
				ingotRarityNeeded.Quantity -= recipe.IngotsRequired;

				// Add artifact to equipment.
				var artifact = Data.Database.Artifacts.FirstOrDefault(x => x.Id == recipe.ArtifactId);
				User.Instance.AddItem(artifact);

				// Remove the recipe used.
				User.Instance.RemoveItem(recipe);

				EquipmentWindow.Instance.UpdateEquipment();
				UpdateBlacksmith();

				// Increase Specialization Crafting amount.
				Account.User.Instance.Specialization.SpecCraftingAmount++;
			}
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["Town"]);
		}

		#endregion Events
	}
}