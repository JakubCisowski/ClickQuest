using ClickQuest.Account;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
