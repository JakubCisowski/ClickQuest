﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClickQuest.ContentManager.Logic.DataTypes.Enums;
using ClickQuest.ContentManager.Logic.DataTypes.Structs;
using ClickQuest.ContentManager.Logic.Models;
using ClickQuest.ContentManager.UserInterface.Windows;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.ContentManager.UserInterface.Panels;

public partial class QuestsPanel : UserControl
{
	private Quest _dataContext;
	private readonly Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();
	private StackPanel _currentPanel;
	private List<QuestRewardPattern> _questRewardPatterns;

	public QuestsPanel()
	{
		InitializeComponent();

		PopulateContentSelectionBox();
	}

	private void PopulateContentSelectionBox()
	{
		ContentSelectionBox.ItemsSource = GameAssets.Quests.Select(x => x.Name);
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
			Name = "StaticInfoPanel"
		};

		var selectedQuest = _dataContext;

		var idBox = new TextBox
		{
			Name = "IdBox",
			Text = selectedQuest.Id.ToString(),
			Margin = new Thickness(10),
			IsEnabled = false
		};
		var nameBox = new TextBox
		{
			Name = "NameBox",
			Text = selectedQuest.Name,
			Margin = new Thickness(10)
		};
		var rareBox = new CheckBox
		{
			Name = "RareBox",
			Content = "Rare?",
			IsChecked = selectedQuest.Rare,
			Margin = new Thickness(10)
		};
		var heroClassBox = new ComboBox
		{
			Name = "HeroClassBox",
			ItemsSource = Enum.GetValues(typeof(HeroClass)),
			SelectedIndex = (int)selectedQuest.HeroClass,
			Margin = new Thickness(10)
		};
		var durationBox = new TextBox
		{
			Name = "DurationBox",
			Text = selectedQuest.Duration.ToString(),
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
			Text = selectedQuest.Description,
			Margin = new Thickness(10)
		};

		// Set TextBox and ComboBox hints.
		HintAssist.SetHint(idBox, "ID");
		HintAssist.SetHint(nameBox, "Name");
		HintAssist.SetHint(rareBox, "Rare");
		HintAssist.SetHint(heroClassBox, "HeroClass");
		HintAssist.SetHint(durationBox, "Duration");
		HintAssist.SetHint(descriptionBox, "Description");

		// Add controls to Dictionary for easier navigation.
		_controls.Clear();

		_controls.Add(idBox.Name, idBox);
		_controls.Add(nameBox.Name, nameBox);
		_controls.Add(rareBox.Name, rareBox);
		_controls.Add(heroClassBox.Name, heroClassBox);
		_controls.Add(durationBox.Name, durationBox);
		_controls.Add(descriptionBox.Name, descriptionBox);

