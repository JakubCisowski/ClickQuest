using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.GameData.Models;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.ContentManager.UserInterface.Windows
{
	public partial class MonsterSpawnPatternWindow : Window
	{
		private readonly Region _recipe;
		private readonly MonsterSpawnPattern _pattern;
		private readonly Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();

		public MonsterSpawnPatternWindow(Region region, MonsterSpawnPattern pattern)
		{
			InitializeComponent();

			_recipe = region;
			_pattern = pattern;

			RefreshWindowControls();
		}

		public void RefreshWindowControls()
		{
			// Add controls to Dictionary for easier navigation.
			_controls.Clear();

			StackPanel panel = new StackPanel
			{
				Name = "MainInfoPanel"
			};

			TextBox idBox = new TextBox
			{
				Name = "IdBox",
				Text = _pattern.MonsterId.ToString(),
				Margin = new Thickness(10),
				IsEnabled = false
			};

			ComboBox nameBox = new ComboBox
			{
				Name = "NameBox",
				ItemsSource = GameContent.Monsters.Select(x => x.Name),
				Margin = new Thickness(10)
			};
			nameBox.SelectedValue = GameContent.Monsters.FirstOrDefault(x => x.Id == _pattern.MonsterId)?.Name;
			nameBox.SelectionChanged += NameBox_SelectionChanged;

			TextBox frequencyBox = new TextBox
			{
				Name = "FrequencyBox",
				Text = _pattern.Frequency.ToString(),
				Margin = new Thickness(10)
			};

			// Set TextBox and ComboBox hints.
			HintAssist.SetHint(idBox, "ID");
			HintAssist.SetHint(nameBox, "Name");
			HintAssist.SetHint(frequencyBox, "Frequency");

			_controls.Add(idBox.Name, idBox);
			_controls.Add(nameBox.Name, nameBox);
			_controls.Add(frequencyBox.Name, frequencyBox);

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

			MainGrid.Children.Add(panel);
		}

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			(sender as TextBox).CaretIndex = int.MaxValue;
		}

		private void NameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			(_controls["IdBox"] as TextBox).Text = GameContent.Monsters.FirstOrDefault(x => x.Name == (sender as ComboBox).SelectedItem.ToString()).Id.ToString();
		}

		private void UpdateMonsterPattern()
		{
			int oldPatternIndex = _recipe.MonsterSpawnPatterns.IndexOf(_recipe.MonsterSpawnPatterns.FirstOrDefault(x => x.MonsterId == _pattern.MonsterId));

			_pattern.MonsterId = int.Parse((_controls["IdBox"] as TextBox).Text);
			_pattern.Frequency = double.Parse((_controls["FrequencyBox"] as TextBox).Text);

			if (oldPatternIndex == -1)
			{
				_recipe.MonsterSpawnPatterns.Add(_pattern);
			}
			else
			{
				_recipe.MonsterSpawnPatterns[oldPatternIndex] = _pattern;
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			UpdateMonsterPattern();
		}
	}
}