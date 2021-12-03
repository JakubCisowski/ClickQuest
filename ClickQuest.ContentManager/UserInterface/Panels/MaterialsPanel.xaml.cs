using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.GameData.Models;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.ContentManager.UserInterface.Panels
{
	public partial class MaterialsPanel : UserControl
	{
		private Material _dataContext;
		private readonly Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();
		private StackPanel _currentPanel;

		public MaterialsPanel()
		{
			InitializeComponent();

			PopulateContentSelectionBox();
		}

		private void PopulateContentSelectionBox()
		{
			ContentSelectionBox.ItemsSource = GameContent.Materials.Select(x => x.Name);
		}

		public void RefreshStaticValuesPanel()
		{
			if (_currentPanel != null)
			{
				_currentPanel.Children.Clear();
				MainGrid.Children.Remove(_currentPanel);
			}

			var panel = new StackPanel
			{
				Name = "MainInfoPanel"
			};

			var selectedMaterial = _dataContext;

			var idBox = new TextBox
			{
				Name = "IdBox",
				Text = selectedMaterial.Id.ToString(),
				Margin = new Thickness(10),
				IsEnabled = false
			};
			var nameBox = new TextBox
			{
				Name = "NameBox",
				Text = selectedMaterial.Name,
				Margin = new Thickness(10)
			};
			var valueBox = new TextBox
			{
				Name = "ValueBox",
				Text = selectedMaterial.Value.ToString(),
				Margin = new Thickness(10)
			};
			var rarityBox = new ComboBox
			{
				Name = "RarityBox",
				ItemsSource = Enum.GetValues(typeof(Rarity)),
				SelectedIndex = (int) selectedMaterial.Rarity,
				Margin = new Thickness(10)
			};
			var descriptionBox = new TextBox
			{
				Name = "DescriptionBox",
				TextWrapping = TextWrapping.Wrap,
				VerticalAlignment = VerticalAlignment.Stretch,
				MinWidth = 280,
				AcceptsReturn = true,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
				Height = 160,
				Text = selectedMaterial.Description,
				Margin = new Thickness(10)
			};

			// Set TextBox and ComboBox hints.
			HintAssist.SetHint(idBox, "ID");
			HintAssist.SetHint(nameBox, "Name");
			HintAssist.SetHint(valueBox, "Value");
			HintAssist.SetHint(rarityBox, "Rarity");
			HintAssist.SetHint(descriptionBox, "Description");

			// Add controls to Dictionary for easier navigation.
			_controls.Clear();

			_controls.Add(idBox.Name, idBox);
			_controls.Add(nameBox.Name, nameBox);
			_controls.Add(valueBox.Name, valueBox);
			_controls.Add(rarityBox.Name, rarityBox);
			_controls.Add(descriptionBox.Name, descriptionBox);

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

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			(sender as TextBox).CaretIndex = int.MaxValue;
		}

		public void Save()
		{
			var material = _dataContext;

			if (material is null)
			{
				return;
			}

			material.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
			material.Name = (_controls["NameBox"] as TextBox).Text;
			material.Value = int.Parse((_controls["ValueBox"] as TextBox).Text);
			material.Rarity = (Rarity) Enum.Parse(typeof(Rarity), (_controls["RarityBox"] as ComboBox).SelectedValue.ToString());
			material.Description = (_controls["DescriptionBox"] as TextBox).Text;

			// Check if this Id is already in the collection (modified).
			if (GameContent.Materials.Select(x => x.Id).Contains(material.Id))
			{
				int indexOfOldMaterial = GameContent.Materials.FindIndex(x => x.Id == material.Id);
				GameContent.Materials[indexOfOldMaterial] = material;
			}
			else
			{
				// If not, add it.
				GameContent.Materials.Add(material);
			}

			PopulateContentSelectionBox();
		}

		private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();

			int nextId = (GameContent.Materials.Max(x => x.Id as int?) ?? 0) + 1;
			_dataContext = new Material
			{
				Id = nextId
			};
			ContentSelectionBox.SelectedIndex = -1;
			RefreshStaticValuesPanel();
		}

		private void DeleteObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();

			var objectToDelete = GameContent.Materials.FirstOrDefault(x => x.Id == int.Parse((_controls["IdBox"] as TextBox).Text));

			var result = MessageBox.Show($"Are you sure you want to delete {objectToDelete.Name}? This action will close ContentManager, check Logs directory (for missing references after deleting).", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.No)
			{
				return;
			}

			GameContent.Materials.Remove(objectToDelete);

			PopulateContentSelectionBox();
			ContentSelectionBox.SelectedIndex = -1;
			_currentPanel.Children.Clear();
			DeleteObjectButton.Visibility = Visibility.Hidden;
			_dataContext = null;

			Application.Current.MainWindow.Close();
		}

		private void ContentSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string? selectedName = (e.Source as ComboBox)?.SelectedValue?.ToString();

			if (selectedName is null)
			{
				return;
			}

			if (_dataContext is not null)
			{
				Save();
			}

			_dataContext = GameContent.Materials.FirstOrDefault(x => x.Name == selectedName);
			RefreshStaticValuesPanel();
			DeleteObjectButton.Visibility = Visibility.Visible;
		}
	}
}