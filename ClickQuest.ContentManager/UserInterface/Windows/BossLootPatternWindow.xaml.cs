using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.ContentManager.Logic.DataTypes.Enums;
using ClickQuest.ContentManager.Logic.Models;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.ContentManager.UserInterface.Windows;

public partial class BossLootPatternWindow : Window
{
	private readonly Boss _boss;
	private readonly BossLootPattern _pattern;
	private readonly Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();

	public BossLootPatternWindow(Boss boss, BossLootPattern pattern)
	{
		InitializeComponent();

		_boss = boss;
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
			Text = _pattern.BossLootId.ToString(),
			Margin = new Thickness(10),
			IsEnabled = false
		};
		_controls.Add(idBox.Name, idBox);

		var nameBox = new ComboBox
		{
			Name = "NameBox",
			Margin = new Thickness(10)
		};
		nameBox.SelectionChanged += NameBox_SelectionChanged;
		_controls.Add(nameBox.Name, nameBox);

		var rewardTypeBox = new ComboBox
		{
			Name = "RewardTypeBox",
			ItemsSource = Enum.GetValues(typeof(RewardType)),
			Margin = new Thickness(10)
		};
		rewardTypeBox.SelectionChanged += RewardTypeBox_SelectionChanged;
		rewardTypeBox.SelectedValue = _pattern.BossLootType;

		var frequencyBox1 = new TextBox
		{
			Name = "FrequencyBox1",
			Text = _pattern.Frequencies[0].ToString(),
			Margin = new Thickness(10)
		};
		var frequencyBox2 = new TextBox
		{
			Name = "FrequencyBox2",
			Text = _pattern.Frequencies[1].ToString(),
			Margin = new Thickness(10)
		};
		var frequencyBox3 = new TextBox
		{
			Name = "FrequencyBox3",
			Text = _pattern.Frequencies[2].ToString(),
			Margin = new Thickness(10)
		};
		var frequencyBox4 = new TextBox
		{
			Name = "FrequencyBox4",
			Text = _pattern.Frequencies[3].ToString(),
			Margin = new Thickness(10)
		};
		var frequencyBox5 = new TextBox
		{
			Name = "FrequencyBox5",
			Text = _pattern.Frequencies[4].ToString(),
			Margin = new Thickness(10)
		};
		var frequencyBox6 = new TextBox
		{
			Name = "FrequencyBox6",
			Text = _pattern.Frequencies[5].ToString(),
			Margin = new Thickness(10)
		};

		// Set TextBox and ComboBox hints.
		HintAssist.SetHint(idBox, "ID");
		HintAssist.SetHint(nameBox, "Name");
		HintAssist.SetHint(rewardTypeBox, "RewardType");
		HintAssist.SetHint(frequencyBox1, "Frequency1");
		HintAssist.SetHint(frequencyBox2, "Frequency2");
		HintAssist.SetHint(frequencyBox3, "Frequency3");
		HintAssist.SetHint(frequencyBox4, "Frequency4");
		HintAssist.SetHint(frequencyBox5, "Frequency5");
		HintAssist.SetHint(frequencyBox6, "Frequency6");

		_controls.Add(rewardTypeBox.Name, rewardTypeBox);
		_controls.Add(frequencyBox1.Name, frequencyBox1);
		_controls.Add(frequencyBox2.Name, frequencyBox2);
		_controls.Add(frequencyBox3.Name, frequencyBox3);
		_controls.Add(frequencyBox4.Name, frequencyBox4);
		_controls.Add(frequencyBox5.Name, frequencyBox5);
		_controls.Add(frequencyBox6.Name, frequencyBox6);

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

		switch (_pattern.BossLootType)
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

		_pattern.BossLootType = (RewardType)Enum.Parse(typeof(RewardType), (sender as ComboBox).SelectedValue.ToString());

		(_controls["NameBox"] as ComboBox).SelectedIndex = 0;
	}

	private void UpdateBossLootPattern()
	{
		var oldPatternIndex = _boss.BossLootPatterns.IndexOf(_boss.BossLootPatterns.FirstOrDefault(x => x.BossLootId == _pattern.BossLootId));

		_pattern.BossLootId = int.Parse((_controls["IdBox"] as TextBox).Text);
		_pattern.BossLootType = (RewardType)Enum.Parse(typeof(RewardType), (_controls["RewardTypeBox"] as ComboBox).SelectedValue.ToString());
		_pattern.Frequencies = new List<double>
		{
			double.Parse((_controls["FrequencyBox1"] as TextBox).Text),
			double.Parse((_controls["FrequencyBox2"] as TextBox).Text),
			double.Parse((_controls["FrequencyBox3"] as TextBox).Text),
			double.Parse((_controls["FrequencyBox4"] as TextBox).Text),
			double.Parse((_controls["FrequencyBox5"] as TextBox).Text),
			double.Parse((_controls["FrequencyBox6"] as TextBox).Text)
		};

		if (oldPatternIndex == -1)
		{
			_boss.BossLootPatterns.Add(_pattern);
		}
		else
		{
			_boss.BossLootPatterns[oldPatternIndex] = _pattern;
		}
	}

	private void Window_Closing(object sender, CancelEventArgs e)
	{
		UpdateBossLootPattern();
	}
}