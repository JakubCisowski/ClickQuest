using System;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.Extensions.UserInterface.ToolTips;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ClickQuest.Game.Extensions.Collections;
using MaterialDesignThemes.Wpf;
using ClickQuest.Game.Extensions.Items;
using ClickQuest.Game.UserInterface.Controls;

namespace ClickQuest.Game.UserInterface.Pages
{
	public partial class EquipmentPage : Page
	{
		private static int _selectedTabIndex;
		private static double _verticalOffset;

		private bool _artifactSetsComboBoxSelectionHandled = true;

		public EquipmentPage()
		{
			InitializeComponent();
			ReloadScrollbarOffset();
			UpdateAllEquipmentTabs();
		}

		private void ReloadScrollbarOffset()
		{
			if (_verticalOffset != 0)
			{
				switch (_selectedTabIndex)
				{
					case 0:
						MaterialsScrollViewer.ScrollToVerticalOffset(_verticalOffset);
						break;

					case 1:
						RecipesScrollViewer.ScrollToVerticalOffset(_verticalOffset);
						break;

					case 2:
						ArtifactsScrollViewer.ScrollToVerticalOffset(_verticalOffset);
						break;
				}

				_verticalOffset = 0;
			}
		}

		public void SaveScrollbarOffset()
		{
			switch (_selectedTabIndex)
			{
				case 0:
					_verticalOffset = MaterialsScrollViewer.VerticalOffset;
					break;

				case 1:
					_verticalOffset = RecipesScrollViewer.VerticalOffset;
					break;

				case 2:
					_verticalOffset = ArtifactsScrollViewer.VerticalOffset;
					break;
			}
		}

		public void UpdateAllEquipmentTabs()
		{
			// Refresh equipment.

			MaterialsPanel.Children.Clear();
			RecipesPanel.Children.Clear();
			ArtifactsPanel.Children.Clear();

			UpdateMaterialsEquipmentTab(User.Instance.CurrentHero?.Materials);
			UpdateRecipesEquipmentTab(User.Instance.CurrentHero?.Recipes);
			UpdateArtifactsEquipmentTab(User.Instance.CurrentHero?.Artifacts);

			RefreshEquippedArtifacts();
			RefreshArtifactSets();

			// Change ActiveTab to what was selected before.
			ChangeActiveTab();
		}

		public void ChangeActiveTab()
		{
			EquipmentTabControl.SelectedIndex = _selectedTabIndex;
		}

		public void RefreshCurrentEquipmentTab()
		{
			switch (_selectedTabIndex)
			{
				// Materials
				case 0:
					MaterialsPanel.Children.Clear();
					UpdateMaterialsEquipmentTab(User.Instance.CurrentHero?.Materials);
					break;
				
				// Recipes
				case 1:
					RecipesPanel.Children.Clear();
					UpdateRecipesEquipmentTab(User.Instance.CurrentHero?.Recipes);
					break;
				
				// Artifacts
				case 2:
					RefreshWholeArtifactsPanel();
					break;
			}
		}

		public void RefreshSpecificEquipmentTab(Type itemType)
		{
			if (itemType == typeof(Material) && _selectedTabIndex == 0)
			{
				MaterialsPanel.Children.Clear();
				UpdateMaterialsEquipmentTab(User.Instance.CurrentHero?.Materials);
			}
			else if (itemType == typeof(Recipe) && _selectedTabIndex == 1)
			{
				RecipesPanel.Children.Clear();
				UpdateRecipesEquipmentTab(User.Instance.CurrentHero?.Recipes);
			}
			else if (itemType == typeof(Artifact) && _selectedTabIndex == 2)
			{
				RefreshWholeArtifactsPanel();
			}
		}

		public void RefreshWholeArtifactsPanel()
		{
			ArtifactsPanel.Children.Clear();
			UpdateArtifactsEquipmentTab(User.Instance.CurrentHero?.Artifacts);
			RefreshEquippedArtifacts();
			RefreshArtifactSets();
		}

		private void UpdateMaterialsEquipmentTab(List<Material> materials)
		{
			if (materials is not null)
			{
				materials = materials.ReorderItemsInList();

				foreach (var material in materials)
				{
					var border = new Border
					{
						BorderThickness = new Thickness(2),
						BorderBrush = (SolidColorBrush)FindResource("BrushBlack"),
						Background = (SolidColorBrush) FindResource("BrushAccent1"),
						Padding = new Thickness(6),
						Margin = new Thickness(2),
						Tag = material,
						CornerRadius = new CornerRadius(3)
					};

					border.PreviewMouseUp += ItemBorder_TryToEquip;

					GeneralToolTipController.SetToolTipDelayAndDuration(border);

					var toolTip = ItemToolTipController.GenerateItemToolTip(material);
					border.ToolTip = toolTip;

					var grid = CreateSingleItemGrid(material);

					border.Child = grid;

					MaterialsPanel.Children.Add(border);
				}
				
				// Toggle ScrollBar visibility based on how many elements there are.
				if (MaterialsPanel.Children.Count > InterfaceController.EquipmentItemsNeededToShowScrollBar)
				{
					MaterialsScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
				}
				else
				{
					MaterialsScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
				}
			}
		}
		
