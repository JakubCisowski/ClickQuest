using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ClickQuest.Game.Core.Heroes;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Extensions.Combat;

namespace ClickQuest.Game.Extensions.UserInterface.ToolTips
{
	public static class HeroStatsToolTipController
	{
		public static ToolTip GenerateSpecizaltionToolTip(SpecializationType typeOfGeneratedSpecialization, int nextUpgrade)
		{
			// Generate SpecTrading ToolTips.
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
	}
}
