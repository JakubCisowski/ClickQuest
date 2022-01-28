using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Models;
using static ClickQuest.Game.UserInterface.Helpers.DescriptionsHelper;
using static ClickQuest.Game.UserInterface.Helpers.ToolTips.GeneralToolTipHelper;

namespace ClickQuest.Game.UserInterface.Helpers.ToolTips;

public static class ItemToolTipHelper
{
	public static ToolTip GenerateItemToolTip<T>(T itemToGenerateToolTipFor) where T : Item
	{
		var fontSizeToolTipname = (double)Application.Current.FindResource("FontSizeToolTipName");
		var fontFamilyRegularDemiBold = (FontFamily)Application.Current.FindResource("FontRegularDemiBold");

		var toolTip = new ToolTip
		{
			BorderBrush = ColorsHelper.GetRarityColor(itemToGenerateToolTipFor.Rarity)
		};

		var toolTipBlock = new TextBlock
		{
			Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase")
		};

		switch (itemToGenerateToolTipFor)
		{
			case Material material:
			{
				toolTipBlock.Inlines.Add(new Run($"{material.Name}")
				{
					FontSize = fontSizeToolTipname
				});
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"*{material.RarityString}*")
				{
					Foreground = ColorsHelper.GetRarityColor(material.Rarity),
					FontFamily = fontFamilyRegularDemiBold
				});

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run($"{material.Description}")
				{
					FontFamily = (FontFamily)Application.Current.FindResource("FontRegularItalic"),
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("Value: "));
				toolTipBlock.Inlines.Add(new Run($"{material.Value} gold")
				{
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGold")
				});
			}
				break;

