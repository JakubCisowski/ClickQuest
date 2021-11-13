using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Interfaces;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Collections;
using ClickQuest.Game.Extensions.Gameplay;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.Extensions.UserInterface.ToolTips;
using ClickQuest.Game.UserInterface.Controls;
using static ClickQuest.Game.Extensions.Randomness.RandomnessController;

namespace ClickQuest.Game.UserInterface.Pages
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
			ItemsListViewMeltMaterials.ItemsSource = User.Instance.CurrentHero.Materials.ReorderItemsInList();
			ItemsListViewMeltArtifacts.ItemsSource = User.Instance.CurrentHero.Artifacts.ReorderItemsInList();
			ItemsListViewCraft.ItemsSource = User.Instance.CurrentHero.Recipes.ReorderItemsInList();

			ItemsListViewMeltMaterials.Items.Refresh();
			ItemsListViewMeltArtifacts.Items.Refresh();
			ItemsListViewCraft.Items.Refresh();
		}

		private void MeltButton_Click(object sender, RoutedEventArgs e)
		{
			var b = sender as Button;

			if (b.CommandParameter is Material material)
			{
				if (material.Rarity == Rarity.Mythic)
				{
					var result = AlertBox.Show($"Are you sure you want to melt {material.Name}?\nThis action will destroy this item and create at least {Material.BaseMeltingIngotBonus} {material.Rarity} ingots.\nYou can get bonus ingots if you master Melter specialization (by melting more materials).", MessageBoxButton.YesNo);

					if (result == MessageBoxResult.No)
					{
						return;
					}
				}
				
				MeltMaterial(material);

				GameController.UpdateSpecializationAmountAndUI(SpecializationType.Melting);
			}
			else if (b.CommandParameter is Artifact artifact)
			{
				string ingotAmountsDescription = "";
				var ingotAmounts = CalculateIngotAmountsWhenMeltingArtifact(artifact);

				for (int i=0;i<6;i++)
				{
					if (ingotAmounts[i] == 0)
					{
						continue;
					}
					
					ingotAmountsDescription += $"\n{ingotAmounts[i]}x {(Rarity)i} Ingots";
				}

				var result = AlertBox.Show($"Are you sure you want to melt {artifact.Name}?\nThis action will destroy this item and create: {ingotAmountsDescription}", MessageBoxButton.YesNo);

				if (result == MessageBoxResult.No)
				{
					return;
				}

				MeltArtifact(artifact);
			}

			UpdateBlacksmithItems();
		}

		public void MeltMaterial(Material meltedMaterial)
		{
			meltedMaterial.RemoveItem();

			int ingotAmount = CalculateIngotAmounstWhenMeltingMaterial(Material.BaseMeltingIngotBonus);

			var ingot = User.Instance.Ingots.FirstOrDefault(x => x.Rarity == meltedMaterial.Rarity);
			ingot.AddItem(ingotAmount);
		}

		public void MeltArtifact(Artifact meltedArtifact)
		{
			meltedArtifact.RemoveItem();

			var ingotAmounts = CalculateIngotAmountsWhenMeltingArtifact(meltedArtifact);

			for (int i=0;i<6;i++)
			{
				var ingot = User.Instance.Ingots.FirstOrDefault(x => x.Rarity == (Rarity)i);
				ingot.AddItem(ingotAmounts[i]);
			}
		}

		private int CalculateIngotAmounstWhenMeltingMaterial(int baseIngotBonus)
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

		private List<int> CalculateIngotAmountsWhenMeltingArtifact(Artifact meltedArtifact)
		{
			var ingotAmounts = new List<int>();

			var artifactRecipe = GameAssets.Recipes.FirstOrDefault(x=>x.ArtifactId==meltedArtifact.Id);

			if (artifactRecipe is not null)
			{
				for (int i = 0; i < 6; i++)
				{
					var materialsOfRarity = artifactRecipe.IngredientPatterns.Where(x=>x.RelatedMaterial.Rarity == (Rarity)i);
					var totalMaterialQuantity = materialsOfRarity.Sum(x => x.Quantity);
					ingotAmounts.Add((int)(totalMaterialQuantity * Material.BaseMeltingIngotBonus * Artifact.MeltingIngredientsRatio));
				}
			}
			else
			{
				for (int i = 0; i < 6; i++)
				{
					if ((Rarity)i == meltedArtifact.Rarity)
					{
						ingotAmounts.Add(Artifact.MeltingWithoutIngredientsValue);
					}
					else
					{
						ingotAmounts.Add(0);
					}
				}
			}

			return ingotAmounts;
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

			bool enoughIngredients = false;

			if (typeof(T) == typeof(Material))
			{
				enoughIngredients = CheckAndRemoveMaterials(recipe);
			}
			else
			{
				enoughIngredients = CheckAndRemoveIngots(recipe);
			}

			if (enoughIngredients)
			{
				recipe.Artifact.CreateMythicTag();

				recipe.Artifact.AddItem();
				recipe.RemoveItem();

				GameController.UpdateSpecializationAmountAndUI(SpecializationType.Crafting);
				
				UpdateBlacksmithItems();
			}
		}

		private bool CheckAndRemoveIngots(Recipe recipe)
		{
			string ingotAmountsDescription = "";
			var ingotAmounts = CalculateIngotAmountsWhenCraftingArtifact(recipe.Artifact);

			for (int i=0;i<6;i++)
			{
				if (ingotAmounts[i] == 0)
				{
					continue;
				}
				
				ingotAmountsDescription += $"- {ingotAmounts[i]}x {(Rarity)i} Ingots \n";
			}


			if (!CheckIfUserHasEnoughIngots(recipe))
			{
				AlertBox.Show($"You dont have enough ingots to craft {recipe.Name}.\nIngots required:\n{ingotAmountsDescription}\n\nGet more ingots by melting materials/artifacts or try to craft this artifact using materials.", MessageBoxButton.OK);
				return false;
			}

			var result = AlertBox.Show($"Are you sure you want to craft {recipe.Name} using ingots?\nIngots needed:\n{ingotAmountsDescription}\n\nThis action will destroy all ingots and this recipe.", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.No)
			{
				return false;
			}

			RemoveIngotsWhenCrafting(recipe);
			return true;
		}

		private void RemoveIngotsWhenCrafting(Recipe recipe)
		{
			var ingotsAmount = CalculateIngotAmountsWhenCraftingArtifact(recipe.Artifact);

			for (int i=0;i<6;i++)
			{
				User.Instance.Ingots.FirstOrDefault(x => x.Rarity == (Rarity)i).Quantity -= ingotsAmount[i];
			}
		}

		private bool CheckIfUserHasEnoughIngots(Recipe recipe)
		{
			var ingotsAmount = CalculateIngotAmountsWhenCraftingArtifact(recipe.Artifact);

			for (int i=0;i<6;i++)
			{
				bool userHasEnoughIngots = User.Instance.Ingots.FirstOrDefault(x => x.Rarity == (Rarity)i).Quantity >= ingotsAmount[i];

				if(!userHasEnoughIngots)
				{
					return false;
				}
			}

			return true;
		}

		private List<int> CalculateIngotAmountsWhenCraftingArtifact(Artifact craftedArtifact)
		{
			var ingotAmounts = new List<int>();

			var artifactRecipe = GameAssets.Recipes.FirstOrDefault(x=>x.ArtifactId==craftedArtifact.Id);

			for (int i = 0; i < 6; i++)
			{
				var materialsOfRarity = artifactRecipe.IngredientPatterns.Where(x=>x.RelatedMaterial.Rarity == (Rarity)i);
				var totalMaterialQuantity = materialsOfRarity.Sum(x => x.Quantity);
				ingotAmounts.Add((int)(totalMaterialQuantity * Material.BaseMeltingIngotBonus * Artifact.CraftingRatio));
			}

			return ingotAmounts;
		}

		private bool CheckAndRemoveMaterials(Recipe recipe)
		{
			if (!CheckIfHeroHasEnoughMaterials(recipe))
			{
				AlertBox.Show($"You don't have enough materials to craft {recipe.Name}.\n{recipe.RequirementsDescription}\nGet more materials by completing quests and killing monsters and boses or try to craft this artifact using ingots.", MessageBoxButton.OK);
				return false;
			}

			var result = AlertBox.Show($"Are you sure you want to craft {recipe.Name} using materials?\n{recipe.RequirementsDescription}\nThis action will destroy all materials and this recipe.", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.No)
			{
				return false;
			}

			RemoveMaterials(recipe);
			return true;
		}

		private void RemoveMaterials(Recipe recipe)
		{
			foreach (var ingredient in recipe.IngredientPatterns)
			{
				var material = User.Instance.CurrentHero.Materials.FirstOrDefault(x => x.Id == ingredient.MaterialId);
				if (material != null && material.Quantity >= ingredient.Quantity)
				{
					material.RemoveItem(ingredient.Quantity);
				}
			}
		}

		private bool CheckIfHeroHasEnoughMaterials(Recipe recipe)
		{
			foreach (var ingredient in recipe.IngredientPatterns)
			{
				var material = User.Instance.CurrentHero.Materials.FirstOrDefault(x => x.Id == ingredient.MaterialId);
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

		private void MeltButton_OnInitialized(object sender, EventArgs e)
		{
			var button = sender as Button;

			if (button?.ToolTip == null)
			{
				var toolTipBlock = new TextBlock()
				{
					Style = (Style)this.FindResource("ToolTipTextBlockBase")
				};

				if (button.CommandParameter is Material material)
				{
					toolTipBlock.Inlines.Add(new Run("Melt for at least"));
					toolTipBlock.Inlines.Add(new LineBreak());

					toolTipBlock.Inlines.Add(new Run($"{Material.BaseMeltingIngotBonus}x {material.RarityString} Ingots") {FontFamily = (FontFamily)this.FindResource("FontRegularDemiBold"), Foreground = ColorsController.GetRarityColor(material.Rarity)});
					toolTipBlock.Inlines.Add(new LineBreak());

					toolTipBlock.Inlines.Add(new Run("Chance for more through Melter specialization"));
				}
				else
				{
					var ingotAmounts = CalculateIngotAmountsWhenMeltingArtifact(button.CommandParameter as Artifact);

					toolTipBlock.Inlines.Add(new Run("Melt for:"));

					for (int i=0;i<6;i++)
					{
						if (ingotAmounts[i] == 0)
						{
							continue;
						}
						
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"{ingotAmounts[i]}x {(Rarity)i} Ingots"){FontFamily = (FontFamily)this.FindResource("FontRegularDemiBold"), Foreground = ColorsController.GetRarityColor((Rarity)i)});
					}
				}

				var toolTip = new ToolTip()
				{
					Style = (Style)this.FindResource("ToolTipSimple")
				};

				GeneralToolTipController.SetToolTipDelayAndDuration(button);

				toolTip.Content = toolTipBlock;

				button.ToolTip = toolTip;
			}
		}

		private void CraftMaterialButton_OnInitialized(object sender, EventArgs e)
		{
			var button = sender as Button;

			if (button?.ToolTip == null)
			{
				var toolTipBlock = new TextBlock()
				{
					Style = (Style)this.FindResource("ToolTipTextBlockBase")
				};

				toolTipBlock.Inlines.Add(new Run("Craft with:"));
				toolTipBlock.Inlines.Add(new LineBreak());

				var listOfRuns = ItemToolTipController.GenerateRecipeIngredientsRuns(button.CommandParameter as Recipe);

				foreach (var run in listOfRuns)
				{
					run.FontFamily = (FontFamily)this.FindResource("FontRegularDemiBold");
					toolTipBlock.Inlines.Add(run);
				}

				var toolTip = new ToolTip()
				{
					Style = (Style)this.FindResource("ToolTipSimple")
				};

				GeneralToolTipController.SetToolTipDelayAndDuration(button);

				toolTip.Content = toolTipBlock;

				button.ToolTip = toolTip;
			}
		}

		private void CraftIngotButton_OnInitialized(object sender, EventArgs e)
		{
			var button = sender as Button;

			if (button?.ToolTip == null)
			{
				var toolTipBlock = new TextBlock()
				{
					Style = (Style)this.FindResource("ToolTipTextBlockBase")
				};

				toolTipBlock.Inlines.Add(new Run("Craft with:"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run("Ingots:"){FontFamily = (FontFamily)this.FindResource("FontRegularDemiBold"), FontSize=(double)this.FindResource("FontSizeToolTipIngredientText")});

				var ingotAmounts = CalculateIngotAmountsWhenCraftingArtifact((button.CommandParameter as Recipe).Artifact);

				for (int i=0;i<6;i++)
				{
					if (ingotAmounts[i] == 0)
					{
						continue;
					}

					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"{ingotAmounts[i]}x {(Rarity)i} Ingots"){FontFamily = (FontFamily)this.FindResource("FontRegularDemiBold"), Foreground = ColorsController.GetRarityColor((Rarity)i)});
				}

				var toolTip = new ToolTip()
				{
					Style = (Style)this.FindResource("ToolTipSimple")
				};

				GeneralToolTipController.SetToolTipDelayAndDuration(button);

				toolTip.Content = toolTipBlock;

				button.ToolTip = toolTip;
			}
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameAssets.Pages["Town"], "Town");
		}
	}
}