		private void UpdateRecipesEquipmentTab(List<Recipe> recipes)
		{
			if (recipes is not null)
			{
				recipes = recipes.ReorderItemsInList();

				foreach (var recipe in recipes)
				{
					var border = new Border
					{
						BorderThickness = new Thickness(2),
						BorderBrush = (SolidColorBrush)FindResource("BrushBlack"),
						Background = (SolidColorBrush) FindResource("BrushAccent1"),
						Padding = new Thickness(6),
						Margin = new Thickness(2),
						Tag = recipe,
						CornerRadius = new CornerRadius(3)
					};

					border.PreviewMouseUp += ItemBorder_TryToEquip;

					GeneralToolTipController.SetToolTipDelayAndDuration(border);

					var toolTip = ItemToolTipController.GenerateItemToolTip(recipe);
					border.ToolTip = toolTip;

					var grid = CreateSingleItemGrid(recipe);

					border.Child = grid;

					RecipesPanel.Children.Add(border);
				}
				
				// Toggle ScrollBar visibility based on how many elements there are.
				if (RecipesPanel.Children.Count > InterfaceController.EquipmentItemsNeededToShowScrollBar)
				{
					RecipesScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
				}
				else
				{
					RecipesScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
				}
			}
		}
		
		private void UpdateArtifactsEquipmentTab(List<Artifact> artifacts)
		{
			if (artifacts is not null)
			{
				artifacts = artifacts.ReorderItemsInList();

				foreach (var artifact in artifacts)
				{
					// If the current Artifact is equipped, and its count is equal to 1 (so the only available Artifact is equipped), skip it
					// Otherwise, we'll create the block but with quantity of one less.
					if (User.Instance.CurrentHero.EquippedArtifacts.Contains(artifact) && artifact.Quantity==1)
					{
						continue;
					}
					
					var border = new Border
					{
						Name="Artifact" + artifact.Id+"ItemBorder",
						BorderThickness = new Thickness(2),
						BorderBrush = (SolidColorBrush)FindResource("BrushBlack"),
						Background = (SolidColorBrush) FindResource("BrushAccent1"),
						Padding = new Thickness(6),
						Margin = new Thickness(2),
						Tag = artifact,
						CornerRadius = new CornerRadius(3)
					};

					border.PreviewMouseUp += ItemBorder_TryToEquip;

					GeneralToolTipController.SetToolTipDelayAndDuration(border);

					var toolTip = ItemToolTipController.GenerateItemToolTip(artifact);
					border.ToolTip = toolTip;

					var grid = CreateArtifactGrid(artifact, User.Instance.CurrentHero.EquippedArtifacts.Contains(artifact));

					border.Child = grid;

					ArtifactsPanel.Children.Add(border);
				}
				
				// Toggle ScrollBar visibility based on how many elements there are.
				if (ArtifactsPanel.Children.Count > InterfaceController.EquipmentItemsNeededToShowScrollBar || (ArtifactsPanel.Children.Count > InterfaceController.EquipmentItemsNeededToShowScrollBarIfArtifactsAreEquipped && User.Instance.CurrentHero.EquippedArtifacts.Count > 0))
				{
					ArtifactsScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
				}
				else
				{
					ArtifactsScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
				}
			}
		}

