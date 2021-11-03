using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.UserInterface;

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
			UpdateEquipment();
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

		public void UpdateEquipment()
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
			EquipmentTabControl.SelectedIndex = _selectedTabIndex;
		}

		private void UpdateEquipmentTab<T>(List<T> specificEquipmentCollection, StackPanel equipmentTabPanel) where T : Item
		{
			if (specificEquipmentCollection != null)
			{
				ReorderItemsInList(ref specificEquipmentCollection);

				foreach (var item in specificEquipmentCollection)
				{
					var border = new Border
					{
						BorderThickness = new Thickness(0.5),
						BorderBrush = new SolidColorBrush(Colors.Gray),
						Padding = new Thickness(6),
						Margin = new Thickness(4),
						Background = FindResource("BrushGame3") as SolidColorBrush,
						Tag = item
					};

					border.PreviewMouseUp += ItemBorder_TryToEquip;

					TooltipController.SetTooltipDelayAndDuration(border);

					var toolTip = TooltipController.GenerateEquipmentItemTooltip(item);
					border.ToolTip = toolTip;

					var grid = CreateSingleItemGrid(item);

					border.Child = grid;

					equipmentTabPanel.Children.Add(border);
				}
			}
		}

		private void ReorderItemsInList<T>(ref List<T> specificEquipmentCollection) where T : Item
		{
			// 1. Items should be ordered based on (name / rarity / type / something else) - currently Name.
			// 2. Equipped Artifacts should be at the top.

			var orderedItemsList = specificEquipmentCollection.OrderByDescending(x => User.Instance.CurrentHero.EquippedArtifacts.Contains(x as Artifact)).ThenBy(y => y.Name).ToList();
			specificEquipmentCollection = orderedItemsList;
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
						(sender as Border).Background = FindResource("BrushGame2") as SolidColorBrush;

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
						(sender as Border).Background = FindResource("BrushGame3") as SolidColorBrush;

						equippedArtifactsChanged = true;
					}
				}

				// Refresh Artifacts tab.
				if (equippedArtifactsChanged)
				{
					ArtifactsPanel.Children.Clear();
					UpdateEquipmentTab(User.Instance.CurrentHero?.Artifacts, ArtifactsPanel);
					RefreshEquippedArtifacts();
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
					artifactBorder.Background = FindResource("BrushGame2") as SolidColorBrush;
				}
			}
		}

		private Grid CreateSingleItemGrid(Item item)
		{
			var grid = new Grid();

			var nameBlock = new TextBlock
			{
				FontSize = 18,
				HorizontalAlignment = HorizontalAlignment.Left
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
			}
		}
	}
}