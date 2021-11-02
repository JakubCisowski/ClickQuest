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
	public partial class ArtifactsPanel : UserControl
	{
		private Artifact _dataContext;
		private readonly Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();
		private StackPanel _currentPanel;

		public ArtifactsPanel()
		{
			InitializeComponent();

			PopulateContentSelectionBox();
		}

		private void PopulateContentSelectionBox()
		{
			ContentSelectionBox.ItemsSource = GameContent.Artifacts.Select(x => x.Name);
		}

		public void RefreshStaticInfoPanel()
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

			var selectedArtifact = _dataContext;

			var idBox = new TextBox
			{
				Name = "IdBox",
				Text = selectedArtifact.Id.ToString(),
				Margin = new Thickness(10),
				IsEnabled = false
			};
			var nameBox = new TextBox
			{
				Name = "NameBox",
				Text = selectedArtifact.Name,
				Margin = new Thickness(10)
			};
			var valueBox = new TextBox
			{
				Name = "ValueBox",
				Text = selectedArtifact.Value.ToString(),
				Margin = new Thickness(10)
			};
			var rarityBox = new ComboBox
			{
				Name = "RarityBox",
				ItemsSource = Enum.GetValues(typeof(Rarity)),
				SelectedIndex = (int) selectedArtifact.Rarity,
				Margin = new Thickness(10)
			};
			var artifactTypeBox = new ComboBox
			{
				Name = "ArtifactTypeBox",
				ItemsSource = Enum.GetValues(typeof(ArtifactType)),
				SelectedIndex = (int) selectedArtifact.ArtifactType,
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
				Text = selectedArtifact.Description,
				Margin = new Thickness(10)
			};

			var loreBox = new TextBox
			{
				Name = "LoreBox",
				TextWrapping = TextWrapping.Wrap,
				VerticalAlignment = VerticalAlignment.Stretch,
				MinWidth = 280,
				AcceptsReturn = true,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
				Height = 160,
				Text = selectedArtifact.Lore,
				Margin = new Thickness(10)
			};

			var extraInfoBox = new TextBox
			{
				Name = "ExtraInfoBox",
				TextWrapping = TextWrapping.Wrap,
				VerticalAlignment = VerticalAlignment.Stretch,
				MinWidth = 280,
				AcceptsReturn = true,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
				Height = 160,
				Text = selectedArtifact.ExtraInfo,
				Margin = new Thickness(10)
			};

			// Set TextBox and ComboBox hints.
			HintAssist.SetHint(idBox, "ID");
			HintAssist.SetHint(nameBox, "Name");
			HintAssist.SetHint(valueBox, "Value");
			HintAssist.SetHint(rarityBox, "Rarity");
			HintAssist.SetHint(artifactTypeBox, "Type");
			HintAssist.SetHint(descriptionBox, "Description");
			HintAssist.SetHint(loreBox, "Lore");
			HintAssist.SetHint(extraInfoBox, "Extra info");

			// Add controls to Dictionary for easier navigation.
			_controls.Clear();

			_controls.Add(idBox.Name, idBox);
			_controls.Add(nameBox.Name, nameBox);
			_controls.Add(valueBox.Name, valueBox);
			_controls.Add(rarityBox.Name, rarityBox);
			_controls.Add(artifactTypeBox.Name, artifactTypeBox);
			_controls.Add(descriptionBox.Name, descriptionBox);
			_controls.Add(loreBox.Name, loreBox);
			_controls.Add(extraInfoBox.Name, extraInfoBox);

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
			var artifact = _dataContext;

			if (artifact is null)
			{
				return;
			}

			artifact.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
			artifact.Name = (_controls["NameBox"] as TextBox).Text;
			artifact.Value = int.Parse((_controls["ValueBox"] as TextBox).Text);
			artifact.Rarity = (Rarity) Enum.Parse(typeof(Rarity), (_controls["RarityBox"] as ComboBox).SelectedValue.ToString());
			artifact.ArtifactType = (ArtifactType) Enum.Parse(typeof(ArtifactType), (_controls["ArtifactTypeBox"] as ComboBox).SelectedValue.ToString());
			artifact.Description = (_controls["DescriptionBox"] as TextBox).Text;
			artifact.Lore = (_controls["LoreBox"] as TextBox).Text;
			artifact.ExtraInfo = (_controls["ExtraInfoBox"] as TextBox).Text;

			// Check if this Id is already in the collection (modified).
			if (GameContent.Artifacts.Select(x => x.Id).Contains(artifact.Id))
			{
				int indexOfOldArtifact = GameContent.Artifacts.FindIndex(x => x.Id == artifact.Id);
				GameContent.Artifacts[indexOfOldArtifact] = artifact;
			}
			else
			{
				// If not, add it.
				GameContent.Artifacts.Add(artifact);
			}

			PopulateContentSelectionBox();
		}

		private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();

			int nextId = GameContent.Artifacts.Max(x => x.Id) + 1;
			_dataContext = new Artifact
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

			var objectToDelete = GameContent.Artifacts.FirstOrDefault(x => x.Id == int.Parse((_controls["IdBox"] as TextBox).Text));

			var result = MessageBox.Show($"Are you sure you want to delete {objectToDelete.Name}? This action will close ContentManager, check Logs directory (for missing references after deleting).", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.No)
			{
				return;
			}

			GameContent.Artifacts.Remove(objectToDelete);

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

			_dataContext = GameContent.Artifacts.FirstOrDefault(x => x.Name == selectedName);
			ContentSelectionBox.SelectedValue = _dataContext.Name;
			RefreshStaticInfoPanel();
			DeleteObjectButton.Visibility = Visibility.Visible;
		}
	}
}