using ClickQuest.Player;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Items;
using ClickQuest.Windows;
using ClickQuest.Heroes;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Heroes.Buffs;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.Interfaces;

namespace ClickQuest.Pages
{
	public partial class BlacksmithPage : Page
	{
		private Random _rng = new Random();

		public BlacksmithPage()
		{
			InitializeComponent();
		}

		public void UpdateBlacksmithItems()
		{
			// Refresh list of items.
			ItemsListViewMeltMaterials.ItemsSource = User.Instance.CurrentHero.Materials;
			ItemsListViewMeltArtifacts.ItemsSource = User.Instance.CurrentHero.Artifacts;
			ItemsListViewCraft.ItemsSource = User.Instance.CurrentHero.Recipes;

			ItemsListViewMeltMaterials.Items.Refresh();
			ItemsListViewMeltArtifacts.Items.Refresh();
			ItemsListViewCraft.Items.Refresh();
		}

		#region Events

		private void MeltButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;

			var result = AlertBox.Show($"Are you sure you want to melt {(b.CommandParameter as Item).Name}?\nThis action will destroy this item and create at least X {(b.CommandParameter as Item).Rarity} ingots.\nYou can get bonus ingots if you master Melter specialization (by melting more items).", MessageBoxButton.YesNo);

			// If user clicked cancel on melt alert - return.
			if (result == MessageBoxResult.Cancel)
			{
				return;
			}

			if (b.CommandParameter is Material material)
			{
				MeltItem<Material>(material);
			}
			else if (b.CommandParameter is Artifact artifact)
			{
				MeltItem<Artifact>(artifact);
			}

			InterfaceController.RefreshStatsAndEquipmentPanelsOnPage(GameData.Pages["Blacksmith"]);
			UpdateBlacksmithItems();

			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Melting]++;
		}

		public void MeltItem<T>(T meltedItem) where T:Item, IMeltable
		{
			meltedItem.RemoveItem();

			var ingotAmount = CalculateIngotAmount(meltedItem.BaseIngotBonus);

			var ingot = User.Instance.Ingots.FirstOrDefault(x => x.Rarity == meltedItem.Rarity);
			ingot.Quantity += ingotAmount;

			ingot.AddAchievementProgress(ingotAmount);
		}

		private int CalculateIngotAmount(int baseIngotBonus)
		{
			var ingotAmount = baseIngotBonus;
			var meltingBuffPercent = User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Melting];
			while (meltingBuffPercent >= 100)
			{
				ingotAmount += baseIngotBonus;
				meltingBuffPercent -= 100;
			}
			if (meltingBuffPercent > 0)
			{
				int randomizedValue = _rng.Next(1, 101);
				if (randomizedValue <= meltingBuffPercent)
				{
					ingotAmount += baseIngotBonus;
				}
			}

			return ingotAmount;
		}

		private void CraftMaterialButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var recipe = b.CommandParameter as Recipe;

			// Check if user meets Crafting Specialization rarity requirements.
			if (User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Crafting] < (int)recipe.Rarity)
			{
				// Error - user doesn't meet requirements - stop this function.
				AlertBox.Show($"You dont meet Craftsmen specialization requirements to craft {(int)recipe.Rarity} artifacts.\nCraft more common items in order to master it.", MessageBoxButton.OK);
				return;
			}

			// Check if user has required materials to craft.
			foreach (var pair in recipe.MaterialIds)
			{
				var material = User.Instance.CurrentHero.Materials.FirstOrDefault(x => x.Id == pair.Key);
				if (!(material != null && material.Quantity >= pair.Value))
				{
					AlertBox.Show($"You don't have enough materials to craft {recipe.Name}.\n{recipe.RequirementsDescription}\nGet more materials by completing quests and killing monsters and boses or try to craft this artifact using ingots.", MessageBoxButton.OK);
					return;
				}
			}

			// Show craft alert.
			var result = AlertBox.Show($"Are you sure you want to craft {recipe.Name} using materials?\n{recipe.RequirementsDescription}\nThis action will destroy all materials and this recipe.", MessageBoxButton.YesNo);

			// If user clicked cancel on craft alert - return.
			if (result == MessageBoxResult.Cancel)
			{
				return;
			}

			// If he has, remove them.
			foreach (var pair in recipe.MaterialIds)
			{
				var material = User.Instance.CurrentHero.Materials.FirstOrDefault(x => x.Id == pair.Key);
				if (material != null && material.Quantity >= pair.Value)
				{
					for (int i = 0; i < pair.Value; i++)
					{
						material.RemoveItem();
					}
				}
			}

			// Add artifact to equipment.
			var artifact = recipe.Artifact;
			artifact.AddItem();

			// Remove the recipe used.
			recipe.RemoveItem();

			(GameData.Pages["Blacksmith"] as BlacksmithPage).EquipmentFrame.Refresh();
			// Refresh stats frame (for specialization update).
			(GameData.Pages["Blacksmith"] as BlacksmithPage).StatsFrame.Refresh();
			UpdateBlacksmithItems();

			// Increase Specialization Crafting amount.
			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Crafting]++;
		}

		private void CraftIngotButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var recipe = b.CommandParameter as Recipe;

			// Check if user meets Crafting Specialization rarity requirements.
			if (User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Crafting] < (int)recipe.Rarity)
			{
				// Error - user doesn't meet requirements - stop this function.
				AlertBox.Show($"You dont meet Craftsmen specialization requirements to craft {(int)recipe.Rarity} artifacts.\nCraft more common items in order to master it.", MessageBoxButton.OK);
				return;
			}

			// Check if user has required ingots to craft.
			var ingotRarityNeeded = User.Instance.Ingots.FirstOrDefault(x => x.Rarity == recipe.Rarity);
			if (recipe.IngotsRequired <= ingotRarityNeeded.Quantity)
			{
				// Show craft alert.
				var result = AlertBox.Show($"Are you sure you want to craft {recipe.Name} using ingots?\nIngots needed: {recipe.IngotsRequired} {recipe.RarityString} ingots.\nThis action will destroy all ingots and this recipe.", MessageBoxButton.YesNo);

				// If user clicked cancel on craft alert - return.
				if (result == MessageBoxResult.Cancel)
				{
					return;
				}

				// If he has, remove them.
				ingotRarityNeeded.Quantity -= recipe.IngotsRequired;

				// Add artifact to equipment.
				var artifact = recipe.Artifact;
				artifact.AddItem();

				// Remove the recipe used.
				recipe.RemoveItem();

				(GameData.Pages["Blacksmith"] as BlacksmithPage).EquipmentFrame.Refresh();
				UpdateBlacksmithItems();

				// Increase Specialization Crafting amount.
				User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Crafting]++;
			}
			else
			{
				// Alert - User does not have enough Ingots.
				AlertBox.Show($"You dont have {recipe.IngotsRequired} {recipe.RarityString}ingots to craft {recipe.Name}.\nGet more ingots by melting materials/artifacts or try to craft this artifact using materials.", MessageBoxButton.OK);
			}
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			// Go back to Town.
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(GameData.Pages["Town"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Town";
			InterfaceController.RefreshStatsAndEquipmentPanelsOnPage(GameData.Pages["Town"]);
		}

		#endregion Events
	}
}