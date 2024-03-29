using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Helpers;
using ClickQuest.Game.UserInterface.Helpers.ToolTips;
using static ClickQuest.Game.Helpers.RandomnessHelper;

namespace ClickQuest.Game.UserInterface.Pages;

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

		// Refresh Scroll Bar visibilities based on the amount of items.
		RefreshScrollBarVisibilities();
	}

	public void RefreshScrollBarVisibilities()
	{
		if (ItemsListViewMeltMaterials.Items.Count > InterfaceHelper.VendorItemsNeededToShowScrollBar)
		{
			ScrollViewer.SetVerticalScrollBarVisibility(ItemsListViewMeltMaterials, ScrollBarVisibility.Visible);
		}
		else
		{
			ScrollViewer.SetVerticalScrollBarVisibility(ItemsListViewMeltMaterials, ScrollBarVisibility.Disabled);
		}

		if (ItemsListViewMeltArtifacts.Items.Count > InterfaceHelper.VendorItemsNeededToShowScrollBar)
		{
			ScrollViewer.SetVerticalScrollBarVisibility(ItemsListViewMeltArtifacts, ScrollBarVisibility.Visible);
		}
		else
		{
			ScrollViewer.SetVerticalScrollBarVisibility(ItemsListViewMeltArtifacts, ScrollBarVisibility.Disabled);
		}

		if (ItemsListViewCraft.Items.Count > InterfaceHelper.VendorItemsNeededToShowScrollBar)
		{
			ScrollViewer.SetVerticalScrollBarVisibility(ItemsListViewCraft, ScrollBarVisibility.Visible);
		}
		else
		{
			ScrollViewer.SetVerticalScrollBarVisibility(ItemsListViewCraft, ScrollBarVisibility.Disabled);
		}
	}

	private void MeltButton_Click(object sender, RoutedEventArgs e)
	{
		var b = sender as Button;

		if (b?.CommandParameter is Material material)
		{
			if (material.Rarity == Rarity.Mythic)
			{
				var listOfInlines = new List<Inline>
				{
					new Run("Are you sure you want to melt "),
					new Run($"{material.Name}")
					{
						FontFamily = (FontFamily)FindResource("FontRegularDemiBold")
					},
					new Run("?\nThis action will destroy this item and create at least "),
					new Run($"{Material.BaseMeltingIngotBonus} {material.Rarity} Ingots")
					{
						Foreground = ColorsHelper.GetRarityColor(material.Rarity),
						FontFamily = (FontFamily)FindResource("FontRegularDemiBold")
					},
					new Run(".\nYou can get bonus ingots if you master Melter specialization (by melting more materials).")
				};

				var result = AlertBox.Show(listOfInlines);

				if (result == MessageBoxResult.No)
				{
					return;
				}
			}

			MeltMaterial(material);

			Specializations.UpdateSpecializationAmountAndUi(SpecializationType.Melting);
		}
		else if (b.CommandParameter is Artifact artifact)
		{
			var listOfInlines = new List<Inline>
			{
				new Run("Are you sure you want to melt "),
				new Run($"{artifact.Name}")
				{
					FontFamily = (FontFamily)FindResource("FontRegularDemiBold")
				},
				new Run("?\nThis action will destroy this item and create:")
			};

			var ingotAmounts = CalculateIngotAmountsWhenMeltingArtifact(artifact);

			for (var i = 0; i < 6; i++)
			{
				if (ingotAmounts[i] == 0)
				{
					continue;
				}

				listOfInlines.Add(new Run($"\n{ingotAmounts[i]}x {(Rarity)i} Ingots")
				{
					Foreground = ColorsHelper.GetRarityColor((Rarity)i),
					FontFamily = (FontFamily)FindResource("FontRegularDemiBold")
				});
			}

			var result = AlertBox.Show(listOfInlines);

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

		var ingotAmount = CalculateIngotAmountsWhenMeltingMaterial(Material.BaseMeltingIngotBonus);

		var ingot = User.Instance.Ingots.FirstOrDefault(x => x.Rarity == meltedMaterial.Rarity);
		ingot?.AddItem(ingotAmount);
	}

	public void MeltArtifact(Artifact meltedArtifact)
	{
		meltedArtifact.RemoveItem();

		var ingotAmounts = CalculateIngotAmountsWhenMeltingArtifact(meltedArtifact);

		for (var i = 0; i < 6; i++)
		{
			var ingot = User.Instance.Ingots.FirstOrDefault(x => x.Rarity == (Rarity)i);
			ingot?.AddItem(ingotAmounts[i]);
		}
	}

	private static int CalculateIngotAmountsWhenMeltingMaterial(int baseIngotBonus)
	{
		var ingotAmount = baseIngotBonus;
		var meltingBuffPercent = User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Melting];
		while (meltingBuffPercent >= 100)
		{
			ingotAmount += baseIngotBonus;
			meltingBuffPercent -= 100;
		}

		if (meltingBuffPercent > 0)
		{
			var randomizedValue = Rng.Next(1, 101);
			if (randomizedValue <= meltingBuffPercent)
			{
				ingotAmount += baseIngotBonus;
			}
		}

		return ingotAmount;
	}

	private static List<int> CalculateIngotAmountsWhenMeltingArtifact(Artifact meltedArtifact)
	{
		var ingotAmounts = new List<int>();

		var artifactRecipe = GameAssets.Recipes.FirstOrDefault(x => x.ArtifactId == meltedArtifact.Id);

		if (artifactRecipe is not null)
		{
			for (var i = 0; i < 6; i++)
			{
				var materialsOfRarity = artifactRecipe.IngredientPatterns.Where(x => x.RelatedMaterial.Rarity == (Rarity)i);
				var totalMaterialQuantity = materialsOfRarity.Sum(x => x.Quantity);

				if (meltedArtifact.ArtifactType == ArtifactType.Ammunition)
				{
					ingotAmounts.Add(Math.Max((int)Math.Floor((totalMaterialQuantity * Material.BaseMeltingIngotBonus * Artifact.MeltingIngredientsRatio) / 50d), 1));
				}
				else
				{
					ingotAmounts.Add((int)(totalMaterialQuantity * Material.BaseMeltingIngotBonus * Artifact.MeltingIngredientsRatio));
				}
			}
		}
		else
		{
			for (var i = 0; i < 6; i++)
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
		var recipe = b?.CommandParameter as Recipe;

		HandleCrafting<Material>(recipe);
	}

	private void HandleCrafting<T>(Recipe recipe)
	{
		if (!MeetsCraftingRequirement(recipe))
		{
			var listOfInlines = new List<Inline>
			{
				new Run("You dont meet Craftsman specialization requirements to craft "),
				new Run($"{recipe.Rarity.ToString()}")
				{
					Foreground = ColorsHelper.GetRarityColor(recipe.Rarity),
					FontFamily = (FontFamily)FindResource("FontRegularDemiBold")
				},
				new Run(" artifacts.\nCraft more common items in order to master it.")
			};

			AlertBox.Show(listOfInlines, MessageBoxButton.OK);
			return;
		}

		bool enoughIngredients;

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

			if (recipe.Artifact.ArtifactType == ArtifactType.Ammunition)
			{
				recipe.Artifact.AddItem(Models.Functionalities.ArtifactTypes.Ammunition.AmountPerCraft);
			}
			else
			{
				recipe.Artifact.AddItem();
			}

			recipe.RemoveItem();

			Specializations.UpdateSpecializationAmountAndUi(SpecializationType.Crafting);

			UpdateBlacksmithItems();
			
			// If hero has all other artifacts, grant Transcendence.
			if (User.Instance.CurrentHero.Artifacts.Select(x => x.Name).Distinct().Count() == GameAssets.Artifacts.Count - 1)
			{
				var transcendence = GameAssets.Artifacts.FirstOrDefault(x => x.Name == "Transcendence");
				transcendence?.AddItem();
			}
		}
	}

	private bool CheckAndRemoveIngots(Recipe recipe)
	{
		var ingotRaritiesInlines = new List<Inline>();

		var ingotAmounts = CalculateIngotAmountsWhenCraftingArtifact(recipe.Artifact);

		for (var i = 0; i < 6; i++)
		{
			if (ingotAmounts[i] == 0)
			{
				continue;
			}

			ingotRaritiesInlines.Add(new Run($"\n{ingotAmounts[i]}x {(Rarity)i} Ingots")
			{
				Foreground = ColorsHelper.GetRarityColor((Rarity)i)
			});
		}

		if (!CheckIfUserHasEnoughIngots(recipe))
		{
			var notEnoughIngotsInlines = new List<Inline>
			{
				new Run("You dont have enough ingots to craft "),
				new Run($"{recipe.Name}")
				{
					FontFamily = (FontFamily)FindResource("FontRegularDemiBold")
				},
				new Run(".\nIngots required:")
			};
			notEnoughIngotsInlines.AddRange(ingotRaritiesInlines);
			notEnoughIngotsInlines.Add(new Run("\n\nGet more ingots by melting materials/artifacts or try to craft this artifact using materials."));

			AlertBox.Show(notEnoughIngotsInlines, MessageBoxButton.OK);
			return false;
		}

		var enoughIngotsInlines = new List<Inline>
		{
			new Run("Are you sure you want to craft "),
			new Run($"{recipe.Name}")
			{
				FontFamily = (FontFamily)FindResource("FontRegularDemiBold")
			},
			new Run(" using ingots?\nIngots required:")
		};
		enoughIngotsInlines.AddRange(ingotRaritiesInlines);
		enoughIngotsInlines.Add(new Run("\n\nThis action will destroy all ingots and this recipe."));

		var result = AlertBox.Show(enoughIngotsInlines);

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

		for (var i = 0; i < 6; i++)
		{
			User.Instance.Ingots.FirstOrDefault(x => x.Rarity == (Rarity)i).Quantity -= ingotsAmount[i];
		}
	}

	private bool CheckIfUserHasEnoughIngots(Recipe recipe)
	{
		var ingotsAmount = CalculateIngotAmountsWhenCraftingArtifact(recipe.Artifact);

		for (var i = 0; i < 6; i++)
		{
			var userHasEnoughIngots = User.Instance.Ingots.FirstOrDefault(x => x.Rarity == (Rarity)i).Quantity >= ingotsAmount[i];

			if (!userHasEnoughIngots)
			{
				return false;
			}
		}

		return true;
	}

	private static List<int> CalculateIngotAmountsWhenCraftingArtifact(Artifact craftedArtifact)
	{
		var ingotAmounts = new List<int>();

		var artifactRecipe = GameAssets.Recipes.FirstOrDefault(x => x.ArtifactId == craftedArtifact.Id);

		for (var i = 0; i < 6; i++)
		{
			var materialsOfRarity = artifactRecipe?.IngredientPatterns.Where(x => x.RelatedMaterial.Rarity == (Rarity)i);
			var totalMaterialQuantity = materialsOfRarity.Sum(x => x.Quantity);
			ingotAmounts.Add((int)(totalMaterialQuantity * Material.BaseMeltingIngotBonus * Artifact.CraftingRatio));
		}

		return ingotAmounts;
	}

	private bool CheckAndRemoveMaterials(Recipe recipe)
	{
		if (!CheckIfHeroHasEnoughMaterials(recipe))
		{
			var notEnoughMaterialsInlines = new List<Inline>
			{
				new Run("You don't have enough materials to craft "),
				new Run($"{recipe.Name}")
				{
					FontFamily = (FontFamily)FindResource("FontRegularDemiBold")
				},
				new Run(".\n")
			};
			notEnoughMaterialsInlines.AddRange(ItemToolTipHelper.GenerateRecipeIngredientsInlines(recipe));
			notEnoughMaterialsInlines.Add(new Run("\n\nGet more materials by completing quests and killing monsters and bosses or try to craft this artifact using ingots."));

			AlertBox.Show(notEnoughMaterialsInlines, MessageBoxButton.OK);
			return false;
		}

		var enoughMaterialsInlines = new List<Inline>
		{
			new Run("Are you sure you want to craft "),
			new Run($"{recipe.Name}")
			{
				FontFamily = (FontFamily)FindResource("FontRegularDemiBold")
			},
			new Run(" using materials?\n")
		};
		enoughMaterialsInlines.AddRange(ItemToolTipHelper.GenerateRecipeIngredientsInlines(recipe));
		enoughMaterialsInlines.Add(new Run("\n\nThis action will destroy all materials and this recipe."));

		var result = AlertBox.Show(enoughMaterialsInlines);

		if (result == MessageBoxResult.No)
		{
			return false;
		}

		RemoveMaterials(recipe);
		return true;
	}

	private static void RemoveMaterials(Recipe recipe)
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

	private static bool CheckIfHeroHasEnoughMaterials(Recipe recipe)
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

	private static bool MeetsCraftingRequirement(Recipe recipe)
	{
		return User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Crafting] >= (int)recipe.Rarity;
	}

	private void CraftIngotButton_Click(object sender, RoutedEventArgs e)
	{
		var b = sender as Button;
		var recipe = b?.CommandParameter as Recipe;

		HandleCrafting<Ingot>(recipe);
	}

	private void MeltButton_OnInitialized(object sender, EventArgs e)
	{
		var button = sender as Button;

		if (button?.ToolTip == null)
		{
			var toolTipBlock = new TextBlock
			{
				Style = (Style)FindResource("ToolTipTextBlockBase")
			};

			if (button?.CommandParameter is Material material)
			{
				toolTipBlock.Inlines.Add(new Run("Melt for at least"));
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run($"{Material.BaseMeltingIngotBonus}x {material.RarityString} Ingots")
				{
					FontFamily = (FontFamily)FindResource("FontRegularDemiBold"),
					Foreground = ColorsHelper.GetRarityColor(material.Rarity)
				});
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("Chance for more through Melter specialization"));
			}
			else
			{
				var ingotAmounts = CalculateIngotAmountsWhenMeltingArtifact(button.CommandParameter as Artifact);

				toolTipBlock.Inlines.Add(new Run("Melt for:"));

				for (var i = 0; i < 6; i++)
				{
					if (ingotAmounts[i] == 0)
					{
						continue;
					}

					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"{ingotAmounts[i]}x {(Rarity)i} Ingots")
					{
						FontFamily = (FontFamily)FindResource("FontRegularDemiBold"),
						Foreground = ColorsHelper.GetRarityColor((Rarity)i)
					});
				}
			}

			var toolTip = new ToolTip
			{
				Style = (Style)FindResource("ToolTipSimple")
			};

			toolTip.Content = toolTipBlock;

			button.ToolTip = toolTip;
		}
	}

	private void CraftMaterialButton_OnInitialized(object sender, EventArgs e)
	{
		var button = sender as Button;

		if (button?.ToolTip == null)
		{
			var toolTipBlock = new TextBlock
			{
				Style = (Style)FindResource("ToolTipTextBlockBase")
			};

			toolTipBlock.Inlines.Add(new Run("Craft with:"));
			toolTipBlock.Inlines.Add(new LineBreak());

			var listOfInlines = ItemToolTipHelper.GenerateRecipeIngredientsInlines(button.CommandParameter as Recipe);

			foreach (var run in listOfInlines)
			{
				run.FontFamily = (FontFamily)FindResource("FontRegularDemiBold");
				run.FontSize = (double)FindResource("FontSizeToolTipBase");
				toolTipBlock.Inlines.Add(run);
			}

			var toolTip = new ToolTip
			{
				Style = (Style)FindResource("ToolTipSimple")
			};

			toolTip.Content = toolTipBlock;

			button.ToolTip = toolTip;
		}
	}

	private void CraftIngotButton_OnInitialized(object sender, EventArgs e)
	{
		var button = sender as Button;

		if (button?.ToolTip == null)
		{
			var toolTipBlock = new TextBlock
			{
				Style = (Style)FindResource("ToolTipTextBlockBase")
			};

			toolTipBlock.Inlines.Add(new Run("Craft with:"));
			toolTipBlock.Inlines.Add(new LineBreak());
			toolTipBlock.Inlines.Add(new Run("Ingots:")
			{
				FontFamily = (FontFamily)FindResource("FontRegularDemiBold"),
				FontSize = (double)FindResource("FontSizeToolTipBase")
			});

			var ingotAmounts = CalculateIngotAmountsWhenCraftingArtifact((button?.CommandParameter as Recipe).Artifact);

			for (var i = 0; i < 6; i++)
			{
				if (ingotAmounts[i] == 0)
				{
					continue;
				}

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"{ingotAmounts[i]}x {(Rarity)i} Ingots")
				{
					FontFamily = (FontFamily)FindResource("FontRegularDemiBold"),
					FontSize = (double)FindResource("FontSizeToolTipBase"),
					Foreground = ColorsHelper.GetRarityColor((Rarity)i)
				});
			}

			var toolTip = new ToolTip
			{
				Style = (Style)FindResource("ToolTipSimple")
			};

			toolTip.Content = toolTipBlock;

			button.ToolTip = toolTip;
		}
	}

	private void TownButton_Click(object sender, RoutedEventArgs e)
	{
		InterfaceHelper.ChangePage(GameAssets.Pages["Town"], "Town");
	}
}