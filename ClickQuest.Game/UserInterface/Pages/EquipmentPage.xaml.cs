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

			UpdateEquipmentTab(User.Instance.CurrentHero?.Materials, MaterialsPanel);
			UpdateEquipmentTab(User.Instance.CurrentHero?.Recipes, RecipesPanel);
			UpdateEquipmentTab(User.Instance.CurrentHero?.Artifacts, ArtifactsPanel);

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
					UpdateEquipmentTab(User.Instance.CurrentHero?.Materials, MaterialsPanel);
					break;
				
				// Recipes
				case 1:
					RecipesPanel.Children.Clear();
					UpdateEquipmentTab(User.Instance.CurrentHero?.Recipes, RecipesPanel);
					break;
				
				// Artifacts
				case 2:
					ArtifactsPanel.Children.Clear();
					UpdateEquipmentTab(User.Instance.CurrentHero?.Artifacts, ArtifactsPanel);
					RefreshEquippedArtifacts();
					break;
			}
		}

		public void RefreshSpecificEquipmentTab(Type itemType)
		{
			if (itemType == typeof(Material) && _selectedTabIndex == 0)
			{
				MaterialsPanel.Children.Clear();
				UpdateEquipmentTab(User.Instance.CurrentHero?.Materials, MaterialsPanel);
			}
			else if (itemType == typeof(Recipe) && _selectedTabIndex == 1)
			{
				RecipesPanel.Children.Clear();
				UpdateEquipmentTab(User.Instance.CurrentHero?.Recipes, RecipesPanel);
			}
			else if (itemType == typeof(Artifact) && _selectedTabIndex == 2)
			{
				ArtifactsPanel.Children.Clear();
				UpdateEquipmentTab(User.Instance.CurrentHero?.Artifacts, ArtifactsPanel);
				RefreshEquippedArtifacts();
			}
		}

		private void UpdateEquipmentTab<T>(List<T> specificEquipmentCollection, StackPanel equipmentTabPanel) where T : Item
		{
			if (specificEquipmentCollection != null)
			{
				specificEquipmentCollection = specificEquipmentCollection.ReorderItemsInList();

				foreach (var item in specificEquipmentCollection)
				{
					var border = new Border
					{
						BorderThickness = new Thickness(2),
						BorderBrush = (SolidColorBrush)FindResource("BrushAccent3"),
						Background = (SolidColorBrush) FindResource("BrushAccent1"),
						Padding = new Thickness(6),
						Margin = new Thickness(2),
						Tag = item
					};

					border.PreviewMouseUp += ItemBorder_TryToEquip;

					GeneralToolTipController.SetToolTipDelayAndDuration(border);

					var toolTip = ItemToolTipController.GenerateItemToolTip(item);
					border.ToolTip = toolTip;

					var grid = CreateSingleItemGrid(item);

					border.Child = grid;

					equipmentTabPanel.Children.Add(border);
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
						(sender as Border).BorderBrush = FindResource("BrushAccent3") as SolidColorBrush;

						equippedArtifactsChanged = true;
					}
				}
				else if (isEquipped)
				{
					bool canBeUnequipped = artifact.ArtifactFunctionality.CanBeUnequipped();

					if (canBeUnequipped)
					{
						User.Instance.CurrentHero.EquippedArtifacts.Remove(artifact);
						artifact.ArtifactFunctionality.OnUnequip();
						(sender as Border).Background = FindResource("BrushAccent1") as SolidColorBrush;
						(sender as Border).BorderBrush = FindResource("BrushAccent3") as SolidColorBrush;


						equippedArtifactsChanged = true;
					}
				}

				// Refresh Artifacts tab.
				if (equippedArtifactsChanged)
				{
					ArtifactsPanel.Children.Clear();
					UpdateEquipmentTab(User.Instance.CurrentHero?.Artifacts, ArtifactsPanel);
					RefreshEquippedArtifacts();
					InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
					ArtifactsScrollViewer.ScrollToTop();
				}
			}
		}

		public void RefreshEquippedArtifacts()
		{
			foreach (Border artifactBorder in ArtifactsPanel.Children)
			{
				var artifact = artifactBorder.Tag as Artifact;
				if (User.Instance.CurrentHero.EquippedArtifacts.Contains(artifact))
				{
					artifactBorder.Background = FindResource("BrushAccent3") as SolidColorBrush;
					artifactBorder.BorderBrush = FindResource("BrushAccent3") as SolidColorBrush;
				}
			}
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
				Margin = new Thickness(20,0,0,0)
			};

			var quantityBlock = new TextBlock
			{
				FontSize = 18,
				HorizontalAlignment = HorizontalAlignment.Right
			};

			var binding = new Binding("Name")
			{
				Source = item,
				StringFormat = "{0}"
			};

			var binding2 = new Binding("Quantity")
			{
				Source = item,
				StringFormat = "x{0}"
			};

			nameBlock.SetBinding(TextBlock.TextProperty, binding);
			quantityBlock.SetBinding(TextBlock.TextProperty, binding2);

			grid.Children.Add(circleIcon);
			grid.Children.Add(nameBlock);
			grid.Children.Add(quantityBlock);

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