		private void ItemBorder_TryToEquip(object sender, MouseButtonEventArgs e)
		{
			object item = (sender as Border).Tag;

			if (item is Artifact artifact)
			{
				bool isEquipped = User.Instance.CurrentHero.EquippedArtifacts.Contains(artifact);
				bool equippedArtifactsChanged = false;

				if (!isEquipped)
				{
					bool canBeEquipped = artifact.ArtifactFunctionality.CanBeEquipped();

					if (canBeEquipped)
					{
						User.Instance.CurrentHero.EquippedArtifacts.Add(artifact);
						User.Instance.CurrentHero.ArtifactSets.FirstOrDefault(x=>x.Id == User.Instance.CurrentHero.CurrentArtifactSetId).ArtifactIds.Add(artifact.Id);
						artifact.ArtifactFunctionality.OnEquip();
						(sender as Border).Background = FindResource("BrushAccent3") as SolidColorBrush;
						(sender as Border).BorderBrush = FindResource("BrushBlack") as SolidColorBrush;

						equippedArtifactsChanged = true;
					}
				}
				else
				{
					// If the player clicked on the Equipped item border (in case they have 2 of this item and only 1 is equipped).
					if ((sender as Border).Name.StartsWith("Equipped"))
					{
						bool canBeUnequipped = artifact.ArtifactFunctionality.CanBeUnequipped();

						if (canBeUnequipped)
						{
							User.Instance.CurrentHero.EquippedArtifacts.Remove(artifact);
							User.Instance.CurrentHero.ArtifactSets.FirstOrDefault(x=>x.Id == User.Instance.CurrentHero.CurrentArtifactSetId).ArtifactIds.Remove(artifact.Id);
							artifact.ArtifactFunctionality.OnUnequip();
							(sender as Border).Background = FindResource("BrushAccent1") as SolidColorBrush;
							(sender as Border).BorderBrush = FindResource("BrushBlack") as SolidColorBrush;


							equippedArtifactsChanged = true;
						}
					}
				}

				// Refresh Artifacts tab.
				if (equippedArtifactsChanged)
				{
					RefreshWholeArtifactsPanel();

					InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
					ArtifactsScrollViewer.ScrollToTop();
				}
			}
		}

		public void RefreshEquippedArtifacts()
		{
			if (User.Instance.CurrentHero is null)
			{
				return;
			}

			// Add "Equipped" block.
			var equippedTextBlock = new TextBlock()
			{
				Text = "Equipped",
				FontSize = 20,
				TextAlignment = TextAlignment.Center,
				FontFamily = (FontFamily) this.FindResource("FontRegularItalic"),
				Margin = new Thickness(5)
			};
			ArtifactsPanel.Children.Insert(0, equippedTextBlock);

			// Add separator between equipped and not-equipped Artifacts.
			var separator = new Separator()
			{
				Height = 2,
				Width = 200,
				Margin = new Thickness(10)
			};
			ArtifactsPanel.Children.Insert(1, separator);

			if (User.Instance.CurrentHero.EquippedArtifacts.Count == 0)
			{
				var noArtifactsEquippedTextBlock = new TextBlock()
				{
					Text = "You have no Artifacts equipped in this set\nEquip Artifacts by clicking them in this tab",
					FontSize = 16,
					TextAlignment = TextAlignment.Center,
					FontFamily = (FontFamily) this.FindResource("FontRegularItalic"),
					Margin = new Thickness(5),
					TextWrapping = TextWrapping.Wrap
				};
				ArtifactsPanel.Children.Insert(1, noArtifactsEquippedTextBlock);

				return;
			}

			var equippedArtifacts = User.Instance.CurrentHero.EquippedArtifacts;

			// Add Equipped Artifacts borders (no quantity) with a different style.
			for (int i = 0; i < User.Instance.CurrentHero.EquippedArtifacts.Count; i++)
			{
				var equippedArtifact = equippedArtifacts[i];
				
				var border = new Border
				{
					Name="EquippedArtifact" + equippedArtifact.Id +"ItemBorder",
					BorderThickness = new Thickness(2),
					BorderBrush = (SolidColorBrush)FindResource("BrushBlack"),
					Background = (SolidColorBrush)FindResource("BrushAccent3"),
					Padding = new Thickness(6),
					Margin = new Thickness(2),
					Tag = equippedArtifact
				};

				border.PreviewMouseUp += ItemBorder_TryToEquip;

				GeneralToolTipController.SetToolTipDelayAndDuration(border);

				var toolTip = ItemToolTipController.GenerateItemToolTip(equippedArtifact);
				border.ToolTip = toolTip;

				var grid = CreateEquippedArtifactGrid(equippedArtifact);

				border.Child = grid;

				ArtifactsPanel.Children.Insert(1 + i, border);
			}
		}

