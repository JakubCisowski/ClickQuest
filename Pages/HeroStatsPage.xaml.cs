using ClickQuest.Account;
using ClickQuest.Heroes;
using MaterialDesignThemes.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ControlzEx;
using System.Windows.Documents;

namespace ClickQuest.Pages
{
	public partial class HeroStatsPage : Page
	{
		#region Private Fields
		private Hero _hero;
		#endregion

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
			ControlzEx.ToolTipAssist.SetAutoMove(tooltip, true);

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
					// Generate ingots tooltips.
					var tooltip = new ToolTip();
					ToolTipService.SetInitialShowDelay(GoldPanel, 100);
					ToolTipService.SetShowDuration(GoldPanel, 20000);
					ControlzEx.ToolTipAssist.SetAutoMove(tooltip, true);

					var block = new TextBlock()
					{
						Style = (Style)this.FindResource("ToolTipTextBlockBase")
					};

					// Ingot panel for icon and amount.
					var panel = new StackPanel()
					{
						Orientation = Orientation.Horizontal,
						Margin = new Thickness(50, 0, 0, 0),
						Background = new SolidColorBrush(Colors.Transparent)
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
					// Generate ingots tooltips.
					var tooltip = new ToolTip();
					ToolTipService.SetInitialShowDelay(GoldPanel, 100);
					ToolTipService.SetShowDuration(GoldPanel, 20000);
					ControlzEx.ToolTipAssist.SetAutoMove(tooltip, true);

					var block = new TextBlock()
					{
						Style = (Style)this.FindResource("ToolTipTextBlockBase")
					};

					// Ingot panel for icon and amount.
					var panel = new StackPanel()
					{
						Orientation = Orientation.Horizontal,
						Margin = new Thickness(50, 0, 0, 0),
						Background = new SolidColorBrush(Colors.Transparent)
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
						StringFormat="   {0}"
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

			if (quest != null)
			{
				// Bind its duration to the panel.
				var binding = new Binding("TicksCountText");
				binding.Source = quest;
				QuestDuration.SetBinding(TextBlock.TextProperty, binding);
			}
		}

		private void RefreshBlessingTimer()
		{
			// Find blessing that is currently active.
			var blessing = User.Instance.CurrentHero?.Blessings.FirstOrDefault();

			if (blessing != null)
			{
				// Bind its duration to the panel.
				var binding = new Binding("DurationText");
				binding.Source = blessing;
				BlessingDuration.SetBinding(TextBlock.TextProperty, binding);
			}
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

				Binding binding = new Binding("SpecBuyingAmount")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding2 = new Binding("SpecBuyingThreshold")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding3 = new Binding("SpecBuyingBuff")
				{
					Source = User.Instance.CurrentHero.Specialization
				};

				MultiBinding multiBinding = new MultiBinding
				{
					StringFormat = " → Shop offer size +{2}"
				};
				multiBinding.Bindings.Add(binding);
				multiBinding.Bindings.Add(binding2);
				multiBinding.Bindings.Add(binding3);

				block.SetBinding(TextBlock.TextProperty, multiBinding);

				Grid.SetRow(nameBlock, 0);
				Grid.SetRow(block, 0);
				Grid.SetColumn(nameBlock, 0);
				Grid.SetColumn(block, 1);

				SpecializationsGrid.Children.Add(nameBlock);
				SpecializationsGrid.Children.Add(block);
			}
			#endregion

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

				Binding binding = new Binding("SpecMeltingAmount")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding2 = new Binding("SpecMeltingThreshold")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding3 = new Binding("SpecMeltingBuff")
				{
					Source = User.Instance.CurrentHero.Specialization
				};

				MultiBinding multiBinding = new MultiBinding
				{
					StringFormat = " → Extra ingot +{2}%"
				};
				multiBinding.Bindings.Add(binding);
				multiBinding.Bindings.Add(binding2);
				multiBinding.Bindings.Add(binding3);

				block.SetBinding(TextBlock.TextProperty, multiBinding);

				Grid.SetRow(nameBlock, 1);
				Grid.SetRow(block, 1);
				Grid.SetColumn(nameBlock, 0);
				Grid.SetColumn(block, 1);

				SpecializationsGrid.Children.Add(nameBlock);
				SpecializationsGrid.Children.Add(block);
			}
			#endregion

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

				Binding binding = new Binding("SpecCraftingAmount")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding2 = new Binding("SpecCraftingThreshold")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding3 = new Binding("SpecCraftingBuff")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding4 = new Binding("SpecCraftingText")
				{
					Source = User.Instance.CurrentHero.Specialization
				};

