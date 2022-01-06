using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Models;
using static ClickQuest.Game.UserInterface.Helpers.ToolTips.GeneralToolTipHelper;

namespace ClickQuest.Game.UserInterface.Helpers.ToolTips;

public static class HeroStatsToolTipHelper
{
	public static ToolTip GenerateSpecializationToolTip(SpecializationType typeOfGeneratedSpecialization, int nextUpgrade)
	{
		var fontSizeToolTipName = (double)Application.Current.FindResource("FontSizeToolTipName");
		var fontFamilyRegularItalic = (FontFamily)Application.Current.FindResource("FontRegularItalic");
		var fontFamilyRegularBlackItalic = (FontFamily)Application.Current.FindResource("FontRegularBlackItalic");

		var specToolTip = new ToolTip();

		var toolTipBlock = new TextBlock
		{
			Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase")
		};

		switch (typeOfGeneratedSpecialization)
		{
			case SpecializationType.Blessing:
			{
				toolTipBlock.Inlines.Add(new Run("Prayer")
				{
					FontSize = fontSizeToolTipName
				});

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("Increases blessing duration by "));
				toolTipBlock.Inlines.Add(new Run($"{User.Instance.CurrentHero?.Specializations.SpecializationBuffs[SpecializationType.Blessing]}")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				toolTipBlock.Inlines.Add(new Run(" seconds"));

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("You can master Prayer by buying blessings in Priest")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});

				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run($"Next upgrade (+{Specializations.SpecBlessingBuffBonus}s duration) in")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				toolTipBlock.Inlines.Add(new Run($" {nextUpgrade} ")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				toolTipBlock.Inlines.Add(new Run("bought blessings")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
			}
				break;

			case SpecializationType.Clicking:
			{
				toolTipBlock.Inlines.Add(new Run("Clicker")
				{
					FontSize = fontSizeToolTipName
				});

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("Your clicks deal a bonus "));
				toolTipBlock.Inlines.Add(new Run($"{User.Instance.CurrentHero?.Specializations.SpecializationBuffs[SpecializationType.Clicking]}")
				{
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushDamageTypeOnHit"),
					FontFamily = fontFamilyRegularBlackItalic
				});
				toolTipBlock.Inlines.Add(new Run(" on-hit damage")
				{
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushDamageTypeOnHit")
				});

				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("On-hit damage from this specialization is applied after other effects eg. crit")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray5")
				});

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("You can master Clicker by clicking on monsters and bosses")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run($"Next upgrade (+{Specializations.SpecClickingBuffBonus} on-hit damage) in")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				toolTipBlock.Inlines.Add(new Run($" {nextUpgrade} ")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				toolTipBlock.Inlines.Add(new Run("clicks")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
			}
				break;

			case SpecializationType.Crafting:
			{
				toolTipBlock.Inlines.Add(new Run("Craftsman")
				{
					FontSize = fontSizeToolTipName
				});

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("Increases crafting rarity limit (current limit is "));
				toolTipBlock.Inlines.Add(new Run($"{User.Instance.CurrentHero?.Specializations.SpecCraftingText}")
				{
					FontFamily = fontFamilyRegularBlackItalic,
					Foreground = ColorsHelper.GetRarityColor((Rarity)User.Instance.CurrentHero?.Specializations.SpecializationBuffs[SpecializationType.Crafting])
				});
				toolTipBlock.Inlines.Add(new Run(" rarity)"));

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("You can master Craftsman by crafting artifacts in Blacksmith")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});

				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("Next rarity limit upgrade in")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				toolTipBlock.Inlines.Add(new Run($" {nextUpgrade} ")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				toolTipBlock.Inlines.Add(new Run("crafted artifacts")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
			}
				break;

			case SpecializationType.Trading:
			{
				toolTipBlock.Inlines.Add(new Run("Tradesman")
				{
					FontSize = fontSizeToolTipName
				});

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("Increases shop offer size (current size is "));
				toolTipBlock.Inlines.Add(new Run($"{User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Trading]}")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				toolTipBlock.Inlines.Add(new Run("),"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run("and materials selling ratio (current bonus ratio is "));
				toolTipBlock.Inlines.Add(new Run($"{User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Trading]}")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				toolTipBlock.Inlines.Add(new Run("%)"));

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("You can master Tradesman by buying recipes or selling materials in shop")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run($"Next upgrade (+{Specializations.SpecTradingBuffBonus} shop offer size and +{Specializations.SpecTradingBuffBonus}% materials selling ratio) in")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				toolTipBlock.Inlines.Add(new Run($" {nextUpgrade} ")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				toolTipBlock.Inlines.Add(new Run("bought recipes or sold materials")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
			}
				break;

			case SpecializationType.Melting:
			{
				toolTipBlock.Inlines.Add(new Run("Melter")
				{
					FontSize = fontSizeToolTipName
				});

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("Increases % chance to get a bonus "));
				toolTipBlock.Inlines.Add(new Run($"{Material.BaseMeltingIngotBonus}")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				toolTipBlock.Inlines.Add(new Run(" ingots when melting materials"));
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run($"Each 100% guarantees additional {Material.BaseMeltingIngotBonus} ingots")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray5")
				});

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("You can master Melter by melting materials in Blacksmith")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run($"Next upgrade (+{Specializations.SpecMeltingBuffBonus}% chance) in")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				toolTipBlock.Inlines.Add(new Run($" {nextUpgrade} ")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				toolTipBlock.Inlines.Add(new Run("melted materials")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
			}
				break;

			case SpecializationType.Questing:
			{
				toolTipBlock.Inlines.Add(new Run("Adventurer")
				{
					FontSize = fontSizeToolTipName
				});

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("Reduces % time to complete quests (current bonus is "));
				toolTipBlock.Inlines.Add(new Run($"{User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Questing]}")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				toolTipBlock.Inlines.Add(new Run("%)"));
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run($"Maximum reduction value is {Specializations.SpecQuestingBuffLimit}%")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray5")
				});

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("You can master Adventurer by completing quests")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"Next upgrade (+{Specializations.SpecQuestingBuffBonus}% reduced time) in")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				toolTipBlock.Inlines.Add(new Run($" {nextUpgrade} ")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				toolTipBlock.Inlines.Add(new Run("completed quests")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
			}
				break;

			case SpecializationType.Dungeon:
			{
				toolTipBlock.Inlines.Add(new Run("Daredevil")
				{
					FontSize = fontSizeToolTipName
				});

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("Increases the amount of time to defeat bosses (current bonus time is "));
				toolTipBlock.Inlines.Add(new Run($"{User.Instance.CurrentHero?.Specializations.SpecializationBuffs[SpecializationType.Dungeon]}")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				toolTipBlock.Inlines.Add(new Run("s)"));

				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(GenerateTextSeparator());
				toolTipBlock.Inlines.Add(new LineBreak());

				toolTipBlock.Inlines.Add(new Run("You can master Daredevil by trading recipes in shop")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"Next upgrade (+{Specializations.SpecDungeonBuffBonus} second) in")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				toolTipBlock.Inlines.Add(new Run($" {nextUpgrade} ")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				toolTipBlock.Inlines.Add(new Run("finished dungeons")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
			}
				break;
		}

		specToolTip.Content = toolTipBlock;

		return specToolTip;
	}

	public static ToolTip GenerateHeroInfoToolTip(HeroRace heroRace, HeroClass heroClass)
	{
		var fontSizeToolTipname = (double)Application.Current.FindResource("FontSizeToolTipName");
		var fontFamilyRegularItalic = (FontFamily)Application.Current.FindResource("FontRegularItalic");
		var fontFamilyRegularBlackItalic = (FontFamily)Application.Current.FindResource("FontRegularBlackItalic");

		var toolTip = new ToolTip();

		var block = new TextBlock
		{
			Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase")
		};

		// Race + Class (eg. Human Slayer)
		block.Inlines.Add(new Run($"{User.Instance.CurrentHero.ThisHeroRace} ")
		{
			FontSize = fontSizeToolTipname
		});
		block.Inlines.Add(new Run($"{User.Instance.CurrentHero.ThisHeroClass}")
		{
			FontSize = fontSizeToolTipname,
			Foreground = ColorsHelper.GetHeroClassColor(User.Instance.CurrentHero.HeroClass)
		});

		block.Inlines.Add(new LineBreak());
		block.Inlines.Add(GenerateTextSeparator());
		block.Inlines.Add(new LineBreak());

		switch (heroRace)
		{
			case HeroRace.Human:
				block.Inlines.Add(new Run("Human race specializes in trading and crafting"));
				block.Inlines.Add(new LineBreak());
				block.Inlines.Add(new Run("This means that human progresses these specializations two times faster than other races")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray5")
				});

				break;

			case HeroRace.Elf:
				block.Inlines.Add(new Run("Elf race specializes in questing and blessings"));
				block.Inlines.Add(new LineBreak());
				block.Inlines.Add(new Run("This means that elf progresses these specializations two times faster than other races")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray5")
				});

				break;

			case HeroRace.Dwarf:
				block.Inlines.Add(new Run("Dwarf race specializes in melting and fighting bosses"));
				block.Inlines.Add(new LineBreak());
				block.Inlines.Add(new Run("This means that dwarf progresses these specializations two times faster than other races")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray5")
				});

				break;
		}

		block.Inlines.Add(new LineBreak());
		block.Inlines.Add(new LineBreak());

		switch (heroClass)
		{
			case HeroClass.Slayer:
				block.Inlines.Add(new Run("Slayer class specializes in powerful critical clicks that deal increased damage"));
				break;

			case HeroClass.Venom:
				block.Inlines.Add(new Run("Venom class specializes in poisonous clicks that deal additional damage over time"));
				break;
		}

		block.Inlines.Add(new LineBreak());
		block.Inlines.Add(GenerateTextSeparator());
		block.Inlines.Add(new LineBreak());

		switch (heroRace)
		{
			case HeroRace.Human:
				block.Inlines.Add(new Run("Tradesman specialization threshold: ")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				block.Inlines.Add(new Run("5")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				block.Inlines.Add(new Run(" (instead of 10)")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});

				block.Inlines.Add(new LineBreak());

				block.Inlines.Add(new Run("Craftsman specialization threshold: ")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				block.Inlines.Add(new Run("5")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				block.Inlines.Add(new Run(" (instead of 10)")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});

				break;

			case HeroRace.Elf:
				block.Inlines.Add(new Run("Adventurer specialization threshold: ")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				block.Inlines.Add(new Run("5")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				block.Inlines.Add(new Run(" (instead of 10)")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});

				block.Inlines.Add(new LineBreak());

				block.Inlines.Add(new Run("Prayer specialization threshold: ")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				block.Inlines.Add(new Run("5")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				block.Inlines.Add(new Run(" (instead of 10)")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});

				break;

			case HeroRace.Dwarf:
				block.Inlines.Add(new Run("Melter specialization threshold: ")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				block.Inlines.Add(new Run("5")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				block.Inlines.Add(new Run(" (instead of 10)")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});

				block.Inlines.Add(new LineBreak());

				block.Inlines.Add(new Run("Daredevil specialization threshold: ")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});
				block.Inlines.Add(new Run("5")
				{
					FontFamily = fontFamilyRegularBlackItalic
				});
				block.Inlines.Add(new Run(" (instead of 10)")
				{
					FontFamily = fontFamilyRegularItalic,
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushGray3")
				});

				break;
		}

		block.Inlines.Add(new LineBreak());
		block.Inlines.Add(new LineBreak());

		block.Inlines.Add(new Run("Click damage")
		{
			FontFamily = (FontFamily)Application.Current.FindResource("FontRegularDemiBold"),
			Foreground = (SolidColorBrush)Application.Current.FindResource("BrushDamageTypeNormal")
		});
		block.Inlines.Add(new Run(": "));
		block.Inlines.Add(new Run("2")
		{
			FontFamily = (FontFamily)Application.Current.FindResource("FontRegularBold"),
			Foreground = (SolidColorBrush)Application.Current.FindResource("BrushDamageTypeNormal")
		});
		block.Inlines.Add(new Run(" (+"));
		block.Inlines.Add(new Run("1")
		{
			FontFamily = (FontFamily)Application.Current.FindResource("FontRegularBold"),
			Foreground = (SolidColorBrush)Application.Current.FindResource("BrushDamageTypeNormal")
		});
		block.Inlines.Add(new Run("/lvl)"));
		block.Inlines.Add(new LineBreak());

		switch (heroClass)
		{
			case HeroClass.Slayer:
				block.Inlines.Add(new Run("Crit chance")
				{
					FontFamily = (FontFamily)Application.Current.FindResource("FontRegularDemiBold"),
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushDamageTypeCritical")
				});
				block.Inlines.Add(new Run(": "));
				block.Inlines.Add(new Run("25%")
				{
					FontFamily = (FontFamily)Application.Current.FindResource("FontRegularBold"),
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushDamageTypeCritical")
				});
				block.Inlines.Add(new Run("(+"));
				block.Inlines.Add(new Run("0.4%")
				{
					FontFamily = (FontFamily)Application.Current.FindResource("FontRegularBold"),
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushDamageTypeCritical")
				});
				block.Inlines.Add(new Run("/lvl)"));
				break;

			case HeroClass.Venom:
				block.Inlines.Add(new Run("Poison damage")
				{
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushDamageTypePoison"),
					FontFamily = (FontFamily)Application.Current.FindResource("FontRegularDemiBold")
				});
				block.Inlines.Add(new Run(": "));
				block.Inlines.Add(new Run("1")
				{
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushDamageTypePoison"),
					FontFamily = (FontFamily)Application.Current.FindResource("FontRegularBold")
				});
				block.Inlines.Add(new Run(" (+"));
				block.Inlines.Add(new Run("2")
				{
					Foreground = (SolidColorBrush)Application.Current.FindResource("BrushDamageTypePoison"),
					FontFamily = (FontFamily)Application.Current.FindResource("FontRegularBold")
				});
				block.Inlines.Add(new Run("/lvl) per tick (5 ticks over 2.5s)"));
				break;
		}

		toolTip.Content = block;

		return toolTip;
	}
}