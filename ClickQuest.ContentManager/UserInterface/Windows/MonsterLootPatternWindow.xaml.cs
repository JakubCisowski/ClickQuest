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
	public partial class MonsterLootPatternWindow : Window
	{
		private Monster _monster;
		private MonsterLootPattern _pattern;
		private Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();

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

			var frequencyBox = new TextBox() { Name = "FrequencyBox", Text = _pattern.Frequency.ToString(), Margin = new Thickness(10) };

			// Set TextBox and ComboBox hints.
			HintAssist.SetHint(idBox, "ID");
			HintAssist.SetHint(nameBox, "Name");
			HintAssist.SetHint(lootTypeBox, "LootType");
			HintAssist.SetHint(frequencyBox, "Frequency");

			_controls.Add(lootTypeBox.Name, lootTypeBox);
			_controls.Add(frequencyBox.Name, frequencyBox);

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

		private void UpdateMonsterLootPattern()
		{
			var oldPatternIndex = _monster.MonsterLootPatterns.IndexOf(_monster.MonsterLootPatterns.FirstOrDefault(x => x.LootId == _pattern.LootId));

			_pattern.LootId = int.Parse((_controls["IdBox"] as TextBox).Text);
			_pattern.LootType = (LootType)Enum.Parse(typeof(LootType), (_controls["LootTypeBox"] as ComboBox).SelectedValue.ToString());
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

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			UpdateMonsterLootPattern();
		}
	}
}
