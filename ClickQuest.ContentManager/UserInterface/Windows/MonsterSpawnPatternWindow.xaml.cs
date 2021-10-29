using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.Models;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.ContentManager.UserInterface.Windows
{
	public partial class MonsterSpawnPatternWindow : Window
	{
		private Region _recipe;
		private MonsterSpawnPattern _pattern;
		private Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();

		public MonsterSpawnPatternWindow(Region region, MonsterSpawnPattern pattern)
		{
			InitializeComponent();

			_recipe = region;
			_pattern = pattern;

			RefreshWindowControls();
		}

		public void RefreshWindowControls()
		{
			// https://stackoverflow.com/questions/63834841/how-to-add-a-materialdesignhint-to-a-textbox-in-code

			// clear grid's first column to avoid duplicating the controls added below
			// how?
			// use the Dictionary

			// Add controls to Dictionary for easier navigation.
			_controls.Clear();

			double gridHeight = this.ActualHeight;
			double gridWidth = this.ActualWidth;
			var panel = new StackPanel() { Name = "MainInfoPanel" };

			var saveButton = new Button() { Name = "SaveButton", Content = "Save", Margin = new Thickness(10) };
			saveButton.Click += SaveButton_Click;
			_controls.Add(saveButton.Name, saveButton);

			var idBox = new TextBox() { Name = "IdBox", Text = _pattern.MonsterId.ToString(), Margin = new Thickness(10), IsEnabled = false };

			var nameBox = new ComboBox() { Name = "NameBox", ItemsSource = GameContent.Monsters.Select(x => x.Name), Margin = new Thickness(10) };
			nameBox.SelectedValue = GameContent.Monsters.FirstOrDefault(x => x.Id == _pattern.MonsterId)?.Name;
			nameBox.SelectionChanged += NameBox_SelectionChanged;

			var frequencyBox = new TextBox() { Name = "FrequencyBox", Text = _pattern.Frequency.ToString(), Margin = new Thickness(10) };

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
					textBox.Style = (Style)this.FindResource("MaterialDesignOutlinedTextBox");
					HintAssist.SetFloatingScale(elem.Value, 1.0);
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

		private void NameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			(_controls["IdBox"] as TextBox).Text = GameContent.Monsters.FirstOrDefault(x => x.Name == (sender as ComboBox).SelectedItem.ToString()).Id.ToString();
		}

		private void UpdateMonsterPattern()
		{
			var oldPatternIndex = _recipe.MonsterSpawnPatterns.IndexOf(_recipe.MonsterSpawnPatterns.FirstOrDefault(x => x.MonsterId == _pattern.MonsterId));

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

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			UpdateMonsterPattern();
		}
	}
}