		public void RefreshArtifactSets()
		{
			if (User.Instance.CurrentHero is null)
			{
				return;
			}

			if (LogicalTreeHelper.FindLogicalNode(ArtifactsPanel, "ArtifactSetsGrid") is Grid oldGrid)
			{
				// Remove Grid from ArtifactsPanel, then clear its children,
				ArtifactsPanel.Children.Remove(oldGrid);
				oldGrid.Children.Clear();
			}

			var artifactSetsGrid = new Grid()
			{
				Name = "ArtifactSetsGrid",
				Margin = new Thickness(0,10,0,10)
			};

			var addButton = new Button()
			{
				HorizontalAlignment = HorizontalAlignment.Right,
				Margin = new Thickness(0, 0, 75, 0),
				Content = new PackIcon(){Kind = PackIconKind.PlusThick}
			};
			addButton.Click += AddArtifactSet_Click;

			var removeButton = new Button()
			{
				HorizontalAlignment = HorizontalAlignment.Right,
				Margin = new Thickness(0, 0, 5, 0),
				Content = new PackIcon(){Kind = PackIconKind.DeleteForever}
			};
			removeButton.Click += RemoveArtifactSet_Click;

			var renameButton = new Button()
			{
				HorizontalAlignment = HorizontalAlignment.Right,
				Margin = new Thickness(0, 0, 40, 0),
				Content = new PackIcon(){Kind = PackIconKind.Pencil}
			};
			renameButton.Click += RenameArtifactSet_Click;

			var artifactSetsComboBox = new ComboBox()
			{
				Name = "ArtifactSetsComboBox",
				HorizontalAlignment = HorizontalAlignment.Left,
				Style = (Style)this.FindResource("MaterialDesignComboBox"),
				ItemsSource = User.Instance.CurrentHero.ArtifactSets.Select(x=>x.Name),
				SelectedIndex = User.Instance.CurrentHero.CurrentArtifactSetId,
				Margin = new Thickness(5,0,0,0),
				Width = 150
			};

			artifactSetsComboBox.SelectionChanged += ArtifactSetsComboBox_SelectionChanged;

			ComboBoxAssist.SetShowSelectedItem(artifactSetsComboBox, true);

			artifactSetsGrid.Children.Add(addButton);
			artifactSetsGrid.Children.Add(removeButton);
			artifactSetsGrid.Children.Add(renameButton);
			artifactSetsGrid.Children.Add(artifactSetsComboBox);

			ArtifactsPanel.Children.Insert(1, artifactSetsGrid);
		}
		
		private void AddArtifactSet_Click(object sender, RoutedEventArgs e)
		{
			if (!ArtifactSetsController.CanArtifactSetsBeChanged())
			{
				AlertBox.Show("Artifact Sets cannot be changed while in combat or questing", MessageBoxButton.OK);
				return;
			}

			var newId = User.Instance.CurrentHero.ArtifactSets.Max(x=>x.Id)+1;
			User.Instance.CurrentHero.ArtifactSets.Add(new ArtifactSet(){Id = newId, Name = "Artifact Set " + newId.ToString()});

			RefreshWholeArtifactsPanel();

			var oldGrid = LogicalTreeHelper.FindLogicalNode(ArtifactsPanel, "ArtifactSetsGrid") as Grid;
			var artifactSetsComboBox = LogicalTreeHelper.FindLogicalNode(oldGrid, "ArtifactSetsComboBox") as ComboBox;
			artifactSetsComboBox.SelectedItem = "Artifact Set " + newId.ToString();
		}

		private void RenameArtifactSet_Click(object sender, RoutedEventArgs e)
		{
			var oldGrid = LogicalTreeHelper.FindLogicalNode(ArtifactsPanel, "ArtifactSetsGrid") as Grid;
			var artifactSetsComboBox = LogicalTreeHelper.FindLogicalNode(oldGrid, "ArtifactSetsComboBox") as ComboBox;
			var selectedName = artifactSetsComboBox.SelectedItem.ToString();

			var newName = RenameBox.Show(selectedName);
			
			User.Instance.CurrentHero.ArtifactSets.FirstOrDefault(x=>x.Name==selectedName).Name = newName;

			RefreshWholeArtifactsPanel();
		}

		private void RemoveArtifactSet_Click(object sender, RoutedEventArgs e)
		{
			if(User.Instance.CurrentHero.ArtifactSets.Count > 1)
			{
				if (!ArtifactSetsController.CanArtifactSetsBeChanged())
				{
					AlertBox.Show("Artifact Sets cannot be changed while in combat or questing", MessageBoxButton.OK);
					return;
				}

				var removedSetId = User.Instance.CurrentHero.CurrentArtifactSetId;

				var firstId = User.Instance.CurrentHero.ArtifactSets.Min(x=>x.Id);
				var firstName = User.Instance.CurrentHero.ArtifactSets.FirstOrDefault(x=>x.Id==firstId).Name;
				
				var oldGrid = LogicalTreeHelper.FindLogicalNode(ArtifactsPanel, "ArtifactSetsGrid") as Grid;
				var artifactSetsComboBox = LogicalTreeHelper.FindLogicalNode(oldGrid, "ArtifactSetsComboBox") as ComboBox;
				artifactSetsComboBox.SelectedItem = firstName;
				
				User.Instance.CurrentHero.ArtifactSets.RemoveAt(removedSetId);

				RefreshWholeArtifactsPanel();
			}
			else
			{
				AlertBox.Show("You cannot delete the only Artifact Set - you must always have at least one.", MessageBoxButton.OK);
			}
		}

