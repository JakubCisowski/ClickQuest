﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.ContentManager.Logic.DataTypes.Enums;
using ClickQuest.ContentManager.Logic.Models;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.ContentManager.UserInterface.Panels;

public partial class DungeonGroupsPanel : UserControl
{
	private DungeonGroup _dataContext;
	private readonly Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();
	private StackPanel _currentPanel;

	public DungeonGroupsPanel()
	{
		InitializeComponent();

		PopulateContentSelectionBox();
	}

	private void PopulateContentSelectionBox()
	{
		ContentSelectionBox.ItemsSource = GameAssets.DungeonGroups.Select(x => x.Name);
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

		var selectedDungeonGroup = _dataContext;

		var idBox = new TextBox
		{
			Name = "IdBox",
			Text = selectedDungeonGroup.Id.ToString(),
			Margin = new Thickness(10),
			IsEnabled = false
		};
		var nameBox = new TextBox
		{
			Name = "NameBox",
			Text = selectedDungeonGroup.Name,
			Margin = new Thickness(10)
		};
		var colorBox = new TextBox
		{
			Name = "ColorBox",
			Text = selectedDungeonGroup.Color,
			Margin = new Thickness(10)
		};

		var rarityBoxGeneral = new TextBox
		{
			Name = "RarityBoxGeneral",
			Text = selectedDungeonGroup.KeyRequirementRarities.Count(x => x == Rarity.General).ToString(),
			Margin = new Thickness(10)
		};
		var rarityBoxFine = new TextBox
		{
			Name = "RarityBoxFine",
			Text = selectedDungeonGroup.KeyRequirementRarities.Count(x => x == Rarity.Fine).ToString(),
			Margin = new Thickness(10)
		};
		var rarityBoxSuperior = new TextBox
		{
			Name = "RarityBoxSuperior",
			Text = selectedDungeonGroup.KeyRequirementRarities.Count(x => x == Rarity.Superior).ToString(),
			Margin = new Thickness(10)
		};
		var rarityBoxExceptional = new TextBox
		{
			Name = "RarityBoxExceptional",
			Text = selectedDungeonGroup.KeyRequirementRarities.Count(x => x == Rarity.Exceptional).ToString(),
			Margin = new Thickness(10)
		};
		var rarityBoxMasterwork = new TextBox
		{
			Name = "RarityBoxMasterwork",
			Text = selectedDungeonGroup.KeyRequirementRarities.Count(x => x == Rarity.Masterwork).ToString(),
			Margin = new Thickness(10)
		};
		var rarityBoxMythic = new TextBox
		{
			Name = "RarityBoxMythic",
			Text = selectedDungeonGroup.KeyRequirementRarities.Count(x => x == Rarity.Mythic).ToString(),
			Margin = new Thickness(10)
		};

		// Set TextBox and ComboBox hints.
		HintAssist.SetHint(idBox, "ID");
		HintAssist.SetHint(nameBox, "Name");
		HintAssist.SetHint(colorBox, "Color");
		HintAssist.SetHint(rarityBoxGeneral, "General Keys Requirement");
		HintAssist.SetHint(rarityBoxFine, "Fine Keys Requirement");
		HintAssist.SetHint(rarityBoxSuperior, "Superior Keys Requirement");
		HintAssist.SetHint(rarityBoxExceptional, "Exceptional Keys Requirement");
		HintAssist.SetHint(rarityBoxMasterwork, "Masterwork Keys Requirement");
		HintAssist.SetHint(rarityBoxMythic, "Mythic Keys Requirement");

		// Add controls to Dictionary for easier navigation.
		_controls.Clear();

		_controls.Add(idBox.Name, idBox);
		_controls.Add(nameBox.Name, nameBox);
		_controls.Add(colorBox.Name, colorBox);
		_controls.Add(rarityBoxGeneral.Name, rarityBoxGeneral);
		_controls.Add(rarityBoxFine.Name, rarityBoxFine);
		_controls.Add(rarityBoxSuperior.Name, rarityBoxSuperior);
		_controls.Add(rarityBoxExceptional.Name, rarityBoxExceptional);
		_controls.Add(rarityBoxMasterwork.Name, rarityBoxMasterwork);
		_controls.Add(rarityBoxMythic.Name, rarityBoxMythic);

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
		var dungeonGroup = _dataContext;

		if (dungeonGroup is null)
		{
			return;
		}

		dungeonGroup.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
		dungeonGroup.Name = (_controls["NameBox"] as TextBox).Text;
		dungeonGroup.Color = (_controls["ColorBox"] as TextBox).Text;
		dungeonGroup.KeyRequirementRarities = new List<Rarity>();

		dungeonGroup.KeyRequirementRarities = dungeonGroup.KeyRequirementRarities.Concat(AddKeyRequirementRarities(_controls["RarityBoxGeneral"] as TextBox, Rarity.General)).ToList();
		dungeonGroup.KeyRequirementRarities = dungeonGroup.KeyRequirementRarities.Concat(AddKeyRequirementRarities(_controls["RarityBoxFine"] as TextBox, Rarity.Fine)).ToList();
		dungeonGroup.KeyRequirementRarities = dungeonGroup.KeyRequirementRarities.Concat(AddKeyRequirementRarities(_controls["RarityBoxSuperior"] as TextBox, Rarity.Superior)).ToList();
		dungeonGroup.KeyRequirementRarities = dungeonGroup.KeyRequirementRarities.Concat(AddKeyRequirementRarities(_controls["RarityBoxExceptional"] as TextBox, Rarity.Exceptional)).ToList();
		dungeonGroup.KeyRequirementRarities = dungeonGroup.KeyRequirementRarities.Concat(AddKeyRequirementRarities(_controls["RarityBoxMasterwork"] as TextBox, Rarity.Masterwork)).ToList();
		dungeonGroup.KeyRequirementRarities = dungeonGroup.KeyRequirementRarities.Concat(AddKeyRequirementRarities(_controls["RarityBoxMythic"] as TextBox, Rarity.Mythic)).ToList();

		// Check if this Id is already in the collection (modified).
		if (GameAssets.DungeonGroups.Select(x => x.Id).Contains(dungeonGroup.Id))
		{
			var indexOfOldDungeonGroup = GameAssets.DungeonGroups.FindIndex(x => x.Id == dungeonGroup.Id);
			GameAssets.DungeonGroups[indexOfOldDungeonGroup] = dungeonGroup;
		}
		else
		{
			// If not, add it.
			GameAssets.DungeonGroups.Add(dungeonGroup);
		}

		PopulateContentSelectionBox();
	}

