﻿using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.Models;
using ClickQuest.ContentManager.UserInterface.Windows;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ClickQuest.ContentManager.UserInterface.Panels
{
	public partial class RecipesPanel : UserControl
	{
		private Recipe _dataContext;
		private Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();
		private StackPanel _currentPanel;
		private List<Ingredient> _ingredients;

		public RecipesPanel()
		{
			InitializeComponent();

			PopulateContentSelectionBox();
		}

		private void PopulateContentSelectionBox()
		{
			ContentSelectionBox.ItemsSource = GameContent.Recipes.Select(x => x.Name);
		}

		public void RefreshStaticValuesPanel()
		{
			// https://stackoverflow.com/questions/63834841/how-to-add-a-materialdesignhint-to-a-textbox-in-code

			// clear grid's first column to avoid duplicating the controls added below
			// how?
			// use the Dictionary

			if (_currentPanel != null)
			{
				_currentPanel.Children.Clear();
				MainGrid.Children.Remove(_currentPanel);
			}

			double gridHeight = this.ActualHeight;
			double gridWidth = this.ActualWidth;
			var panel = new StackPanel() { Name = "StaticInfoPanel" };

			var selectedRecipe = _dataContext;

			var idBox = new TextBox() { Name = "IdBox", Text = selectedRecipe.Id.ToString(), Margin = new Thickness(10), IsEnabled = false };
			var nameBox = new TextBox() { Name = "NameBox", Text = selectedRecipe.Name, Margin = new Thickness(10) };
			var valueBox = new TextBox() { Name = "ValueBox", Text = selectedRecipe.Value.ToString(), Margin = new Thickness(10) };
			var rarityBox = new ComboBox() { Name = "RarityBox", ItemsSource = Enum.GetValues(typeof(Rarity)), SelectedIndex = (int)selectedRecipe.Rarity, Margin = new Thickness(10) };
			var artifactIdBox = new TextBox() { Name = "ArtifactIDBox", Text = selectedRecipe.ArtifactId.ToString(), Margin = new Thickness(10), IsEnabled = false };
			var artifactNameBox = new ComboBox() { Name = "ArtifactNameBox", ItemsSource = GameContent.Artifacts.Select(x => x.Name), Margin = new Thickness(10) };
			artifactNameBox.SelectedValue = GameContent.Artifacts.FirstOrDefault(x => x.Id == selectedRecipe.ArtifactId).Name;
			artifactNameBox.SelectionChanged += ArtifactNameBox_SelectionChanged;

			// Set TextBox and ComboBox hints.
			HintAssist.SetHint(idBox, "ID");
			HintAssist.SetHint(nameBox, "Name");
			HintAssist.SetHint(valueBox, "Value");
			HintAssist.SetHint(rarityBox, "Rarity");
			HintAssist.SetHint(artifactIdBox, "ArtifactID");
			HintAssist.SetHint(artifactNameBox, "ArtifactName");

			// Add controls to Dictionary for easier navigation.
			_controls.Clear();

			_controls.Add(idBox.Name, idBox);
			_controls.Add(nameBox.Name, nameBox);
			_controls.Add(valueBox.Name, valueBox);
			_controls.Add(rarityBox.Name, rarityBox);
			_controls.Add(artifactIdBox.Name, artifactIdBox);
			_controls.Add(artifactNameBox.Name, artifactNameBox);

			foreach (var elem in _controls)
			{
				// Set style of each control to MaterialDesignFloatingHint, and set floating hint scale.
				if (elem.Value is TextBox textBox)
				{
					textBox.Style = (Style)this.FindResource("MaterialDesignOutlinedTextBox");
					HintAssist.SetFloatingScale(elem.Value, 1.0);
				}
				else if (elem.Value is ComboBox comboBox)
				{
					comboBox.Style = (Style)this.FindResource("MaterialDesignOutlinedComboBox");
					HintAssist.SetFloatingScale(elem.Value, 1.0);
				}

				panel.Children.Add(elem.Value);
			}

			Grid.SetColumn(panel, 1);

			_currentPanel = panel;

			MainGrid.Children.Add(panel);
		}

		private void ArtifactNameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			(_controls["ArtifactIDBox"] as TextBox).Text = GameContent.Artifacts.FirstOrDefault(x => x.Name == (sender as ComboBox).SelectedItem.ToString()).Id.ToString();
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			var recipe = _dataContext;

			recipe.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
			recipe.Name = (_controls["NameBox"] as TextBox).Text;
			recipe.Value = int.Parse((_controls["ValueBox"] as TextBox).Text);
			recipe.Rarity = (Rarity)Enum.Parse(typeof(Rarity), (_controls["RarityBox"] as ComboBox).SelectedValue.ToString());
			recipe.ArtifactId = int.Parse((_controls["ArtifactIDBox"] as TextBox).Text);

			// Check if this Id is already in the collection (modified).
			if (GameContent.Recipes.Select(x => x.Id).Contains(recipe.Id))
			{
				int indexOfOldRecipe = GameContent.Recipes.FindIndex(x => x.Id == recipe.Id);
				GameContent.Recipes[indexOfOldRecipe] = recipe;
			}
			else
			{
				// If not, add it.
				GameContent.Recipes.Add(recipe);
			}
		}

		private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
		{
			int nextId = GameContent.Recipes.Max(x => x.Id) + 1;
			_dataContext = new Recipe() { Id = nextId };
			RefreshStaticValuesPanel();
		}

		private void ContentSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedName = (e.Source as ComboBox)?.SelectedValue.ToString();

			_dataContext = GameContent.Recipes.FirstOrDefault(x => x.Name == selectedName);
			_ingredients = _dataContext.Ingredients;
			RefreshStaticValuesPanel();
			RefreshDynamicValuesPanel();
		}

		public void RefreshDynamicValuesPanel()
		{
			DynamicValuesPanel.Children.Clear();

			foreach (var ingredient in _ingredients)
			{
				var border = new Border
				{
					BorderThickness = new Thickness(0.5),
					BorderBrush = new SolidColorBrush(Colors.Gray),
					Padding = new Thickness(6),
					Margin = new Thickness(4),
					Tag = ingredient
				};

				border.PreviewMouseUp += EditDynamicValue_Click;

				var grid = CreateDynamicValueGrid(ingredient);

				border.Child = grid;

				DynamicValuesPanel.Children.Add(border);
			}
		}

		private Grid CreateDynamicValueGrid(Ingredient ingredient)
		{
			var grid = new Grid();

			var idBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(10, 0, 0, 0),
				FontStyle = FontStyles.Italic,
				Text = $"[{ingredient.Id}]"
			};

			var nameBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(80, 0, 0, 0),
				Text = GameContent.Materials.FirstOrDefault(x => x.Id == ingredient.Id).Name
			};

			var quantityBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(480, 0, 0, 0),
				Text = ingredient.Quantity.ToString(CultureInfo.InvariantCulture)
			};

			var deleteButton = new Button
			{
				Width = 30,
				Height = 30,
				Margin = new Thickness(5, 0, 50, 0),
				Tag = ingredient,
				Padding = new Thickness(0),
				HorizontalAlignment = HorizontalAlignment.Right
			};

			var deleteIcon = new PackIcon
			{
				Width = 20,
				Height = 20,
				Kind = PackIconKind.DeleteForever,
				Foreground = new SolidColorBrush(Colors.Gray)
			};

			deleteButton.Content = deleteIcon;

			deleteButton.Click += DeleteDynamicValue_Click;

			grid.Children.Add(idBlock);
			grid.Children.Add(nameBlock);
			grid.Children.Add(quantityBlock);
			grid.Children.Add(deleteButton);

			return grid;
		}

		private void EditDynamicValue_Click(object sender, MouseButtonEventArgs e)
		{
			var ingredient = (sender as Border).Tag as Ingredient;

			var ingredientWindow = new IngredientWindow(_dataContext, ingredient) { Owner = Application.Current.MainWindow };
			ingredientWindow.ShowDialog();

			RefreshDynamicValuesPanel();
		}

		private void DeleteDynamicValue_Click(object sender, RoutedEventArgs e)
		{
			var ingredient = (sender as Button).Tag as Ingredient;
			_dataContext.Ingredients.Remove(_dataContext.Ingredients.FirstOrDefault(x => x.Id == ingredient.Id));

			RefreshDynamicValuesPanel();
		}

		private void CreateDynamicValueButton_Click(object sender, RoutedEventArgs e)
		{
			var newIngredient = new Ingredient();
			_ingredients.Add(newIngredient);

			var tempBorder = new Border() { Tag = newIngredient };
			EditDynamicValue_Click(tempBorder, null);
		}

	}
}