			case Artifact artifact:
			{
				toolTipBlock.Inlines.Add(new Run($"{artifact.Name}")
				{
					FontSize = fontSizeToolTipname
				});
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"*{artifact.RarityString}*")
				{
					Foreground = ColorsHelper.GetRarityColor(artifact.Rarity),
					FontFamily = fontFamilyRegularDemiBold
				});

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				var listOfDescriptionRuns = GenerateDescriptionRuns(artifact.Description);
				foreach (var run in listOfDescriptionRuns)
				{
					toolTipBlock.Inlines.Add(run);
				}

				if (!string.IsNullOrEmpty(artifact.ExtraInfo))
				{
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"{artifact.ExtraInfo}")
					{
						FontFamily = (FontFamily)Application.Current.FindResource("FontRegularItalic"),
						Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray5")
					});
				}

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				// Split capital letters.
				var artifactTypeString = string.Concat(artifact.ArtifactType.ToString().Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
				toolTipBlock.Inlines.Add(new Run($"{artifactTypeString}"));
				
				toolTipBlock.Inlines.Add(new LineBreak());
				
				toolTipBlock.Inlines.Add(new Run($"{artifact.ArtifactFunctionality.ArtifactTypeFunctionality.Description}")
				{
					FontFamily = (FontFamily)Application.Current.FindResource("FontRegularItalic"),
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray5")
				});

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run($"{artifact.Lore}")
				{
					FontFamily = (FontFamily)Application.Current.FindResource("FontRegularItalic"),
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});

				if (artifact.Rarity == Rarity.Mythic)
				{
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"{artifact.MythicTag}")
					{
						FontFamily = (FontFamily)Application.Current.FindResource("FontFancy"),
						Foreground = (SolidColorBrush)Application.Current.FindResource("BrushMythicTag")
					});
				}
			}
				break;

			case Recipe recipe:
			{
				toolTipBlock.Inlines.Add(new Run($"{recipe.FullName}")
				{
					FontSize = fontSizeToolTipname
				});
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"*{recipe.RarityString}*")
				{
					Foreground = ColorsHelper.GetRarityColor(recipe.Rarity),
					FontFamily = fontFamilyRegularDemiBold
				});
				toolTipBlock.Inlines.Add(new LineBreak());

				// Split capital letters.
				var artifactTypeString = string.Concat(recipe.Artifact.ArtifactType.ToString().Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
				toolTipBlock.Inlines.Add(new Run($"{artifactTypeString}"));

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				var listOfDescriptionRuns = GenerateDescriptionRuns(recipe.Description);
				foreach (var run in listOfDescriptionRuns)
				{
					toolTipBlock.Inlines.Add(run);
				}

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				var listOfIngredientRuns = GenerateRecipeIngredientsRuns(recipe);
				foreach (var run in listOfIngredientRuns)
				{
					toolTipBlock.Inlines.Add(run);
				}

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("Value: "));
				toolTipBlock.Inlines.Add(new Run($"{recipe.Value} gold")
				{
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGold")
				});
			}
				break;
		}

		toolTip.Content = toolTipBlock;

		return toolTip;
	}

	public static ToolTip GenerateCurrencyToolTip<T>(int rarityValue) where T : Item
	{
		var fontFamilyRegularDemiBold = (FontFamily)Application.Current.FindResource("FontRegularDemiBold");

		var currencyToolTip = new ToolTip
		{
			Style = (Style)Application.Current.FindResource("ToolTipSimple")
		};

		var currencyToolTipTextBlock = new TextBlock
		{
			Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase"),
			FontFamily = fontFamilyRegularDemiBold
		};

		var currencyColor = ColorsHelper.GetRarityColor((Rarity)rarityValue);
		currencyToolTipTextBlock.Foreground = currencyColor;

		currencyToolTipTextBlock.Text = (Rarity)rarityValue + " " + (typeof(T) == typeof(Ingot) ? "Ingots" : "Dungeon Keys");

		currencyToolTip.Content = currencyToolTipTextBlock;

		return currencyToolTip;
	}

	public static ToolTip GenerateBlessingToolTip(Blessing blessing)
	{
		var fontSizeToolTipname = (double)Application.Current.FindResource("FontSizeToolTipName");
		var fontFamilyRegularDemiBold = (FontFamily)Application.Current.FindResource("FontRegularDemiBold");

		var blessingToolTip = new ToolTip();

		var blessingToolTipBlock = new TextBlock
		{
			Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase")
		};

		if (blessing != null)
		{
			blessingToolTipBlock.Inlines.Add(new Run($"{blessing.Name}")
			{
				FontSize = fontSizeToolTipname
			});
			blessingToolTipBlock.Inlines.Add(new LineBreak());
			blessingToolTipBlock.Inlines.Add(new Run($"*{blessing.RarityString}*")
			{
				Foreground = ColorsHelper.GetRarityColor(blessing.Rarity),
				FontFamily = fontFamilyRegularDemiBold
			});

			blessingToolTipBlock.Inlines.Add(new LineBreak());
			blessingToolTipBlock.Inlines.Add(new Run($"*{blessing.TypeString}*"));

			blessingToolTipBlock.Inlines.Add(new LineBreak());
			blessingToolTipBlock.Inlines.Add(GenerateTextSeparator());
			blessingToolTipBlock.Inlines.Add(new LineBreak());

			blessingToolTipBlock.Inlines.Add(new Run($"{blessing.Description}"));

			blessingToolTipBlock.Inlines.Add(new LineBreak());
			blessingToolTipBlock.Inlines.Add(GenerateTextSeparator());
			blessingToolTipBlock.Inlines.Add(new LineBreak());

			blessingToolTipBlock.Inlines.Add(new Run($"{blessing.Lore}")
			{
				FontFamily = (FontFamily)Application.Current.FindResource("FontRegularItalic"),
				Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
			});
		}
		else
		{
			blessingToolTipBlock.Text = "No blessing is currently active";
			blessingToolTip.Style = (Style)Application.Current.FindResource("ToolTipSimple");
		}

		blessingToolTip.Content = blessingToolTipBlock;

		return blessingToolTip;
	}

	public static ToolTip GenerateQuestToolTip(Quest currentQuest)
	{
		var fontSizeToolTipname = (double)Application.Current.FindResource("FontSizeToolTipName");

		var questToolTip = new ToolTip();

		var questToolTipTextBlock = new TextBlock
		{
			Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase")
		};

		if (currentQuest != null && currentQuest.IsFinished == false)
		{
			questToolTipTextBlock.Inlines.Add(new Run($"{currentQuest.Name}")
			{
				FontSize = fontSizeToolTipname
			});
			if (currentQuest.Rare)
			{
				questToolTipTextBlock.Inlines.Add(new LineBreak());
				questToolTipTextBlock.Inlines.Add(new Run("*Rare Quest*")
				{
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushQuestRare"),
					FontFamily = (FontFamily)Application.Current.FindResource("FontRegularBold")
				});
			}

			questToolTipTextBlock.Inlines.Add(new LineBreak());
			questToolTipTextBlock.Inlines.Add(GenerateTextSeparator());
			questToolTipTextBlock.Inlines.Add(new LineBreak());

			questToolTipTextBlock.Inlines.Add(new Run($"{currentQuest.Description}"));

			questToolTipTextBlock.Inlines.Add(new LineBreak());
			questToolTipTextBlock.Inlines.Add(GenerateTextSeparator());
			questToolTipTextBlock.Inlines.Add(new LineBreak());

			var listOfIngredientRuns = GenerateQuestRewardRuns(currentQuest);
			foreach (var run in listOfIngredientRuns)
			{
				questToolTipTextBlock.Inlines.Add(run);
			}
		}
		else
		{
			questToolTipTextBlock.Text = "No quest is currently active";
			questToolTip.Style = (Style)Application.Current.FindResource("ToolTipSimple");
		}

		questToolTip.Content = questToolTipTextBlock;

		return questToolTip;
	}

	public static List<Run> GenerateRecipeIngredientsRuns(Recipe recipe)
	{
		var ingredientRuns = new List<Run>
		{
			new Run("Ingredients:\n")
			{
				FontSize = (double)Application.Current.FindResource("FontSizeToolTipIngredientText")
			}
		};

		foreach (var ingredient in recipe.IngredientPatterns.OrderBy(x => x.RelatedMaterial.Rarity))
		{
			var relatedMaterial = ingredient.RelatedMaterial;
			ingredientRuns.Add(new Run($"{ingredient.Quantity}x "));
			ingredientRuns.Add(new Run($"{relatedMaterial.Name}")
			{
				Foreground = ColorsHelper.GetRarityColor(relatedMaterial.Rarity)
			});
			ingredientRuns.Add(new Run("\n"));
		}

		ingredientRuns.RemoveAt(ingredientRuns.Count - 1);

		return ingredientRuns;
	}

	public static List<Run> GenerateQuestRewardRuns(Quest quest)
	{
		var questRewardRuns = new List<Run>
		{
			new Run("Rewards:\n")
			{
				FontSize = (double)Application.Current.FindResource("FontSizeToolTipIngredientText")
			}
		};

		foreach (var pattern in quest.QuestRewardPatterns)
		{
			questRewardRuns.Add(new Run($"{pattern.Quantity}x "));

			switch (pattern.QuestRewardType)
			{
				case RewardType.Material:
					var material = GameAssets.Materials.FirstOrDefault(x => x.Id == pattern.QuestRewardId);
					questRewardRuns.Add(new Run($"{material.Name}")
					{
						Foreground = ColorsHelper.GetRarityColor(material.Rarity)
					});

					break;

				case RewardType.Artifact:
					var artifact = GameAssets.Artifacts.FirstOrDefault(x => x.Id == pattern.QuestRewardId);
					questRewardRuns.Add(new Run($"{artifact.Name}")
					{
						Foreground = ColorsHelper.GetRarityColor(artifact.Rarity)
					});

					break;

				case RewardType.Recipe:
					var recipe = GameAssets.Recipes.FirstOrDefault(x => x.Id == pattern.QuestRewardId);
					questRewardRuns.Add(new Run($"{recipe.Name}")
					{
						Foreground = ColorsHelper.GetRarityColor(recipe.Rarity)
					});

					break;

				case RewardType.Ingot:
					var ingot = GameAssets.Materials.FirstOrDefault(x => x.Id == pattern.QuestRewardId);
					questRewardRuns.Add(new Run($"{ingot.Name}")
					{
						Foreground = ColorsHelper.GetRarityColor(ingot.Rarity)
					});

					break;

				case RewardType.Blessing:
					var blessing = GameAssets.Blessings.FirstOrDefault(x => x.Id == pattern.QuestRewardId);
					questRewardRuns.Add(new Run($"{blessing.Name}")
					{
						Foreground = ColorsHelper.GetRarityColor(blessing.Rarity)
					});

					break;
			}

			questRewardRuns.Add(new Run("\n"));
		}

		questRewardRuns.RemoveAt(questRewardRuns.Count - 1);

		return questRewardRuns;
	}

	public static ToolTip GenerateUndiscoveredItemToolTip()
	{
		var toolTip = new ToolTip
		{
			Style = (Style)Application.Current.FindResource("ToolTipSimple")
		};

		var toolTipBlock = new TextBlock
		{
			Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase"),
			Text = "You have not discovered this item yet\nIt will show up here once you first loot it"
		};

		toolTip.Content = toolTipBlock;

		return toolTip;
	}
}