	private List<Rarity> AddKeyRequirementRarities(TextBox textBox, Rarity rarity)
	{
		var newList = new List<Rarity>();

		for (var i = 0; i < int.Parse(textBox.Text); i++)
		{
			newList.Add(rarity);
		}

		return newList;
	}

	private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
	{
		Save();

		var nextId = (GameAssets.DungeonGroups.Max(x => x.Id as int?) ?? 0) + 1;
		_dataContext = new DungeonGroup
		{
			Id = nextId,
			KeyRequirementRarities = new List<Rarity>()
		};
		ContentSelectionBox.SelectedIndex = -1;
		RefreshStaticValuesPanel();

		DeleteObjectButton.Visibility = Visibility.Visible;
	}

	private void DeleteObjectButton_Click(object sender, RoutedEventArgs e)
	{
		Save();

		var objectToDelete = GameAssets.DungeonGroups.FirstOrDefault(x => x.Id == int.Parse((_controls["IdBox"] as TextBox).Text));

		var result = MessageBox.Show($"Are you sure you want to delete {objectToDelete.Name}? This action will close ContentManager, check Logs directory (for missing references after deleting).", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

		if (result == MessageBoxResult.No)
		{
			return;
		}

		GameAssets.DungeonGroups.Remove(objectToDelete);

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

		_dataContext = GameAssets.DungeonGroups.FirstOrDefault(x => x.Name == selectedName);
		RefreshStaticValuesPanel();
		DeleteObjectButton.Visibility = Visibility.Visible;
	}
}