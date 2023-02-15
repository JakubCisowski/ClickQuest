using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models;
using ClickQuest.Game.UserInterface.Helpers;
using ClickQuest.Game.UserInterface.Helpers.ToolTips;
using MaterialDesignThemes.Wpf;
using static ClickQuest.Game.UserInterface.Helpers.ToolTips.GeneralToolTipHelper;

namespace ClickQuest.Game.UserInterface.Pages;

public partial class HeroStatsPage : Page
{
	private readonly Hero _hero;

	public HeroStatsPage()
	{
		InitializeComponent();

		_hero = User.Instance.CurrentHero;
		DataContext = _hero;

		if (_hero != null)
		{
			switch (_hero.HeroClass)
			{
				case HeroClass.Slayer:
					RunHeroClass.Foreground = (SolidColorBrush)FindResource("BrushSlayerRelated");
					break;

				case HeroClass.Venom:
					RunHeroClass.Foreground = (SolidColorBrush)FindResource("BrushVenomRelated");
					break;
			}
		}

		// Since the values of Ingots and DungeonKeys are binded, and their ToolTips are static, we only call their update functions here
		// (so only when refreshing HeroStatsPage - upon selecting a hero, creating one or loading the game).
		// We do not need to update them in any other situation.
		UpdateGoldInterface();
		UpdateIngotInterface(User.Instance.Ingots);
		UpdateDungeonKeyInterface(User.Instance.DungeonKeys);
		GenerateHeroInfoToolTip();

		RefreshAllDynamicStatsAndToolTips();

#if DEBUG
		HeroNameBlock.PreviewMouseUp += EnableMushroomMode;
#endif
	}

	public void RefreshAllDynamicStatsAndToolTips()
	{
		// Clear everything.
		ClearAllDynamicControls();

		UpdateQuestTimer();
		UpdateBlessingTimer();
		UpdateAllSpecializationsInterface();
		GenerateStatValueAuraToolTip();
		GenerateStatValueDamageToolTip();
		GenerateStatValuePoisonToolTip();
		GenerateStatValueCritToolTip();
	}

	public void ClearAllDynamicControls()
	{
		// We do not need to clear Gold, Ingots or DungeonKeys panels, because their values are binded and their ToolTips are static.
		SpecializationsGrid.Children.Clear();
	}

	private void UpdateGoldInterface()
	{
		var goldAmountBinding = new Binding("Gold")
		{
			Source = User.Instance,
			StringFormat = "{0}"
		};

		GoldBlock.SetBinding(TextBlock.TextProperty, goldAmountBinding);

		GoldPanel.ToolTip = GenerateGoldToolTip();
	}

	private ToolTip GenerateGoldToolTip()
	{
		var goldInfoToolTip = new ToolTip
		{
			Style = (Style)FindResource("ToolTipSimple")
		};

		var toolTipTextBlock = new TextBlock
		{
			Style = (Style)FindResource("ToolTipTextBlockBase"),
			Text = "Gold",
			FontFamily = (FontFamily)FindResource("FontRegularDemiBold"),
			Foreground = (SolidColorBrush)FindResource("BrushGold")
		};

		goldInfoToolTip.Content = toolTipTextBlock;

		return goldInfoToolTip;
	}

