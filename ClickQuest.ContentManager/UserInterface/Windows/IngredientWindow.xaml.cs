using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.GameData.Models;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.ContentManager.UserInterface.Windows
{
	public partial class IngredientWindow : Window
	{
		private readonly Recipe _recipe;
		private readonly IngredientPattern _ingredient;
		private readonly Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();

		public IngredientWindow(Recipe recipe, IngredientPattern ingredient)
		{
			InitializeComponent();

			_recipe = recipe;
			_ingredient = ingredient;

			RefreshWindowControls();
		}

		public void RefreshWindowControls()
		{
			// Add controls to Dictionary for easier navigation.
			_controls.Clear();

			double gridHeight = ActualHeight;
			double gridWidth = ActualWidth;
			var panel = new StackPanel
			{
				Name = "MainInfoPanel"
			};

			var idBox = new TextBox
			{
				Name = "IdBox",
				Text = _ingredient.MaterialId.ToString(),
				Margin = new Thickness(10),
				IsEnabled = false
			};

			var nameBox = new ComboBox
			{
				Name = "NameBox",
				ItemsSource = GameContent.Materials.Select(x => x.Name),
				Margin = new Thickness(10)
			};
			nameBox.SelectedValue = GameContent.Materials.FirstOrDefault(x => x.Id == _ingredient.MaterialId)?.Name;
			nameBox.SelectionChanged += NameBox_SelectionChanged;

			var quantityBox = new TextBox
			{
				Name = "QuantityBox",
				Text = _ingredient.Quantity.ToString(),
				Margin = new Thickness(10)
			};

			// Set TextBox and ComboBox hints.
			HintAssist.SetHint(idBox, "ID");
			HintAssist.SetHint(nameBox, "Name");
			HintAssist.SetHint(quantityBox, "Quantity");

			_controls.Add(idBox.Name, idBox);
			_controls.Add(nameBox.Name, nameBox);
			_controls.Add(quantityBox.Name, quantityBox);

			foreach (var elem in _controls)
			{
				// Set style of each control to MaterialDesignFloatingHint, and set floating hint scale.
				if (elem.Value is TextBox textBox)
				{
					textBox.Style = (Style) FindResource("MaterialDesignOutlinedTextBox");
					HintAssist.SetFloatingScale(elem.Value, 1.0);
					textBox.GotFocus += TextBox_GotFocus;
				}
				else if (elem.Value is ComboBox comboBox)
				{
					comboBox.Style = (Style) FindResource("MaterialDesignOutlinedComboBox");
					HintAssist.SetFloatingScale(elem.Value, 1.0);
				}

				panel.Children.Add(elem.Value);
			}

			MainGrid.Children.Add(panel);
		}

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			(sender as TextBox).CaretIndex = int.MaxValue;
		}

		private void NameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			(_controls["IdBox"] as TextBox).Text = GameContent.Materials.FirstOrDefault(x => x.Name == (sender as ComboBox).SelectedItem.ToString()).Id.ToString();
		}

		private void UpdateIngredient()
		{
			int oldIngredientIndex = _recipe.IngredientPatterns.IndexOf(_recipe.IngredientPatterns.FirstOrDefault(x => x.MaterialId == _ingredient.MaterialId));

			_ingredient.MaterialId = int.Parse((_controls["IdBox"] as TextBox).Text);
			_ingredient.Quantity = int.Parse((_controls["QuantityBox"] as TextBox).Text);

			if (oldIngredientIndex == -1)
			{
				_recipe.IngredientPatterns.Add(_ingredient);
			}
			else
			{
				_recipe.IngredientPatterns[oldIngredientIndex] = _ingredient;
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			UpdateIngredient();
		}
	}
}