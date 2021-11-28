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
	public partial class RegionsPanel : UserControl
	{
		private Region _dataContext;
		private readonly Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();
		private StackPanel _currentPanel;
		private List<MonsterSpawnPattern> _monsterSpawnPatterns;

		public RegionsPanel()
		{
			InitializeComponent();

			PopulateContentSelectionBox();
		}

		private void PopulateContentSelectionBox()
		{
			ContentSelectionBox.ItemsSource = GameContent.Regions.Select(x => x.Name);
		}

		public void RefreshStaticValuesPanel()
		{
			if (_currentPanel != null)
			{
				_currentPanel.Children.Clear();
				MainGrid.Children.Remove(_currentPanel);
			}

			double gridHeight = ActualHeight;
			double gridWidth = ActualWidth;
			var panel = new StackPanel
			{
				Name = "StaticInfoPanel"
			};

			var selectedRegion = _dataContext;

			var idBox = new TextBox
			{
				Name = "IdBox",
				Text = selectedRegion.Id.ToString(),
				Margin = new Thickness(10),
				IsEnabled = false
			};
			var nameBox = new TextBox
			{
				Name = "NameBox",
				Text = selectedRegion.Name,
				Margin = new Thickness(10)
			};
			var levelRequirementBox = new TextBox
			{
				Name = "LevelRequirementBox",
				Text = selectedRegion.LevelRequirement.ToString(),
				Margin = new Thickness(10)
			};
			var backgroundBox = new TextBox
			{
				Name = "BackgroundBox",
				Text = selectedRegion.Background,
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
				Text = selectedRegion.Description,
				Margin = new Thickness(10)
			};

			// Set TextBox and ComboBox hints.
			HintAssist.SetHint(idBox, "ID");
			HintAssist.SetHint(nameBox, "Name");
			HintAssist.SetHint(levelRequirementBox, "LevelRequirement");
			HintAssist.SetHint(backgroundBox, "Background");
			HintAssist.SetHint(descriptionBox, "Description");

			// Add controls to Dictionary for easier navigation.
			_controls.Clear();

			_controls.Add(idBox.Name, idBox);
			_controls.Add(nameBox.Name, nameBox);
			_controls.Add(levelRequirementBox.Name, levelRequirementBox);
			_controls.Add(backgroundBox.Name, backgroundBox);
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
			var region = _dataContext;

			if (region is null)
			{
				return;
			}

			region.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
			region.Name = (_controls["NameBox"] as TextBox).Text;
			region.LevelRequirement = int.Parse((_controls["LevelRequirementBox"] as TextBox).Text);
			region.Background = (_controls["BackgroundBox"] as TextBox).Text;
			region.Description = (_controls["DescriptionBox"] as TextBox).Text;

			// Check if this Id is already in the collection (modified).
			if (GameContent.Regions.Select(x => x.Id).Contains(region.Id))
			{
				int indexOfOldRegion = GameContent.Regions.FindIndex(x => x.Id == region.Id);
				GameContent.Regions[indexOfOldRegion] = region;
			}
			else
			{
				// If not, add it.
				GameContent.Regions.Add(region);
			}

			PopulateContentSelectionBox();
		}

		private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();

			int nextId = GameContent.Regions.Max(x => x.Id) + 1;

			_dataContext = new Region
			{
				Id = nextId,
				MonsterSpawnPatterns = new List<MonsterSpawnPattern>()
			};
			_monsterSpawnPatterns = _dataContext.MonsterSpawnPatterns;

			ContentSelectionBox.SelectedIndex = -1;
			RefreshStaticValuesPanel();
			DynamicValuesPanel.Children.Clear();

			DeleteObjectButton.Visibility = Visibility.Visible;
		}

		private void DeleteObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();

			var objectToDelete = GameContent.Regions.FirstOrDefault(x => x.Id == int.Parse((_controls["IdBox"] as TextBox).Text));

			var result = MessageBox.Show($"Are you sure you want to delete {objectToDelete.Name}? This action will close ContentManager, check Logs directory (for missing references after deleting).", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.No)
			{
				return;
			}

			GameContent.Regions.Remove(objectToDelete);

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

			_dataContext = GameContent.Regions.FirstOrDefault(x => x.Name == selectedName);
			_monsterSpawnPatterns = _dataContext.MonsterSpawnPatterns;
			RefreshStaticValuesPanel();
			RefreshDynamicValuesPanel();
			DeleteObjectButton.Visibility = Visibility.Visible;
		}

		public void RefreshDynamicValuesPanel()
		{
			DynamicValuesPanel.Children.Clear();

			CreateDynamicValueButton.Visibility = Visibility.Visible;

			foreach (var pattern in _monsterSpawnPatterns)
			{
				var border = new Border
				{
					BorderThickness = new Thickness(0.5),
					BorderBrush = (SolidColorBrush) FindResource("BrushGray2"),
					Padding = new Thickness(6),
					Margin = new Thickness(4)
				};

				var grid = CreateDynamicValueGrid(pattern);

				border.Child = grid;

				DynamicValuesPanel.Children.Add(border);
			}
		}

		private Grid CreateDynamicValueGrid(MonsterSpawnPattern pattern)
		{
			var grid = new Grid();

			var idBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(10, 0, 0, 0),
				FontStyle = FontStyles.Italic,
				Text = $"[{pattern.MonsterId}]"
			};

			var nameBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(80, 0, 0, 0),
				Text = GameContent.Monsters.FirstOrDefault(x => x.Id == pattern.MonsterId).Name
			};

			var frequencyBlock = new TextBlock
			{
				FontSize = 18,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = new Thickness(480, 0, 0, 0),
				Text = pattern.Frequency.ToString()
			};

			var editButton = new Button
			{
				Width = 30,
				Height = 30,
				Margin = new Thickness(5, 0, 90, 0),
				Padding = new Thickness(0),
				HorizontalAlignment = HorizontalAlignment.Right,
				Tag = pattern
			};

			var editIcon = new PackIcon
			{
				Width = 20,
				Height = 20,
				Kind = PackIconKind.Edit,
				Foreground = (SolidColorBrush) FindResource("BrushGray2")
			};

			editButton.Content = editIcon;

			editButton.Click += EditDynamicValue_Click;

			var deleteButton = new Button
			{
				Width = 30,
				Height = 30,
				Margin = new Thickness(5, 0, 50, 0),
				Tag = pattern,
				Padding = new Thickness(0),
				HorizontalAlignment = HorizontalAlignment.Right
			};

			var deleteIcon = new PackIcon
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
			grid.Children.Add(frequencyBlock);
			grid.Children.Add(editButton);
			grid.Children.Add(deleteButton);

			return grid;
		}

		private void EditDynamicValue_Click(object sender, RoutedEventArgs e)
		{
			var monsterSpawnPattern = (sender as Button).Tag as MonsterSpawnPattern;

			var monsterSpawnPatternWindow = new MonsterSpawnPatternWindow(_dataContext, monsterSpawnPattern)
			{
				Owner = Application.Current.MainWindow
			};
			monsterSpawnPatternWindow.ShowDialog();

			RefreshDynamicValuesPanel();
		}

		private void DeleteDynamicValue_Click(object sender, RoutedEventArgs e)
		{
			var pattern = (sender as Button).Tag as MonsterSpawnPattern;

			var result = MessageBox.Show($"Are you sure you want to delete pattern of Id: {pattern.MonsterId}?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.No)
			{
				return;
			}

			_dataContext.MonsterSpawnPatterns.Remove(_dataContext.MonsterSpawnPatterns.FirstOrDefault(x => x.MonsterId == pattern.MonsterId));

			RefreshDynamicValuesPanel();
		}

		private void CreateDynamicValueButton_Click(object sender, RoutedEventArgs e)
		{
			var newMonsterSpawnPattern = new MonsterSpawnPattern();
			_monsterSpawnPatterns.Add(newMonsterSpawnPattern);

			var tempButton = new Button
			{
				Tag = newMonsterSpawnPattern
			};
			EditDynamicValue_Click(tempButton, null);
		}
	}
}