	private void UpdateIngotInterface(List<Ingot> currencyList)
	{
		for (var currencyRarityValue = 0; currencyRarityValue < currencyList.Count; currencyRarityValue++)
		{
			var currencyPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Margin = new Thickness(60, 0, 0, 0),
				Background = new SolidColorBrush(Colors.Transparent)
			};

			currencyPanel.Children.Add(GenerateCurrencyIcon<Ingot>(currencyRarityValue));

			var currencyAmountTextBlock = new TextBlock
			{
				Name = "Ingot" + currencyRarityValue,
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center
			};

			var currencyAmountBinding = new Binding("Quantity")
			{
				Source = currencyList[currencyRarityValue],
				StringFormat = "   {0}"
			};
			currencyAmountTextBlock.SetBinding(TextBlock.TextProperty, currencyAmountBinding);

			currencyPanel.Children.Add(currencyAmountTextBlock);
			IngotKeyGrid.Children.Add(currencyPanel);

			Grid.SetColumn(currencyPanel, 0);
			Grid.SetRow(currencyPanel, currencyRarityValue);

			currencyPanel.ToolTip = ItemToolTipHelper.GenerateCurrencyToolTip<Ingot>(currencyRarityValue);
		}
	}

	private void UpdateDungeonKeyInterface(List<DungeonKey> currencyList)
	{
		for (var currencyRarityValue = 0; currencyRarityValue < currencyList.Count; currencyRarityValue++)
		{
			var currencyPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Margin = new Thickness(0, 0, 60, 0),
				HorizontalAlignment = HorizontalAlignment.Right,
				Background = new SolidColorBrush(Colors.Transparent)
			};

			var currencyAmountTextBlock = new TextBlock
			{
				Name = "Ingot" + currencyRarityValue,
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center
			};

			var currencyAmountBinding = new Binding("Quantity")
			{
				Source = currencyList[currencyRarityValue],
				StringFormat = "{0}   "
			};
			currencyAmountTextBlock.SetBinding(TextBlock.TextProperty, currencyAmountBinding);

			currencyPanel.Children.Add(currencyAmountTextBlock);
			currencyPanel.Children.Add(GenerateCurrencyIcon<DungeonKey>(currencyRarityValue));

			IngotKeyGrid.Children.Add(currencyPanel);

			Grid.SetColumn(currencyPanel, 1);
			Grid.SetRow(currencyPanel, currencyRarityValue);

			currencyPanel.ToolTip = ItemToolTipHelper.GenerateCurrencyToolTip<DungeonKey>(currencyRarityValue);
		}
	}

	private static PackIcon GenerateCurrencyIcon<T>(int rarityValue) where T : Item
	{
		var currencyIcon = new PackIcon
		{
			Kind = typeof(T) == typeof(Ingot) ? PackIconKind.Gold : PackIconKind.Key,
			Width = 22,
			Height = 22,
			VerticalAlignment = VerticalAlignment.Center
		};

		var currencyIconColor = ColorsHelper.GetRarityColor((Rarity)rarityValue);
		currencyIcon.Foreground = currencyIconColor;

		return currencyIcon;
	}

	public void UpdateQuestTimer()
	{
		var currentQuest = User.Instance.CurrentHero?.Quests.FirstOrDefault(x => x.EndDate != default);

		var questDurationBinding = new Binding("TicksCountText")
		{
			Source = currentQuest
		};
		QuestDurationBlock.SetBinding(TextBlock.TextProperty, questDurationBinding);

		QuestDurationBlock.ToolTip = ItemToolTipHelper.GenerateQuestToolTip(currentQuest);
	}

	public void UpdateBlessingTimer()
	{
		var currentBlessing = User.Instance.CurrentHero?.Blessing;

		var binding = new Binding("DurationStatsPanelText")
		{
			Source = currentBlessing
		};
		BlessingDurationBlock.SetBinding(TextBlock.TextProperty, binding);

		BlessingDurationBlock.ToolTip = ItemToolTipHelper.GenerateBlessingToolTip(currentBlessing);
	}

	private void GenerateSingleSpecializationToolTip(SpecializationType specializationType)
	{
		var nextUpgrade = User.Instance.CurrentHero.Specializations.SpecializationAmounts[specializationType];
		while (nextUpgrade >= User.Instance.CurrentHero.Specializations.SpecializationThresholds[specializationType])
		{
			nextUpgrade -= User.Instance.CurrentHero.Specializations.SpecializationThresholds[specializationType];
		}

		nextUpgrade = User.Instance.CurrentHero.Specializations.SpecializationThresholds[specializationType] - nextUpgrade;

		var toolTip = HeroStatsToolTipHelper.GenerateSpecializationToolTip(specializationType, nextUpgrade);

		var buffBlock = (TextBlock)LogicalTreeHelper.FindLogicalNode(this, "Spec" + specializationType + "Buff");

		buffBlock.ToolTip = toolTip;
	}

	private void GenerateAllSpecializationsToolTips()
	{
		if (User.Instance.CurrentHero is null)
		{
			// This function was called before selecting a hero - return.
			return;
		}

		var specializationTypes = Enum.GetValues(typeof(SpecializationType));

		var usedSpecializationTypes = RemoveUnusedSpecializations(specializationTypes.OfType<SpecializationType>());

		foreach (var specializationType in usedSpecializationTypes)
		{
			GenerateSingleSpecializationToolTip(specializationType);
		}
	}

	public void UpdateSingleSpecializationInterface(SpecializationType specializationType)
	{
		var buffBlockName = "Spec" + specializationType + "Buff";

		var buffBlock = SpecializationsGrid.Children.Cast<TextBlock>().FirstOrDefault(textBlock => textBlock.Name == buffBlockName);

		// Find the existing control in SpecializationsGrid.

		if (buffBlock != null)
		{
			buffBlock.Inlines.Clear();

			var fontFamilyRegularBold = (FontFamily)FindResource("FontRegularBold");

			switch (specializationType)
			{
				case SpecializationType.Blessing:
					buffBlock.Inlines.Add(new Run("Blessing duration +"));
					buffBlock.Inlines.Add(new Run(User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Blessing] + "s")
					{
						FontFamily = fontFamilyRegularBold
					});
					break;

				case SpecializationType.Clicking:
					buffBlock.Inlines.Add(new Run("On-hit damage "));
					buffBlock.Inlines.Add(new Run("+" + User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Clicking])
					{
						Foreground = (SolidColorBrush)FindResource("BrushDamageTypeOnHit"),
						FontFamily = fontFamilyRegularBold
					});
					break;

				case SpecializationType.Crafting:
					buffBlock.Inlines.Add("Can craft ");
					buffBlock.Inlines.Add(new Run(User.Instance.CurrentHero.Specializations.SpecCraftingText)
					{
						Foreground = ColorsHelper.GetRarityColor((Rarity)User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Crafting]),
						FontFamily = fontFamilyRegularBold
					});
					buffBlock.Inlines.Add(" artifacts");
					break;

				case SpecializationType.Trading:
					buffBlock.Inlines.Add(new Run("Shop size & ratio "));
					buffBlock.Inlines.Add(new Run("+" + User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Trading])
					{
						FontFamily = fontFamilyRegularBold
					});
					break;

				case SpecializationType.Melting:
					buffBlock.Inlines.Add(new Run("Extra ingots "));
					buffBlock.Inlines.Add(new Run("+" + User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Melting] + "%")
					{
						FontFamily = fontFamilyRegularBold
					});
					break;

				case SpecializationType.Questing:
					buffBlock.Inlines.Add(new Run("Quest time "));
					buffBlock.Inlines.Add(new Run("-" + User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Questing] + "%")
					{
						FontFamily = fontFamilyRegularBold
					});
					break;

				case SpecializationType.Dungeon:
					buffBlock.Inlines.Add(new Run("Bossfight timer "));
					buffBlock.Inlines.Add(new Run("+" + User.Instance.CurrentHero.Specializations.SpecializationBuffs[SpecializationType.Dungeon] + "s")
					{
						FontFamily = fontFamilyRegularBold
					});
					break;
			}

			GenerateSingleSpecializationToolTip(specializationType);
		}
	}

	public void UpdateAllSpecializationsInterface()
	{
		if (User.Instance.CurrentHero is null)
		{
			// This function was called before selecting a hero - return.
			return;
		}

		var specializationTypes = Enum.GetValues(typeof(SpecializationType));

		User.Instance.CurrentHero.Specializations.UpdateSpecialization();

		var usedSpecializationTypes = RemoveUnusedSpecializations(specializationTypes.OfType<SpecializationType>());

		var i = 0;

		for (i = 0; i < usedSpecializationTypes.Count; i++)
		{
			var specializationType = usedSpecializationTypes[i];

			var block = new TextBlock
			{
				Name = "Spec" + specializationType + "Buff",
				FontSize = 18,
				Margin = new Thickness(0, 1, 0, 1),
				HorizontalAlignment = HorizontalAlignment.Center,
				TextAlignment = TextAlignment.Center
			};

			Grid.SetRow(block, i);

			SpecializationsGrid.Children.Add(block);

			UpdateSingleSpecializationInterface(specializationType);
		}

		for (; i < specializationTypes.Length; i++)
		{
			var block = new TextBlock
			{
				Name = "Spec" + i + "Buff",
				FontSize = 18,
				Margin = new Thickness(0, 1, 0, 1),
				HorizontalAlignment = HorizontalAlignment.Center,
				TextAlignment = TextAlignment.Center
			};

			Grid.SetRow(block, i);

			SpecializationsGrid.Children.Add(block);
		}

		GenerateAllSpecializationsToolTips();
	}

	public static List<SpecializationType> RemoveUnusedSpecializations(IEnumerable<SpecializationType> specializationTypes)
	{
		return specializationTypes.Where(specType => User.Instance.CurrentHero.Specializations.SpecializationBuffs[specType] != 0).ToList();
	}

	private void GenerateHeroInfoToolTip()
	{
		if (User.Instance.CurrentHero is null)
		{
			return;
		}

		var toolTip = HeroStatsToolTipHelper.GenerateHeroInfoToolTip(User.Instance.CurrentHero.HeroRace, User.Instance.CurrentHero.HeroClass);

		HeroNameBlock.ToolTip = toolTip;
	}

	public void GenerateStatValueDamageToolTip()
	{
		if (User.Instance.CurrentHero is null)
		{
			return;
		}

		var fontFamilyRegularBold = (FontFamily)FindResource("FontRegularBold");
		var fontFamilyRegularLightItalic = (FontFamily)FindResource("FontRegularLightItalic");

		var toolTipDamage = new ToolTip
		{
			BorderBrush = ColorsHelper.GetDamageTypeColor(DamageType.Normal),
			BorderThickness = new Thickness(1)
		};

		var blockDamage = new TextBlock
		{
			Style = (Style)FindResource("ToolTipTextBlockBase")
		};

		// ["You deal X damage per click"]
		var bindingDamageTotal = new Binding("ClickDamage")
		{
			Source = User.Instance.CurrentHero
		};
		var runDamageTotal = new Run
		{
			FontFamily = fontFamilyRegularBold
		};
		runDamageTotal.SetBinding(Run.TextProperty, bindingDamageTotal);
		blockDamage.Inlines.Add("You deal ");
		blockDamage.Inlines.Add(runDamageTotal);
		blockDamage.Inlines.Add(" damage per click");

		blockDamage.Inlines.Add(new LineBreak());
		blockDamage.Inlines.Add(GenerateTextSeparator());
		blockDamage.Inlines.Add(new LineBreak());

		// ["Click damage: X (base) + X (X/lvl) = X"]
		var bindingDamagePerLevel = new Binding("ClickDamagePerLevel")
		{
			Source = User.Instance.CurrentHero
		};
		var runDamagePerLevel = new Run
		{
			FontFamily = fontFamilyRegularLightItalic
		};
		runDamagePerLevel.SetBinding(Run.TextProperty, bindingDamagePerLevel);
		var bindingLevelDamageBonus = new Binding("LevelDamageBonus")
		{
			Source = User.Instance.CurrentHero,
			Mode = BindingMode.OneWay
		};
		var runLevelDamageBonus = new Run
		{
			FontFamily = fontFamilyRegularBold
		};
		runLevelDamageBonus.SetBinding(Run.TextProperty, bindingLevelDamageBonus);
		var bindingLevelDamageBonusTotal = new Binding("LevelDamageBonusTotal")
		{
			Source = User.Instance.CurrentHero,
			Mode = BindingMode.OneWay
		};
		var runLevelDamageBonusTotal = new Run
		{
			FontFamily = fontFamilyRegularBold
		};
		runLevelDamageBonusTotal.SetBinding(Run.TextProperty, bindingLevelDamageBonusTotal);
		blockDamage.Inlines.Add("Click damage: ");
		blockDamage.Inlines.Add(new Run("10")
		{
			FontFamily = fontFamilyRegularBold
		});
		blockDamage.Inlines.Add(new Run(" (base)")
		{
			FontFamily = fontFamilyRegularLightItalic
		});
		blockDamage.Inlines.Add(" + ");
		blockDamage.Inlines.Add(runLevelDamageBonus);
		blockDamage.Inlines.Add(new Run(" (")
		{
			FontFamily = fontFamilyRegularLightItalic
		});
		blockDamage.Inlines.Add(runDamagePerLevel);
		blockDamage.Inlines.Add(new Run("/lvl)")
		{
			FontFamily = fontFamilyRegularLightItalic
		});
		blockDamage.Inlines.Add(" = ");
		blockDamage.Inlines.Add(runLevelDamageBonusTotal);

		var clickDamageFromBlessing = 0;

		// ["BlessingName, damage: X"]
		if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.ClickDamage)
		{
			clickDamageFromBlessing = User.Instance.CurrentHero.Blessing.Buff;

			var bindingBlessingName = new Binding("Name")
			{
				Source = User.Instance.CurrentHero.Blessing
			};
			var runBlessingName = new Run
			{
				FontFamily = fontFamilyRegularBold
			};
			runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
			var bindingBlessingBuff = new Binding("Buff")
			{
				Source = User.Instance.CurrentHero.Blessing
			};
			var runBlessingBuff = new Run
			{
				FontFamily = fontFamilyRegularBold
			};
			runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
			blockDamage.Inlines.Add(new LineBreak());
			blockDamage.Inlines.Add(runBlessingName);
			blockDamage.Inlines.Add(new Run(", damage: "));
			blockDamage.Inlines.Add(runBlessingBuff);
		}

		// ["Artifacts, damage: X"]
		var clickDamageFromArtifacts = User.Instance.CurrentHero.ClickDamage - (User.Instance.CurrentHero.ClickDamageBase + User.Instance.CurrentHero.ClickDamagePerLevel * User.Instance.CurrentHero.Level) - clickDamageFromBlessing;

		if (clickDamageFromArtifacts > 0.009 || clickDamageFromArtifacts < -0.009)
		{
			blockDamage.Inlines.Add(new LineBreak());
			blockDamage.Inlines.Add(new Run("Artifacts")
			{
				FontFamily = fontFamilyRegularBold
			});
			blockDamage.Inlines.Add(new Run(", damage: "));
			blockDamage.Inlines.Add(new Run(clickDamageFromArtifacts.ToString())
			{
				FontFamily = fontFamilyRegularBold
			});
		}

		if (User.Instance.CurrentHero?.Specializations.SpecializationBuffs[SpecializationType.Clicking] > 0)
		{
			blockDamage.Inlines.Add(new LineBreak());
			blockDamage.Inlines.Add(new LineBreak());
			blockDamage.Inlines.Add(new Run("You also deal ")
			{
				FontFamily = (FontFamily)FindResource("FontRegularItalic")
			});
			blockDamage.Inlines.Add(new Run(User.Instance.CurrentHero?.Specializations.SpecializationBuffs[SpecializationType.Clicking].ToString())
			{
				FontFamily = (FontFamily)FindResource("FontRegularBoldItalic")
			});
			blockDamage.Inlines.Add(new Run(" on-hit damage from Clicker Specialization, but it doesn't get multiplied upon critting")
			{
				FontFamily = fontFamilyRegularLightItalic
			});
		}

		toolTipDamage.Content = blockDamage;
		ClickDamageBlock.ToolTip = toolTipDamage;
	}

	public void GenerateStatValueCritToolTip()
	{
		if (User.Instance.CurrentHero is null)
		{
			return;
		}

		var fontFamilyRegularBold = (FontFamily)FindResource("FontRegularBold");
		var fontFamilyRegularLightItalic = (FontFamily)FindResource("FontRegularLightItalic");

		var toolTipCrit = new ToolTip
		{
			BorderBrush = ColorsHelper.GetDamageTypeColor(DamageType.Critical),
			BorderThickness = new Thickness(1)
		};

		var blockCrit = new TextBlock
		{
			Style = (Style)FindResource("ToolTipTextBlockBase")
		};

		// ["You have X% chance to crit (deal double damage) when clicking"]
		var bindingCritTotal = new Binding("CritChanceText")
		{
			Source = User.Instance.CurrentHero,
			Mode = BindingMode.OneWay
		};
		var runCritTotal = new Run
		{
			FontFamily = fontFamilyRegularBold
		};
		runCritTotal.SetBinding(Run.TextProperty, bindingCritTotal);
		blockCrit.Inlines.Add("You have ");
		blockCrit.Inlines.Add(runCritTotal);
		blockCrit.Inlines.Add(" chance to crit (deal increased damage) when clicking");

		blockCrit.Inlines.Add(new LineBreak());

		// ["Your crits deal X% damage"]
		var bindingCritDamageTotal = new Binding("CritDamageText")
		{
			Source = User.Instance.CurrentHero,
			Mode = BindingMode.OneWay
		};
		var runCritDamageTotal = new Run
		{
			FontFamily = fontFamilyRegularBold
		};
		runCritDamageTotal.SetBinding(Run.TextProperty, bindingCritDamageTotal);
		blockCrit.Inlines.Add("Your crits deal ");
		blockCrit.Inlines.Add(runCritDamageTotal);
		blockCrit.Inlines.Add(" damage");

		var separatorInserted = false;

		// Section only for slayer since its the only class with built-in crit bonuses.
		// ["Crit chance: X (base) + X (X/lvl) = X"]
		var bindingLevelCritBonus = new Binding("LevelCritBonus")
		{
			Source = User.Instance.CurrentHero,
			Mode = BindingMode.OneWay
		};
		var runLevelCritBonus = new Run
		{
			FontFamily = fontFamilyRegularBold
		};
		runLevelCritBonus.SetBinding(Run.TextProperty, bindingLevelCritBonus);
		var bindingLevelCritBonusTotal = new Binding("LevelCritBonusTotal")
		{
			Source = User.Instance.CurrentHero,
			Mode = BindingMode.OneWay
		};
		var runLevelCritBonusTotal = new Run
		{
			FontFamily = fontFamilyRegularBold
		};
		runLevelCritBonusTotal.SetBinding(Run.TextProperty, bindingLevelCritBonusTotal);
		if (User.Instance.CurrentHero.HeroClass == HeroClass.Slayer)
		{
			if (!separatorInserted)
			{
				blockCrit.Inlines.Add(new LineBreak());
				blockCrit.Inlines.Add(GenerateTextSeparator());
				separatorInserted = true;
			}

			blockCrit.Inlines.Add(new LineBreak());
			blockCrit.Inlines.Add("Crit chance: ");
			blockCrit.Inlines.Add(new Run("25% ")
			{
				FontFamily = fontFamilyRegularBold
			});
			blockCrit.Inlines.Add(new Run("(base)")
			{
				FontFamily = fontFamilyRegularLightItalic
			});
			blockCrit.Inlines.Add(new Run(" + "));
			blockCrit.Inlines.Add(runLevelCritBonus);
			blockCrit.Inlines.Add(new Run("%")
			{
				FontFamily = fontFamilyRegularBold
			});
			blockCrit.Inlines.Add(new Run(" (0.4%/lvl)")
			{
				FontFamily = fontFamilyRegularLightItalic
			});
			blockCrit.Inlines.Add(" = ");
			blockCrit.Inlines.Add(runLevelCritBonusTotal);
			blockCrit.Inlines.Add(new Run("%")
			{
				FontFamily = fontFamilyRegularBold
			});
		}

		double critChanceFromBlessing = 0;
		double critDamageFromBlessing = 0;

		// ["BlessingName, damage: X"] - for crit chance
		if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.CritChance)
		{
			if (!separatorInserted)
			{
				blockCrit.Inlines.Add(new LineBreak());
				blockCrit.Inlines.Add(GenerateTextSeparator());
				separatorInserted = true;
			}

			critChanceFromBlessing = User.Instance.CurrentHero.Blessing.Buff * 0.01;

			var bindingBlessingName = new Binding("Name")
			{
				Source = User.Instance.CurrentHero.Blessing
			};
			var runBlessingName = new Run
			{
				FontFamily = fontFamilyRegularBold
			};
			runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
			var bindingBlessingBuff = new Binding("Buff")
			{
				Source = User.Instance.CurrentHero.Blessing
			};
			var runBlessingBuff = new Run
			{
				FontFamily = fontFamilyRegularBold
			};
			runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
			blockCrit.Inlines.Add(new LineBreak());
			blockCrit.Inlines.Add(runBlessingName);
			blockCrit.Inlines.Add(new Run(", crit chance: "));
			blockCrit.Inlines.Add(runBlessingBuff);
			blockCrit.Inlines.Add(new Run("%")
			{
				FontFamily = fontFamilyRegularBold
			});
		}

		// ["BlessingName, damage: X"] - for crit damage
		if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.CritDamage)
		{
			if (!separatorInserted)
			{
				blockCrit.Inlines.Add(new LineBreak());
				blockCrit.Inlines.Add(GenerateTextSeparator());
				separatorInserted = true;
			}

			critDamageFromBlessing = User.Instance.CurrentHero.Blessing.Buff * 0.01;

			var bindingBlessingName = new Binding("Name")
			{
				Source = User.Instance.CurrentHero.Blessing
			};
			var runBlessingName = new Run
			{
				FontFamily = fontFamilyRegularBold
			};
			runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
			var bindingBlessingBuff = new Binding("Buff")
			{
				Source = User.Instance.CurrentHero.Blessing
			};
			var runBlessingBuff = new Run
			{
				FontFamily = fontFamilyRegularBold
			};
			runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
			blockCrit.Inlines.Add(new LineBreak());
			blockCrit.Inlines.Add(runBlessingName);
			blockCrit.Inlines.Add(new Run(", crit damage: "));
			blockCrit.Inlines.Add(runBlessingBuff);
			blockCrit.Inlines.Add(new Run("%")
			{
				FontFamily = fontFamilyRegularBold
			});
		}

		// ["Artifacts, damage: X"] - for crit chance
		var critChanceFromArtifacts = User.Instance.CurrentHero.CritChance - ((User.Instance.CurrentHero.HeroClass == HeroClass.Slayer ? 0.25 : 0) + User.Instance.CurrentHero.CritChancePerLevel * User.Instance.CurrentHero.Level) - critChanceFromBlessing;

		if (critChanceFromArtifacts is > 0.009 or < -0.009)
		{
			if (!separatorInserted)
			{
				blockCrit.Inlines.Add(new LineBreak());
				blockCrit.Inlines.Add(GenerateTextSeparator());
				separatorInserted = true;
			}

			blockCrit.Inlines.Add(new LineBreak());
			blockCrit.Inlines.Add(new Run("Artifacts")
			{
				FontFamily = fontFamilyRegularBold
			});
			blockCrit.Inlines.Add(new Run(", crit chance: "));
			blockCrit.Inlines.Add(new Run(Math.Floor(critChanceFromArtifacts * 100).ToString())
			{
				FontFamily = fontFamilyRegularBold
			});
			blockCrit.Inlines.Add(new Run("%")
			{
				FontFamily = fontFamilyRegularBold
			});
		}

		// ["Artifacts, damage: X"] - for crit damage
		var critDamageFromArtifacts = User.Instance.CurrentHero.CritDamage - 2 - critDamageFromBlessing;

		if (critDamageFromArtifacts is > 0.009 or < -0.009)
		{
			if (!separatorInserted)
			{
				blockCrit.Inlines.Add(new LineBreak());
				blockCrit.Inlines.Add(GenerateTextSeparator());
				separatorInserted = true;
			}

			blockCrit.Inlines.Add(new LineBreak());
			blockCrit.Inlines.Add(new Run("Artifacts")
			{
				FontFamily = fontFamilyRegularBold
			});
			blockCrit.Inlines.Add(new Run(", crit damage: "));
			blockCrit.Inlines.Add(new Run(Math.Floor(critDamageFromArtifacts * 100).ToString())
			{
				FontFamily = fontFamilyRegularBold
			});
			blockCrit.Inlines.Add(new Run("%")
			{
				FontFamily = fontFamilyRegularBold
			});
		}

		toolTipCrit.Content = blockCrit;
		CritChanceBlock.ToolTip = toolTipCrit;
	}

	public void GenerateStatValuePoisonToolTip()
	{
		if (User.Instance.CurrentHero is null)
		{
			return;
		}

		var fontFamilyRegularBold = (FontFamily)FindResource("FontRegularBold");
		var fontFamilyRegularLightItalic = (FontFamily)FindResource("FontRegularLightItalic");

		var toolTipPoison = new ToolTip
		{
			BorderBrush = ColorsHelper.GetDamageTypeColor(DamageType.Poison),
			BorderThickness = new Thickness(1)
		};

		var blockPoison = new TextBlock
		{
			Style = (Style)FindResource("ToolTipTextBlockBase")
		};

		// ["You deal X bonus poison damage per tick"]
		var bindingPoisonTotal = new Binding("PoisonDamage")
		{
			Source = User.Instance.CurrentHero
		};
		var runPoisonTotal = new Run
		{
			FontFamily = fontFamilyRegularBold
		};
		runPoisonTotal.SetBinding(Run.TextProperty, bindingPoisonTotal);
		blockPoison.Inlines.Add("You deal ");
		blockPoison.Inlines.Add(runPoisonTotal);
		blockPoison.Inlines.Add(" bonus poison damage per tick");

		var separatorInserted = false;

		// Section only for Venom since its the only class with built-in poison bonuses.
		// ["Poison damage: X (base) + X (X/lvl) = X"]
		var bindingLevelPoisonBonus = new Binding("LevelPoisonBonus")
		{
			Source = User.Instance.CurrentHero,
			Mode = BindingMode.OneWay
		};
		var runLevelPoisonBonus = new Run
		{
			FontFamily = fontFamilyRegularBold
		};
		runLevelPoisonBonus.SetBinding(Run.TextProperty, bindingLevelPoisonBonus);
		var bindingLevelPoisonBonusTotal = new Binding("LevelPoisonBonusTotal")
		{
			Source = User.Instance.CurrentHero,
			Mode = BindingMode.OneWay
		};
		var runLevelPoisonBonusTotal = new Run
		{
			FontFamily = fontFamilyRegularBold
		};
		runLevelPoisonBonusTotal.SetBinding(Run.TextProperty, bindingLevelPoisonBonusTotal);
		if (User.Instance.CurrentHero.HeroClass == HeroClass.Venom)
		{
			if (!separatorInserted)
			{
				blockPoison.Inlines.Add(new LineBreak());
				blockPoison.Inlines.Add(GenerateTextSeparator());
				separatorInserted = true;
			}

			blockPoison.Inlines.Add(new LineBreak());
			blockPoison.Inlines.Add("Poison damage: ");
			blockPoison.Inlines.Add(new Run("1 ")
			{
				FontFamily = fontFamilyRegularBold
			});
			blockPoison.Inlines.Add(new Run("(base)")
			{
				FontFamily = fontFamilyRegularLightItalic
			});
			blockPoison.Inlines.Add(new Run(" + "));
			blockPoison.Inlines.Add(runLevelPoisonBonus);
			blockPoison.Inlines.Add(new Run(" (2/lvl)")
			{
				FontFamily = fontFamilyRegularLightItalic
			});
			blockPoison.Inlines.Add(" = ");
			blockPoison.Inlines.Add(runLevelPoisonBonusTotal);
		}

		var poisonDamageFromBlessing = 0;

		// ["BlessingName, damage: X"]
		if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.PoisonDamage)
		{
			if (!separatorInserted)
			{
				blockPoison.Inlines.Add(new LineBreak());
				blockPoison.Inlines.Add(GenerateTextSeparator());
				separatorInserted = true;
			}

			poisonDamageFromBlessing = User.Instance.CurrentHero.Blessing.Buff;

			var bindingBlessingName = new Binding("Name")
			{
				Source = User.Instance.CurrentHero.Blessing
			};
			var runBlessingName = new Run
			{
				FontFamily = fontFamilyRegularBold
			};
			runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
			var bindingBlessingBuff = new Binding("Buff")
			{
				Source = User.Instance.CurrentHero.Blessing
			};
			var runBlessingBuff = new Run
			{
				FontFamily = fontFamilyRegularBold
			};
			runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
			blockPoison.Inlines.Add(new LineBreak());
			blockPoison.Inlines.Add(runBlessingName);
			blockPoison.Inlines.Add(new Run(", poison damage: "));
			blockPoison.Inlines.Add(runBlessingBuff);
		}

		// ["Artifacts, damage: X"] - for poison damage
		double poisonDamageFromArtifacts = User.Instance.CurrentHero.PoisonDamage - ((User.Instance.CurrentHero.HeroClass == HeroClass.Venom ? 1 : 0) + User.Instance.CurrentHero.PoisonDamagePerLevel * User.Instance.CurrentHero.Level) - poisonDamageFromBlessing;

		if (poisonDamageFromArtifacts is > 0.009 or < -0.009)
		{
			if (!separatorInserted)
			{
				blockPoison.Inlines.Add(new LineBreak());
				blockPoison.Inlines.Add(GenerateTextSeparator());
				separatorInserted = true;
			}

			blockPoison.Inlines.Add(new LineBreak());
			blockPoison.Inlines.Add(new Run("Artifacts")
			{
				FontFamily = fontFamilyRegularBold
			});
			blockPoison.Inlines.Add(new Run(", poison damage: "));
			blockPoison.Inlines.Add(new Run(poisonDamageFromArtifacts.ToString())
			{
				FontFamily = fontFamilyRegularBold
			});
		}

		toolTipPoison.Content = blockPoison;
		PoisonDamageBlock.ToolTip = toolTipPoison;
	}

	public void GenerateStatValueAuraToolTip()
	{
		if (User.Instance.CurrentHero is null)
		{
			return;
		}

		var fontFamilyRegularBold = (FontFamily)FindResource("FontRegularBold");
		var fontFamilyRegularLightItalic = (FontFamily)FindResource("FontRegularLightItalic");

		var toolTipAura = new ToolTip
		{
			BorderBrush = ColorsHelper.GetDamageTypeColor(DamageType.Aura),
			BorderThickness = new Thickness(1)
		};

		var blockAura = new TextBlock
		{
			Style = (Style)FindResource("ToolTipTextBlockBase")
		};

		// ["You deal X% of monster's hp Aura damage per second"]
		var bindingAuraDpsTotal = new Binding("AuraDpsText")
		{
			Source = User.Instance.CurrentHero,
			Mode = BindingMode.OneWay
		};
		var runAuraDpsTotal = new Run
		{
			FontFamily = fontFamilyRegularBold
		};
		runAuraDpsTotal.SetBinding(Run.TextProperty, bindingAuraDpsTotal);
		blockAura.Inlines.Add("You deal ");
		blockAura.Inlines.Add(runAuraDpsTotal);
		blockAura.Inlines.Add(" of monster's hp Aura damage per second");
		blockAura.Inlines.Add(new LineBreak());

		// ["You kill a monster every X seconds"]
		var timeToKill = (int)Math.Ceiling(1 / (User.Instance.CurrentHero.AuraDamage * User.Instance.CurrentHero.AuraAttackSpeed));
		blockAura.Inlines.Add("You kill a monster every ");
		blockAura.Inlines.Add(new Run(timeToKill.ToString())
		{
			FontFamily = fontFamilyRegularBold
		});
		blockAura.Inlines.Add(timeToKill == 1 ? " second" : " seconds");
		blockAura.Inlines.Add(new LineBreak());

		// ["Your Aura tick damage is X%; Your Aura tick speed is X/s"]
		var bindingAuraDamageTotal = new Binding("AuraDamageText")
		{
			Source = User.Instance.CurrentHero,
			Mode = BindingMode.OneWay
		};
		var runAuraDamageTotal = new Run
		{
			FontFamily = fontFamilyRegularBold
		};
		runAuraDamageTotal.SetBinding(Run.TextProperty, bindingAuraDamageTotal);
		var bindingAuraAttackSpeedTotal = new Binding("AuraSpeedText")
		{
			Source = User.Instance.CurrentHero,
			Mode = BindingMode.OneWay
		};
		var runAuraAttackSpeedTotal = new Run
		{
			FontFamily = fontFamilyRegularBold
		};
		runAuraAttackSpeedTotal.SetBinding(Run.TextProperty, bindingAuraAttackSpeedTotal);
		blockAura.Inlines.Add("Your Aura tick damage is ");
		blockAura.Inlines.Add(runAuraDamageTotal);
		blockAura.Inlines.Add(new LineBreak());
		blockAura.Inlines.Add("Your Aura tick speed is ");
		blockAura.Inlines.Add(runAuraAttackSpeedTotal);
		blockAura.Inlines.Add(new Run("/s")
		{
			FontFamily = fontFamilyRegularBold
		});

		blockAura.Inlines.Add(new LineBreak());
		blockAura.Inlines.Add(GenerateTextSeparator());
		blockAura.Inlines.Add(new LineBreak());

		// ["Aura tick damage: X% (base)"]
		blockAura.Inlines.Add("Aura tick damage: ");
		blockAura.Inlines.Add(new Run($"{(Hero.AuraDamageBase * 100).ToString("#.#")}%")
		{
			FontFamily = fontFamilyRegularBold
		});
		blockAura.Inlines.Add(new Run(" (base)")
		{
			FontFamily = fontFamilyRegularLightItalic
		});

		blockAura.Inlines.Add(new LineBreak());

		// ["Aura tick speed: X/s (base) + X/s (X/s/lvl) = X/s"]
		var bindingAuraSpeedLevelBonus = new Binding("LevelAuraBonusText")
		{
			Source = User.Instance.CurrentHero,
			Mode = BindingMode.OneWay
		};
		var runAuraSpeedLevelBonus = new Run
		{
			FontFamily = fontFamilyRegularBold
		};
		runAuraSpeedLevelBonus.SetBinding(Run.TextProperty, bindingAuraSpeedLevelBonus);
		var bindingAuraSpeedLevelBonusTotal = new Binding("LevelAuraBonusTotalText")
		{
			Source = User.Instance.CurrentHero,
			Mode = BindingMode.OneWay
		};
		var runAuraSpeedLevelBonusTotal = new Run
		{
			FontFamily = fontFamilyRegularBold
		};
		runAuraSpeedLevelBonusTotal.SetBinding(Run.TextProperty, bindingAuraSpeedLevelBonusTotal);
		blockAura.Inlines.Add("Aura tick speed: ");
		blockAura.Inlines.Add(new Run($"{Hero.AuraSpeedBase}/s")
		{
			FontFamily = fontFamilyRegularBold
		});
		blockAura.Inlines.Add(new Run(" (base)")
		{
			FontFamily = fontFamilyRegularLightItalic
		});
		blockAura.Inlines.Add(" + ");
		blockAura.Inlines.Add(runAuraSpeedLevelBonus);
		blockAura.Inlines.Add(new Run("/s")
		{
			FontFamily = fontFamilyRegularBold
		});
		blockAura.Inlines.Add(new Run($" ({Hero.AuraSpeedPerLevel}/s/lvl)")
		{
			FontFamily = fontFamilyRegularLightItalic
		});
		blockAura.Inlines.Add(" = ");
		blockAura.Inlines.Add(runAuraSpeedLevelBonusTotal);
		blockAura.Inlines.Add(new Run("/s")
		{
			FontFamily = fontFamilyRegularBold
		});

		double auraDamageFromBlessing = 0;

		// ["BlessingName, tick damage: X%"]
		if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.AuraDamage)
		{
			auraDamageFromBlessing = User.Instance.CurrentHero.Blessing.Buff * 0.01;

			var bindingBlessingName = new Binding("Name")
			{
				Source = User.Instance.CurrentHero.Blessing
			};
			var runBlessingName = new Run
			{
				FontFamily = fontFamilyRegularBold
			};
			runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
			var bindingBlessingBuff = new Binding("Buff")
			{
				Source = User.Instance.CurrentHero.Blessing
			};
			var runBlessingBuff = new Run
			{
				FontFamily = fontFamilyRegularBold
			};
			runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
			blockAura.Inlines.Add(new LineBreak());
			blockAura.Inlines.Add(runBlessingName);
			blockAura.Inlines.Add(new Run(", Aura tick damage: "));
			blockAura.Inlines.Add(runBlessingBuff);
			blockAura.Inlines.Add(new Run("%"));
		}

		double auraSpeedFromBlessing = 0;

		// ["BlessingName, tick speed: X/s"]
		if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.AuraSpeed)
		{
			auraSpeedFromBlessing = User.Instance.CurrentHero.Blessing.Buff * 0.01;

			var bindingBlessingName = new Binding("Name")
			{
				Source = User.Instance.CurrentHero.Blessing
			};
			var runBlessingName = new Run
			{
				FontFamily = fontFamilyRegularBold
			};
			runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
			var bindingBlessingBuff = new Binding("Buff")
			{
				Source = User.Instance.CurrentHero.Blessing
			};
			var runBlessingBuff = new Run
			{
				FontFamily = fontFamilyRegularBold
			};
			runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
			blockAura.Inlines.Add(new LineBreak());
			blockAura.Inlines.Add(runBlessingName);
			blockAura.Inlines.Add(new Run(", Aura tick speed: "));
			blockAura.Inlines.Add(runBlessingBuff);
			blockAura.Inlines.Add(new Run("/s"));
		}

		// ["Artifacts, tick damage: X%"]
		var auraDamageFromArtifacts = User.Instance.CurrentHero.AuraDamage - Hero.AuraDamageBase - auraDamageFromBlessing;

		if (auraDamageFromArtifacts is > 0.009 or < -0.009)
		{
			blockAura.Inlines.Add(new LineBreak());
			blockAura.Inlines.Add(new Run("Artifacts")
			{
				FontFamily = fontFamilyRegularBold
			});
			blockAura.Inlines.Add(new Run(", aura damage: "));
			blockAura.Inlines.Add(new Run(Math.Floor(auraDamageFromArtifacts * 100).ToString())
			{
				FontFamily = fontFamilyRegularBold
			});
			blockAura.Inlines.Add(new Run("%"));
		}

		// ["Artifacts, tick speed: X%"]
		var auraSpeedFromArtifacts = User.Instance.CurrentHero.AuraAttackSpeed - (Hero.AuraSpeedBase + User.Instance.CurrentHero.LevelAuraBonus) - auraSpeedFromBlessing;

		if (auraSpeedFromArtifacts is > 0.009 or < -0.009)
		{
			blockAura.Inlines.Add(new LineBreak());
			blockAura.Inlines.Add(new Run("Artifacts")
			{
				FontFamily = fontFamilyRegularBold
			});
			blockAura.Inlines.Add(new Run(", aura speed: "));
			blockAura.Inlines.Add(new Run((Math.Floor(auraSpeedFromArtifacts * 10) / 10).ToString())
			{
				FontFamily = fontFamilyRegularBold
			});
			blockAura.Inlines.Add(new Run("/s"));
		}

		toolTipAura.Content = blockAura;
		AuraDamageBlock.ToolTip = toolTipAura;
	}

	#if DEBUG
	private static void EnableMushroomMode(object sender, EventArgs e)
	{
		foreach (var artifact in GameAssets.Artifacts)
		{
			artifact.CreateMythicTag("FunctionSeedingArtifacts");

			CollectionsHelper.AddItemToCollection(artifact, User.Instance.CurrentHero.Artifacts);
		}

		foreach (var key in User.Instance.DungeonKeys)
		{
			key.AddItem(100);
		}

		foreach (var ingot in User.Instance.Ingots)
		{
			ingot.AddItem(100);
		}

		foreach (var material in GameAssets.Materials)
		{
			CollectionsHelper.AddItemToCollection(material, User.Instance.CurrentHero.Materials, 100);
		}
		
		foreach (var recipe in GameAssets.Recipes)
		{
			CollectionsHelper.AddItemToCollection(recipe, User.Instance.CurrentHero.Recipes);
		}

		User.Instance.Gold += 100000;
	}
	#endif
}