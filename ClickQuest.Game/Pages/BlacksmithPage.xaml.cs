using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.Game.Controls;
using ClickQuest.Game.Data.GameData;
using ClickQuest.Game.Extensions.InterfaceManager;
using ClickQuest.Game.Heroes.Buffs;
using ClickQuest.Game.Interfaces;
using ClickQuest.Game.Items;
using ClickQuest.Game.Player;
using static ClickQuest.Game.Extensions.RandomnessManager.RandomnessController;

namespace ClickQuest.Game.Pages
{
	public partial class BlacksmithPage : Page
	{
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

		private void MeltButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;

			var result = AlertBox.Show($"Are you sure you want to melt {(b.CommandParameter as Item).Name}?\nThis action will destroy this item and create at least X {(b.CommandParameter as Item).Rarity} ingots.\nYou can get bonus ingots if you master Melter specialization (by melting more items).");

			if (result == MessageBoxResult.Cancel)
			{
				return;
			}

			if (b.CommandParameter is Material material)
			{
				MeltItem(material);
			}
			else if (b.CommandParameter is Artifact artifact)
			{
				MeltItem(artifact);
			}

			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
			UpdateBlacksmithItems();

			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Melting]++;
		}

		public void MeltItem<T>(T meltedItem) where T : Item, IMeltable
		{
			meltedItem.RemoveItem();

			int ingotAmount = CalculateIngotAmount(meltedItem.BaseIngotBonus);

			var ingot = User.Instance.Ingots.FirstOrDefault(x => x.Rarity == meltedItem.Rarity);
			ingot.Quantity += ingotAmount;

			ingot.AddAchievementProgress(ingotAmount);
		}

		private int CalculateIngotAmount(int baseIngotBonus)
		{
			int ingotAmount = baseIngotBonus;
			int meltingBuffPercent = User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Melting];
			while (meltingBuffPercent >= 100)
			{
				ingotAmount += baseIngotBonus;
				meltingBuffPercent -= 100;
			}

			if (meltingBuffPercent > 0)
			{
				int randomizedValue = RNG.Next(1, 101);
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

			HandleCrafting<Material>(recipe);
		}

		private void HandleCrafting<T>(Recipe recipe)
		{
			if (!MeetsCraftingRequirement(recipe))
			{
				AlertBox.Show($"You dont meet Craftsmen specialization requirements to craft {recipe.Rarity.ToString()} artifacts.\nCraft more common items in order to master it.", MessageBoxButton.OK);
				return;
			}

			if (typeof(T) == typeof(Material))
			{
				CheckAndRemoveMaterials(recipe);
			}
			else
			{
				CheckAndRemoveIngots(recipe);
			}

			recipe.Artifact.AddItem();
			recipe.RemoveItem();

			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Crafting]++;

			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
			;
			UpdateBlacksmithItems();
		}

		private void CheckAndRemoveIngots(Recipe recipe)
		{
			if (!CheckIfUserHasEnoughIngots(recipe))
			{
				AlertBox.Show($"You dont have {recipe.IngotsRequired} {recipe.RarityString} ingots to craft {recipe.Name}.\nGet more ingots by melting materials/artifacts or try to craft this artifact using materials.", MessageBoxButton.OK);
				return;
			}

			var result = AlertBox.Show($"Are you sure you want to craft {recipe.Name} using ingots?\nIngots needed: {recipe.IngotsRequired} {recipe.RarityString} ingots.\nThis action will destroy all ingots and this recipe.");

			if (result == MessageBoxResult.Cancel)
			{
				return;
			}

			RemoveIngots(recipe);
		}

		private void RemoveIngots(Recipe recipe)
		{
			User.Instance.Ingots.FirstOrDefault(x => x.Rarity == recipe.Rarity).Quantity -= recipe.IngotsRequired;
		}

		private bool CheckIfUserHasEnoughIngots(Recipe recipe)
		{
			var ingotRarityNeeded = User.Instance.Ingots.FirstOrDefault(x => x.Rarity == recipe.Rarity);
			return recipe.IngotsRequired <= ingotRarityNeeded.Quantity;
		}

		private void CheckAndRemoveMaterials(Recipe recipe)
		{
			if (!CheckIfHeroHasEnoughMaterials(recipe))
			{
				AlertBox.Show($"You don't have enough materials to craft {recipe.Name}.\n{recipe.RequirementsDescription}\nGet more materials by completing quests and killing monsters and boses or try to craft this artifact using ingots.", MessageBoxButton.OK);
				return;
			}

			var result = AlertBox.Show($"Are you sure you want to craft {recipe.Name} using materials?\n{recipe.RequirementsDescription}\nThis action will destroy all materials and this recipe.");

			if (result == MessageBoxResult.Cancel)
			{
				return;
			}

			RemoveMaterials(recipe);
		}

		private void RemoveMaterials(Recipe recipe)
		{
			foreach (var ingredient in recipe.Ingredients)
			{
				var material = User.Instance.CurrentHero.Materials.FirstOrDefault(x => x.Id == ingredient.Id);
				if (material != null && material.Quantity >= ingredient.Quantity)
				{
					material.RemoveItem(ingredient.Quantity);
				}
			}
		}

		private bool CheckIfHeroHasEnoughMaterials(Recipe recipe)
		{
			foreach (var ingredient in recipe.Ingredients)
			{
				var material = User.Instance.CurrentHero.Materials.FirstOrDefault(x => x.Id == ingredient.Id);
				if (!(material != null && material.Quantity >= ingredient.Quantity))
				{
					return false;
				}
			}

			return true;
		}

		private bool MeetsCraftingRequirement(Recipe recipe)
		{
			return User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Crafting] >= (int) recipe.Rarity;
		}

		private void CraftIngotButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;
			var recipe = b.CommandParameter as Recipe;

			HandleCrafting<Ingot>(recipe);
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameData.Pages["Town"], "Town");
		}
	}
}