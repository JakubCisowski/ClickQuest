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
	public partial class DungeonKeysPanel : UserControl
	{
		private DungeonKey _dataContext;
		private readonly Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();
		private StackPanel _currentPanel;

		public DungeonKeysPanel()
		{
			InitializeComponent();

			PopulateContentSelectionBox();
		}

		private void PopulateContentSelectionBox()
		{
			ContentSelectionBox.ItemsSource = GameContent.DungeonKeys.Select(x => x.Name);
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

			DungeonKey selectedDungeonKey = _dataContext;

			TextBox idBox = new TextBox
			{
				Name = "IdBox",
				Text = selectedDungeonKey.Id.ToString(),
				Margin = new Thickness(10),
				IsEnabled = false
			};
			TextBox nameBox = new TextBox
			{
				Name = "NameBox",
				Text = selectedDungeonKey.Name,
				Margin = new Thickness(10)
			};
			TextBox valueBox = new TextBox
			{
				Name = "ValueBox",
				Text = selectedDungeonKey.Value.ToString(),
				Margin = new Thickness(10)
			};
			ComboBox rarityBox = new ComboBox
			{
				Name = "RarityBox",
				ItemsSource = Enum.GetValues(typeof(Rarity)),
				SelectedIndex = (int) selectedDungeonKey.Rarity,
				Margin = new Thickness(10)
			};
			TextBox descriptionBox = new TextBox
			{
				Name = "DescriptionBox",
				TextWrapping = TextWrapping.Wrap,
				VerticalAlignment = VerticalAlignment.Stretch,
				MinWidth = 280,
				AcceptsReturn = true,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
				Height = 160,
				Text = selectedDungeonKey.Description,
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
			DungeonKey dungeonKey = _dataContext;

			if (dungeonKey is null)
			{
				return;
			}

			dungeonKey.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
			dungeonKey.Name = (_controls["NameBox"] as TextBox).Text;
			dungeonKey.Value = int.Parse((_controls["ValueBox"] as TextBox).Text);
			dungeonKey.Rarity = (Rarity) Enum.Parse(typeof(Rarity), (_controls["RarityBox"] as ComboBox).SelectedValue.ToString());
			dungeonKey.Description = (_controls["DescriptionBox"] as TextBox).Text;

			// Check if this Id is already in the collection (modified).
			if (GameContent.DungeonKeys.Select(x => x.Id).Contains(dungeonKey.Id))
			{
				int indexOfDungeonKey = GameContent.DungeonKeys.FindIndex(x => x.Id == dungeonKey.Id);
				GameContent.DungeonKeys[indexOfDungeonKey] = dungeonKey;
			}
			else
			{
				// If not, add it.
				GameContent.DungeonKeys.Add(dungeonKey);
			}

			PopulateContentSelectionBox();
		}

		private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();

			int nextId = (GameContent.DungeonKeys.Max(x => x.Id as int?) ?? 0) + 1;
			_dataContext = new DungeonKey
			{
				Id = nextId
			};
			ContentSelectionBox.SelectedIndex = -1;
			RefreshStaticValuesPanel();

			DeleteObjectButton.Visibility = Visibility.Visible;
		}

		private void DeleteObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();

			DungeonKey? objectToDelete = GameContent.DungeonKeys.FirstOrDefault(x => x.Id == int.Parse((_controls["IdBox"] as TextBox).Text));

			MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {objectToDelete.Name}? This action will close ContentManager, check Logs directory (for missing references after deleting).", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.No)
			{
				return;
			}

			GameContent.DungeonKeys.Remove(objectToDelete);

			PopulateContentSelectionBox();
			ContentSelectionBox.SelectedIndex = -1;
			_currentPanel.Children.Clear();
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

			_dataContext = GameContent.DungeonKeys.FirstOrDefault(x => x.Name == selectedName);
			RefreshStaticValuesPanel();
			DeleteObjectButton.Visibility = Visibility.Visible;
		}
	}
}