		foreach (var elem in _controls)
		{
			// Set style of each control to MaterialDesignFloatingHint, and set floating hint scale.
			if (elem.Value is TextBox textBox)
			{
				textBox.Style = (Style)FindResource("MaterialDesignOutlinedTextBox");
				HintAssist.SetFloatingScale(elem.Value, 1.0);
				textBox.GotFocus += TextBox_GotFocus;
			}
			else if (elem.Value is ComboBox comboBox)
			{
				comboBox.Style = (Style)FindResource("MaterialDesignOutlinedComboBox");
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
		var quest = _dataContext;

		if (quest is null)
		{
			return;
		}

		quest.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
		quest.Name = (_controls["NameBox"] as TextBox).Text;
		quest.Rare = (_controls["RareBox"] as CheckBox).IsChecked.Value;
		quest.HeroClass = (HeroClass)Enum.Parse(typeof(HeroClass), (_controls["HeroClassBox"] as ComboBox).SelectedValue.ToString());
		quest.Duration = int.Parse((_controls["DurationBox"] as TextBox).Text);
		quest.Description = (_controls["DescriptionBox"] as TextBox).Text;

		// Check if this Id is already in the collection (modified).
		if (GameAssets.Quests.Select(x => x.Id).Contains(quest.Id))
		{
			var indexOfOldQuest = GameAssets.Quests.FindIndex(x => x.Id == quest.Id);
			GameAssets.Quests[indexOfOldQuest] = quest;
		}
		else
		{
			// If not, add it.
			GameAssets.Quests.Add(quest);
		}

		PopulateContentSelectionBox();
	}

	private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
	{
		Save();

		var nextId = (GameAssets.Quests.Max(x => x.Id as int?) ?? 0) + 1;

		_dataContext = new Quest
		{
			Id = nextId,
			QuestRewardPatterns = new List<QuestRewardPattern>()
		};
		_questRewardPatterns = _dataContext.QuestRewardPatterns;

		ContentSelectionBox.SelectedIndex = -1;
		RefreshStaticValuesPanel();
		DynamicValuesPanel.Children.Clear();

		DeleteObjectButton.Visibility = Visibility.Visible;
	}

	private void DeleteObjectButton_Click(object sender, RoutedEventArgs e)
	{
		Save();

		var objectToDelete = GameAssets.Quests.FirstOrDefault(x => x.Id == int.Parse((_controls["IdBox"] as TextBox).Text));

		var result = MessageBox.Show($"Are you sure you want to delete {objectToDelete.Name}?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

		if (result == MessageBoxResult.No)
		{
			return;
		}

		GameAssets.Quests.Remove(objectToDelete);

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

		_dataContext = GameAssets.Quests.FirstOrDefault(x => x.Name == selectedName);
		_questRewardPatterns = _dataContext.QuestRewardPatterns;
		RefreshStaticValuesPanel();
		RefreshDynamicValuesPanel();
		DeleteObjectButton.Visibility = Visibility.Visible;
	}

	public void RefreshDynamicValuesPanel()
	{
		DynamicValuesPanel.Children.Clear();

		CreateDynamicValueButton.Visibility = Visibility.Visible;

		foreach (var pattern in _questRewardPatterns)
		{
			var border = new Border
			{
				BorderThickness = new Thickness(0.5),
				BorderBrush = (SolidColorBrush)FindResource("BrushGray2"),
				Padding = new Thickness(6),
				Margin = new Thickness(4)
			};

			var grid = CreateDynamicValueGrid(pattern);

			border.Child = grid;

			DynamicValuesPanel.Children.Add(border);
		}
	}

	private Grid CreateDynamicValueGrid(QuestRewardPattern pattern)
	{
		var grid = new Grid();

		var idBlock = new TextBlock
		{
			FontSize = 18,
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Left,
			Margin = new Thickness(10, 0, 0, 0),
			FontStyle = FontStyles.Italic,
			Text = $"[{pattern.QuestRewardId}]"
		};

		// new TextBlock
		// {
		// 	FontSize = 18,
		// 	VerticalAlignment = VerticalAlignment.Center,
		// 	HorizontalAlignment = HorizontalAlignment.Left,
		// 	Margin = new Thickness(80, 0, 0, 0),
		// 	Text = pattern.QuestRewardType.ToString()
		// };

		var nameBlock = new TextBlock
		{
			FontSize = 18,
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Left,
			Margin = new Thickness(80, 0, 0, 0)
		};

		var quantityBlock = new TextBlock()
		{
			FontSize = 18,
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Left,
			Margin = new Thickness(480, 0, 0, 0),
			Text = pattern.Quantity.ToString()
		};

		switch (pattern.QuestRewardType)
		{
			case RewardType.Material:
				nameBlock.Text = GameAssets.Materials.FirstOrDefault(x => x.Id == pattern.QuestRewardId).Name;
				break;

			case RewardType.Recipe:
				nameBlock.Text = GameAssets.Recipes.FirstOrDefault(x => x.Id == pattern.QuestRewardId).Name;
				break;

			case RewardType.Artifact:
				nameBlock.Text = GameAssets.Artifacts.FirstOrDefault(x => x.Id == pattern.QuestRewardId).Name;
				break;

			case RewardType.Blessing:
				nameBlock.Text = GameAssets.Blessings.FirstOrDefault(x => x.Id == pattern.QuestRewardId).Name;
				break;

			case RewardType.Ingot:
				nameBlock.Text = GameAssets.Ingots.FirstOrDefault(x => x.Id == pattern.QuestRewardId).Name;
				break;
		}

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
			Foreground = (SolidColorBrush)FindResource("BrushGray2")
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
			Foreground = (SolidColorBrush)FindResource("BrushGray2")
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
		var pattern = (sender as Button).Tag as QuestRewardPattern;

		var questRewardPatternWindow = new QuestRewardPatternWindow(_dataContext, pattern)
		{
			Owner = Application.Current.MainWindow
		};
		questRewardPatternWindow.ShowDialog();

		RefreshDynamicValuesPanel();
	}

	private void DeleteDynamicValue_Click(object sender, RoutedEventArgs e)
	{
		var pattern = (sender as Button).Tag as QuestRewardPattern;

		var result = MessageBox.Show($"Are you sure you want to delete pattern of Id: {pattern.QuestRewardId}? This action will close ContentManager, check Logs directory (for missing references after deleting).", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

		if (result == MessageBoxResult.No)
		{
			return;
		}

		_dataContext.QuestRewardPatterns.Remove(pattern);

		RefreshDynamicValuesPanel();
	}

	private void CreateDynamicValueButton_Click(object sender, RoutedEventArgs e)
	{
		var newQuestRewardPattern = new QuestRewardPattern
		{
			Quantity = 1
		};
		_questRewardPatterns.Add(newQuestRewardPattern);

		var tempButton = new Button
		{
			Tag = newQuestRewardPattern
		};
		EditDynamicValue_Click(tempButton, null);
	}
}