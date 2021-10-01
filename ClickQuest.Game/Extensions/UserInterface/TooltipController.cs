using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Items;
using Colors = ClickQuest.Game.Extensions.UserInterface.ColorsController;

namespace ClickQuest.Game.Extensions.UserInterface
{
	public static class TooltipController
	{
		public static ToolTip GenerateEquipmentItemTooltip<T>(T itemToGenerateTooltipFor) where T : Item
		{
			var toolTip = new ToolTip();

			var toolTipBlock = new TextBlock
			{
				Style = (Style) Application.Current.FindResource("ToolTipTextBlockBase")
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
						FontWeight = FontWeights.DemiBold
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
						FontWeight = FontWeights.DemiBold
					});
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"{artifact.Description}"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"{artifact.Lore}"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"{artifact.ExtraInfo}"));
					
					if (artifact.Rarity == Rarity.Mythic)
					{
						toolTipBlock.Inlines.Add(new LineBreak());
						toolTipBlock.Inlines.Add(new Run($"{artifact.MythicTag}"));
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
						FontWeight = FontWeights.DemiBold
					});
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"{recipe.Description}"));
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
				Style = (Style) Application.Current.FindResource("ToolTipTextBlockBase"),
				FontWeight = FontWeights.DemiBold
			};

			var currencyColor = Colors.GetRarityColor((Rarity) rarityValue);
			currencyTooltipTextBlock.Foreground = currencyColor;

			currencyTooltipTextBlock.Text = (Rarity) rarityValue + " " + (typeof(T) == typeof(Ingot) ? "Ingots" : "Dungeon Keys");

			currencyTooltip.Content = currencyTooltipTextBlock;

			return currencyTooltip;
		}

		public static ToolTip GenerateSpecizaltionTooltip(SpecializationType typeOfGeneratedSpecialization, int nextUpgrade)
		{
			// Generate SpecBuying tooltips.
			var specToolTip = new ToolTip();

			var toolTipBlock = new TextBlock
			{
				Style = (Style) Application.Current.FindResource("ToolTipTextBlockBase")
			};

			switch (typeOfGeneratedSpecialization)
			{
				case SpecializationType.Blessing:
				{
					toolTipBlock.Inlines.Add(new Run("Increases blessing duration in seconds"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run("You can master Prayer by buying blessings in Priest"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run("Next upgrade (+15s duration) in"));
					toolTipBlock.Inlines.Add(new Bold(new Run($" {nextUpgrade} ")));
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
					toolTipBlock.Inlines.Add(new Run("Next upgrade (+1 click damage) in"));
					toolTipBlock.Inlines.Add(new Bold(new Run($" {nextUpgrade} ")));
					toolTipBlock.Inlines.Add(new Run("clicks"));
				}
					break;

				case SpecializationType.Crafting:
				{
					toolTipBlock.Inlines.Add(new Run("Increases crafting rarity limit (base limit is Fine rarity)"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run("You can master Craftsman by crafting artifacts in Blacksmith"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run("Next rarity limit upgrade in"));
					toolTipBlock.Inlines.Add(new Bold(new Run($" {nextUpgrade} ")));
					toolTipBlock.Inlines.Add(new Run("crafted artifacts"));
				}
					break;

				case SpecializationType.Buying:
				{
					toolTipBlock.Inlines.Add(new Run("Increases shop offer size (base size is 5)"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run("You can master Tradesman by buying recipes in shop"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run("Next upgrade (+1 shop offer size) in"));
					toolTipBlock.Inlines.Add(new Bold(new Run($" {nextUpgrade} ")));
					toolTipBlock.Inlines.Add(new Run("bought recipes"));
				}
					break;

				case SpecializationType.Melting:
				{
					toolTipBlock.Inlines.Add(new Run("Increases % chance to get bonus ingots when melting (base chance is 0%)"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run("You can master Melter by melting materials in Blacksmith"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run("Each 100% guarantees additional ingot"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run("Next upgrade (+5% chance) in"));
					toolTipBlock.Inlines.Add(new Bold(new Run($" {nextUpgrade} ")));
					toolTipBlock.Inlines.Add(new Run("melted materials"));
				}
					break;

				case SpecializationType.Questing:
				{
					toolTipBlock.Inlines.Add(new Run("Reduces % time to complete quests (limit is 50%)"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run("You can master Adventurer by completing quests"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run("Next upgrade (+5% reduced time) in"));
					toolTipBlock.Inlines.Add(new Bold(new Run($" {nextUpgrade} ")));
					toolTipBlock.Inlines.Add(new Run("completed quests"));
				}
					break;

				case SpecializationType.Dungeon:
				{
					toolTipBlock.Inlines.Add(new Run("Increases amount of time to defeat boss in seconds (base time is 30s)"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run("You can master Daredevil by buying recipes in shop"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run("Next upgrade (+1 second) in"));
					toolTipBlock.Inlines.Add(new Bold(new Run($" {nextUpgrade} ")));
					toolTipBlock.Inlines.Add(new Run("finished dungeons"));
				}
					break;
			}

			specToolTip.Content = toolTipBlock;

			return specToolTip;
		}

		public static void SetTooltipDelayAndDuration(DependencyObject control)
		{
			ToolTipService.SetInitialShowDelay(control, 100);
			ToolTipService.SetShowDuration(control, 20000);
		}
	}
}