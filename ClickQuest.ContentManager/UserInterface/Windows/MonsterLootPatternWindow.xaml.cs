using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.ContentManager.Logic.DataTypes.Enums;
using ClickQuest.ContentManager.Logic.DataTypes.Structs;
using ClickQuest.ContentManager.Logic.Models;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.ContentManager.UserInterface.Windows;

public partial class MonsterLootPatternWindow : Window
{
	private readonly Monster _monster;
	private readonly MonsterLootPattern _pattern;
	private readonly Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();

	public MonsterLootPatternWindow(Monster monster, MonsterLootPattern pattern)
	{
		InitializeComponent();

		_monster = monster;
		_pattern = pattern;

		RefreshWindowControls();
	}

	public void RefreshWindowControls()
	{
		// Add controls to Dictionary for easier navigation.
		_controls.Clear();

		var panel = new StackPanel
		{
			Name = "MainInfoPanel"
		};

		var idBox = new TextBox
		{
			Name = "IdBox",
			Text = _pattern.MonsterLootId.ToString(),
			Margin = new Thickness(10),
			IsEnabled = false
		};
		_controls.Add(idBox.Name, idBox);

		var nameBox = new ComboBox
		{
			Name = "NameBox",
			Margin = new Thickness(10)
		};
		_controls.Add(nameBox.Name, nameBox);

		var rewardTypeBox = new ComboBox
		{
			Name = "RewardTypeBox",
			ItemsSource = Enum.GetValues(typeof(RewardType)),
			Margin = new Thickness(10)
		};
		rewardTypeBox.SelectionChanged += RewardTypeBox_SelectionChanged;
		rewardTypeBox.SelectedValue = _pattern.MonsterLootType;
		
		nameBox.SelectionChanged += NameBox_SelectionChanged;
		nameBox.SelectedValue = _pattern.Item?.Name;

		var frequencyBox = new TextBox
		{
			Name = "FrequencyBox",
			Text = _pattern.Frequency.ToString(),
			Margin = new Thickness(10)
		};

		// Set TextBox and ComboBox hints.
		HintAssist.SetHint(idBox, "ID");
		HintAssist.SetHint(nameBox, "Name");
		HintAssist.SetHint(rewardTypeBox, "RewardType");
		HintAssist.SetHint(frequencyBox, "Frequency");

		_controls.Add(rewardTypeBox.Name, rewardTypeBox);
		_controls.Add(frequencyBox.Name, frequencyBox);

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

		MainGrid.Children.Add(panel);
	}

	private void TextBox_GotFocus(object sender, RoutedEventArgs e)
	{
		(sender as TextBox).CaretIndex = int.MaxValue;
	}

	private void NameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (!_controls.ContainsKey("IdBox"))
		{
			return;
		}

		var comboBox = sender as ComboBox;

		if (comboBox.SelectedValue is null)
		{
			return;
		}

		switch (_pattern.MonsterLootType)
		{
			case RewardType.Material:
				(_controls["IdBox"] as TextBox).Text = GameAssets.Materials.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
				break;

			case RewardType.Recipe:
				(_controls["IdBox"] as TextBox).Text = GameAssets.Recipes.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
				break;

			case RewardType.Artifact:
				(_controls["IdBox"] as TextBox).Text = GameAssets.Artifacts.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
				break;

			case RewardType.Blessing:
				(_controls["IdBox"] as TextBox).Text = GameAssets.Blessings.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
				break;

			case RewardType.Ingot:
				(_controls["IdBox"] as TextBox).Text = GameAssets.Ingots.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
				break;
		}
	}

	private void RewardTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (!_controls.ContainsKey("NameBox"))
		{
			return;
		}

		switch ((RewardType)Enum.Parse(typeof(RewardType), (sender as ComboBox).SelectedValue.ToString()))
		{
			case RewardType.Material:
				(_controls["NameBox"] as ComboBox).ItemsSource = GameAssets.Materials.Select(x => x.Name);
				break;

			case RewardType.Recipe:
				(_controls["NameBox"] as ComboBox).ItemsSource = GameAssets.Recipes.Select(x => x.Name);
				break;

			case RewardType.Artifact:
				(_controls["NameBox"] as ComboBox).ItemsSource = GameAssets.Artifacts.Select(x => x.Name);
				break;

			case RewardType.Blessing:
				(_controls["NameBox"] as ComboBox).ItemsSource = GameAssets.Blessings.Select(x => x.Name);
				break;

			case RewardType.Ingot:
				(_controls["NameBox"] as ComboBox).ItemsSource = GameAssets.Ingots.Select(x => x.Name);
				break;
		}

		_pattern.MonsterLootType = (RewardType)Enum.Parse(typeof(RewardType), (sender as ComboBox).SelectedValue.ToString());

		(_controls["NameBox"] as ComboBox).SelectedIndex = 0;
	}

	private void UpdateMonsterLootPattern()
	{
		var oldPatternIndex = _monster.MonsterLootPatterns.IndexOf(_monster.MonsterLootPatterns.FirstOrDefault(x => x.MonsterLootId == _pattern.MonsterLootId));

		_pattern.MonsterLootId = int.Parse((_controls["IdBox"] as TextBox).Text);
		_pattern.MonsterLootType = (RewardType)Enum.Parse(typeof(RewardType), (_controls["RewardTypeBox"] as ComboBox).SelectedValue.ToString());
		_pattern.Frequency = double.Parse((_controls["FrequencyBox"] as TextBox).Text);

		if (oldPatternIndex == -1)
		{
			_monster.MonsterLootPatterns.Add(_pattern);
		}
		else
		{
			_monster.MonsterLootPatterns[oldPatternIndex] = _pattern;
		}
	}

	private void Window_Closing(object sender, CancelEventArgs e)
	{
		UpdateMonsterLootPattern();
	}
}