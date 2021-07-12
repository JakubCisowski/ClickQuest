using ClickQuest.Player;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Items;
using ClickQuest.Windows;
using ClickQuest.Heroes;
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

			// Show melt alert
			var result = AlertBox.Show($"Are you sure you want to melt {(b.CommandParameter as Item).Name}?\nThis action will destroy this item and create at least X {(b.CommandParameter as Item).Rarity} ingots.\nYou can get bonus ingots if you master Melter specialization (by melting more items).", MessageBoxButton.YesNo);

			// If user clicked cancel on melt alert - return.
			if (result == MessageBoxResult.Cancel)
			{
				return;
			}

			// If user clicked ok - melt item.
			if (b.CommandParameter is Material material)
			{
				// A material is being melted.

				// Remove the material from inventory.
				User.Instance.CurrentHero.RemoveItem(material);

				// Add ingots of that rarity.
				var ingot = User.Instance.Ingots.Where(x => x.Rarity == material.Rarity).FirstOrDefault();
				// Calculate ingot bonus based on Melting Specialization.
				var ingotAmount = 1;

				var meltingBuff = User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Melting];
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

				// Increase achievement amount.
				switch(material.Rarity)
				{
					case Rarity.General:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.GeneralIngotsEarned] += ingotAmount;
						break;
					case Rarity.Fine:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.FineIngotsEarned] += ingotAmount; 
						break;
					case Rarity.Superior:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.SuperiorIngotsEarned] += ingotAmount;
						break;
					case Rarity.Exceptional:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.ExceptionalIngotsEarned] += ingotAmount;
						break;
					case Rarity.Mythic:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.MythicIngotsEarned] += ingotAmount;
						break;
					case Rarity.Masterwork:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.MasterworkIngotsEarned] += ingotAmount;
						break;
				}
				AchievementsWindow.Instance.UpdateAchievements();

				// Update blacksmith page.
				(GameData.Pages["Blacksmith"] as BlacksmithPage).EquipmentFrame.Refresh();
				
				// Refresh stats frame (for specialization update).
				(GameData.Pages["Blacksmith"] as BlacksmithPage).StatsFrame.Refresh();
				UpdateBlacksmith();

				// Increase Specialization Melting amount.
				User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Melting]++;
			}
			else if (b.CommandParameter is Artifact artifact)
			{
				// An artifact is being melted.

				// Remove the artifact from inventory.
				User.Instance.CurrentHero.RemoveItem(artifact);

				// Add ingots of that rarity.
				var ingot = User.Instance.Ingots.Where(x => x.Rarity == artifact.Rarity).FirstOrDefault();
				// Calculate ingot bonus based on Melting Specialization.
				var ingotAmount = 100;

				var meltingBuff = User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Melting];
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

				// Increase achievement amount.
				switch(artifact.Rarity)
				{
					case Rarity.General:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.GeneralIngotsEarned] += ingotAmount;
						break;
					case Rarity.Fine:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.FineIngotsEarned] += ingotAmount; 
						break;
					case Rarity.Superior:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.SuperiorIngotsEarned] += ingotAmount;
						break;
					case Rarity.Exceptional:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.ExceptionalIngotsEarned] += ingotAmount;
						break;
					case Rarity.Mythic:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.MythicIngotsEarned] += ingotAmount;
						break;
					case Rarity.Masterwork:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.MasterworkIngotsEarned] += ingotAmount;
						break;
				}
				AchievementsWindow.Instance.UpdateAchievements();

				// Update blacksmith page.
				(GameData.Pages["Blacksmith"] as BlacksmithPage).EquipmentFrame.Refresh();
				UpdateBlacksmith();

				// Increase Specialization Melting amount.
				User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Melting]++;
			}
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
						User.Instance.CurrentHero.RemoveItem(material);
					}
				}
			}

			// Add artifact to equipment.
			var artifact = recipe.Artifact;
			User.Instance.CurrentHero.AddItem(artifact);

			// Remove the recipe used.
			User.Instance.CurrentHero.RemoveItem(recipe);

			(GameData.Pages["Blacksmith"] as BlacksmithPage).EquipmentFrame.Refresh();
			// Refresh stats frame (for specialization update).
			(GameData.Pages["Blacksmith"] as BlacksmithPage).StatsFrame.Refresh();
			UpdateBlacksmith();

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
				User.Instance.CurrentHero.AddItem(artifact);

				// Remove the recipe used.
				User.Instance.CurrentHero.RemoveItem(recipe);

				(GameData.Pages["Blacksmith"] as BlacksmithPage).EquipmentFrame.Refresh();
				UpdateBlacksmith();

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
			(GameData.Pages["Town"] as TownPage).EquipmentFrame.Refresh();
			(GameData.Pages["Town"] as TownPage).StatsFrame.Refresh();
		}

		#endregion Events
	}
}