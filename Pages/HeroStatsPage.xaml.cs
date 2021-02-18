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

			_hero = Account.User.Instance.CurrentHero;
			this.DataContext = _hero;

			GenerateIngots();
			GenerateDungeonKeys();
			GenerateGold();
			GenerateSpecializations();
			RefreshQuestTimer();
		}

		private void GenerateGold()
		{
			// Create a binding for the amount of Gold.
			Binding binding = new Binding("Gold");
			binding.Source = Account.User.Instance;
			binding.StringFormat = "Gold: {0}";

			GoldBlock.SetBinding(TextBlock.TextProperty, binding);
		}

		private void GenerateIngots()
		{
			// Make sure hero isn't null (constructor calls this function while loading database).
			if (_hero != null)
			{
				IngotKeyGrid.Children.Clear();

				for (int i = 0; i < Account.User.Instance.Ingots.Count; i++)
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

					Binding binding = new Binding("Quantity");
					binding.Source = Account.User.Instance.Ingots[i];
					// Binding binding2 = new Binding("Rarity");
					// binding2.Source = Account.User.Instance.Ingots[i];

					MultiBinding multiBinding = new MultiBinding();
					multiBinding.StringFormat = "   {0}";
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
				for (int i = 0; i < Account.User.Instance.DungeonKeys.Count; i++)
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

					Binding binding = new Binding("Quantity");
					binding.Source = Account.User.Instance.DungeonKeys[i];
					// Binding binding2 = new Binding("Rarity");
					// binding2.Source = Account.User.Instance.DungeonKeys[i];

					MultiBinding multiBinding = new MultiBinding();
					multiBinding.StringFormat = "   {0}";
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
			var quest = Account.User.Instance.CurrentHero?.Quests.FirstOrDefault(x => x.EndDate != default(DateTime));

			if (quest != null)
			{
				// Bind its duration to the panel.
				var binding = new Binding("TicksCountText");
				binding.Source = quest;
				testQuestDuration.SetBinding(TextBlock.TextProperty, binding);
			}
		}

		private void GenerateSpecializations()
		{
			if (Account.User.Instance.CurrentHero is null)
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

				Binding binding = new Binding("SpecBuyingAmount");
				binding.Source = Account.User.Instance.CurrentHero.Specialization;
				Binding binding2 = new Binding("SpecBuyingThreshold");
				binding2.Source = Account.User.Instance.CurrentHero.Specialization;
				Binding binding3 = new Binding("SpecBuyingBuff");
				binding3.Source = Account.User.Instance.CurrentHero.Specialization;

				MultiBinding multiBinding = new MultiBinding();
				multiBinding.StringFormat = " → Shop offer size +{2}";
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

				Binding binding = new Binding("SpecMeltingAmount");
				binding.Source = Account.User.Instance.CurrentHero.Specialization;
				Binding binding2 = new Binding("SpecMeltingThreshold");
				binding2.Source = Account.User.Instance.CurrentHero.Specialization;
				Binding binding3 = new Binding("SpecMeltingBuff");
				binding3.Source = Account.User.Instance.CurrentHero.Specialization;

				MultiBinding multiBinding = new MultiBinding();
				multiBinding.StringFormat = " → Extra ingot +{2}%";
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

				Binding binding = new Binding("SpecCraftingAmount");
				binding.Source = Account.User.Instance.CurrentHero.Specialization;
				Binding binding2 = new Binding("SpecCraftingThreshold");
				binding2.Source = Account.User.Instance.CurrentHero.Specialization;
				Binding binding3 = new Binding("SpecCraftingBuff");
				binding3.Source = Account.User.Instance.CurrentHero.Specialization;
				Binding binding4 = new Binding("SpecCraftingText");
				binding4.Source = Account.User.Instance.CurrentHero.Specialization;

				MultiBinding multiBinding = new MultiBinding();
				multiBinding.StringFormat = " → Can craft {3} recipes";
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

				Binding binding = new Binding("SpecQuestingAmount");
				binding.Source = Account.User.Instance.CurrentHero.Specialization;
				Binding binding2 = new Binding("SpecQuestingThreshold");
				binding2.Source = Account.User.Instance.CurrentHero.Specialization;
				Binding binding3 = new Binding("SpecQuestingBuff");
				binding3.Source = Account.User.Instance.CurrentHero.Specialization;

				MultiBinding multiBinding = new MultiBinding();
				multiBinding.StringFormat = " → Quest time -{2}%";
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

				Binding binding = new Binding("SpecKillingAmount");
				binding.Source = Account.User.Instance.CurrentHero.Specialization;
				Binding binding2 = new Binding("SpecKillingThreshold");
				binding2.Source = Account.User.Instance.CurrentHero.Specialization;
				Binding binding3 = new Binding("SpecKillingBuff");
				binding3.Source = Account.User.Instance.CurrentHero.Specialization;

				MultiBinding multiBinding = new MultiBinding();
				multiBinding.StringFormat = " → Click damage +{2}";
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

				Binding binding = new Binding("SpecBlessingAmount");
				binding.Source = Account.User.Instance.CurrentHero.Specialization;
				Binding binding2 = new Binding("SpecBlessingThreshold");
				binding2.Source = Account.User.Instance.CurrentHero.Specialization;
				Binding binding3 = new Binding("SpecBlessingBuff");
				binding3.Source = Account.User.Instance.CurrentHero.Specialization;

				MultiBinding multiBinding = new MultiBinding();
				multiBinding.StringFormat = " → Blessing duration +{2}s";
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

				Binding binding = new Binding("SpecDungeonAmount");
				binding.Source = Account.User.Instance.CurrentHero.Specialization;
				Binding binding2 = new Binding("SpecDungeonThreshold");
				binding2.Source = Account.User.Instance.CurrentHero.Specialization;
				Binding binding3 = new Binding("SpecDungeonBuff");
				binding3.Source = Account.User.Instance.CurrentHero.Specialization;

				MultiBinding multiBinding = new MultiBinding();
				multiBinding.StringFormat = " → Bossfight timer +{2}s";
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