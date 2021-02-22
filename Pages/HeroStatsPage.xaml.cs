using ClickQuest.Account;
using ClickQuest.Heroes;
using MaterialDesignThemes.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

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
		}

		private void GenerateGold()
		{
			// Create a binding for the amount of Gold.
			Binding binding = new Binding("Gold")
			{
				Source = User.Instance,
				StringFormat = "Gold: {0}"
			};

			GoldBlock.SetBinding(TextBlock.TextProperty, binding);
		}

		private void GenerateIngots()
		{
			// Make sure hero isn't null (constructor calls this function while loading database).
			if (_hero != null)
			{
				IngotKeyGrid.Children.Clear();

				for (int i = 0; i < User.Instance.Ingots.Count; i++)
				{
					var panel = new StackPanel()
					{
						Orientation = Orientation.Horizontal,
						Margin = new Thickness(50, 0, 0, 0)
					};

					var icon = new PackIcon()
					{
						Kind = PackIconKind.Gold,
						Width = 22,
						Height = 22,
						VerticalAlignment = VerticalAlignment.Center
					};

					switch (i)
					{
						case 0:
							icon.Foreground = new SolidColorBrush(Colors.Gray);
							break;

						case 1:
							icon.Foreground = new SolidColorBrush(Colors.Brown);
							break;

						case 2:
							icon.Foreground = new SolidColorBrush(Colors.Green);
							break;

						case 3:
							icon.Foreground = new SolidColorBrush(Colors.Blue);
							break;

						case 4:
							icon.Foreground = new SolidColorBrush(Colors.Purple);
							break;

						case 5:
							icon.Foreground = new SolidColorBrush(Colors.Gold);
							break;
					}

					panel.Children.Add(icon);

					var block = new TextBlock()
					{
						Name = "Ingot" + i.ToString(),
						FontSize = 18,
						VerticalAlignment = VerticalAlignment.Center
					};

					Binding binding = new Binding("Quantity")
					{
						Source = User.Instance.Ingots[i]
					};
					// Binding binding2 = new Binding("Rarity");
					// binding2.Source = Account.User.Instance.Ingots[i];

					MultiBinding multiBinding = new MultiBinding
					{
						StringFormat = "   {0}"
					};
					multiBinding.Bindings.Add(binding);
					// multiBinding.Bindings.Add(binding2);

					block.SetBinding(TextBlock.TextProperty, multiBinding);

					panel.Children.Add(block);

					IngotKeyGrid.Children.Add(panel);

					Grid.SetColumn(panel, 0);
					Grid.SetRow(panel, i);
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
					var panel = new StackPanel()
					{
						Orientation = Orientation.Horizontal,
						Margin = new Thickness(50, 0, 0, 0)
					};

					var icon = new PackIcon()
					{
						Kind = PackIconKind.Key,
						Width = 22,
						Height = 22,
						VerticalAlignment = VerticalAlignment.Center
					};

					switch (i)
					{
						case 0:
							icon.Foreground = new SolidColorBrush(Colors.Gray);
							break;

						case 1:
							icon.Foreground = new SolidColorBrush(Colors.Brown);
							break;

						case 2:
							icon.Foreground = new SolidColorBrush(Colors.Green);
							break;

						case 3:
							icon.Foreground = new SolidColorBrush(Colors.Blue);
							break;

						case 4:
							icon.Foreground = new SolidColorBrush(Colors.Purple);
							break;

						case 5:
							icon.Foreground = new SolidColorBrush(Colors.Gold);
							break;
					}

					panel.Children.Add(icon);

					var block = new TextBlock()
					{
						Name = "Key" + i.ToString(),
						FontSize = 18,
						VerticalAlignment = VerticalAlignment.Center
					};

					Binding binding = new Binding("Quantity")
					{
						Source = User.Instance.DungeonKeys[i]
					};
					// Binding binding2 = new Binding("Rarity");
					// binding2.Source = Account.User.Instance.DungeonKeys[i];

					MultiBinding multiBinding = new MultiBinding
					{
						StringFormat = "   {0}"
					};
					multiBinding.Bindings.Add(binding);
					// multiBinding.Bindings.Add(binding2);

					block.SetBinding(TextBlock.TextProperty, multiBinding);

					panel.Children.Add(block);

					IngotKeyGrid.Children.Add(panel);

					Grid.SetColumn(panel, 1);
					Grid.SetRow(panel, i);
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

			#region SpecializationKilling
			{
				var nameBlock = new TextBlock()
				{
					Name = "SpedKillingName",
					Text = "Clicker",
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				var block = new TextBlock()
				{
					Name = "SpecKilling",
					FontSize = 18,
					Margin = new Thickness(0, 1, 0, 1)
				};

				Binding binding = new Binding("SpecKillingAmount")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding2 = new Binding("SpecKillingThreshold")
				{
					Source = User.Instance.CurrentHero.Specialization
				};
				Binding binding3 = new Binding("SpecKillingBuff")
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
	}
}