				MultiBinding multiBinding = new MultiBinding
				{
					StringFormat = " → Can craft {3} recipes"
				};
				multiBinding.Bindings.Add(binding);
				multiBinding.Bindings.Add(binding2);
				multiBinding.Bindings.Add(binding3);
				multiBinding.Bindings.Add(binding4);

				block.SetBinding(TextBlock.TextProperty, multiBinding);

				Grid.SetRow(nameBlock, 2);
				Grid.SetRow(block, 2);
				Grid.SetColumn(nameBlock, 0);
				Grid.SetColumn(block, 1);

				SpecializationsGrid.Children.Add(nameBlock);
				SpecializationsGrid.Children.Add(block);
			}
			#endregion

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

				Binding binding = new Binding("SpecQuestingAmount")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding2 = new Binding("SpecQuestingThreshold")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding3 = new Binding("SpecQuestingBuff")
				{
					Source = User.Instance.CurrentHero.Specialization
				};

				MultiBinding multiBinding = new MultiBinding
				{
					StringFormat = " → Quest time -{2}%"
				};
				multiBinding.Bindings.Add(binding);
				multiBinding.Bindings.Add(binding2);
				multiBinding.Bindings.Add(binding3);

				block.SetBinding(TextBlock.TextProperty, multiBinding);

				Grid.SetRow(nameBlock, 3);
				Grid.SetRow(block, 3);
				Grid.SetColumn(nameBlock, 0);
				Grid.SetColumn(block, 1);

				SpecializationsGrid.Children.Add(nameBlock);
				SpecializationsGrid.Children.Add(block);
			}
			#endregion

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

				Binding binding = new Binding("SpecClickingAmount")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding2 = new Binding("SpecClickingThreshold")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding3 = new Binding("SpecClickingBuff")
				{
					Source = User.Instance.CurrentHero.Specialization
				};

				MultiBinding multiBinding = new MultiBinding
				{
					StringFormat = " → Click damage +{2}"
				};
				multiBinding.Bindings.Add(binding);
				multiBinding.Bindings.Add(binding2);
				multiBinding.Bindings.Add(binding3);

				block.SetBinding(TextBlock.TextProperty, multiBinding);

				Grid.SetRow(nameBlock, 4);
				Grid.SetRow(block, 4);
				Grid.SetColumn(nameBlock, 0);
				Grid.SetColumn(block, 1);

				SpecializationsGrid.Children.Add(nameBlock);
				SpecializationsGrid.Children.Add(block);
			}
			#endregion

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

				Binding binding = new Binding("SpecBlessingAmount")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding2 = new Binding("SpecBlessingThreshold")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding3 = new Binding("SpecBlessingBuff")
				{
					Source = User.Instance.CurrentHero.Specialization
				};

				MultiBinding multiBinding = new MultiBinding
				{
					StringFormat = " → Blessing duration +{2}s"
				};
				multiBinding.Bindings.Add(binding);
				multiBinding.Bindings.Add(binding2);
				multiBinding.Bindings.Add(binding3);

				block.SetBinding(TextBlock.TextProperty, multiBinding);

				Grid.SetRow(nameBlock, 5);
				Grid.SetRow(block, 5);
				Grid.SetColumn(nameBlock, 0);
				Grid.SetColumn(block, 1);

				SpecializationsGrid.Children.Add(nameBlock);
				SpecializationsGrid.Children.Add(block);
			}
			#endregion

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

				Binding binding = new Binding("SpecDungeonAmount")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding2 = new Binding("SpecDungeonThreshold")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding3 = new Binding("SpecDungeonBuff")
				{
					Source = User.Instance.CurrentHero.Specialization
				};

				MultiBinding multiBinding = new MultiBinding
				{
					StringFormat = " → Bossfight timer +{2}s"
				};
				multiBinding.Bindings.Add(binding);
				multiBinding.Bindings.Add(binding2);
				multiBinding.Bindings.Add(binding3);

				block.SetBinding(TextBlock.TextProperty, multiBinding);

				Grid.SetRow(nameBlock, 6);
				Grid.SetRow(block, 6);
				Grid.SetColumn(nameBlock, 0);
				Grid.SetColumn(block, 1);

				SpecializationsGrid.Children.Add(nameBlock);
				SpecializationsGrid.Children.Add(block);
			}
			#endregion
		}
	
		private void GenerateTooltips()
		{
			// Generate hero info tooltip.
			var tooltip = new ToolTip();

			ToolTipService.SetInitialShowDelay(HeroNameBlock, 400);
			ToolTipService.SetShowDuration(HeroNameBlock, 20000);
			ControlzEx.ToolTipAssist.SetAutoMove(tooltip, true);

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
		}
	}
}