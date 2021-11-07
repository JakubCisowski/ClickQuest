using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Heroes;
using ClickQuest.Game.Extensions.Combat;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Colors = ClickQuest.Game.Extensions.UserInterface.ColorsController;

namespace ClickQuest.Game.Extensions.UserInterface
{
	public static class TooltipController
	{
		const string TagOpeningStart = "<";
		const string TagClosingStart = "</";
		const string TagEnd = ">";

		public static ToolTip GenerateEquipmentItemTooltip<T>(T itemToGenerateTooltipFor) where T : Item
		{
			var toolTip = new ToolTip();

			var toolTipBlock = new TextBlock
			{
				Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase")
			};

			switch (itemToGenerateTooltipFor)
			{
				case Material material:
					{
						toolTipBlock.Inlines.Add(new Run($"{material.Name}"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"*{material.RarityString}*")
						{
							Foreground = Colors.GetRarityColor(material.Rarity),
							FontFamily = (FontFamily)Application.Current.FindResource("FontRegularDemiBold")
						});
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"{material.Description}"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"Value: {material.Value} gold"));
					}
					break;

				case Artifact artifact:
					{
						toolTipBlock.Inlines.Add(new Run($"{artifact.Name}"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"*{artifact.RarityString}*")
						{
							Foreground = Colors.GetRarityColor(artifact.Rarity),
							FontFamily = (FontFamily)Application.Current.FindResource("FontRegularDemiBold")
						});
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new LineBreak());

