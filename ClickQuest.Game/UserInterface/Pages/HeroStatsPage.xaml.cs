using ClickQuest.Game.Core.Adventures;
using ClickQuest.Game.Core.Heroes;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.UserInterface;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace ClickQuest.Game.UserInterface.Pages
{
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

			UpdateGoldInterface();
			UpdateIngotOrDungeonKeyInterface(User.Instance.Ingots);
			UpdateIngotOrDungeonKeyInterface(User.Instance.DungeonKeys);
			UpdateQuestTimer();
			UpdateBlessingTimer();
			UpdateSpecializationsInterface();
			GenerateHeroInfoTooltip();
			GenerateStatValueAuraTooltip();
			GenerateStatValueDamageTooltip();
			GenerateStatValuePoisonTooltip();
			GenerateStatValueCritTooltip();
		}

		private void UpdateGoldInterface()
		{
			var goldAmountBinding = new Binding("Gold")
			{
				Source = User.Instance,
				StringFormat = "{0}"
			};

			GoldBlock.SetBinding(TextBlock.TextProperty, goldAmountBinding);

			GoldPanel.ToolTip = GenerateGoldTooltip();
		}

		private ToolTip GenerateGoldTooltip()
		{
			var goldInfoTooltip = new ToolTip();

			TooltipController.SetTooltipDelayAndDuration(GoldPanel);

			var tooltipTextBlock = new TextBlock
			{
				Style = (Style)FindResource("ToolTipTextBlockBase"),
				Text = "Gold"
			};

			goldInfoTooltip.Content = tooltipTextBlock;

			return goldInfoTooltip;
		}

		private void UpdateIngotOrDungeonKeyInterface<T>(List<T> currencyList) where T : Item
		{
			for (int currencyRarityValue = 0; currencyRarityValue < currencyList.Count; currencyRarityValue++)
			{
				var currencyPanel = new StackPanel
				{
					Orientation = Orientation.Horizontal,
					Margin = new Thickness(50, 0, 0, 0),
					Background = new SolidColorBrush(Colors.Transparent)
				};

				TooltipController.SetTooltipDelayAndDuration(currencyPanel);

				currencyPanel.Children.Add(GenerateCurrencyIcon<T>(currencyRarityValue));

				var currencyAmountTextBlock = new TextBlock
				{
					Name = (typeof(T) == typeof(Ingot) ? "Ingot" : "DungeonKey") + currencyRarityValue,
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

				int column = typeof(T) == typeof(Ingot) ? 0 : 1;
				Grid.SetColumn(currencyPanel, column);
				Grid.SetRow(currencyPanel, currencyRarityValue);

				currencyPanel.ToolTip = TooltipController.GenerateCurrencyTooltip<T>(currencyRarityValue);
			}
		}

		private PackIcon GenerateCurrencyIcon<T>(int rarityValue) where T : Item
		{
			var currencyIcon = new PackIcon
			{
				Kind = typeof(T) == typeof(Ingot) ? PackIconKind.Gold : PackIconKind.Key,
				Width = 22,
				Height = 22,
				VerticalAlignment = VerticalAlignment.Center
			};

			var curerncyIconColor = ColorsController.GetRarityColor((Rarity)rarityValue);
			currencyIcon.Foreground = curerncyIconColor;

			return currencyIcon;
		}

		private void UpdateQuestTimer()
		{
			var currentQuest = User.Instance.CurrentHero?.Quests.FirstOrDefault(x => x.EndDate != default);

			var questDurationBinding = new Binding("TicksCountText")
			{
				Source = currentQuest
			};
			QuestDurationBlock.SetBinding(TextBlock.TextProperty, questDurationBinding);

			QuestDurationBlock.ToolTip = GenerateQuestTooltip(currentQuest);
		}

		private ToolTip GenerateQuestTooltip(Quest currentQuest)
		{
			var questTooltip = new ToolTip();
			TooltipController.SetTooltipDelayAndDuration(QuestDurationBlock);

			var questToolTipTextBlock = new TextBlock
			{
				Style = (Style)FindResource("ToolTipTextBlockBase")
			};

			if (currentQuest != null)
			{
				questToolTipTextBlock.Inlines.Add(new Run($"{currentQuest.Name}"));
				if (currentQuest.Rare)
				{
					questToolTipTextBlock.Inlines.Add(new LineBreak());
					questToolTipTextBlock.Inlines.Add(new Run("*Rare Quest*")
					{
						Foreground = (SolidColorBrush)FindResource("BrushQuestRare"),
						FontFamily = (FontFamily)this.FindResource("FontRegularBold")
					});
				}

				questToolTipTextBlock.Inlines.Add(new LineBreak());
				questToolTipTextBlock.Inlines.Add(new Run($"Class: {currentQuest.HeroClass}") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
				questToolTipTextBlock.Inlines.Add(new LineBreak());
				questToolTipTextBlock.Inlines.Add(new LineBreak());
				questToolTipTextBlock.Inlines.Add(new Run($"{currentQuest.Description}"));
				questToolTipTextBlock.Inlines.Add(new LineBreak());
				questToolTipTextBlock.Inlines.Add(new LineBreak());
				questToolTipTextBlock.Inlines.Add(new Run($"{currentQuest.RewardsDescription}"));
			}
			else
			{
				questToolTipTextBlock.Text = "No quest is currently active";
			}

			questTooltip.Content = questToolTipTextBlock;

			return questTooltip;
		}

		private void UpdateBlessingTimer()
		{
			var currentBlessing = User.Instance.CurrentHero?.Blessing;

			var binding = new Binding("DurationText")
			{
				Source = currentBlessing
			};
			BlessingDurationBlock.SetBinding(TextBlock.TextProperty, binding);

			BlessingDurationBlock.ToolTip = GenerateBlessingTooltip(currentBlessing);
		}

		private ToolTip GenerateBlessingTooltip(Blessing currentBlessing)
		{
			var blessingToolTip = new ToolTip();
			TooltipController.SetTooltipDelayAndDuration(BlessingDurationBlock);

			var blessingToolTipBlock = new TextBlock
			{
				Style = (Style)FindResource("ToolTipTextBlockBase")
			};

			if (currentBlessing != null)
			{
				blessingToolTipBlock.Inlines.Add(new Run($"{currentBlessing.Name}"));
				blessingToolTipBlock.Inlines.Add(new LineBreak());
				blessingToolTipBlock.Inlines.Add(new Run($"*{currentBlessing.RarityString}*")
				{
					Foreground = ColorsController.GetRarityColor(currentBlessing.Rarity),
					FontFamily = (FontFamily)this.FindResource("FontRegularDemiBold")
				});
				blessingToolTipBlock.Inlines.Add(new LineBreak());
				blessingToolTipBlock.Inlines.Add(new LineBreak());
				blessingToolTipBlock.Inlines.Add(new Run($"{currentBlessing.Description}"));
				blessingToolTipBlock.Inlines.Add(new LineBreak());
				blessingToolTipBlock.Inlines.Add(new LineBreak());
				blessingToolTipBlock.Inlines.Add(new Run($"{currentBlessing.Lore}"));
			}
			else
			{
				blessingToolTipBlock.Text = "No blessing is currently active";
			}

			blessingToolTip.Content = blessingToolTipBlock;

			return blessingToolTip;
		}

		private void GenerateSpecializationsTooltips()
		{
			if (User.Instance.CurrentHero is null)
			{
				// This function was called before selecting a hero - return.
				return;
			}

			var specializationTypes = Enum.GetValues(typeof(SpecializationType));

			TooltipController.SetTooltipDelayAndDuration(SpecializationsGrid);

			for (int i = 0; i < specializationTypes.Length; i++)
			{
				int nextUpgrade = User.Instance.CurrentHero.Specialization.SpecializationAmounts[(SpecializationType)specializationTypes.GetValue(i)];
				while (nextUpgrade >= User.Instance.CurrentHero.Specialization.SpecializationThresholds[(SpecializationType)specializationTypes.GetValue(i)])
				{
					nextUpgrade -= User.Instance.CurrentHero.Specialization.SpecializationThresholds[(SpecializationType)specializationTypes.GetValue(i)];
				}

				nextUpgrade = User.Instance.CurrentHero.Specialization.SpecializationThresholds[(SpecializationType)specializationTypes.GetValue(i)] - nextUpgrade;

				var toolTip = TooltipController.GenerateSpecizaltionTooltip((SpecializationType)specializationTypes.GetValue(i), nextUpgrade);

				var nameBlock = (TextBlock)LogicalTreeHelper.FindLogicalNode(this, "Spec" + specializationTypes.GetValue(i) + "Name");
				var buffBlock = (TextBlock)LogicalTreeHelper.FindLogicalNode(this, "Spec" + specializationTypes.GetValue(i) + "Buff");

				nameBlock.ToolTip = toolTip;
				buffBlock.ToolTip = toolTip;
			}
		}

		public void UpdateSpecializationsInterface()
		{
			if (User.Instance.CurrentHero is null)
			{
				// This function was called before selecting a hero - return.
				return;
			}

			var specializationTypes = Enum.GetValues(typeof(SpecializationType));

			for (int i = 0; i < specializationTypes.Length; i++)
			{
				string nameText = "";
				string buffText = "";

				switch (specializationTypes.GetValue(i))
				{
					case SpecializationType.Blessing:
						nameText = "Prayer";
						buffText = " → Blessing duration +" + User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Blessing] + "s";
						break;

					case SpecializationType.Clicking:
						nameText = "Clicker";
						buffText = " → Click damage +" + User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Clicking];
						break;

					case SpecializationType.Crafting:
						nameText = "Craftsman";
						buffText = " → Can craft " + User.Instance.CurrentHero.Specialization.SpecCraftingText + " recipes";
						break;

					case SpecializationType.Trading:
						nameText = "Tradesman";
						buffText = " → Shop offer size +" + User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Trading];
						break;

					case SpecializationType.Melting:
						nameText = "Melter";
						buffText = " → Extra ingots +" + User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Melting] + "%";
						break;

					case SpecializationType.Questing:
						nameText = "Adventurer";
						buffText = " → Quest time -" + User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Questing] + "%";
						break;

					case SpecializationType.Dungeon:
						nameText = "Daredevil";
						buffText = " → Bossfight timer +" + User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Dungeon] + "s";
						break;
				}

				var nameBlock = new TextBlock
				{
					Name = "Spec" + specializationTypes.GetValue(i) + "Name",
					Text = nameText,
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				var block = new TextBlock
				{
					Name = "Spec" + specializationTypes.GetValue(i) + "Buff",
					Text = buffText,
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				Grid.SetRow(nameBlock, i);
				Grid.SetRow(block, i);
				Grid.SetColumn(nameBlock, 0);
				Grid.SetColumn(block, 1);

				SpecializationsGrid.Children.Add(nameBlock);
				SpecializationsGrid.Children.Add(block);
			}

			GenerateSpecializationsTooltips();
		}

		private void GenerateHeroInfoTooltip()
		{
			var tooltip = new ToolTip();

			TooltipController.SetTooltipDelayAndDuration(HeroNameBlock);

			var block = new TextBlock
			{
				Style = (Style)FindResource("ToolTipTextBlockBase")
			};

			switch (User.Instance.CurrentHero?.HeroClass)
			{
				case HeroClass.Slayer:
					block.Inlines.Add(new Run("This class specializes in powerful critical clicks that deal double damage"));
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new Run("Click damage: 2 (+1/lvl)"));
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new Run("Crit chance: 25% (+0.4%/lvl)"));
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new LineBreak());
					break;

				case HeroClass.Venom:
					block.Inlines.Add(new Run("This class specializes in poisonous clicks that deal additional damage over time"));
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new Run("Click damage: 2 (+1/lvl)"));
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new Run("Poison damage: 1 (+2/lvl) per tick (5 ticks over 2.5s)"));
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new LineBreak());
					break;
			}

			switch (User.Instance.CurrentHero?.HeroRace)
			{
				case HeroRace.Human:
					block.Inlines.Add(new Run("Human race specializes in trading and crafting"));
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new Run("Tradesman specialization threshold: 5 (instead of 10)"));
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new Run("Craftsman specialization threshold: 5 (instead of 10)"));
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new Run("This means that human progresses these specializations two times faster than other races"));
					break;

				case HeroRace.Elf:
					block.Inlines.Add(new Run("Elf race specializes in questing and blessings"));
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new Run("Adventurer specialization threshold: 5 (instead of 10)"));
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new Run("Prayer specialization threshold: 5 (instead of 10)"));
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new Run("This means that elf progresses these specializations two times faster than other races"));
					break;

				case HeroRace.Dwarf:
					block.Inlines.Add(new Run("Dwarf race specializes in melting and fighting bosses"));
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new Run("Melter specialization threshold: 5 (instead of 10)"));
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new Run("Daredevil specialization threshold: 5 (instead of 10)"));
					block.Inlines.Add(new LineBreak());
					block.Inlines.Add(new Run("This means that dwarf progresses these specializations two times faster than other races"));
					break;
			}

			tooltip.Content = block;
			HeroNameBlock.ToolTip = tooltip;
		}

		private void GenerateStatValueDamageTooltip()
		{
			var tooltipDamage = new ToolTip();

			TooltipController.SetTooltipDelayAndDuration(ClickDamageBlock);

			var blockDamage = new TextBlock
			{
				Style = (Style)FindResource("ToolTipTextBlockBase")
			};

			// ["You deal X damage per click"]
			var bindingDamageTotal = new Binding("ClickDamage")
			{
				Source = User.Instance.CurrentHero
			};
			var runDamageTotal = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
			runDamageTotal.SetBinding(Run.TextProperty, bindingDamageTotal);
			blockDamage.Inlines.Add("You deal ");
			blockDamage.Inlines.Add(runDamageTotal);
			blockDamage.Inlines.Add(" damage per click");

			blockDamage.Inlines.Add(new LineBreak());
			blockDamage.Inlines.Add(new LineBreak());

			// ["Click damage: X (base) + X (X/lvl) = X"]
			var bindingDamagePerLevel = new Binding("ClickDamagePerLevel")
			{
				Source = User.Instance.CurrentHero
			};
			var runDamagePerLevel = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularItalic") };
			runDamagePerLevel.SetBinding(Run.TextProperty, bindingDamagePerLevel);
			var bindingLevelDamageBonus = new Binding("LevelDamageBonus")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runLevelDamageBonus = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
			runLevelDamageBonus.SetBinding(Run.TextProperty, bindingLevelDamageBonus);
			var bindingLevelDamageBonusTotal = new Binding("LevelDamageBonusTotal")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runLevelDamageBonusTotal = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
			runLevelDamageBonusTotal.SetBinding(Run.TextProperty, bindingLevelDamageBonusTotal);
			blockDamage.Inlines.Add("Click damage: ");
			blockDamage.Inlines.Add(new Run("2") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
			blockDamage.Inlines.Add(new Run(" (base)") { FontFamily = (FontFamily)this.FindResource("FontRegularItalic") });
			blockDamage.Inlines.Add(" + ");
			blockDamage.Inlines.Add(runLevelDamageBonus);
			blockDamage.Inlines.Add(new Run(" (") { FontFamily = (FontFamily)this.FindResource("FontRegularItalic") });
			blockDamage.Inlines.Add(runDamagePerLevel);
			blockDamage.Inlines.Add(new Run("/lvl)") { FontFamily = (FontFamily)this.FindResource("FontRegularItalic") });
			blockDamage.Inlines.Add(" = ");
			blockDamage.Inlines.Add(runLevelDamageBonusTotal);

			blockDamage.Inlines.Add(new LineBreak());

			int clickDamageFromBlessing = 0;

			// ["BlessingName, damage: X"]
			if (User.Instance.CurrentHero != null)
			{
				if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.ClickDamage)
				{
					clickDamageFromBlessing = User.Instance.CurrentHero.Blessing.Buff;

					var bindingBlessingName = new Binding("Name")
					{
						Source = User.Instance.CurrentHero.Blessing
					};
					var runBlessingName = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
					runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
					var bindingBlessingBuff = new Binding("Buff")
					{
						Source = User.Instance.CurrentHero.Blessing
					};
					var runBlessingBuff = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
					runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
					blockDamage.Inlines.Add(runBlessingName);
					blockDamage.Inlines.Add(new Run(", damage: "));
					blockDamage.Inlines.Add(runBlessingBuff);
					blockDamage.Inlines.Add(new LineBreak());
				}
			}

			// ["Artifacts, damage: X"]
			if (User.Instance.CurrentHero != null)
			{
				int clickDamageFromArtifacts = User.Instance.CurrentHero.ClickDamage - (2 + User.Instance.CurrentHero.ClickDamagePerLevel * User.Instance.CurrentHero.Level) - clickDamageFromBlessing;

				if (clickDamageFromArtifacts > 0)
				{
					blockDamage.Inlines.Add(new Run("Artifacts") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockDamage.Inlines.Add(new Run(", damage: "));
					blockDamage.Inlines.Add(new Run(clickDamageFromArtifacts.ToString()) { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockDamage.Inlines.Add(new LineBreak());
				}
			}

			if (User.Instance.CurrentHero?.Specialization.SpecializationBuffs[SpecializationType.Clicking] > 0)
			{
				blockDamage.Inlines.Add(new LineBreak());
				blockDamage.Inlines.Add(new Run("You also deal ") { FontFamily = (FontFamily)this.FindResource("FontRegularItalic") });
				blockDamage.Inlines.Add(new Run(User.Instance.CurrentHero?.Specialization.SpecializationBuffs[SpecializationType.Clicking].ToString()) { FontFamily = (FontFamily)this.FindResource("FontRegularBoldItalic") });
				blockDamage.Inlines.Add(new Run(" on-hit damage from Clicker Specialization, but it doesn't get multiplied upon critting") { FontFamily = (FontFamily)this.FindResource("FontRegularItalic") });
			}

			tooltipDamage.Content = blockDamage;
			ClickDamageBlock.ToolTip = tooltipDamage;
		}

		private void GenerateStatValueCritTooltip()
		{
			var toolTipCrit = new ToolTip();

			TooltipController.SetTooltipDelayAndDuration(CritChanceBlock);

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
			var runCritTotal = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
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
			var runCritDamageTotal = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
			runCritDamageTotal.SetBinding(Run.TextProperty, bindingCritDamageTotal);
			blockCrit.Inlines.Add("Your crits deal ");
			blockCrit.Inlines.Add(runCritDamageTotal);
			blockCrit.Inlines.Add(" damage");

			blockCrit.Inlines.Add(new LineBreak());

			// Section only for slayer since its the only class with built-in crit bonuses.
			// ["Crit chance: X (base) + X (X/lvl) = X"]
			var bindingLevelCritBonus = new Binding("LevelCritBonus")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runLevelCritBonus = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
			runLevelCritBonus.SetBinding(Run.TextProperty, bindingLevelCritBonus);
			var bindingLevelCritBonusTotal = new Binding("LevelCritBonusTotal")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runLevelCritBonusTotal = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
			runLevelCritBonusTotal.SetBinding(Run.TextProperty, bindingLevelCritBonusTotal);
			if (User.Instance.CurrentHero != null)
			{
				if (User.Instance.CurrentHero.HeroClass == HeroClass.Slayer)
				{
					blockCrit.Inlines.Add(new LineBreak());
					blockCrit.Inlines.Add("Crit chance: ");
					blockCrit.Inlines.Add(new Run("25% ") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockCrit.Inlines.Add(new Run("(base)") { FontFamily = (FontFamily)this.FindResource("FontRegularItalic") });
					blockCrit.Inlines.Add(new Run(" + "));
					blockCrit.Inlines.Add(runLevelCritBonus);
					blockCrit.Inlines.Add(new Run("%") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockCrit.Inlines.Add(new Run(" (0.4%/lvl)") { FontFamily = (FontFamily)this.FindResource("FontRegularItalic") });
					blockCrit.Inlines.Add(" = ");
					blockCrit.Inlines.Add(runLevelCritBonusTotal);
					blockCrit.Inlines.Add(new Run("%") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockCrit.Inlines.Add(new LineBreak());
				}
			}

			double critChanceFromBlessing = 0;
			double critDamageFromBlessing = 0;

			// ["BlessingName, damage: X"] - for crit chance
			if (User.Instance.CurrentHero != null)
			{
				if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.CritChance)
				{
					critChanceFromBlessing = User.Instance.CurrentHero.Blessing.Buff * 0.01;

					var bindingBlessingName = new Binding("Name")
					{
						Source = User.Instance.CurrentHero.Blessing
					};
					var runBlessingName = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
					runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
					var bindingBlessingBuff = new Binding("Buff")
					{
						Source = User.Instance.CurrentHero.Blessing
					};
					var runBlessingBuff = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
					runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
					blockCrit.Inlines.Add(runBlessingName);
					blockCrit.Inlines.Add(new Run(", crit chance: "));
					blockCrit.Inlines.Add(runBlessingBuff);
					blockCrit.Inlines.Add(new Run("%") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
				}
			}

			// ["BlessingName, damage: X"] - for crit damage
			if (User.Instance.CurrentHero != null)
			{
				if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.CritDamage)
				{
					critDamageFromBlessing = User.Instance.CurrentHero.Blessing.Buff * 0.01;

					var bindingBlessingName = new Binding("Name")
					{
						Source = User.Instance.CurrentHero.Blessing
					};
					var runBlessingName = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
					runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
					var bindingBlessingBuff = new Binding("Buff")
					{
						Source = User.Instance.CurrentHero.Blessing
					};
					var runBlessingBuff = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
					runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
					blockCrit.Inlines.Add(runBlessingName);
					blockCrit.Inlines.Add(new Run(", crit damage: "));
					blockCrit.Inlines.Add(runBlessingBuff);
					blockCrit.Inlines.Add(new Run("%") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockCrit.Inlines.Add(new LineBreak());
				}
			}

			// ["Artifacts, damage: X"] - for crit chance
			if (User.Instance.CurrentHero != null)
			{
				double critChanceFromArtifacts = User.Instance.CurrentHero.CritChance - (User.Instance.CurrentHero.HeroClass == HeroClass.Slayer ? 0.25 : 0 + User.Instance.CurrentHero.CritChancePerLevel * User.Instance.CurrentHero.Level) - critChanceFromBlessing;

				if (critChanceFromArtifacts > 0)
				{
					blockCrit.Inlines.Add(new Run("Artifacts") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockCrit.Inlines.Add(new Run(", crit chance: "));
					blockCrit.Inlines.Add(new Run(Math.Round(critChanceFromArtifacts * 100, 2).ToString()) { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockCrit.Inlines.Add(new Run("%") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockCrit.Inlines.Add(new LineBreak());
				}
			}

			// ["Artifacts, damage: X"] - for crit damage
			if (User.Instance.CurrentHero != null)
			{
				double critDamageFromArtifacts = User.Instance.CurrentHero.CritDamage - 2 - critDamageFromBlessing;

				if (critDamageFromArtifacts > 0)
				{
					blockCrit.Inlines.Add(new Run("Artifacts") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockCrit.Inlines.Add(new Run(", crit damage: "));
					blockCrit.Inlines.Add(new Run(Math.Round(critDamageFromArtifacts * 100, 2).ToString()) { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockCrit.Inlines.Add(new Run("%") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockCrit.Inlines.Add(new LineBreak());
				}
			}

			toolTipCrit.Content = blockCrit;
			CritChanceBlock.ToolTip = toolTipCrit;
		}

		private void GenerateStatValuePoisonTooltip()
		{
			var toolTipPoison = new ToolTip();

			TooltipController.SetTooltipDelayAndDuration(PoisonDamageBlock);

			var blockPoison = new TextBlock
			{
				Style = (Style)FindResource("ToolTipTextBlockBase")
			};

			// ["You deal X bonus poison damage per tick"]
			var bindingPoisonTotal = new Binding("PoisonDamage")
			{
				Source = User.Instance.CurrentHero
			};
			var runPoisonTotal = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
			runPoisonTotal.SetBinding(Run.TextProperty, bindingPoisonTotal);
			blockPoison.Inlines.Add("You deal ");
			blockPoison.Inlines.Add(runPoisonTotal);
			blockPoison.Inlines.Add(" bonus poison damage per tick");

			blockPoison.Inlines.Add(new LineBreak());

			// Section only for Venom since its the only class with built-in poison bonuses.
			// ["Poison damage: X (base) + X (X/lvl) = X"]
			var bindingLevelPoisonBonus = new Binding("LevelPoisonBonus")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runLevelPoisonBonus = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
			runLevelPoisonBonus.SetBinding(Run.TextProperty, bindingLevelPoisonBonus);
			var bindingLevelPoisonBonusTotal = new Binding("LevelPoisonBonusTotal")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runLevelPoisonBonusTotal = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
			runLevelPoisonBonusTotal.SetBinding(Run.TextProperty, bindingLevelPoisonBonusTotal);
			if (User.Instance.CurrentHero != null)
			{
				if (User.Instance.CurrentHero.HeroClass == HeroClass.Venom)
				{
					blockPoison.Inlines.Add(new LineBreak());
					blockPoison.Inlines.Add("Poison damage: ");
					blockPoison.Inlines.Add((new Run("1 ") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") }));
					blockPoison.Inlines.Add(new Run("(base)") { FontFamily = (FontFamily)this.FindResource("FontRegularItalic") });
					blockPoison.Inlines.Add(new Run(" + "));
					blockPoison.Inlines.Add(runLevelPoisonBonus);
					blockPoison.Inlines.Add(new Run(" (2/lvl)") { FontFamily = (FontFamily)this.FindResource("FontRegularItalic") });
					blockPoison.Inlines.Add(" = ");
					blockPoison.Inlines.Add(runLevelPoisonBonusTotal);
					blockPoison.Inlines.Add(new LineBreak());
				}
			}

			int poisonDamageFromBlessing = 0;

			// ["BlessingName, damage: X"]
			if (User.Instance.CurrentHero != null)
			{
				if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.PoisonDamage)
				{
					poisonDamageFromBlessing = User.Instance.CurrentHero.Blessing.Buff;

					var bindingBlessingName = new Binding("Name")
					{
						Source = User.Instance.CurrentHero.Blessing
					};
					var runBlessingName = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
					runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
					var bindingBlessingBuff = new Binding("Buff")
					{
						Source = User.Instance.CurrentHero.Blessing
					};
					var runBlessingBuff = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
					runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
					blockPoison.Inlines.Add(runBlessingName);
					blockPoison.Inlines.Add(new Run(", poison damage: "));
					blockPoison.Inlines.Add(runBlessingBuff);
					blockPoison.Inlines.Add(new LineBreak());
				}
			}

			// ["Artifacts, damage: X"] - for crit chance
			if (User.Instance.CurrentHero != null)
			{
				double poisonDamageFromArtifacts = User.Instance.CurrentHero.PoisonDamage - (User.Instance.CurrentHero.HeroClass == HeroClass.Venom ? 1 : 0 + User.Instance.CurrentHero.PoisonDamagePerLevel * User.Instance.CurrentHero.Level) - poisonDamageFromBlessing;

				if (poisonDamageFromArtifacts > 0)
				{
					blockPoison.Inlines.Add(new LineBreak());
					blockPoison.Inlines.Add(new Run("Artifacts") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockPoison.Inlines.Add(new Run(", poison damage: "));
					blockPoison.Inlines.Add(new Run(poisonDamageFromArtifacts.ToString()) { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
				}
			}

			toolTipPoison.Content = blockPoison;
			PoisonDamageBlock.ToolTip = toolTipPoison;
		}

		private void GenerateStatValueAuraTooltip()
		{
			var tooltipAura = new ToolTip();

			TooltipController.SetTooltipDelayAndDuration(AuraDamageBlock);

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
			var runAuraDpsTotal = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
			runAuraDpsTotal.SetBinding(Run.TextProperty, bindingAuraDpsTotal);
			blockAura.Inlines.Add("You deal ");
			blockAura.Inlines.Add(runAuraDpsTotal);
			blockAura.Inlines.Add(" of monster's hp Aura damage per second");
			blockAura.Inlines.Add(new LineBreak());

			// ["Your Aura tick damage is X%; Your Aura tick speed is X/s"]
			var bindingAuraDamageTotal = new Binding("AuraDamageText")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runAuraDamageTotal = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
			runAuraDamageTotal.SetBinding(Run.TextProperty, bindingAuraDamageTotal);
			var bindingAuraAttackSpeedTotal = new Binding("AuraAttackSpeed")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runAuraAttackSpeedTotal = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
			runAuraAttackSpeedTotal.SetBinding(Run.TextProperty, bindingAuraAttackSpeedTotal);
			blockAura.Inlines.Add("Your Aura tick damage is ");
			blockAura.Inlines.Add(runAuraDamageTotal);
			blockAura.Inlines.Add(new LineBreak());
			blockAura.Inlines.Add("Your Aura tick speed is ");
			blockAura.Inlines.Add(runAuraAttackSpeedTotal);
			blockAura.Inlines.Add((new Run("/s") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") }));
			blockAura.Inlines.Add(new LineBreak());
			blockAura.Inlines.Add(new LineBreak());

			// ["Aura tick damage: X% (base)"]
			blockAura.Inlines.Add("Aura tick damage: ");
			blockAura.Inlines.Add(new Run("10%") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
			blockAura.Inlines.Add(new Run(" (base)") { FontFamily = (FontFamily)this.FindResource("FontRegularItalic") });

			blockAura.Inlines.Add(new LineBreak());

			// ["Aura tick speed: X/s (base) + X/s (X/s/lvl) = X/s"]
			var bindingAuraSpeedLevelBonus = new Binding("LevelAuraBonus")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runAuraSpeedLevelBonus = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
			runAuraSpeedLevelBonus.SetBinding(Run.TextProperty, bindingAuraSpeedLevelBonus);
			var bindingAuraSpeedLevelBonusTotal = new Binding("LevelAuraBonusTotal")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runAuraSpeedLevelBonusTotal = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
			runAuraSpeedLevelBonusTotal.SetBinding(Run.TextProperty, bindingAuraSpeedLevelBonusTotal);
			blockAura.Inlines.Add("Aura tick speed: ");
			blockAura.Inlines.Add((new Run($"{Hero.AURA_SPEED_BASE}/s") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") }));
			blockAura.Inlines.Add(new Run(" (base)") { FontFamily = (FontFamily)this.FindResource("FontRegularItalic") });
			blockAura.Inlines.Add(" + ");
			blockAura.Inlines.Add(runAuraSpeedLevelBonus);
			blockAura.Inlines.Add(new Run("/s") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
			blockAura.Inlines.Add(new Run($" ({Hero.AURA_SPEED_PER_LEVEL}/s/lvl)") { FontFamily = (FontFamily)this.FindResource("FontRegularItalic") });
			blockAura.Inlines.Add(" = ");
			blockAura.Inlines.Add(runAuraSpeedLevelBonusTotal);
			blockAura.Inlines.Add(new Run("/s") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
			blockAura.Inlines.Add(new LineBreak());

			double auraDamageFromBlessing = 0;

			// ["BlessingName, tick damage: X%"]
			if (User.Instance.CurrentHero != null)
			{
				if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.AuraDamage)
				{
					auraDamageFromBlessing = User.Instance.CurrentHero.Blessing.Buff * 0.01;

					var bindingBlessingName = new Binding("Name")
					{
						Source = User.Instance.CurrentHero.Blessing
					};
					var runBlessingName = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
					runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
					var bindingBlessingBuff = new Binding("Buff")
					{
						Source = User.Instance.CurrentHero.Blessing
					};
					var runBlessingBuff = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
					runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
					blockAura.Inlines.Add(runBlessingName);
					blockAura.Inlines.Add(new Run(", Aura tick damage: "));
					blockAura.Inlines.Add(runBlessingBuff);
					blockAura.Inlines.Add(new Run("%"));
				}
			}

			blockAura.Inlines.Add(new LineBreak());

			double auraSpeedFromBlessing = 0;

			// ["BlessingName, tick speed: X/s"]
			if (User.Instance.CurrentHero != null)
			{
				if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.AuraSpeed)
				{
					auraSpeedFromBlessing = User.Instance.CurrentHero.Blessing.Buff * 0.01;

					var bindingBlessingName = new Binding("Name")
					{
						Source = User.Instance.CurrentHero.Blessing
					};
					var runBlessingName = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
					runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
					var bindingBlessingBuff = new Binding("Buff")
					{
						Source = User.Instance.CurrentHero.Blessing
					};
					var runBlessingBuff = new Run() { FontFamily = (FontFamily)this.FindResource("FontRegularBold") };
					runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
					blockAura.Inlines.Add(runBlessingName);
					blockAura.Inlines.Add(new Run(", Aura tick speed: "));
					blockAura.Inlines.Add(runBlessingBuff);
					blockAura.Inlines.Add(new Run("/s"));
				}
			}

			// ["Artifacts, tick damage: X%"]
			if (User.Instance.CurrentHero != null)
			{
				double auraDamageFromArtifacts = User.Instance.CurrentHero.AuraDamage - 0.1 - auraDamageFromBlessing;

				if (auraDamageFromArtifacts > 0)
				{
					blockAura.Inlines.Add(new LineBreak());
					blockAura.Inlines.Add(new Run("Artifacts") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockAura.Inlines.Add(new Run(", aura damage: "));
					blockAura.Inlines.Add(new Run(Math.Round(auraDamageFromArtifacts * 100, 2).ToString()) { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockAura.Inlines.Add(new Run("%"));
					blockAura.Inlines.Add(new LineBreak());
				}
			}

			// ["Artifacts, tick speed: X%"]
			if (User.Instance.CurrentHero != null)
			{
				double auraSpeedFromArtifacts = User.Instance.CurrentHero.AuraAttackSpeed - (Hero.AURA_SPEED_BASE + User.Instance.CurrentHero.LevelAuraBonus) - auraSpeedFromBlessing;

				if (auraSpeedFromArtifacts > 0)
				{
					blockAura.Inlines.Add(new LineBreak());
					blockAura.Inlines.Add(new Run("Artifacts") { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockAura.Inlines.Add(new Run(", aura speed: "));
					blockAura.Inlines.Add(new Run(Math.Round(auraSpeedFromArtifacts, 2).ToString()) { FontFamily = (FontFamily)this.FindResource("FontRegularBold") });
					blockAura.Inlines.Add(new Run("/s"));
					blockAura.Inlines.Add(new LineBreak());
				}
			}

			tooltipAura.Content = blockAura;
			AuraDamageBlock.ToolTip = tooltipAura;
		}
	}
}