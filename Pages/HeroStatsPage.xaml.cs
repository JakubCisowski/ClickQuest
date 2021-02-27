using ClickQuest.Account;
using ClickQuest.Heroes;
using MaterialDesignThemes.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace ClickQuest.Pages
{
	public partial class HeroStatsPage : Page
	{
		#region Private Fields

		private Hero _hero;

		#endregion Private Fields

		public HeroStatsPage()
		{
			InitializeComponent();

			_hero = User.Instance.CurrentHero;
			this.DataContext = _hero;

			GenerateIngots();
			GenerateDungeonKeys();
			GenerateGold();
			GenerateSpecializations();
			RefreshQuestTimer();
			RefreshBlessingTimer();
			GenerateTooltips();
		}

		private void GenerateGold()
		{
			// Create a binding for the amount of Gold.
			Binding binding = new Binding("Gold")
			{
				Source = User.Instance,
				StringFormat = "{0}"
			};

			GoldBlock.SetBinding(TextBlock.TextProperty, binding);

			// Generate gold tooltip.
			var tooltip = new ToolTip();

			ToolTipService.SetInitialShowDelay(GoldPanel, 400);
			ToolTipService.SetShowDuration(GoldPanel, 20000);

			var block = new TextBlock()
			{
				Style = (Style)this.FindResource("ToolTipTextBlockBase"),
				Text = "Gold"
			};

			tooltip.Content = block;
			GoldPanel.ToolTip = tooltip;
		}

		private void GenerateIngots()
		{
			// Make sure hero isn't null (constructor calls this function while loading database).
			if (_hero != null)
			{
				IngotKeyGrid.Children.Clear();

				for (int i = 0; i < User.Instance.Ingots.Count; i++)
				{
					// Ingot panel for icon and amount.
					var panel = new StackPanel()
					{
						Orientation = Orientation.Horizontal,
						Margin = new Thickness(50, 0, 0, 0),
						Background = new SolidColorBrush(Colors.Transparent)
					};

					// Generate ingots tooltips.
					var tooltip = new ToolTip();
					ToolTipService.SetInitialShowDelay(panel, 100);
					ToolTipService.SetShowDuration(panel, 20000);

					var block = new TextBlock()
					{
						Style = (Style)this.FindResource("ToolTipTextBlockBase")
					};

					// Ingot icon.
					var icon = new PackIcon()
					{
						Kind = PackIconKind.Gold,
						Width = 22,
						Height = 22,
						VerticalAlignment = VerticalAlignment.Center
					};

					// Select ingot icon color and tooltip text.
					switch (i)
					{
						case 0:
							icon.Foreground = new SolidColorBrush(Colors.Gray);
							block.Text = "General Ingots";
							break;

						case 1:
							icon.Foreground = new SolidColorBrush(Colors.Brown);
							block.Text = "Fine Ingots";
							break;

						case 2:
							icon.Foreground = new SolidColorBrush(Colors.Green);
							block.Text = "Superior Ingots";
							break;

						case 3:
							icon.Foreground = new SolidColorBrush(Colors.Blue);
							block.Text = "Exceptional Ingots";
							break;

						case 4:
							icon.Foreground = new SolidColorBrush(Colors.Purple);
							block.Text = "Mythic Ingots";
							break;

						case 5:
							icon.Foreground = new SolidColorBrush(Colors.Gold);
							block.Text = "Masterwork Ingots";
							break;
					}

					// Add ingot icon to the panel.
					panel.Children.Add(icon);

					// Create ingot amount textblock.
					var block2 = new TextBlock()
					{
						Name = "Ingot" + i.ToString(),
						FontSize = 18,
						VerticalAlignment = VerticalAlignment.Center
					};

					// Add ingot binding.
					Binding binding = new Binding("Quantity")
					{
						Source = User.Instance.Ingots[i],
						StringFormat = "   {0}"
					};
					block2.SetBinding(TextBlock.TextProperty, binding);

					// Add ingot amount text to the panel.
					panel.Children.Add(block2);

					// Add the panel to IngotKey grid.
					IngotKeyGrid.Children.Add(panel);

					// Set its row and column.
					Grid.SetColumn(panel, 0);
					Grid.SetRow(panel, i);

					// Add tooltip to the panel.
					tooltip.Content = block;
					panel.ToolTip = tooltip;
				}
			}
		}

		private void GenerateDungeonKeys()
		{
			//Make sure hero isn't null (constructor calls this function while loading database).
			if (_hero != null)
			{
				for (int i = 0; i < User.Instance.DungeonKeys.Count; i++)
				{
					// Ingot panel for icon and amount.
					var panel = new StackPanel()
					{
						Orientation = Orientation.Horizontal,
						Margin = new Thickness(50, 0, 0, 0),
						Background = new SolidColorBrush(Colors.Transparent)
					};

					// Generate ingots tooltips.
					var tooltip = new ToolTip();
					ToolTipService.SetInitialShowDelay(panel, 100);
					ToolTipService.SetShowDuration(panel, 20000);

					var block = new TextBlock()
					{
						Style = (Style)this.FindResource("ToolTipTextBlockBase")
					};

					// Ingot icon.
					var icon = new PackIcon()
					{
						Kind = PackIconKind.Key,
						Width = 22,
						Height = 22,
						VerticalAlignment = VerticalAlignment.Center
					};

					// Select ingot icon color and tooltip text.
					switch (i)
					{
						case 0:
							icon.Foreground = new SolidColorBrush(Colors.Gray);
							block.Text = "General dungeon keys";
							break;

						case 1:
							icon.Foreground = new SolidColorBrush(Colors.Brown);
							block.Text = "Fine dungeon keys";
							break;

						case 2:
							icon.Foreground = new SolidColorBrush(Colors.Green);
							block.Text = "Superior dungeon keys";
							break;

						case 3:
							icon.Foreground = new SolidColorBrush(Colors.Blue);
							block.Text = "Exceptional dungeon keys";
							break;

						case 4:
							icon.Foreground = new SolidColorBrush(Colors.Purple);
							block.Text = "Mythic dungeon keys";
							break;

						case 5:
							icon.Foreground = new SolidColorBrush(Colors.Gold);
							block.Text = "Masterwork dungeon keys";
							break;
					}

					// Add dungeon key icon to the panel.
					panel.Children.Add(icon);

					// Create dungeon key amount textblock.
					var block2 = new TextBlock()
					{
						Name = "Key" + i.ToString(),
						FontSize = 18,
						VerticalAlignment = VerticalAlignment.Center
					};

					// Add dungeon key binding.
					Binding binding = new Binding("Quantity")
					{
						Source = User.Instance.DungeonKeys[i],
						StringFormat = "   {0}"
					};
					block2.SetBinding(TextBlock.TextProperty, binding);

					// Add dungeon key amount text to the panel.
					panel.Children.Add(block2);

					// Add the panel to IngotKey grid.
					IngotKeyGrid.Children.Add(panel);

					// Set its row and column.
					Grid.SetColumn(panel, 1);
					Grid.SetRow(panel, i);

					// Add tooltip to the panel.
					tooltip.Content = block;
					panel.ToolTip = tooltip;
				}
			}
		}

		private void RefreshQuestTimer()
		{
			// Find quest that is currently being completed.
			var quest = User.Instance.CurrentHero?.Quests.FirstOrDefault(x => x.EndDate != default(DateTime));

			// Generate Quest tooltips.
			var toolTip = new ToolTip();
			ToolTipService.SetInitialShowDelay(QuestDuration, 100);
			ToolTipService.SetShowDuration(QuestDuration, 20000);

			var toolTipBlock = new TextBlock()
			{
				Style = (Style)this.FindResource("ToolTipTextBlockBase")
			};

			if (quest != null)
			{
				// Bind its duration to the panel.
				var binding = new Binding("TicksCountText");
				binding.Source = quest;
				QuestDuration.SetBinding(TextBlock.TextProperty, binding);

				// Create quest tooltip
				toolTipBlock.Inlines.Add(new Run($"{quest.Name}"));
				if (quest.Rare)
				{
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run("*Rare quest*"));
				}
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"Class: {quest.HeroClass}"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"{quest.Description}"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"{quest.RewardsDescription}"));
			}
			else
			{
				toolTipBlock.Text="No quest is currently active";
			}

			toolTip.Content = toolTipBlock;
			QuestDuration.ToolTip = toolTip;
		}

		private void RefreshBlessingTimer()
		{
			// Find blessing that is currently active.
			var blessing = User.Instance.CurrentHero?.Blessings.FirstOrDefault();

			// Generate Blessing tooltips.
			var toolTip = new ToolTip();
			ToolTipService.SetInitialShowDelay(BlessingDuration, 100);
			ToolTipService.SetShowDuration(BlessingDuration, 20000);

			var toolTipBlock = new TextBlock()
			{
				Style = (Style)this.FindResource("ToolTipTextBlockBase")
			};

			if (blessing != null)
			{
				// Bind its duration to the panel.
				var binding = new Binding("DurationText");
				binding.Source = blessing;
				BlessingDuration.SetBinding(TextBlock.TextProperty, binding);
				
				// Create blessing tooltip
				toolTipBlock.Inlines.Add(new Run($"{blessing.Name}"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"*{blessing.RarityString}*"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"{blessing.Description}"));
			}
			else
			{
				toolTipBlock.Text="No blessing is currently active";
			}

			toolTip.Content = toolTipBlock;
			BlessingDuration.ToolTip = toolTip;
		}

		private void GenerateSpecializations()
		{
			if (User.Instance.CurrentHero is null)
			{
				// This function was called before selecting a hero - return.
				return;
			}

			SpecializationsGrid.Children.Clear();

			#region SpecializationBuying

			{
				var nameBlock = new TextBlock()
				{
					Name = "SpecBuyingName",
					Text = "Tradesman",
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				var block = new TextBlock()
				{
					Name = "SpecBuying",
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				Binding binding = new Binding("SpecBuyingBuff")
				{
					Source = User.Instance.CurrentHero.Specialization,
					StringFormat = " → Shop offer size +{0}"
				};

				block.SetBinding(TextBlock.TextProperty, binding);

				// Generate SpecBuying tooltips.
				var toolTip = new ToolTip();
				ToolTipService.SetInitialShowDelay(GoldPanel, 100);
				ToolTipService.SetShowDuration(GoldPanel, 20000);

				// Calculate remaining amount to upgrade specialization.
				int nextUpgrade = User.Instance.CurrentHero.Specialization.SpecBuyingAmount;
				while (nextUpgrade >= User.Instance.CurrentHero.Specialization.SpecBuyingThreshold)
				{
					nextUpgrade -= User.Instance.CurrentHero.Specialization.SpecBuyingThreshold;
				}
				nextUpgrade = User.Instance.CurrentHero.Specialization.SpecBuyingThreshold - nextUpgrade;

				var toolTipBlock = new TextBlock()
				{
					Style = (Style)this.FindResource("ToolTipTextBlockBase")
				};
				toolTipBlock.Inlines.Add(new Run("Increases shop offer size (base size is 5)"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run("You can master Tradesman by buying recipes in shop"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"Next upgrade (+1 shop offer size) in"));
				toolTipBlock.Inlines.Add(new Bold(new Run($" {nextUpgrade} ")));
				toolTipBlock.Inlines.Add(new Run($"bought recipes"));

				toolTip.Content = toolTipBlock;

				nameBlock.ToolTip = toolTip;
				block.ToolTip = toolTip;

				Grid.SetRow(nameBlock, 0);
				Grid.SetRow(block, 0);
				Grid.SetColumn(nameBlock, 0);
				Grid.SetColumn(block, 1);

				SpecializationsGrid.Children.Add(nameBlock);
				SpecializationsGrid.Children.Add(block);
			}

			#endregion SpecializationBuying

			#region SpecializationMelting

			{
				var nameBlock = new TextBlock()
				{
					Name = "SpecMeltingName",
					Text = "Melter",
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				var block = new TextBlock()
				{
					Name = "SpecMelting",
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				Binding binding = new Binding("SpecMeltingBuff")
				{
					Source = Account.User.Instance.CurrentHero.Specialization,
					StringFormat = " → Extra ingot +{0}%"
				};

				block.SetBinding(TextBlock.TextProperty, binding);

				// Generate SpecBuying tooltips.
				var toolTip = new ToolTip();
				ToolTipService.SetInitialShowDelay(GoldPanel, 100);
				ToolTipService.SetShowDuration(GoldPanel, 20000);

				// Calculate remaining amount to upgrade specialization.
				int nextUpgrade = User.Instance.CurrentHero.Specialization.SpecMeltingAmount;
				while (nextUpgrade >= User.Instance.CurrentHero.Specialization.SpecMeltingThreshold)
				{
					nextUpgrade -= User.Instance.CurrentHero.Specialization.SpecMeltingThreshold;
				}
				nextUpgrade = User.Instance.CurrentHero.Specialization.SpecMeltingThreshold - nextUpgrade;

				var toolTipBlock = new TextBlock()
				{
					Style = (Style)this.FindResource("ToolTipTextBlockBase")
				};
				toolTipBlock.Inlines.Add(new Run("Increases % chance to get bonus ingots when melting (base chance is 0%)"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run("You can master Melter by melting materials in Blacksmith"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run("Each 100% guarantees additional ingot"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"Next upgrade (+5% chance) in"));
				toolTipBlock.Inlines.Add(new Bold(new Run($" {nextUpgrade} ")));
				toolTipBlock.Inlines.Add(new Run($"melted materials"));

				toolTip.Content = toolTipBlock;

				nameBlock.ToolTip = toolTip;
				block.ToolTip = toolTip;

				Grid.SetRow(nameBlock, 1);
				Grid.SetRow(block, 1);
				Grid.SetColumn(nameBlock, 0);
				Grid.SetColumn(block, 1);

				SpecializationsGrid.Children.Add(nameBlock);
				SpecializationsGrid.Children.Add(block);
			}

			#endregion SpecializationMelting

			#region SpecializationCrafting

			{
				var nameBlock = new TextBlock()
				{
					Name = "SpecCraftingName",
					Text = "Craftsman",
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				var block = new TextBlock()
				{
					Name = "SpecCrafting",
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				Binding binding = new Binding("SpecCraftingText")
				{
					Source = User.Instance.CurrentHero.Specialization,
					StringFormat = " → Can craft {0} recipes"
				};

				block.SetBinding(TextBlock.TextProperty, binding);

				// Generate SpecBuying tooltips.
				var toolTip = new ToolTip();
				ToolTipService.SetInitialShowDelay(GoldPanel, 100);
				ToolTipService.SetShowDuration(GoldPanel, 20000);

				// Calculate remaining amount to upgrade specialization.
				int nextUpgrade = User.Instance.CurrentHero.Specialization.SpecCraftingAmount;
				while (nextUpgrade >= User.Instance.CurrentHero.Specialization.SpecCraftingThreshold)
				{
					nextUpgrade -= User.Instance.CurrentHero.Specialization.SpecCraftingThreshold;
				}
				nextUpgrade = User.Instance.CurrentHero.Specialization.SpecCraftingThreshold - nextUpgrade;

				var toolTipBlock = new TextBlock()
				{
					Style = (Style)this.FindResource("ToolTipTextBlockBase")
				};
				toolTipBlock.Inlines.Add(new Run("Increases crafting rarity limit (base limit is Fine rarity)"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run("You can master Craftsman by crafting artifacts in Blacksmith"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"Next rarity limit upgrade in"));
				toolTipBlock.Inlines.Add(new Bold(new Run($" {nextUpgrade} ")));
				toolTipBlock.Inlines.Add(new Run($"crafted artifacts"));

				toolTip.Content = toolTipBlock;

				nameBlock.ToolTip = toolTip;
				block.ToolTip = toolTip;

				Grid.SetRow(nameBlock, 2);
				Grid.SetRow(block, 2);
				Grid.SetColumn(nameBlock, 0);
				Grid.SetColumn(block, 1);

				SpecializationsGrid.Children.Add(nameBlock);
				SpecializationsGrid.Children.Add(block);
			}

			#endregion SpecializationCrafting

			#region SpecializationQuesting

			{
				var nameBlock = new TextBlock()
				{
					Name = "SpecQuestingName",
					Text = "Adventurer",
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				var block = new TextBlock()
				{
					Name = "SpecQuesting",
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				Binding binding = new Binding("SpecQuestingBuff")
				{
					Source = User.Instance.CurrentHero.Specialization,
					StringFormat = " → Quest time -{0}%"
				};

				block.SetBinding(TextBlock.TextProperty, binding);

				// Generate SpecBuying tooltips.
				var toolTip = new ToolTip();
				ToolTipService.SetInitialShowDelay(GoldPanel, 100);
				ToolTipService.SetShowDuration(GoldPanel, 20000);

				// Calculate remaining amount to upgrade specialization.
				int nextUpgrade = User.Instance.CurrentHero.Specialization.SpecQuestingAmount;
				while (nextUpgrade >= User.Instance.CurrentHero.Specialization.SpecQuestingThreshold)
				{
					nextUpgrade -= User.Instance.CurrentHero.Specialization.SpecQuestingThreshold;
				}
				nextUpgrade = User.Instance.CurrentHero.Specialization.SpecQuestingThreshold - nextUpgrade;

				var toolTipBlock = new TextBlock()
				{
					Style = (Style)this.FindResource("ToolTipTextBlockBase")
				};
				toolTipBlock.Inlines.Add(new Run("Reduces % time to complete quests (limit is 50%)"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run("You can master Adventurer by completing quests"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"Next upgrade (+5% reduced time) in"));
				toolTipBlock.Inlines.Add(new Bold(new Run($" {nextUpgrade} ")));
				toolTipBlock.Inlines.Add(new Run($"completed quests"));

				toolTip.Content = toolTipBlock;

				nameBlock.ToolTip = toolTip;
				block.ToolTip = toolTip;

				Grid.SetRow(nameBlock, 3);
				Grid.SetRow(block, 3);
				Grid.SetColumn(nameBlock, 0);
				Grid.SetColumn(block, 1);

				SpecializationsGrid.Children.Add(nameBlock);
				SpecializationsGrid.Children.Add(block);
			}

			#endregion SpecializationQuesting

			#region SpecializationClicking

			{
				var nameBlock = new TextBlock()
				{
					Name = "SpecClickingName",
					Text = "Clicker",
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				var block = new TextBlock()
				{
					Name = "SpecClicking",
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				Binding binding = new Binding("SpecClickingBuff")
				{
					Source = User.Instance.CurrentHero.Specialization,
					StringFormat = " → Click damage +{0}"
				};

				block.SetBinding(TextBlock.TextProperty, binding);

				// Generate SpecBuying tooltips.
				var toolTip = new ToolTip();
				ToolTipService.SetInitialShowDelay(GoldPanel, 100);
				ToolTipService.SetShowDuration(GoldPanel, 20000);

				// Calculate remaining amount to upgrade specialization.
				int nextUpgrade = User.Instance.CurrentHero.Specialization.SpecClickingAmount;
				while (nextUpgrade >= User.Instance.CurrentHero.Specialization.SpecClickingThreshold)
				{
					nextUpgrade -= User.Instance.CurrentHero.Specialization.SpecClickingThreshold;
				}
				nextUpgrade = User.Instance.CurrentHero.Specialization.SpecClickingThreshold - nextUpgrade;

				var toolTipBlock = new TextBlock()
				{
					Style = (Style)this.FindResource("ToolTipTextBlockBase")
				};
				toolTipBlock.Inlines.Add(new Run("Increases click damage"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run("You can master Clicker by clicking on monsters and bosses"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run("Click damage from this specialization is applied after other effects eg. crit"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"Next upgrade (+1 click damage) in"));
				toolTipBlock.Inlines.Add(new Bold(new Run($" {nextUpgrade} ")));
				toolTipBlock.Inlines.Add(new Run($"clicks"));

				toolTip.Content = toolTipBlock;

				nameBlock.ToolTip = toolTip;
				block.ToolTip = toolTip;

				Grid.SetRow(nameBlock, 4);
				Grid.SetRow(block, 4);
				Grid.SetColumn(nameBlock, 0);
				Grid.SetColumn(block, 1);

				SpecializationsGrid.Children.Add(nameBlock);
				SpecializationsGrid.Children.Add(block);
			}

			#endregion SpecializationClicking

			#region SpecializationBlessing

			{
				var nameBlock = new TextBlock()
				{
					Name = "SpecBlessingName",
					Text = "Prayer",
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				var block = new TextBlock()
				{
					Name = "SpecBlessing",
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				Binding binding = new Binding("SpecBlessingBuff")
				{
					Source = User.Instance.CurrentHero.Specialization,
					StringFormat = " → Blessing duration +{0}s"
				};

				block.SetBinding(TextBlock.TextProperty, binding);

				// Generate SpecBuying tooltips.
				var toolTip = new ToolTip();
				ToolTipService.SetInitialShowDelay(GoldPanel, 100);
				ToolTipService.SetShowDuration(GoldPanel, 20000);

				// Calculate remaining amount to upgrade specialization.
				int nextUpgrade = User.Instance.CurrentHero.Specialization.SpecBlessingAmount;
				while (nextUpgrade >= User.Instance.CurrentHero.Specialization.SpecBlessingThreshold)
				{
					nextUpgrade -= User.Instance.CurrentHero.Specialization.SpecBlessingThreshold;
				}
				nextUpgrade = User.Instance.CurrentHero.Specialization.SpecBlessingThreshold - nextUpgrade;

				var toolTipBlock = new TextBlock()
				{
					Style = (Style)this.FindResource("ToolTipTextBlockBase")
				};
				toolTipBlock.Inlines.Add(new Run("Increases blessing duration in seconds"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run("You can master Prayer by buying blessings in Priest"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"Next upgrade (+15s duration) in"));
				toolTipBlock.Inlines.Add(new Bold(new Run($" {nextUpgrade} ")));
				toolTipBlock.Inlines.Add(new Run($"bought blessings"));

				toolTip.Content = toolTipBlock;

				nameBlock.ToolTip = toolTip;
				block.ToolTip = toolTip;

				Grid.SetRow(nameBlock, 5);
				Grid.SetRow(block, 5);
				Grid.SetColumn(nameBlock, 0);
				Grid.SetColumn(block, 1);

				SpecializationsGrid.Children.Add(nameBlock);
				SpecializationsGrid.Children.Add(block);
			}

			#endregion SpecializationBlessing

			#region SpecializationDungeon

			{
				var nameBlock = new TextBlock()
				{
					Name = "SpecDungeonName",
					Text = "Daredevil",
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				var block = new TextBlock()
				{
					Name = "SpecDungeon",
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				Binding binding = new Binding("SpecDungeonBuff")
				{
					Source = User.Instance.CurrentHero.Specialization,
					StringFormat = " → Bossfight timer +{0}s"
				};

				block.SetBinding(TextBlock.TextProperty, binding);

				// Generate SpecBuying tooltips.
				var toolTip = new ToolTip();
				ToolTipService.SetInitialShowDelay(GoldPanel, 100);
				ToolTipService.SetShowDuration(GoldPanel, 20000);

				// Calculate remaining amount to upgrade specialization.
				int nextUpgrade = User.Instance.CurrentHero.Specialization.SpecDungeonAmount;
				while (nextUpgrade >= User.Instance.CurrentHero.Specialization.SpecDungeonThreshold)
				{
					nextUpgrade -= User.Instance.CurrentHero.Specialization.SpecDungeonThreshold;
				}
				nextUpgrade = User.Instance.CurrentHero.Specialization.SpecDungeonThreshold - nextUpgrade;

				var toolTipBlock = new TextBlock()
				{
					Style = (Style)this.FindResource("ToolTipTextBlockBase")
				};
				toolTipBlock.Inlines.Add(new Run("Increases amount of time to defeat boss in seconds (base time is 30s)"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run("You can master Daredevil by buying recipes in shop"));
				toolTipBlock.Inlines.Add(new LineBreak());
				toolTipBlock.Inlines.Add(new Run($"Next upgrade (+1 second) in"));
				toolTipBlock.Inlines.Add(new Bold(new Run($" {nextUpgrade} ")));
				toolTipBlock.Inlines.Add(new Run($"finished dungeons"));

				toolTip.Content = toolTipBlock;

				nameBlock.ToolTip = toolTip;
				block.ToolTip = toolTip;

				Grid.SetRow(nameBlock, 6);
				Grid.SetRow(block, 6);
				Grid.SetColumn(nameBlock, 0);
				Grid.SetColumn(block, 1);

				SpecializationsGrid.Children.Add(nameBlock);
				SpecializationsGrid.Children.Add(block);
			}

			#endregion SpecializationDungeon
		}

		private void GenerateTooltips()
		{
			#region HeroInfoTooltip
			var tooltip = new ToolTip();

			ToolTipService.SetInitialShowDelay(HeroNameBlock, 400);
			ToolTipService.SetShowDuration(HeroNameBlock, 20000);

			var block = new TextBlock()
			{
				Style = (Style)this.FindResource("ToolTipTextBlockBase")
			};

			switch (Account.User.Instance.CurrentHero?.HeroClass)
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

			switch (Account.User.Instance.CurrentHero?.HeroRace)
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
			#endregion

			#region StatValueDamageTooltip
			
			var tooltipDamage = new ToolTip();

			ToolTipService.SetInitialShowDelay(ClickDamageBlock, 400);
			ToolTipService.SetShowDuration(ClickDamageBlock, 20000);

			var blockDamage = new TextBlock()
			{
				Style = (Style)this.FindResource("ToolTipTextBlockBase")
			};

			// ["You deal X damage per click"]
			var bindingDamageTotal = new Binding("ClickDamage")
			{
				Source = User.Instance.CurrentHero
			};
			var runDamageTotal = new Run();
			runDamageTotal.SetBinding(Run.TextProperty,bindingDamageTotal);
			blockDamage.Inlines.Add("You deal ");
			blockDamage.Inlines.Add(new Bold(runDamageTotal));
			blockDamage.Inlines.Add(" damage per click");

			blockDamage.Inlines.Add(new LineBreak());
			blockDamage.Inlines.Add(new LineBreak());

			// ["Click damage: X (base) + X (X/lvl) = X"]
			var bindingDamagePerLevel = new Binding("ClickDamagePerLevel")
			{
				Source = User.Instance.CurrentHero
			};
			var runDamagePerLevel = new Run();
			runDamagePerLevel.SetBinding(Run.TextProperty,bindingDamagePerLevel);
			var bindingLevelDamageBonus = new Binding("LevelDamageBonus")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runLevelDamageBonus = new Run();
			runLevelDamageBonus.SetBinding(Run.TextProperty,bindingLevelDamageBonus);
			var bindingLevelDamageBonusTotal= new Binding("LevelDamageBonusTotal")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runLevelDamageBonusTotal = new Run();
			runLevelDamageBonusTotal.SetBinding(Run.TextProperty,bindingLevelDamageBonusTotal);
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
			if(Account.User.Instance.CurrentHero != null)
			{
				if(Account.User.Instance.CurrentHero.Blessings.Any(x=>x.Type==Items.BlessingType.ClickDamage))
				{
					var bindingBlessingName = new Binding("Name")
					{
						Source = User.Instance.CurrentHero.Blessings[0]
					};
					var runBlessingName = new Run();
					runBlessingName.SetBinding(Run.TextProperty,bindingBlessingName);
					var bindingBlessingBuff = new Binding("Buff")
					{
						Source = User.Instance.CurrentHero.Blessings[0]
					};
					var runBlessingBuff = new Run();
					runBlessingBuff.SetBinding(Run.TextProperty,bindingBlessingBuff);
					blockDamage.Inlines.Add(new Bold(runBlessingName));
					blockDamage.Inlines.Add(new Run(", damage: "));
					blockDamage.Inlines.Add(new Bold(runBlessingBuff));
				}
			}
			var bindingSpecClicking = new Binding("SpecClickingBuff")
			{
				Source = User.Instance.CurrentHero?.Specialization
			};
			var runSpecClicking = new Run();
			runSpecClicking.SetBinding(Run.TextProperty,bindingSpecClicking);
			blockDamage.Inlines.Add(new LineBreak());
			blockDamage.Inlines.Add(new LineBreak());
			blockDamage.Inlines.Add(new Run("Damage from artifacts is not displayed here"));
			blockDamage.Inlines.Add(new LineBreak());
			blockDamage.Inlines.Add(new Italic(new Run("You also deal ")));
			blockDamage.Inlines.Add(new Italic(new Bold(runSpecClicking)));
			blockDamage.Inlines.Add(new Italic(new Run(" on-hit damage from Clicker Specialization, but it doesn't get multiplied upon critting")));

			tooltipDamage.Content = blockDamage;
			ClickDamageBlock.ToolTip = tooltipDamage;

			#endregion

			#region StatValueCritTooltip
			
			var toolTipCrit = new ToolTip();

			ToolTipService.SetInitialShowDelay(CritChanceBlock, 400);
			ToolTipService.SetShowDuration(CritChanceBlock, 20000);

			var blockCrit = new TextBlock()
			{
				Style = (Style)this.FindResource("ToolTipTextBlockBase")
			};

			// ["You have X% chance to crit (deal double damage) when clicking"]
			var bindingCritTotal = new Binding("CritChanceText")
			{
				Source = User.Instance.CurrentHero
			};
			var runCritTotal = new Run();
			runCritTotal.SetBinding(Run.TextProperty,bindingCritTotal);
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
			runLevelCritBonus.SetBinding(Run.TextProperty,bindingLevelCritBonus);
			var bindingLevelCritBonusTotal= new Binding("LevelCritBonusTotal")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runLevelCritBonusTotal = new Run();
			runLevelCritBonusTotal.SetBinding(Run.TextProperty,bindingLevelCritBonusTotal);
			if(User.Instance.CurrentHero != null)
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
			if(Account.User.Instance.CurrentHero != null)
			{
				if(Account.User.Instance.CurrentHero.Blessings.Any(x=>x.Type==Items.BlessingType.CritChance))
				{
					var bindingBlessingName = new Binding("Name")
					{
						Source = User.Instance.CurrentHero.Blessings[0]
					};
					var runBlessingName = new Run();
					runBlessingName.SetBinding(Run.TextProperty,bindingBlessingName);
					var bindingBlessingBuff = new Binding("Buff")
					{
						Source = User.Instance.CurrentHero.Blessings[0]
					};
					var runBlessingBuff = new Run();
					runBlessingBuff.SetBinding(Run.TextProperty,bindingBlessingBuff);
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

			#endregion

			#region StatValuePoisonTooltip
			
			var toolTipPoison = new ToolTip();

			ToolTipService.SetInitialShowDelay(PoisonDamageBlock, 400);
			ToolTipService.SetShowDuration(PoisonDamageBlock, 20000);

			var blockPoison = new TextBlock()
			{
				Style = (Style)this.FindResource("ToolTipTextBlockBase")
			};

			// ["You deal X bonus poison damage per tick"]
			var bindingPoisonTotal = new Binding("PoisonDamage")
			{
				Source = User.Instance.CurrentHero
			};
			var runPoisonTotal = new Run();
			runPoisonTotal.SetBinding(Run.TextProperty,bindingPoisonTotal);
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
			runLevelPoisonBonus.SetBinding(Run.TextProperty,bindingLevelPoisonBonus);
			var bindingLevelPoisonBonusTotal= new Binding("LevelPoisonBonusTotal")
			{
				Source = User.Instance.CurrentHero,
				Mode = BindingMode.OneWay
			};
			var runLevelPoisonBonusTotal = new Run();
			runLevelPoisonBonusTotal.SetBinding(Run.TextProperty,bindingLevelPoisonBonusTotal);
			if(User.Instance.CurrentHero != null)
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
			if(Account.User.Instance.CurrentHero != null)
			{
				if(Account.User.Instance.CurrentHero.Blessings.Any(x=>x.Type==Items.BlessingType.PoisonDamage))
				{
					var bindingBlessingName = new Binding("Name")
					{
						Source = User.Instance.CurrentHero.Blessings[0]
					};
					var runBlessingName = new Run();
					runBlessingName.SetBinding(Run.TextProperty,bindingBlessingName);
					var bindingBlessingBuff = new Binding("Buff")
					{
						Source = User.Instance.CurrentHero.Blessings[0]
					};
					var runBlessingBuff = new Run();
					runBlessingBuff.SetBinding(Run.TextProperty,bindingBlessingBuff);
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

			#endregion
		}
	}
}