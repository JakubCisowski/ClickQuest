using ClickQuest.Game.Core.Adventures;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Items.Patterns;
using ClickQuest.Game.Core.Items.Types;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using static ClickQuest.Game.Extensions.UserInterface.DescriptionsController;
using static ClickQuest.Game.Extensions.UserInterface.ToolTips.GeneralToolTipController;
using Colors = ClickQuest.Game.Extensions.UserInterface.ColorsController;

namespace ClickQuest.Game.Extensions.UserInterface.ToolTips
{
    public static class ItemToolTipController
    {
        public static ToolTip GenerateItemToolTip<T>(T itemToGenerateToolTipFor) where T : Item
        {
            var fontSizeToolTipname = (double)Application.Current.FindResource("FontSizeToolTipName");
            FontFamily fontFamilyRegularDemiBold = (FontFamily)Application.Current.FindResource("FontRegularDemiBold");

            ToolTip toolTip = new ToolTip
            {
                BorderBrush = Colors.GetRarityColor(itemToGenerateToolTipFor.Rarity)
            };

            TextBlock toolTipBlock = new TextBlock
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
                            Foreground = Colors.GetRarityColor(material.Rarity),
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
                            Foreground = Colors.GetRarityColor(artifact.Rarity),
                            FontFamily = fontFamilyRegularDemiBold
                        });
                        toolTipBlock.Inlines.Add(new LineBreak());
                        toolTipBlock.Inlines.Add(new Run($"{artifact.ArtifactType.ToString()}"));

                        toolTipBlock.Inlines.Add(new LineBreak());
                        toolTipBlock.Inlines.Add(GenerateTextSeparator());
                        toolTipBlock.Inlines.Add(new LineBreak());

                        var listOfDescriptionRuns = GenerateDescriptionRuns(artifact.Description);
                        foreach (Run run in listOfDescriptionRuns)
                        {
                            toolTipBlock.Inlines.Add(run);
                        }

                        toolTipBlock.Inlines.Add(new LineBreak());
                        toolTipBlock.Inlines.Add(new LineBreak());
                        toolTipBlock.Inlines.Add(new Run($"{artifact.ArtifactFunctionality.ArtifactTypeFunctionality.Description + artifact.ExtraInfo}")
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
                            Foreground = Colors.GetRarityColor(recipe.Rarity),
                            FontFamily = fontFamilyRegularDemiBold
                        });

                        toolTipBlock.Inlines.Add(new LineBreak());
                        toolTipBlock.Inlines.Add(GenerateTextSeparator());
                        toolTipBlock.Inlines.Add(new LineBreak());

                        var listOfDescriptionRuns = GenerateDescriptionRuns(recipe.Description);
                        foreach (Run run in listOfDescriptionRuns)
                        {
                            toolTipBlock.Inlines.Add(run);
                        }

                        toolTipBlock.Inlines.Add(new LineBreak());
                        toolTipBlock.Inlines.Add(GenerateTextSeparator());
                        toolTipBlock.Inlines.Add(new LineBreak());

                        var listOfIngredientRuns = GenerateRecipeIngredientsRuns(recipe);
                        foreach (Run run in listOfIngredientRuns)
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
            FontFamily fontFamilyRegularDemiBold = (FontFamily)Application.Current.FindResource("FontRegularDemiBold");

            ToolTip currencyToolTip = new ToolTip
            {
                Style = (Style)Application.Current.FindResource("ToolTipSimple")
            };

            TextBlock currencyToolTipTextBlock = new TextBlock
            {
                Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase"),
                FontFamily = fontFamilyRegularDemiBold
            };

            SolidColorBrush currencyColor = Colors.GetRarityColor((Rarity)rarityValue);
            currencyToolTipTextBlock.Foreground = currencyColor;

            currencyToolTipTextBlock.Text = (Rarity)rarityValue + " " + (typeof(T) == typeof(Ingot) ? "Ingots" : "Dungeon Keys");

            currencyToolTip.Content = currencyToolTipTextBlock;

            return currencyToolTip;
        }

        public static ToolTip GenerateBlessingToolTip(Blessing blessing)
        {
            var fontSizeToolTipname = (double)Application.Current.FindResource("FontSizeToolTipName");
            FontFamily fontFamilyRegularDemiBold = (FontFamily)Application.Current.FindResource("FontRegularDemiBold");

            ToolTip blessingToolTip = new ToolTip();

            TextBlock blessingToolTipBlock = new TextBlock
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
                    Foreground = ColorsController.GetRarityColor(blessing.Rarity),
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

            ToolTip questToolTip = new ToolTip();

            TextBlock questToolTipTextBlock = new TextBlock
            {
                Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase")
            };

            if (currentQuest != null)
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
                foreach (Run run in listOfIngredientRuns)
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
                    FontSize = (double) Application.Current.FindResource("FontSizeToolTipIngredientText")
                }
            };

            foreach (IngredientPattern ingredient in recipe.IngredientPatterns)
            {
                Material relatedMaterial = ingredient.RelatedMaterial;
                ingredientRuns.Add(new Run($"{ingredient.Quantity}x "));
                ingredientRuns.Add(new Run($"{relatedMaterial.Name}")
                {
                    Foreground = Colors.GetRarityColor(relatedMaterial.Rarity)
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
                    FontSize = (double) Application.Current.FindResource("FontSizeToolTipIngredientText")
                }
            };

            foreach (QuestRewardPattern pattern in quest.QuestRewardPatterns)
            {
                questRewardRuns.Add(new Run($"{pattern.Quantity}x "));

                switch (pattern.QuestRewardType)
                {
                    case RewardType.Material:
                        Material? material = GameAssets.Materials.FirstOrDefault(x => x.Id == pattern.QuestRewardId);
                        questRewardRuns.Add(new Run($"{material.Name}")
                        {
                            Foreground = Colors.GetRarityColor(material.Rarity)
                        });

                        break;

                    case RewardType.Artifact:
                        Artifact? artifact = GameAssets.Artifacts.FirstOrDefault(x => x.Id == pattern.QuestRewardId);
                        questRewardRuns.Add(new Run($"{artifact.Name}")
                        {
                            Foreground = Colors.GetRarityColor(artifact.Rarity)
                        });

                        break;

                    case RewardType.Recipe:
                        Recipe? recipe = GameAssets.Recipes.FirstOrDefault(x => x.Id == pattern.QuestRewardId);
                        questRewardRuns.Add(new Run($"{recipe.Name}")
                        {
                            Foreground = Colors.GetRarityColor(recipe.Rarity)
                        });

                        break;

                    case RewardType.Ingot:
                        Material? ingot = GameAssets.Materials.FirstOrDefault(x => x.Id == pattern.QuestRewardId);
                        questRewardRuns.Add(new Run($"{ingot.Name}")
                        {
                            Foreground = Colors.GetRarityColor(ingot.Rarity)
                        });

                        break;

                    case RewardType.Blessing:
                        Blessing? blessing = GameAssets.Blessings.FirstOrDefault(x => x.Id == pattern.QuestRewardId);
                        questRewardRuns.Add(new Run($"{blessing.Name}")
                        {
                            Foreground = Colors.GetRarityColor(blessing.Rarity)
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
            ToolTip toolTip = new ToolTip
            {
                Style = (Style)Application.Current.FindResource("ToolTipSimple")
            };

            TextBlock toolTipBlock = new TextBlock
            {
                Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase"),
                Text = "You have not discovered this item yet\nIt will show up here once you first loot it"
            };

            toolTip.Content = toolTipBlock;

            return toolTip;
        }
    }
}