using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ClickQuest.Data.GameData;
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

			RefreshEquippedArtifacts();

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
						Background = FindResource("GameBackgroundAdditional") as SolidColorBrush,
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

		private void ItemBorder_TryToEquip(object sender, MouseButtonEventArgs e)
		{
			var item = (sender as Border).Tag;
			bool isFighting = GameData.CurrentPage is RegionPage || GameData.CurrentPage is DungeonBossPage;
			bool isQuesting = User.Instance.CurrentHero.Quests.Any(x => x.EndDate != default);

			if (item is Artifact artifact && !isFighting && !isQuesting)
			{
				bool isEquipped = User.Instance.CurrentHero.EquippedArtifacts.Contains(artifact);

				if(!isEquipped)
				{
					bool canBeEquipped = artifact.ArtifactFunctionality.CanBeEquipped();

					if (canBeEquipped)
					{
						User.Instance.CurrentHero.EquippedArtifacts.Add(artifact);
						artifact.ArtifactFunctionality.OnEquip();
						(sender as Border).Background = FindResource("GameBackgroundSecondary") as SolidColorBrush;
					}
				}
				else
				{
					User.Instance.CurrentHero.EquippedArtifacts.Remove(artifact);
					artifact.ArtifactFunctionality.OnUnequip();
					(sender as Border).Background = FindResource("GameBackgroundAdditional") as SolidColorBrush;
				}
				
			}
		}

		public void RefreshEquippedArtifacts()
		{
			foreach(Border artifactBorder in ArtifactsPanel.Children)
			{
				var artifact = artifactBorder.Tag as Artifact;
				if(User.Instance.CurrentHero.EquippedArtifacts.Contains(artifact))
				{
					artifactBorder.Background = FindResource("GameBackgroundSecondary") as SolidColorBrush;
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
			}
		}
	}
}