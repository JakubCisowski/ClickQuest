using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.GameData.Models;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.ContentManager.UserInterface.Panels
{
	public partial class GameMechanicsTabsPanel : UserControl
	{
		private GameMechanicsTab _dataContext;
		private readonly Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();
		private StackPanel _currentPanel;

		public GameMechanicsTabsPanel()
		{
			InitializeComponent();

			PopulateContentSelectionBox();
		}

		private void PopulateContentSelectionBox()
		{
			ContentSelectionBox.ItemsSource = GameContent.GameMechanicsTabs.Select(x => x.Name);
		}

		public void RefreshStaticInfoPanel()
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

			GameMechanicsTab selectedGameMechanicTab = _dataContext;

			TextBox idBox = new TextBox
			{
				Name = "IdBox",
				Text = selectedGameMechanicTab.Id.ToString(),
				Margin = new Thickness(10),
				IsEnabled = false
			};
			TextBox nameBox = new TextBox
			{
				Name = "NameBox",
				Text = selectedGameMechanicTab.Name,
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
				Text = selectedGameMechanicTab.Description,
				Margin = new Thickness(10)
			};

			// Set TextBox and ComboBox hints.
			HintAssist.SetHint(idBox, "ID");
			HintAssist.SetHint(nameBox, "Name");
			HintAssist.SetHint(descriptionBox, "Description");

			// Add controls to Dictionary for easier navigation.
			_controls.Clear();

			_controls.Add(idBox.Name, idBox);
			_controls.Add(nameBox.Name, nameBox);
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
			GameMechanicsTab gameMechanicTab = _dataContext;

			if (gameMechanicTab is null)
			{
				return;
			}

			gameMechanicTab.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
			gameMechanicTab.Name = (_controls["NameBox"] as TextBox).Text;
			gameMechanicTab.Description = (_controls["DescriptionBox"] as TextBox).Text;

			// Check if this Id is already in the collection (modified).
			if (GameContent.GameMechanicsTabs.Select(x => x.Id).Contains(gameMechanicTab.Id))
			{
				int indexOfOldGameMechanicTab = GameContent.GameMechanicsTabs.FindIndex(x => x.Id == gameMechanicTab.Id);
				GameContent.GameMechanicsTabs[indexOfOldGameMechanicTab] = gameMechanicTab;
			}
			else
			{
				// If not, add it.
				GameContent.GameMechanicsTabs.Add(gameMechanicTab);
			}

			PopulateContentSelectionBox();
		}

		private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();

			int nextId = (GameContent.GameMechanicsTabs.Max(x => x.Id as int?) ?? 0) + 1;
			_dataContext = new GameMechanicsTab
			{
				Id = nextId
			};
			ContentSelectionBox.SelectedIndex = -1;
			RefreshStaticInfoPanel();

			DeleteObjectButton.Visibility = Visibility.Visible;
		}

		private void DeleteObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();

			var objectToDelete = GameContent.GameMechanicsTabs.FirstOrDefault(x => x.Id == int.Parse((_controls["IdBox"] as TextBox).Text));

			var result = MessageBox.Show($"Are you sure you want to delete {objectToDelete.Name}? This action will close ContentManager, check Logs directory (for missing references after deleting).", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.No)
			{
				return;
			}

			GameContent.GameMechanicsTabs.Remove(objectToDelete);

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

			_dataContext = GameContent.GameMechanicsTabs.FirstOrDefault(x => x.Name == selectedName);
			RefreshStaticInfoPanel();
			DeleteObjectButton.Visibility = Visibility.Visible;
		}
	}
}