		private void ArtifactSetsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_artifactSetsComboBoxSelectionHandled)
			{
				if (!ArtifactSetsController.CanArtifactSetsBeChanged())
				{
					AlertBox.Show("Artifact Sets cannot be changed while in combat or questing", MessageBoxButton.OK);

					var comboBox = sender as ComboBox;
					_artifactSetsComboBoxSelectionHandled = false;
					comboBox.SelectedItem = e.RemovedItems[0];

					return;
				}

				var artifactSetName = e.AddedItems[0].ToString();

				var artifactSetId = User.Instance.CurrentHero.ArtifactSets.FirstOrDefault(x=>x.Name == artifactSetName).Id;

				ArtifactSetsController.SwitchArtifactSet(artifactSetId);

				// Refresh the entire panel.
				RefreshWholeArtifactsPanel();
				ArtifactsScrollViewer.ScrollToTop();
			}

			_artifactSetsComboBoxSelectionHandled = true;
		}

		private Grid CreateSingleItemGrid(Item item)
		{
			var grid = new Grid();

			var circleIcon = new PackIcon
			{
				Kind = PackIconKind.Circle,
				Width = 15,
				Height = 15,
				VerticalAlignment = VerticalAlignment.Center,
				Foreground = ColorsController.GetRarityColor(item.Rarity)
			};

			var nameBlock = new TextBlock
			{
				FontSize = 18,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(20,0,0,0),
				Text = item.Name
			};

			grid.Children.Add(circleIcon);
			grid.Children.Add(nameBlock);
			
			if (item.Quantity > 1)
			{
				var quantityBlock = new TextBlock
				{
					FontSize = 18,
					HorizontalAlignment = HorizontalAlignment.Right,
					Text="x"+item.Quantity
				};
				
				grid.Children.Add(quantityBlock);
			}

			return grid;
		}
		
		private Grid CreateEquippedArtifactGrid(Artifact artifact)
		{
			var grid = new Grid();

			var circleIcon = new PackIcon
			{
				Kind = PackIconKind.Circle,
				Width = 15,
				Height = 15,
				VerticalAlignment = VerticalAlignment.Center,
				Foreground = ColorsController.GetRarityColor(artifact.Rarity)
			};

			var nameBlock = new TextBlock
			{
				FontSize = 18,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(20,0,0,0),
				Text = artifact.Name
			};

			grid.Children.Add(circleIcon);
			grid.Children.Add(nameBlock);

			return grid;
		}

		private Grid CreateArtifactGrid(Artifact artifact, bool isAlreadyEquipped)
		{
			// If the Artifact is already equipped (but this exact one isn't), treat it as if it had 1 less quantity (but don't display quantity if it's 1).
			// If not and quantity is equal to 1, do not display its quantity.
			
			var grid = new Grid();

			var circleIcon = new PackIcon
			{
				Kind = PackIconKind.Circle,
				Width = 15,
				Height = 15,
				VerticalAlignment = VerticalAlignment.Center,
				Foreground = ColorsController.GetRarityColor(artifact.Rarity)
			};

			var nameBlock = new TextBlock
			{
				FontSize = 18,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(20,0,0,0),
				Text = artifact.Name
			};
			
			grid.Children.Add(circleIcon);
			grid.Children.Add(nameBlock);

			if (isAlreadyEquipped && artifact.Quantity > 2)
			{
				var quantityBlock = new TextBlock
				{
					FontSize = 18,
					HorizontalAlignment = HorizontalAlignment.Right,
					Text="x"+(artifact.Quantity - 1)
				};
				
				grid.Children.Add(quantityBlock);
			}
			else if (!isAlreadyEquipped && artifact.Quantity > 1)
			{
				var quantityBlock = new TextBlock
				{
					FontSize = 18,
					HorizontalAlignment = HorizontalAlignment.Right,
					Text="x"+artifact.Quantity
				};
				
				grid.Children.Add(quantityBlock);
			}

			return grid;
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.Source is TabControl)
			{
				// Save tab selection (to set it after updating equipment page).
				_selectedTabIndex = EquipmentTabControl.SelectedIndex;
				_verticalOffset = 0;
				
				// Update the tab that is being selected.
				RefreshCurrentEquipmentTab();
			}
		}
	}
}