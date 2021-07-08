using ClickQuest.Player;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace ClickQuest.Pages
{
	public partial class EquipmentPage : Page
	{
		private static int _activeTab;
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

			#region Materials
			if (User.Instance.CurrentHero?.Materials != null)
			{
				foreach (var material in User.Instance.CurrentHero.Materials)
				{
					var border = new Border()
					{
						BorderThickness = new Thickness(0.5),
						BorderBrush = new SolidColorBrush(Colors.Gray),
						Padding = new Thickness(6),
						Margin = new Thickness(4),
						Background = this.FindResource("GameBackgroundAdditional") as SolidColorBrush
					};

					// Generate Materials tooltips.
					var toolTip = new ToolTip();
					ToolTipService.SetInitialShowDelay(border, 100);
					ToolTipService.SetShowDuration(border, 20000);

					var toolTipBlock = new TextBlock()
					{
						Style = (Style)this.FindResource("ToolTipTextBlockBase")
					};

					// Create blessing tooltip
					toolTipBlock.Inlines.Add(new Run($"{material.Name}"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"*{material.RarityString}*") { Foreground = Styles.Colors.GetRarityColor(material.Rarity), FontWeight = FontWeights.DemiBold });
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"{material.Description}"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"Value: {material.Value} gold"));

					toolTip.Content = toolTipBlock;
					border.ToolTip = toolTip;

					var grid = new Grid();

					var nameBlock = new TextBlock()
					{
						FontSize = 18,
						HorizontalAlignment = HorizontalAlignment.Left
					};

					var quantityBlock = new TextBlock()
					{
						FontSize = 18,
						HorizontalAlignment = HorizontalAlignment.Right
					};

					var binding = new Binding("Name")
					{
						Source = material,
						StringFormat = "{0}"
					};

					var binding2 = new Binding("Quantity")
					{
						Source = material,
						StringFormat = "x{0}"
					};

					nameBlock.SetBinding(TextBlock.TextProperty, binding);
					quantityBlock.SetBinding(TextBlock.TextProperty, binding2);

					grid.Children.Add(nameBlock);
					grid.Children.Add(quantityBlock);

					border.Child = grid;

					MaterialsPanel.Children.Add(border);
				}
			}
			#endregion

			#region Recipes
			if (User.Instance.CurrentHero?.Recipes != null)
			{
				foreach (var recipe in User.Instance.CurrentHero.Recipes)
				{
					var border = new Border()
					{
						BorderThickness = new Thickness(0.5),
						BorderBrush = new SolidColorBrush(Colors.Gray),
						Padding = new Thickness(6),
						Margin = new Thickness(4),
						Background = this.FindResource("GameBackgroundAdditional") as SolidColorBrush
					};

					// Generate Recipes tooltip.
					var toolTip = new ToolTip();
					ToolTipService.SetInitialShowDelay(border, 100);
					ToolTipService.SetShowDuration(border, 20000);

					var toolTipBlock = new TextBlock()
					{
						Style = (Style)this.FindResource("ToolTipTextBlockBase")
					};

					// Create blessing tooltip
					toolTipBlock.Inlines.Add(new Run($"{recipe.Name}"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"*{recipe.RarityString}*") { Foreground = Styles.Colors.GetRarityColor(recipe.Rarity), FontWeight = FontWeights.DemiBold });
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"{recipe.Description}"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"{recipe.RequirementsDescription}"));

					toolTip.Content = toolTipBlock;
					border.ToolTip = toolTip;

					var grid = new Grid();

					var nameBlock = new TextBlock()
					{
						FontSize = 18,
						HorizontalAlignment = HorizontalAlignment.Left
					};

					var quantityBlock = new TextBlock()
					{
						FontSize = 18,
						HorizontalAlignment = HorizontalAlignment.Right
					};

					var binding = new Binding("Name")
					{
						Source = recipe,
						StringFormat = "{0}"
					};

					var binding2 = new Binding("Quantity")
					{
						Source = recipe,
						StringFormat = "x{0}"
					};

					nameBlock.SetBinding(TextBlock.TextProperty, binding);
					quantityBlock.SetBinding(TextBlock.TextProperty, binding2);

					grid.Children.Add(nameBlock);
					grid.Children.Add(quantityBlock);

					border.Child = grid;

					RecipesPanel.Children.Add(border);
				}
			}
			#endregion

			#region Artifacts
			if (User.Instance.CurrentHero?.Artifacts != null)
			{
				foreach (var artifact in User.Instance.CurrentHero.Artifacts)
				{
					var border = new Border()
					{
						BorderThickness = new Thickness(1),
						BorderBrush = new SolidColorBrush(Colors.Gray),
						Padding = new Thickness(6),
						Margin = new Thickness(4),
						Background = this.FindResource("GameBackgroundAdditional") as SolidColorBrush
					};

					// Generate Artifact tooltip.
					var toolTip = new ToolTip();
					ToolTipService.SetInitialShowDelay(border, 100);
					ToolTipService.SetShowDuration(border, 20000);

					var toolTipBlock = new TextBlock()
					{
						Style = (Style)this.FindResource("ToolTipTextBlockBase")
					};

					// Create blessing tooltip
					toolTipBlock.Inlines.Add(new Run($"{artifact.Name}"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"*{artifact.RarityString}*") { Foreground = Styles.Colors.GetRarityColor(artifact.Rarity), FontWeight = FontWeights.DemiBold });
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"{artifact.Description}"));

					toolTip.Content = toolTipBlock;
					border.ToolTip = toolTip;


					var grid = new Grid();

					var nameBlock = new TextBlock()
					{
						FontSize = 18,
						HorizontalAlignment = HorizontalAlignment.Left
					};

					var quantityBlock = new TextBlock()
					{
						FontSize = 18,
						HorizontalAlignment = HorizontalAlignment.Right
					};

					var binding = new Binding("Name")
					{
						Source = artifact,
						StringFormat = "{0}"
					};

					var binding2 = new Binding("Quantity")
					{
						Source = artifact,
						StringFormat = "x{0}"
					};

					nameBlock.SetBinding(TextBlock.TextProperty, binding);
					quantityBlock.SetBinding(TextBlock.TextProperty, binding2);

					grid.Children.Add(nameBlock);
					grid.Children.Add(quantityBlock);

					border.Child = grid;

					ArtifactsPanel.Children.Add(border);
				}
			}
			#endregion

			// Change ActiveTab to what was selected before.
			EquipmentTabControl.SelectedIndex = _activeTab;
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs  e)
		{
			if (e.Source is TabControl)
			{
				// Save ActiveTab selection (to set it after updating equipment page).
				_activeTab = EquipmentTabControl.SelectedIndex;
			}
		}
	}
}