						var listOfRuns = GenerateArtifactDescriptionRuns(artifact.Description);
						foreach (var run in listOfRuns)
						{
							toolTipBlock.Inlines.Add(run);
						}

						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"{artifact.Lore}"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"{artifact.ExtraInfo}"));

						if (artifact.Rarity == Rarity.Mythic)
						{
							toolTipBlock.Inlines.Add(new LineBreak());
							toolTipBlock.Inlines.Add(new Run($"{artifact.MythicTag}") { FontFamily = (FontFamily)Application.Current.FindResource("FontFancy") });
						}
					}
					break;

				case Recipe recipe:
					{
						toolTipBlock.Inlines.Add(new Run($"{recipe.Name}"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"*{recipe.RarityString}*")
						{
							Foreground = Colors.GetRarityColor(recipe.Rarity),
							FontFamily = (FontFamily)Application.Current.FindResource("FontRegularDemiBold")
						});
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new LineBreak());

						var listOfRuns = GenerateArtifactDescriptionRuns(recipe.Description);
						foreach (var run in listOfRuns)
						{
							toolTipBlock.Inlines.Add(run);
						}

						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"{recipe.RequirementsDescription}"));
					}
					break;
			}

			toolTip.Content = toolTipBlock;

			return toolTip;
		}

		public static ToolTip GenerateCurrencyTooltip<T>(int rarityValue) where T : Item
		{
			var currencyTooltip = new ToolTip();

			var currencyTooltipTextBlock = new TextBlock
			{
				Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase"),
				FontFamily = (FontFamily)Application.Current.FindResource("FontRegularDemiBold")
			};

			var currencyColor = Colors.GetRarityColor((Rarity)rarityValue);
			currencyTooltipTextBlock.Foreground = currencyColor;

			currencyTooltipTextBlock.Text = (Rarity)rarityValue + " " + (typeof(T) == typeof(Ingot) ? "Ingots" : "Dungeon Keys");

			currencyTooltip.Content = currencyTooltipTextBlock;

			return currencyTooltip;
		}

		public static ToolTip GenerateSpecizaltionTooltip(SpecializationType typeOfGeneratedSpecialization, int nextUpgrade)
		{
			// Generate SpecTrading tooltips.
			var specToolTip = new ToolTip();

			var toolTipBlock = new TextBlock
			{
				Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase")
			};

			switch (typeOfGeneratedSpecialization)
			{
				case SpecializationType.Blessing:
					{
						toolTipBlock.Inlines.Add(new Run("Increases blessing duration in seconds"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run("You can master Prayer by buying blessings in Priest"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"Next upgrade (+{Specialization.SpecBlessingBuffBonus}s duration) in"));
						toolTipBlock.Inlines.Add(new Run($" {nextUpgrade} ") { FontFamily = (FontFamily)Application.Current.FindResource("FontRegularBold") });
						toolTipBlock.Inlines.Add(new Run("bought blessings"));
					}
					break;

				case SpecializationType.Clicking:
					{
						toolTipBlock.Inlines.Add(new Run("Increases click damage"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run("You can master Clicker by clicking on monsters and bosses"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run("Click damage from this specialization is applied after other effects eg. crit"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"Next upgrade (+{Specialization.SpecClickingBuffBonus} click damage) in"));
						toolTipBlock.Inlines.Add(new Run($" {nextUpgrade} ") { FontFamily = (FontFamily)Application.Current.FindResource("FontRegularBold") });
						toolTipBlock.Inlines.Add(new Run("clicks"));
					}
					break;

				case SpecializationType.Crafting:
					{
						toolTipBlock.Inlines.Add(new Run("Increases crafting rarity limit (base limit is "));
						toolTipBlock.Inlines.Add(new Run($"{(Rarity)Specialization.SpecCraftingBuffBase}"){Foreground = ColorsController.GetRarityColor((Rarity)Specialization.SpecCraftingBuffBase)});
						toolTipBlock.Inlines.Add(new Run(" rarity)"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run("You can master Craftsman by crafting artifacts in Blacksmith"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run("Next rarity limit upgrade in"));
						toolTipBlock.Inlines.Add(new Run($" {nextUpgrade} ") { FontFamily = (FontFamily)Application.Current.FindResource("FontRegularBold") });
						toolTipBlock.Inlines.Add(new Run("crafted artifacts"));
					}
					break;

				case SpecializationType.Trading:
					{
						toolTipBlock.Inlines.Add(new Run($"Increases shop offer size (base size is {Specialization.SpecTradingBuffBase}) and materials selling ratio (base bonus is {Specialization.SpecTradingBuffBase}%)"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run("You can master Tradesman by buying recipes or selling materials in shop"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"Next upgrade (+{Specialization.SpecTradingBuffBonus} shop offer size and +{Specialization.SpecTradingBuffBonus}% materials selling ratio) in"));
						toolTipBlock.Inlines.Add(new Run($" {nextUpgrade} ") { FontFamily = (FontFamily)Application.Current.FindResource("FontRegularBold") });
						toolTipBlock.Inlines.Add(new Run("bought recipes or sold materials"));
					}
					break;

				case SpecializationType.Melting:
					{
						toolTipBlock.Inlines.Add(new Run($"Increases % chance to get a bonus {Material.BaseMeltingIngotBonus} ingots when melting materials (base chance is 0%)"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run("You can master Melter by melting materials in Blacksmith"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"Each 100% guarantees additional {Material.BaseMeltingIngotBonus} ingots"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"Next upgrade (+{Specialization.SpecMeltingBuffBonus}% chance) in"));
						toolTipBlock.Inlines.Add(new Run($" {nextUpgrade} ") { FontFamily = (FontFamily)Application.Current.FindResource("FontRegularBold") });
						toolTipBlock.Inlines.Add(new Run("melted materials"));
					}
					break;

				case SpecializationType.Questing:
					{
						toolTipBlock.Inlines.Add(new Run($"Reduces % time to complete quests (limit is {Specialization.SpecQuestingBuffLimit}%)"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run("You can master Adventurer by completing quests"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"Next upgrade (+{Specialization.SpecQuestingBuffBonus}% reduced time) in"));
						toolTipBlock.Inlines.Add(new Run($" {nextUpgrade} ") { FontFamily = (FontFamily)Application.Current.FindResource("FontRegularBold") });
						toolTipBlock.Inlines.Add(new Run("completed quests"));
					}
					break;

				case SpecializationType.Dungeon:
					{
						toolTipBlock.Inlines.Add(new Run($"Increases amount of time to defeat boss in seconds (base time is {Specialization.SpecDungeonBuffBase}s)"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run("You can master Daredevil by trading recipes in shop"));
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"Next upgrade (+{Specialization.SpecDungeonBuffBonus} second) in"));
						toolTipBlock.Inlines.Add(new Run($" {nextUpgrade} ") { FontFamily = (FontFamily)Application.Current.FindResource("FontRegularBold") });
						toolTipBlock.Inlines.Add(new Run("finished dungeons"));
					}
					break;
			}

			specToolTip.Content = toolTipBlock;

			return specToolTip;
		}

		public static ToolTip GenerateBlessingTooltip(Blessing blessing)
		{
			/*
			 * <ToolTip>
											<TextBlock Style="{StaticResource ToolTipTextBlockBase}">
												<Run Text="{Binding Name}" />
												<LineBreak />
												<Run Text="*" />
												<Run Text="{Binding RarityString, Mode=OneWay}" />
												<Run Text="*" />
												<LineBreak />
												<LineBreak />
												<Run Text="{Binding Description}" />
												<LineBreak />
												<Run Text="Duration:" />
												<Run Text="{Binding Duration}" /><Run Text="s" />
											</TextBlock>
										</ToolTip>
			 */

			var blessingTooltip = new ToolTip();

			var blessingTooltipTextBlock = new TextBlock
			{
				Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase")
			};
			
			blessingTooltipTextBlock.Inlines.Add(new Run($"{blessing.Name}"));
			blessingTooltipTextBlock.Inlines.Add(new LineBreak());
			blessingTooltipTextBlock.Inlines.Add(new Run($"*{blessing.RarityString}*")
			{
				Foreground = Colors.GetRarityColor((Rarity)blessing.Rarity),
				FontFamily = (FontFamily)Application.Current.FindResource("FontRegularDemiBold")
			});
			blessingTooltipTextBlock.Inlines.Add(new LineBreak());
			blessingTooltipTextBlock.Inlines.Add(new LineBreak());
			blessingTooltipTextBlock.Inlines.Add(new Run($"{blessing.Description}"));
			blessingTooltipTextBlock.Inlines.Add(new LineBreak());
			blessingTooltipTextBlock.Inlines.Add(new Run($"Duration: {blessing.Duration}s"));

			blessingTooltip.Content = blessingTooltipTextBlock;

			return blessingTooltip;
		}

		public static void SetTooltipDelayAndDuration(DependencyObject control)
		{
			ToolTipService.SetInitialShowDelay(control, 100);
			ToolTipService.SetShowDuration(control, 20000);
		}

		public static List<Run> GenerateArtifactDescriptionRuns(string description)
		{
			var artifactDescriptionRuns = new List<Run>();

			// If it's called before loading descriptions.
			if (description is null)
			{
				return artifactDescriptionRuns;
			}

			while (true)
			{
				int indexOfTagOpeningStart = description.IndexOf(TagOpeningStart);

				if (indexOfTagOpeningStart == -1)
				{
					// Tag opening not found - create a normal Run with the remainder of description.
					artifactDescriptionRuns.Add(new Run(description));
					break;
				}
				else
				{
					if (indexOfTagOpeningStart != 0)
					{
						// If tag opening index is not zero, first create a normal Run with that part of description.
						string taglessPart = description.Substring(0, indexOfTagOpeningStart);
						artifactDescriptionRuns.Add(new Run(taglessPart));
						description = description.Remove(0, indexOfTagOpeningStart);
						indexOfTagOpeningStart = 0;
					}
					
					// Find closing tag.
					int indexOfTagOpeningEnd = description.IndexOf(TagEnd);
					int indexOfTagClosingStart = description.IndexOf(TagClosingStart);

					string tagType = description.Substring(1, indexOfTagOpeningEnd - indexOfTagOpeningStart - 1).ToUpper();
					
					string taggedPart = description.Substring(indexOfTagOpeningEnd + 1, indexOfTagClosingStart - indexOfTagOpeningEnd - 1);

					var coloredRun = new Run(taggedPart);

					switch (tagType)
					{
						// Damage type
						case "NORMAL":
							coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.Normal);
							break;
						case "CRITICAL":
							coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.Critical);
							break;
						case "POISON":
							coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.Poison);
							break;
						case "AURA":
							coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.Aura);
							break;
						case "ONHIT":
							coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.OnHit);
							break;
						case "ARTIFACT":
							coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.Artifact);
							break;
							
						// Class 
						case "SLAYER":
							coloredRun.Foreground = ColorsController.GetHeroClassColor(HeroClass.Slayer);
							break;
						case "VENOM":
							coloredRun.Foreground = ColorsController.GetHeroClassColor(HeroClass.Venom);
							break;

						// Rarity
						case "GENERAL":
							coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.General);
							break;
						case "FINE":
							coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.Fine);
							break;
						case "SUPERIOR":
							coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.Superior);
							break;
						case "EXCEPTIONAL":
							coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.Exceptional);
							break;
						case "MASTERWORK":
							coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.Masterwork);
							break;
						case "MYTHIC":
							coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.Mythic);
							break;
					}

					artifactDescriptionRuns.Add(coloredRun);

					// Remove opening tag, tagged part and closing tag from description.
					description = description.Remove(0, indexOfTagClosingStart);
					int indexOfTagClosingEnd = description.IndexOf(TagEnd);
					description = description.Remove(0, indexOfTagClosingEnd + 1);
				}
			}

			return artifactDescriptionRuns;
		}
	}
}