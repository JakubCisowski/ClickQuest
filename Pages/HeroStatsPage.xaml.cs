using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using ClickQuest.Adventures;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Heroes;
using ClickQuest.Heroes.Buffs;
using ClickQuest.Items;
using ClickQuest.Player;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.Pages
{
	public partial class HeroStatsPage : Page
	{
		private readonly Hero _hero;

		public HeroStatsPage()
		{
			InitializeComponent();

			_hero = User.Instance.CurrentHero;
			DataContext = _hero;

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
				Style = (Style) FindResource("ToolTipTextBlockBase"),
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

			var curerncyIconColor = Styles.Colors.GetRarityColor((Rarity) rarityValue);
			currencyIcon.Foreground = curerncyIconColor;

			return currencyIcon;
		}

		private void UpdateQuestTimer()
		{
			var currentQuest = User.Instance.CurrentHero?.Quests.FirstOrDefault(x => x.EndDate != default);

			var questDurationBinding = new Binding("TicksCountText") {Source = currentQuest};
			QuestDurationBlock.SetBinding(TextBlock.TextProperty, questDurationBinding);

			QuestDurationBlock.ToolTip = GenerateQuestTooltip(currentQuest);
		}

		private ToolTip GenerateQuestTooltip(Quest currentQuest)
		{
			var questTooltip = new ToolTip();
			TooltipController.SetTooltipDelayAndDuration(QuestDurationBlock);

			var questToolTipTextBlock = new TextBlock {Style = (Style) FindResource("ToolTipTextBlockBase")};

			if (currentQuest != null)
			{
				questToolTipTextBlock.Inlines.Add(new Run($"{currentQuest.Name}"));
				if (currentQuest.Rare)
				{
					questToolTipTextBlock.Inlines.Add(new LineBreak());
					questToolTipTextBlock.Inlines.Add(new Bold(new Run("*Rare Quest*") {Foreground = (SolidColorBrush) FindResource("ColorQuestRare")}));
				}

				questToolTipTextBlock.Inlines.Add(new LineBreak());
				questToolTipTextBlock.Inlines.Add(new Run($"Class: {currentQuest.HeroClass}"));
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

			var binding = new Binding("DurationText") {Source = currentBlessing};
			BlessingDurationBlock.SetBinding(TextBlock.TextProperty, binding);

			BlessingDurationBlock.ToolTip = GenerateBlessingTooltip(currentBlessing);
		}

		private ToolTip GenerateBlessingTooltip(Blessing currentBlessing)
		{
			var blessingToolTip = new ToolTip();
			TooltipController.SetTooltipDelayAndDuration(BlessingDurationBlock);

			var blessingToolTipBlock = new TextBlock {Style = (Style) FindResource("ToolTipTextBlockBase")};

			if (currentBlessing != null)
			{
				blessingToolTipBlock.Inlines.Add(new Run($"{currentBlessing.Name}"));
				blessingToolTipBlock.Inlines.Add(new LineBreak());
				blessingToolTipBlock.Inlines.Add(new Run($"*{currentBlessing.RarityString}*")
				{
					Foreground = Styles.Colors.GetRarityColor(currentBlessing.Rarity),
					FontWeight = FontWeights.DemiBold
				});
				blessingToolTipBlock.Inlines.Add(new LineBreak());
				blessingToolTipBlock.Inlines.Add(new LineBreak());
				blessingToolTipBlock.Inlines.Add(new Run($"{currentBlessing.Description}"));
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
				int nextUpgrade = User.Instance.CurrentHero.Specialization.SpecializationAmounts[(SpecializationType) specializationTypes.GetValue(i)];
				while (nextUpgrade >= User.Instance.CurrentHero.Specialization.SpecializationThresholds[(SpecializationType) specializationTypes.GetValue(i)])
				{
					nextUpgrade -= User.Instance.CurrentHero.Specialization.SpecializationThresholds[(SpecializationType) specializationTypes.GetValue(i)];
				}

				nextUpgrade = User.Instance.CurrentHero.Specialization.SpecializationThresholds[(SpecializationType) specializationTypes.GetValue(i)] - nextUpgrade;

				var toolTip = TooltipController.GenerateSpecizaltionTooltip((SpecializationType) specializationTypes.GetValue(i), nextUpgrade);

				var nameBlock = (TextBlock) LogicalTreeHelper.FindLogicalNode(this, "Spec" + specializationTypes.GetValue(i) + "Name");
				var buffBlock = (TextBlock) LogicalTreeHelper.FindLogicalNode(this, "Spec" + specializationTypes.GetValue(i) + "Buff");

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

					case SpecializationType.Buying:
						nameText = "Tradesman";
						buffText = " → Shop offer size +" + User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Buying];
						break;

					case SpecializationType.Melting:
						nameText = "Melter";
						buffText = " → Extra ingot +" + User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Melting] + "%";
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

			var block = new TextBlock {Style = (Style) FindResource("ToolTipTextBlockBase")};

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
					block.Inlines.Add(new Run("Human race specializes in buying and crafting"));
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

			var blockDamage = new TextBlock {Style = (Style) FindResource("ToolTipTextBlockBase")};

			// ["You deal X damage per click"]
			var bindingDamageTotal = new Binding("ClickDamage") {Source = User.Instance.CurrentHero};
			var runDamageTotal = new Run();
			runDamageTotal.SetBinding(Run.TextProperty, bindingDamageTotal);
			blockDamage.Inlines.Add("You deal ");
			blockDamage.Inlines.Add(new Bold(runDamageTotal));
			blockDamage.Inlines.Add(" damage per click");

			blockDamage.Inlines.Add(new LineBreak());
			blockDamage.Inlines.Add(new LineBreak());

			// ["Click damage: X (base) + X (X/lvl) = X"]
			var bindingDamagePerLevel = new Binding("ClickDamagePerLevel") {Source = User.Instance.CurrentHero};
			var runDamagePerLevel = new Run();
			runDamagePerLevel.SetBinding(Run.TextProperty, bindingDamagePerLevel);
			var bindingLevelDamageBonus = new Binding("LevelDamageBonus")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runLevelDamageBonus = new Run();
			runLevelDamageBonus.SetBinding(Run.TextProperty, bindingLevelDamageBonus);
			var bindingLevelDamageBonusTotal = new Binding("LevelDamageBonusTotal")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runLevelDamageBonusTotal = new Run();
			runLevelDamageBonusTotal.SetBinding(Run.TextProperty, bindingLevelDamageBonusTotal);
			blockDamage.Inlines.Add("Click damage: ");
			blockDamage.Inlines.Add(new Bold(new Run("2")));
			blockDamage.Inlines.Add(new Italic(new Run(" (base)")));
			blockDamage.Inlines.Add(" + ");
			blockDamage.Inlines.Add(new Bold(runLevelDamageBonus));
			blockDamage.Inlines.Add(new Italic(new Run(" (")));
			blockDamage.Inlines.Add(new Italic(runDamagePerLevel));
			blockDamage.Inlines.Add(new Italic(new Run("/lvl)")));
			blockDamage.Inlines.Add(" = ");
			blockDamage.Inlines.Add(new Bold(runLevelDamageBonusTotal));

			blockDamage.Inlines.Add(new LineBreak());

			// ["BlessingName, damage: X"]
			if (User.Instance.CurrentHero != null)
			{
				if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.ClickDamage)
				{
					var bindingBlessingName = new Binding("Name") {Source = User.Instance.CurrentHero.Blessing};
					var runBlessingName = new Run();
					runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
					var bindingBlessingBuff = new Binding("Buff") {Source = User.Instance.CurrentHero.Blessing};
					var runBlessingBuff = new Run();
					runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
					blockDamage.Inlines.Add(new Bold(runBlessingName));
					blockDamage.Inlines.Add(new Run(", damage: "));
					blockDamage.Inlines.Add(new Bold(runBlessingBuff));
				}
			}

			var bindingSpecClicking = new Binding("SpecClickingBuff") {Source = User.Instance.CurrentHero?.Specialization};
			var runSpecClicking = new Run();
			runSpecClicking.SetBinding(Run.TextProperty, bindingSpecClicking);
			blockDamage.Inlines.Add(new LineBreak());
			blockDamage.Inlines.Add(new LineBreak());
			blockDamage.Inlines.Add(new Run("Damage from artifacts is not displayed here"));
			blockDamage.Inlines.Add(new LineBreak());
			blockDamage.Inlines.Add(new Italic(new Run("You also deal ")));
			blockDamage.Inlines.Add(new Italic(new Bold(runSpecClicking)));
			blockDamage.Inlines.Add(new Italic(new Run(" on-hit damage from Clicker Specialization, but it doesn't get multiplied upon critting")));

			tooltipDamage.Content = blockDamage;
			ClickDamageBlock.ToolTip = tooltipDamage;
		}

		private void GenerateStatValueCritTooltip()
		{
			var toolTipCrit = new ToolTip();

			TooltipController.SetTooltipDelayAndDuration(CritChanceBlock);

			var blockCrit = new TextBlock {Style = (Style) FindResource("ToolTipTextBlockBase")};

			// ["You have X% chance to crit (deal double damage) when clicking"]
			var bindingCritTotal = new Binding("CritChanceText")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runCritTotal = new Run();
			runCritTotal.SetBinding(Run.TextProperty, bindingCritTotal);
			blockCrit.Inlines.Add("You have ");
			blockCrit.Inlines.Add(new Bold(runCritTotal));
			blockCrit.Inlines.Add(" chance to crit (deal double damage) when clicking");

			blockCrit.Inlines.Add(new LineBreak());

			// Section only for slayer since its the only class with built-in crit bonuses.
			// ["Crit chance: X (base) + X (X/lvl) = X"]
			var bindingLevelCritBonus = new Binding("LevelCritBonus")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runLevelCritBonus = new Run();
			runLevelCritBonus.SetBinding(Run.TextProperty, bindingLevelCritBonus);
			var bindingLevelCritBonusTotal = new Binding("LevelCritBonusTotal")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runLevelCritBonusTotal = new Run();
			runLevelCritBonusTotal.SetBinding(Run.TextProperty, bindingLevelCritBonusTotal);
			if (User.Instance.CurrentHero != null)
			{
				if (User.Instance.CurrentHero.HeroClass == HeroClass.Slayer)
				{
					blockCrit.Inlines.Add(new LineBreak());
					blockCrit.Inlines.Add("Crit chance: ");
					blockCrit.Inlines.Add(new Bold(new Run("25% ")));
					blockCrit.Inlines.Add(new Italic(new Run("(base)")));
					blockCrit.Inlines.Add(new Run(" + "));
					blockCrit.Inlines.Add(new Bold(runLevelCritBonus));
					blockCrit.Inlines.Add(new Bold(new Run("%")));
					blockCrit.Inlines.Add(new Italic(new Run(" (0.4%/lvl)")));
					blockCrit.Inlines.Add(" = ");
					blockCrit.Inlines.Add(new Bold(runLevelCritBonusTotal));
					blockCrit.Inlines.Add(new Bold(new Run("%")));
					blockCrit.Inlines.Add(new LineBreak());
				}
			}

			// ["BlessingName, damage: X"]
			if (User.Instance.CurrentHero != null)
			{
				if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.CritChance)
				{
					var bindingBlessingName = new Binding("Name") {Source = User.Instance.CurrentHero.Blessing};
					var runBlessingName = new Run();
					runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
					var bindingBlessingBuff = new Binding("Buff") {Source = User.Instance.CurrentHero.Blessing};
					var runBlessingBuff = new Run();
					runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
					blockCrit.Inlines.Add(new Bold(runBlessingName));
					blockCrit.Inlines.Add(new Run(", crit chance: "));
					blockCrit.Inlines.Add(new Bold(runBlessingBuff));
					blockCrit.Inlines.Add(new Bold(new Run("%")));
				}
			}

			blockCrit.Inlines.Add(new LineBreak());
			blockCrit.Inlines.Add(new LineBreak());
			blockCrit.Inlines.Add(new Run("Crit chance from artifacts is not displayed here"));

			toolTipCrit.Content = blockCrit;
			CritChanceBlock.ToolTip = toolTipCrit;
		}

		private void GenerateStatValuePoisonTooltip()
		{
			var toolTipPoison = new ToolTip();

			TooltipController.SetTooltipDelayAndDuration(PoisonDamageBlock);

			var blockPoison = new TextBlock {Style = (Style) FindResource("ToolTipTextBlockBase")};

			// ["You deal X bonus poison damage per tick"]
			var bindingPoisonTotal = new Binding("PoisonDamage") {Source = User.Instance.CurrentHero};
			var runPoisonTotal = new Run();
			runPoisonTotal.SetBinding(Run.TextProperty, bindingPoisonTotal);
			blockPoison.Inlines.Add("You deal ");
			blockPoison.Inlines.Add(new Bold(runPoisonTotal));
			blockPoison.Inlines.Add(" bonus poison damage per tick");

			blockPoison.Inlines.Add(new LineBreak());

			// Section only for Venom since its the only class with built-in poison bonuses.
			// ["Poison damage: X (base) + X (X/lvl) = X"]
			var bindingLevelPoisonBonus = new Binding("LevelPoisonBonus")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runLevelPoisonBonus = new Run();
			runLevelPoisonBonus.SetBinding(Run.TextProperty, bindingLevelPoisonBonus);
			var bindingLevelPoisonBonusTotal = new Binding("LevelPoisonBonusTotal")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runLevelPoisonBonusTotal = new Run();
			runLevelPoisonBonusTotal.SetBinding(Run.TextProperty, bindingLevelPoisonBonusTotal);
			if (User.Instance.CurrentHero != null)
			{
				if (User.Instance.CurrentHero.HeroClass == HeroClass.Venom)
				{
					blockPoison.Inlines.Add(new LineBreak());
					blockPoison.Inlines.Add("Poison damage: ");
					blockPoison.Inlines.Add(new Bold(new Run("1 ")));
					blockPoison.Inlines.Add(new Italic(new Run("(base)")));
					blockPoison.Inlines.Add(new Run(" + "));
					blockPoison.Inlines.Add(new Bold(runLevelPoisonBonus));
					blockPoison.Inlines.Add(new Italic(new Run(" (2/lvl)")));
					blockPoison.Inlines.Add(" = ");
					blockPoison.Inlines.Add(new Bold(runLevelPoisonBonusTotal));
					blockPoison.Inlines.Add(new LineBreak());
				}
			}

			// ["BlessingName, damage: X"]
			if (User.Instance.CurrentHero != null)
			{
				if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.PoisonDamage)
				{
					var bindingBlessingName = new Binding("Name") {Source = User.Instance.CurrentHero.Blessing};
					var runBlessingName = new Run();
					runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
					var bindingBlessingBuff = new Binding("Buff") {Source = User.Instance.CurrentHero.Blessing};
					var runBlessingBuff = new Run();
					runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
					blockPoison.Inlines.Add(new Bold(runBlessingName));
					blockPoison.Inlines.Add(new Run(", poison damage: "));
					blockPoison.Inlines.Add(new Bold(runBlessingBuff));
				}
			}

			blockPoison.Inlines.Add(new LineBreak());
			blockPoison.Inlines.Add(new LineBreak());
			blockPoison.Inlines.Add(new Run("Posion damage from artifacts is not displayed here"));

			toolTipPoison.Content = blockPoison;
			PoisonDamageBlock.ToolTip = toolTipPoison;
		}

		private void GenerateStatValueAuraTooltip()
		{
			var tooltipAura = new ToolTip();

			TooltipController.SetTooltipDelayAndDuration(AuraDamageBlock);

			var blockAura = new TextBlock {Style = (Style) FindResource("ToolTipTextBlockBase")};

			// ["You deal X% of monster's hp Aura damage per second"]
			var bindingAuraDpsTotal = new Binding("AuraDpsText")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runAuraDpsTotal = new Run();
			runAuraDpsTotal.SetBinding(Run.TextProperty, bindingAuraDpsTotal);
			blockAura.Inlines.Add("You deal ");
			blockAura.Inlines.Add(new Bold(runAuraDpsTotal));
			blockAura.Inlines.Add(" of monster's hp Aura damage per second");
			blockAura.Inlines.Add(new LineBreak());

			// ["Your Aura tick damage is X%; Your Aura tick speed is X/s"]
			var bindingAuraDamageTotal = new Binding("AuraDamageText")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runAuraDamageTotal = new Run();
			runAuraDamageTotal.SetBinding(Run.TextProperty, bindingAuraDamageTotal);
			var bindingAuraAttackSpeedTotal = new Binding("AuraAttackSpeed") {Source = User.Instance.CurrentHero, Mode=BindingMode.OneWay};
			var runAuraAttackSpeedTotal = new Run();
			runAuraAttackSpeedTotal.SetBinding(Run.TextProperty, bindingAuraAttackSpeedTotal);
			blockAura.Inlines.Add("Your Aura tick damage is ");
			blockAura.Inlines.Add(new Bold(runAuraDamageTotal));
			blockAura.Inlines.Add(new LineBreak());
			blockAura.Inlines.Add("Your Aura tick speed is ");
			blockAura.Inlines.Add(new Bold(runAuraAttackSpeedTotal));
			blockAura.Inlines.Add(new Bold(new Run("/s")));
			blockAura.Inlines.Add(new LineBreak());
			blockAura.Inlines.Add(new LineBreak());

			// ["Aura tick damage: X% (base)"]
			blockAura.Inlines.Add("Aura tick damage: ");
			blockAura.Inlines.Add(new Bold(new Run("10%")));
			blockAura.Inlines.Add(new Italic(new Run(" (base)")));

			blockAura.Inlines.Add(new LineBreak());

			// ["Aura tick speed: X/s (base) + X/s (X/s/lvl) = X/s"]
			var bindingAuraSpeedLevelBonus = new Binding("LevelAuraBonus")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runAuraSpeedLevelBonus = new Run();
			runAuraSpeedLevelBonus.SetBinding(Run.TextProperty, bindingAuraSpeedLevelBonus);
			var bindingAuraSpeedLevelBonusTotal = new Binding("AuraAttackSpeed")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runAuraSpeedLevelBonusTotal = new Run();
			runAuraSpeedLevelBonusTotal.SetBinding(Run.TextProperty, bindingAuraSpeedLevelBonusTotal);
			blockAura.Inlines.Add("Aura tick speed: ");
			blockAura.Inlines.Add(new Bold(new Run($"{Hero.AURA_SPEED_BASE}/s")));
			blockAura.Inlines.Add(new Italic(new Run(" (base)")));
			blockAura.Inlines.Add(" + ");
			blockAura.Inlines.Add(new Bold(runAuraSpeedLevelBonus));
			blockAura.Inlines.Add(new Bold(new Run("/s")));
			blockAura.Inlines.Add(new Italic(new Run($" ({Hero.AURA_SPEED_PER_LEVEL}/s/lvl)")));
			blockAura.Inlines.Add(" = ");
			blockAura.Inlines.Add(new Bold(runAuraSpeedLevelBonusTotal));
			blockAura.Inlines.Add(new Bold(new Run("/s")));
			blockAura.Inlines.Add(new LineBreak());

			// ["BlessingName, tick damage: X%"]
			if (User.Instance.CurrentHero != null)
			{
				if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.AuraDamage)
				{
					var bindingBlessingName = new Binding("Name") {Source = User.Instance.CurrentHero.Blessing};
					var runBlessingName = new Run();
					runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
					var bindingBlessingBuff = new Binding("Buff") {Source = User.Instance.CurrentHero.Blessing};
					var runBlessingBuff = new Run();
					runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
					blockAura.Inlines.Add(new Bold(runBlessingName));
					blockAura.Inlines.Add(new Run(", Aura tick damage: "));
					blockAura.Inlines.Add(new Bold(runBlessingBuff));
					blockAura.Inlines.Add(new Run("%"));
				}
			}

			blockAura.Inlines.Add(new LineBreak());

			// ["BlessingName, tick speed: X/s"]
			if (User.Instance.CurrentHero != null)
			{
				if (User.Instance.CurrentHero.Blessing?.Type == BlessingType.AuraSpeed)
				{
					var bindingBlessingName = new Binding("Name") {Source = User.Instance.CurrentHero.Blessing};
					var runBlessingName = new Run();
					runBlessingName.SetBinding(Run.TextProperty, bindingBlessingName);
					var bindingBlessingBuff = new Binding("Buff") {Source = User.Instance.CurrentHero.Blessing};
					var runBlessingBuff = new Run();
					runBlessingBuff.SetBinding(Run.TextProperty, bindingBlessingBuff);
					blockAura.Inlines.Add(new Bold(runBlessingName));
					blockAura.Inlines.Add(new Run(", Aura tick speed: "));
					blockAura.Inlines.Add(new Bold(runBlessingBuff));
					blockAura.Inlines.Add(new Run("/s"));
				}
			}

			blockAura.Inlines.Add(new LineBreak());
			blockAura.Inlines.Add(new LineBreak());
			blockAura.Inlines.Add(new Run("Aura tick damage and speed from artifacts is not displayed here"));

			tooltipAura.Content = blockAura;
			AuraDamageBlock.ToolTip = tooltipAura;
		}
	}
}