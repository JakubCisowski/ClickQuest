using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.GameData.Models;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.ContentManager.UserInterface.Windows
{
	public partial class IngredientWindow : Window
	{
		private Recipe _recipe;
		private Ingredient _ingredient;
		private Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();

		public IngredientWindow(Recipe recipe, Ingredient ingredient)
		{
			InitializeComponent();

			_recipe = recipe;
			_ingredient = ingredient;

			RefreshWindowControls();
		}

		public void RefreshWindowControls()
		{
			// https://stackoverflow.com/questions/63834841/how-to-add-a-materialdesignhint-to-a-textbox-in-code

			// clear grid's first column to avoid duplicating the controls added below
			// how?
			// use the Dictionary

			// Add controls to Dictionary for easier navigation.
			_controls.Clear();

			double gridHeight = this.ActualHeight;
			double gridWidth = this.ActualWidth;
			var panel = new StackPanel() { Name = "MainInfoPanel" };

			var saveButton = new Button() { Name = "SaveButton", Content = "Save", Margin = new Thickness(10) };
			saveButton.Click += SaveButton_Click;
			_controls.Add(saveButton.Name, saveButton);

			var idBox = new TextBox() { Name = "IdBox", Text = _ingredient.Id.ToString(), Margin = new Thickness(10), IsEnabled = false };

			var nameBox = new ComboBox() { Name = "NameBox", ItemsSource = GameContent.Materials.Select(x => x.Name), Margin = new Thickness(10) };
			nameBox.SelectedValue = GameContent.Materials.FirstOrDefault(x => x.Id == _ingredient.Id)?.Name;
			nameBox.SelectionChanged += NameBox_SelectionChanged;

			var quantityBox = new TextBox() { Name = "QuantityBox", Text = _ingredient.Quantity.ToString(), Margin = new Thickness(10) };

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
					textBox.Style = (Style)this.FindResource("MaterialDesignOutlinedTextBox");
					HintAssist.SetFloatingScale(elem.Value, 1.0);
					textBox.GotFocus += TextBox_GotFocus;
				}
				else if (elem.Value is ComboBox comboBox)
				{
					comboBox.Style = (Style)this.FindResource("MaterialDesignOutlinedComboBox");
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
			var oldIngredientIndex = _recipe.Ingredients.IndexOf(_recipe.Ingredients.FirstOrDefault(x => x.Id == _ingredient.Id));

			_ingredient.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
			_ingredient.Quantity = int.Parse((_controls["QuantityBox"] as TextBox).Text);

			if (oldIngredientIndex == -1)
			{
				_recipe.Ingredients.Add(_ingredient);
			}
			else
			{
				_recipe.Ingredients[oldIngredientIndex] = _ingredient;
			}
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			UpdateIngredient();
		}
	}
}
