using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.ContentManager.Models;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.ContentManager.UserInterface
{
	// zrobić jakiś osobny control typu StackPanel i tam, w zależności od datacontext (typu obiektu i jego pól) generować textboxy itp
	public partial class InputPanel : UserControl
	{
		private ContentType _currentContentType;
		private object _dataContext;
		private Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();
		private Grid _currentGrid;

		public InputPanel(ContentType contentType)
		{
			InitializeComponent();

			_currentContentType = contentType;

			PopulateContentSelectionBox();
		}

		private void PopulateContentSelectionBox()
		{
			switch (_currentContentType)
			{
				case ContentType.Artifacts:
					ContentSelectionBox.ItemsSource = GameContent.Artifacts.Select(x => x.Name);
					break;

				case ContentType.Materials:
					ContentSelectionBox.ItemsSource = GameContent.Materials.Select(x => x.Name);
					break;

				case ContentType.Recipes:
					ContentSelectionBox.ItemsSource = GameContent.Recipes.Select(x => x.Name);
					break;

				case ContentType.Blessings:
					ContentSelectionBox.ItemsSource = GameContent.Blessings.Select(x => x.Name);
					break;

				case ContentType.Monsters:
					ContentSelectionBox.ItemsSource = GameContent.Monsters.Select(x => x.Name);
					break;

				case ContentType.Bosses:
					ContentSelectionBox.ItemsSource = GameContent.Bosses.Select(x => x.Name);
					break;

				case ContentType.Regions:
					ContentSelectionBox.ItemsSource = GameContent.Regions.Select(x => x.Name);
					break;

				case ContentType.Quests:
					ContentSelectionBox.ItemsSource = GameContent.Quests.Select(x => x.Name);
					break;

				case ContentType.Shop:
					// special
					break;

				case ContentType.Priest:
					// special
					break;
			}
		}

		public void RefreshControls<T>()
		{
			// https://stackoverflow.com/questions/63834841/how-to-add-a-materialdesignhint-to-a-textbox-in-code

			// clear grid's first column to avoid duplicating the controls added below
			// how?
			// use the Dictionary

			if (_currentGrid != null)
			{
				_currentGrid.Children.Clear();
				MainGrid.Children.Remove(_currentGrid);
			}

			double gridHeight = this.ActualHeight;
			var grid = new Grid() {Name = "ControlsGrid"};
			
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(gridHeight / 8)});
			grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(gridHeight / 8)});
			grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(gridHeight / 4)});
			grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(gridHeight / 4)});
			grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(gridHeight / 4)});

			if (typeof(T) == typeof(Artifact))
			{
				var selectedArtifact = _dataContext as Artifact;

				var idBox = new TextBox() {Name = "IdBox", Text = selectedArtifact.Id.ToString(), Margin = new Thickness(10)};
				var nameBox = new TextBox() {Name="NameBox", Text = selectedArtifact.Name, Margin = new Thickness(10)};
				var valueBox = new TextBox() {Name="ValueBox", Text = selectedArtifact.Value.ToString(), Margin = new Thickness(10)};
				var rarityBox = new ComboBox() {Name="RarityBox", ItemsSource = Enum.GetValues(typeof(Rarity)), SelectedIndex = (int)selectedArtifact.Rarity, Margin = new Thickness(10)};
				var artifactTypeBox = new ComboBox() {Name="ArtifactTypeBox", ItemsSource = Enum.GetValues(typeof(ArtifactType)), SelectedIndex = (int)selectedArtifact.ArtifactType, Margin = new Thickness(10)};
				var descriptionBox = new TextBox()
				{
					Name="DescriptionBox",
					TextWrapping = TextWrapping.Wrap,
					VerticalAlignment = VerticalAlignment.Stretch,
					MinWidth = 280,
					AcceptsReturn = true,
					VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
					Height = 80,
					Text = selectedArtifact.Description,
					Margin = new Thickness(10)
				};

				var loreBox = new TextBox()
				{
					Name="LoreBox",
					TextWrapping = TextWrapping.Wrap,
					VerticalAlignment = VerticalAlignment.Stretch,
					MinWidth = 280,
					AcceptsReturn = true,
					VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
					Height = 80,
					Text=selectedArtifact.Lore,
					Margin = new Thickness(10)
				};

				var extraInfoBox = new TextBox()
				{
					Name="ExtraInfoBox",
					TextWrapping = TextWrapping.Wrap,
					VerticalAlignment = VerticalAlignment.Stretch,
					MinWidth = 280,
					AcceptsReturn = true,
					VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
					Height = 80,
					Text = selectedArtifact.ExtraInfo,
					Margin = new Thickness(10)
				};

				var makeChangesButton = new Button()
				{
					Name="MakeChangesButton",
					Content = "Make changes",
					Margin = new Thickness(10)
				};
				makeChangesButton.Click += MakeChangesButton_Click;
				
				// Set Grid positions.
				Grid.SetColumn(idBox, 0);
				Grid.SetRow(idBox, 0);
				Grid.SetColumn(nameBox, 1);
				Grid.SetRow(nameBox, 0);
				Grid.SetColumn(valueBox, 2);
				Grid.SetRow(valueBox, 0);
				
				Grid.SetColumn(rarityBox, 0);
				Grid.SetRow(rarityBox, 1);
				Grid.SetColumn(artifactTypeBox, 2);
				Grid.SetRow(artifactTypeBox, 1);
				
				Grid.SetColumnSpan(descriptionBox, 3);
				Grid.SetRow(descriptionBox, 2);
				Grid.SetColumnSpan(loreBox, 3);
				Grid.SetRow(loreBox, 3);
				Grid.SetColumnSpan(extraInfoBox, 3);
				Grid.SetRow(extraInfoBox, 4);
				
				Grid.SetColumn(makeChangesButton, 1);
				Grid.SetRow(makeChangesButton, 1);
				
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
				_controls.Add(makeChangesButton.Name, makeChangesButton);
				
				foreach (var elem in _controls)
				{
					// Set style of each control to MaterialDesignFloatingHint, and set floating hint scale.
					if (elem.Value is TextBox textBox)
					{
						textBox.Style = (Style) this.FindResource("MaterialDesignFloatingHintTextBox");
						HintAssist.SetFloatingScale(elem.Value, 1.0);
					}
					else if (elem.Value is ComboBox comboBox)
					{
						comboBox.Style = (Style) this.FindResource("MaterialDesignFloatingHintComboBox");
						HintAssist.SetFloatingScale(elem.Value, 1.0);
					}
					
					grid.Children.Add(elem.Value);
				}
			}
			
			Grid.SetColumn(grid, 1);

			_currentGrid = grid;

			MainGrid.Children.Add(grid);
		}
		
		private void MakeChangesButton_Click(object sender, RoutedEventArgs e)
		{
			switch (_currentContentType)
			{
				case ContentType.Artifacts:
				{
					var artifact = _dataContext as Artifact;
					
					artifact.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
					artifact.Name = (_controls["NameBox"] as TextBox).Text;
					artifact.Value=int.Parse((_controls["ValueBox"] as TextBox).Text);
					artifact.Rarity = (Rarity)Enum.Parse(typeof(Rarity), (_controls["RarityBox"] as ComboBox).SelectedValue.ToString());
					artifact.ArtifactType = (ArtifactType)Enum.Parse(typeof(ArtifactType), (_controls["ArtifactTypeBox"] as ComboBox).SelectedValue.ToString());
					artifact.Description = (_controls["DescriptionBox"] as TextBox).Text;
					artifact.Lore = (_controls["LoreBox"] as TextBox).Text;
					artifact.ExtraInfo = (_controls["ExtraInfoBox"] as TextBox).Text;
			
					// Check if this Id is already in the collection (modified).
					if (GameContent.Artifacts.Select(x=>x.Id).Contains(artifact.Id))
					{
						int indexOfOldArtifact = GameContent.Artifacts.FindIndex(x => x.Id == artifact.Id);
						GameContent.Artifacts[indexOfOldArtifact] = artifact;
					}
					else
					{
						// If not, add it.
						GameContent.Artifacts.Add(artifact);
					}
				}
					break;
			}
		}

		private void AddNewButton_Click(object sender, RoutedEventArgs e)
		{
			switch (_currentContentType)
			{
				case ContentType.Artifacts:
					int nextId = GameContent.Artifacts.Max(x => x.Id)+1;
					_dataContext = new Artifact() {Id = nextId};
					RefreshControls<Artifact>();
					break;
			}
		}

		private void ContentSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedName = (e.Source as ComboBox)?.SelectedValue.ToString();

			// nie wiem czy nie trzeba jednak oddzielnych paneli od poszczególnych kontrolek bo sie robi zadyma tutaj
			
			switch (_currentContentType)
			{
				case ContentType.Artifacts:
					_dataContext = GameContent.Artifacts.FirstOrDefault(x => x.Name == selectedName);
					RefreshControls<Artifact>();
					break;
			}
		}
	}
}