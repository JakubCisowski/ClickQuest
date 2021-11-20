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

namespace ClickQuest.Game.UserInterface.Pages
{
	public partial class EquipmentPage : Page
	{
		private static int _selectedTabIndex;
		private static double _verticalOffset;

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
					ArtifactsPanel.Children.Clear();
					UpdateArtifactsEquipmentTab(User.Instance.CurrentHero?.Artifacts);
					RefreshEquippedArtifacts();
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
				ArtifactsPanel.Children.Clear();
				UpdateArtifactsEquipmentTab(User.Instance.CurrentHero?.Artifacts);
				RefreshEquippedArtifacts();
			}
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
				if (MaterialsPanel.Children.Count > 15)
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
				if (RecipesPanel.Children.Count > 15)
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
				if (ArtifactsPanel.Children.Count > 15)
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
					ArtifactsPanel.Children.Clear();
					UpdateArtifactsEquipmentTab(User.Instance.CurrentHero?.Artifacts);
					RefreshEquippedArtifacts();
					
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

			if (User.Instance.CurrentHero.EquippedArtifacts.Count == 0)
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

			var orderedEquippedArtifacts = User.Instance.CurrentHero.EquippedArtifacts.OrderBy(x => x.Name);

			// Add Equipped Artifacts borders (no quantity) with a different style.
			for (int i = 0; i < User.Instance.CurrentHero.EquippedArtifacts.Count; i++)
			{
				var equippedArtifact = orderedEquippedArtifacts.ElementAt(i);
				
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

				ArtifactsPanel.Children.Insert(1+i, border);
			}
			
			// Add separator between equipped and not-equipped Artifacts.
			var separator = new Separator()
			{
				Height = 2,
				Width = 200,
				Margin = new Thickness(10)
			};
			ArtifactsPanel.Children.Insert(User.Instance.CurrentHero.EquippedArtifacts.Count + 1, separator);
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
			
			var quantityBlock = new TextBlock
			{
				FontSize = 18,
				HorizontalAlignment = HorizontalAlignment.Right,
				Text="x"+item.Quantity
			};

			grid.Children.Add(circleIcon);
			grid.Children.Add(nameBlock);
			grid.Children.Add(quantityBlock);

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