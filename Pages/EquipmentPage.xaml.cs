using ClickQuest.Account;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ClickQuest.Pages
{
	public partial class EquipmentPage : Page
	{
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

			foreach (var material in User.Instance.Materials)
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

				var binding = new Binding("Name");
				binding.Source = material;
				binding.StringFormat = "{0}";

				var binding2 = new Binding("Quantity");
				binding2.Source = material;
				binding2.StringFormat = "x{0}";

				nameBlock.SetBinding(TextBlock.TextProperty, binding);
				quantityBlock.SetBinding(TextBlock.TextProperty, binding2);

				grid.Children.Add(nameBlock);
				grid.Children.Add(quantityBlock);

				border.Child = grid;

				MaterialsPanel.Children.Add(border);
			}

			foreach (var recipe in User.Instance.Recipes)
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

				var binding = new Binding("Name");
				binding.Source = recipe;
				binding.StringFormat = "{0}";

				var binding2 = new Binding("Quantity");
				binding2.Source = recipe;
				binding2.StringFormat = "x{0}";

				nameBlock.SetBinding(TextBlock.TextProperty, binding);
				quantityBlock.SetBinding(TextBlock.TextProperty, binding2);

				grid.Children.Add(nameBlock);
				grid.Children.Add(quantityBlock);

				border.Child = grid;

				RecipesPanel.Children.Add(border);
			}

			foreach (var artifact in User.Instance.Artifacts)
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

				var binding = new Binding("Name");
				binding.Source = artifact;
				binding.StringFormat = "{0}";

				var binding2 = new Binding("Quantity");
				binding2.Source = artifact;
				binding2.StringFormat = "x{0}";

				nameBlock.SetBinding(TextBlock.TextProperty, binding);
				quantityBlock.SetBinding(TextBlock.TextProperty, binding2);

				grid.Children.Add(nameBlock);
				grid.Children.Add(quantityBlock);

				border.Child = grid;

				ArtifactsPanel.Children.Add(border);
			}
		}
	}
}
