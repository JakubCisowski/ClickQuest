using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Pages
{
	public partial class EquipmentPage : Page
	{
		private static int _selectedTabIndex;

		public EquipmentPage()
		{
			InitializeComponent();
			UpdateEquipment();
		}

		public void UpdateEquipment()
		{
			// Refresh equipment.

			MaterialsPanel.Children.Clear();
			RecipesPanel.Children.Clear();
			ArtifactsPanel.Children.Clear();

			UpdateEquipmentTab(User.Instance.CurrentHero?.Materials, MaterialsPanel);
			UpdateEquipmentTab(User.Instance.CurrentHero?.Artifacts, ArtifactsPanel);
			UpdateEquipmentTab(User.Instance.CurrentHero?.Recipes, RecipesPanel);

			// Change ActiveTab to what was selected before.
			EquipmentTabControl.SelectedIndex = _selectedTabIndex;
		}

		private void UpdateEquipmentTab<T>(List<T> specificEquipmentCollection, StackPanel equipmentTabPanel) where T : Item
		{
			if (specificEquipmentCollection != null)
			{
				foreach (var item in specificEquipmentCollection)
				{
					var border = new Border
					{
						BorderThickness = new Thickness(0.5),
						BorderBrush = new SolidColorBrush(Colors.Gray),
						Padding = new Thickness(6),
						Margin = new Thickness(4),
						Background = FindResource("GameBackgroundAdditional") as SolidColorBrush
					};

					TooltipController.SetTooltipDelayAndDuration(border);

					var toolTip = TooltipController.GenerateEquipmentItemTooltip(item);
					border.ToolTip = toolTip;

					var grid = CreateSingleItemGrid(item);

					border.Child = grid;

					equipmentTabPanel.Children.Add(border);
				}
			}
		}

		private Grid CreateSingleItemGrid(Item item)
		{
			var grid = new Grid();

			var nameBlock = new TextBlock {FontSize = 18, HorizontalAlignment = HorizontalAlignment.Left};

			var quantityBlock = new TextBlock {FontSize = 18, HorizontalAlignment = HorizontalAlignment.Right};

			var binding = new Binding("Name") {Source = item, StringFormat = "{0}"};

			var binding2 = new Binding("Quantity") {Source = item, StringFormat = "x{0}"};

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
			}
		}
	}
}