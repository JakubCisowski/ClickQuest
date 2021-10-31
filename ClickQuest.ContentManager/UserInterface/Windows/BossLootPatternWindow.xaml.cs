using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.GameData.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.ContentManager.UserInterface.Windows
{
	public partial class BossLootPatternWindow : Window
	{
		private Boss _boss;
		private BossLootPattern _pattern;
		private Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();

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

			double gridHeight = this.ActualHeight;
			double gridWidth = this.ActualWidth;
			var panel = new StackPanel() { Name = "MainInfoPanel" };

			var idBox = new TextBox() { Name = "IdBox", Text = _pattern.LootId.ToString(), Margin = new Thickness(10), IsEnabled = false };
			_controls.Add(idBox.Name, idBox);

			var nameBox = new ComboBox() { Name = "NameBox", Margin = new Thickness(10) };
			nameBox.SelectionChanged += NameBox_SelectionChanged;
			_controls.Add(nameBox.Name, nameBox);

			var lootTypeBox = new ComboBox() { Name = "LootTypeBox", ItemsSource = Enum.GetValues(typeof(LootType)), Margin = new Thickness(10) };
			lootTypeBox.SelectionChanged += LootTypeBox_SelectionChanged;
			lootTypeBox.SelectedValue = _pattern.LootType;

			var frequencyBox1 = new TextBox() { Name = "FrequencyBox1", Text = _pattern.Frequencies[0].ToString(), Margin = new Thickness(10) };
			var frequencyBox2 = new TextBox() { Name = "FrequencyBox2", Text = _pattern.Frequencies[1].ToString(), Margin = new Thickness(10) };
			var frequencyBox3 = new TextBox() { Name = "FrequencyBox3", Text = _pattern.Frequencies[2].ToString(), Margin = new Thickness(10) };
			var frequencyBox4 = new TextBox() { Name = "FrequencyBox4", Text = _pattern.Frequencies[3].ToString(), Margin = new Thickness(10) };
			var frequencyBox5 = new TextBox() { Name = "FrequencyBox5", Text = _pattern.Frequencies[4].ToString(), Margin = new Thickness(10) };
			var frequencyBox6 = new TextBox() { Name = "FrequencyBox6", Text = _pattern.Frequencies[5].ToString(), Margin = new Thickness(10) };

			// Set TextBox and ComboBox hints.
			HintAssist.SetHint(idBox, "ID");
			HintAssist.SetHint(nameBox, "Name");
			HintAssist.SetHint(lootTypeBox, "LootType");
			HintAssist.SetHint(frequencyBox1, "Frequency1");
			HintAssist.SetHint(frequencyBox2, "Frequency2");
			HintAssist.SetHint(frequencyBox3, "Frequency3");
			HintAssist.SetHint(frequencyBox4, "Frequency4");
			HintAssist.SetHint(frequencyBox5, "Frequency5");
			HintAssist.SetHint(frequencyBox6, "Frequency6");

			_controls.Add(lootTypeBox.Name, lootTypeBox);
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
					textBox.Style = (Style)this.FindResource("MaterialDesignOutlinedTextBox");
					HintAssist.SetFloatingScale(elem.Value, 1.0);
					textBox.GotFocus += TextBox_GotFocus;
				}
				else if (elem.Value is ComboBox comboBox)
				{
					comboBox.Style = (Style)this.FindResource("MaterialDesignOutlinedComboBox");
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

			switch (_pattern.LootType)
			{
				case LootType.Material:
					(_controls["IdBox"] as TextBox).Text = GameContent.Materials.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
					break;
				case LootType.Recipe:
					(_controls["IdBox"] as TextBox).Text = GameContent.Recipes.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
					break;
				case LootType.Artifact:
					(_controls["IdBox"] as TextBox).Text = GameContent.Artifacts.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
					break;
				case LootType.Blessing:
					(_controls["IdBox"] as TextBox).Text = GameContent.Blessings.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
					break;
			}
		}

		private void LootTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!_controls.ContainsKey("NameBox"))
			{
				return;
			}

			switch ((LootType)Enum.Parse(typeof(LootType), (sender as ComboBox).SelectedValue.ToString()))
			{
				case LootType.Material:
					(_controls["NameBox"] as ComboBox).ItemsSource = GameContent.Materials.Select(x=>x.Name);
					(_controls["NameBox"] as ComboBox).SelectedValue = GameContent.Materials.FirstOrDefault(x=>x.Id == _pattern.LootId)?.Name;
					break;
				case LootType.Recipe:
					(_controls["NameBox"] as ComboBox).ItemsSource = GameContent.Recipes.Select(x=>x.Name);
					(_controls["NameBox"] as ComboBox).SelectedValue = GameContent.Recipes.FirstOrDefault(x=>x.Id == _pattern.LootId)?.Name;
					break;
				case LootType.Artifact:
					(_controls["NameBox"] as ComboBox).ItemsSource = GameContent.Artifacts.Select(x=>x.Name);
					(_controls["NameBox"] as ComboBox).SelectedValue = GameContent.Artifacts.FirstOrDefault(x=>x.Id == _pattern.LootId)?.Name;
					break;
				case LootType.Blessing:
					(_controls["NameBox"] as ComboBox).ItemsSource = GameContent.Blessings.Select(x=>x.Name);
					(_controls["NameBox"] as ComboBox).SelectedValue = GameContent.Blessings.FirstOrDefault(x=>x.Id == _pattern.LootId)?.Name;
					break;
			}

			_pattern.LootType = (LootType)Enum.Parse(typeof(LootType), (sender as ComboBox).SelectedValue.ToString());
		}

		private void UpdateBossLootPattern()
		{
			var oldPatternIndex = _boss.BossLootPatterns.IndexOf(_boss.BossLootPatterns.FirstOrDefault(x => x.LootId == _pattern.LootId));

			_pattern.LootId = int.Parse((_controls["IdBox"] as TextBox).Text);
			_pattern.LootType = (LootType)Enum.Parse(typeof(LootType), (_controls["LootTypeBox"] as ComboBox).SelectedValue.ToString());
			_pattern.Frequencies = new List<double>()
			{
				double.Parse((_controls["FrequencyBox1"] as TextBox).Text),
				double.Parse((_controls["FrequencyBox2"] as TextBox).Text),
				double.Parse((_controls["FrequencyBox3"] as TextBox).Text),
				double.Parse((_controls["FrequencyBox4"] as TextBox).Text),
				double.Parse((_controls["FrequencyBox5"] as TextBox).Text),
				double.Parse((_controls["FrequencyBox6"] as TextBox).Text),
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

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			UpdateBossLootPattern();
		}
	}
}
