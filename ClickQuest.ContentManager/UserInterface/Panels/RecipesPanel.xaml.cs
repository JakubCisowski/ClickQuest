using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.GameData.Models;
using ClickQuest.ContentManager.UserInterface.Windows;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.ContentManager.UserInterface.Panels
{
	public partial class RecipesPanel : UserControl
	{
		private Recipe _dataContext;
		private readonly Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();
		private StackPanel _currentPanel;
		private List<IngredientPattern> _ingredients;

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
			if (_currentPanel != null)
			{
				_currentPanel.Children.Clear();
				MainGrid.Children.Remove(_currentPanel);
			}

			StackPanel panel = new StackPanel
			{
				Name = "StaticInfoPanel"
			};

			Recipe selectedRecipe = _dataContext;

			TextBox idBox = new TextBox
			{
				Name = "IdBox",
				Text = selectedRecipe.Id.ToString(),
				Margin = new Thickness(10),
				IsEnabled = false
			};
			TextBox nameBox = new TextBox
			{
				Name = "NameBox",
				Text = selectedRecipe.Name,
				Margin = new Thickness(10),
				IsEnabled = false
			};
			TextBox valueBox = new TextBox
			{
				Name = "ValueBox",
				Text = selectedRecipe.Value.ToString(),
				Margin = new Thickness(10)
			};
			ComboBox rarityBox = new ComboBox
			{
				Name = "RarityBox",
				ItemsSource = Enum.GetValues(typeof(Rarity)),
				SelectedIndex = (int) selectedRecipe.Rarity,
				Margin = new Thickness(10)
			};
			TextBox artifactIdBox = new TextBox
			{
				Name = "ArtifactIDBox",
				Text = selectedRecipe.ArtifactId.ToString(),
				Margin = new Thickness(10),
				IsEnabled = false
			};
			ComboBox artifactNameBox = new ComboBox
			{
				Name = "ArtifactNameBox",
				ItemsSource = GameContent.Artifacts.Select(x => x.Name),
				Margin = new Thickness(10)
			};
			artifactNameBox.SelectedValue = GameContent.Artifacts.FirstOrDefault(x => x.Id == selectedRecipe.ArtifactId)?.Name;
			nameBox.Text = GameContent.Artifacts.FirstOrDefault(x => x.Id == selectedRecipe.ArtifactId)?.Name;
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

			Grid.SetColumn(panel, 1);

			_currentPanel = panel;

			MainGrid.Children.Add(panel);
		}

		private void ArtifactNameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			(_controls["ArtifactIDBox"] as TextBox).Text = GameContent.Artifacts.FirstOrDefault(x => x.Name == (sender as ComboBox).SelectedItem.ToString()).Id.ToString();
			(_controls["NameBox"] as TextBox).Text = GameContent.Artifacts.FirstOrDefault(x => x.Name == (sender as ComboBox).SelectedItem.ToString()).Name;
		}

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			(sender as TextBox).CaretIndex = int.MaxValue;
		}

		public void Save()
		{
			Recipe recipe = _dataContext;

			if (recipe is null)
			{
				return;
			}

			recipe.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
			recipe.Name = (_controls["NameBox"] as TextBox).Text;
			recipe.Value = int.Parse((_controls["ValueBox"] as TextBox).Text);
			recipe.Rarity = (Rarity) Enum.Parse(typeof(Rarity), (_controls["RarityBox"] as ComboBox).SelectedValue.ToString());
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

			PopulateContentSelectionBox();
		}

		private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();

			int nextId = (GameContent.Recipes.Max(x => x.Id as int?) ?? 0) + 1;

			_dataContext = new Recipe
			{
				Id = nextId,
				IngredientPatterns = new List<IngredientPattern>()
			};
			_ingredients = _dataContext.IngredientPatterns;

			ContentSelectionBox.SelectedIndex = -1;
			RefreshStaticValuesPanel();
			DynamicValuesPanel.Children.Clear();

			DeleteObjectButton.Visibility = Visibility.Visible;
		}

		private void DeleteObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();

			var objectToDelete = GameContent.Recipes.FirstOrDefault(x => x.Id == int.Parse((_controls["IdBox"] as TextBox).Text));

			var result = MessageBox.Show($"Are you sure you want to delete {objectToDelete.Name}? This action will close ContentManager, check Logs directory (for missing references after deleting).", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.No)
			{
				return;
			}

			GameContent.Recipes.Remove(objectToDelete);

			PopulateContentSelectionBox();
			ContentSelectionBox.SelectedIndex = -1;

			_currentPanel.Children.Clear();
			DynamicValuesPanel.Children.Clear();

			CreateDynamicValueButton.Visibility = Visibility.Hidden;
			DeleteObjectButton.Visibility = Visibility.Hidden;
			_dataContext = null;

			Application.Current.MainWindow.Close();
		}

		private void ContentSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedName = (e.Source as ComboBox)?.SelectedValue?.ToString();

			if (selectedName is null)
			{
				return;
			}

			if (_dataContext is not null)
			{
				Save();
			}

			_dataContext = GameContent.Recipes.FirstOrDefault(x => x.Name == selectedName);
			_ingredients = _dataContext.IngredientPatterns;
			RefreshStaticValuesPanel();
			RefreshDynamicValuesPanel();
			DeleteObjectButton.Visibility = Visibility.Visible;
		}

		public void RefreshDynamicValuesPanel()
		{
			DynamicValuesPanel.Children.Clear();

			CreateDynamicValueButton.Visibility = Visibility.Visible;

			foreach (IngredientPattern ingredient in _ingredients)
			{
				Border border = new Border
				{
					BorderThickness = new Thickness(0.5),
					BorderBrush = (SolidColorBrush) FindResource("BrushGray2"),
					Padding = new Thickness(6),
					Margin = new Thickness(4)
				};

				Grid grid = CreateDynamicValueGrid(ingredient);

				border.Child = grid;

				DynamicValuesPanel.Children.Add(border);
			}
		}

		private Grid CreateDynamicValueGrid(IngredientPattern ingredient)
		{
			Grid grid = new Grid();

			TextBlock idBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(10, 0, 0, 0),
				FontStyle = FontStyles.Italic,
				Text = $"[{ingredient.MaterialId}]"
			};

			TextBlock nameBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(80, 0, 0, 0),
				Text = GameContent.Materials.FirstOrDefault(x => x.Id == ingredient.MaterialId).Name
			};

			TextBlock quantityBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(480, 0, 0, 0),
				Text = ingredient.Quantity.ToString()
			};

			Button editButton = new Button
			{
				Width = 30,
				Height = 30,
				Margin = new Thickness(5, 0, 90, 0),
				Padding = new Thickness(0),
				HorizontalAlignment = HorizontalAlignment.Right,
				Tag = ingredient
			};

			PackIcon editIcon = new PackIcon
			{
				Width = 20,
				Height = 20,
				Kind = PackIconKind.Edit,
				Foreground = (SolidColorBrush) FindResource("BrushGray2")
			};

			editButton.Content = editIcon;

			editButton.Click += EditDynamicValue_Click;

			Button deleteButton = new Button
			{
				Width = 30,
				Height = 30,
				Margin = new Thickness(5, 0, 50, 0),
				Tag = ingredient,
				Padding = new Thickness(0),
				HorizontalAlignment = HorizontalAlignment.Right
			};

			PackIcon deleteIcon = new PackIcon
			{
				Width = 20,
				Height = 20,
				Kind = PackIconKind.DeleteForever,
				Foreground = (SolidColorBrush) FindResource("BrushGray2")
			};

			deleteButton.Content = deleteIcon;

			deleteButton.Click += DeleteDynamicValue_Click;

			grid.Children.Add(idBlock);
			grid.Children.Add(nameBlock);
			grid.Children.Add(quantityBlock);
			grid.Children.Add(editButton);
			grid.Children.Add(deleteButton);

			return grid;
		}

		private void EditDynamicValue_Click(object sender, RoutedEventArgs e)
		{
			IngredientPattern ingredient = (sender as Button).Tag as IngredientPattern;

			IngredientWindow ingredientWindow = new IngredientWindow(_dataContext, ingredient)
			{
				Owner = Application.Current.MainWindow
			};
			ingredientWindow.ShowDialog();

			RefreshDynamicValuesPanel();
		}

		private void DeleteDynamicValue_Click(object sender, RoutedEventArgs e)
		{
			IngredientPattern ingredient = (sender as Button).Tag as IngredientPattern;

			var result = MessageBox.Show($"Are you sure you want to delete ingredient of Id: {ingredient.MaterialId}?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.No)
			{
				return;
			}

			_dataContext.IngredientPatterns.Remove(_dataContext.IngredientPatterns.FirstOrDefault(x => x.MaterialId == ingredient.MaterialId));

			RefreshDynamicValuesPanel();
		}

		private void CreateDynamicValueButton_Click(object sender, RoutedEventArgs e)
		{
			IngredientPattern newIngredient = new IngredientPattern();
			_ingredients.Add(newIngredient);

			Button tempButton = new Button
			{
				Tag = newIngredient
			};
			EditDynamicValue_Click(tempButton, null);
		}
	}
}