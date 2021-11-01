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
	public partial class QuestRewardPatternWindow : Window
	{
		private Quest _quest;
		private QuestRewardPattern _pattern;
		private Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();

		public QuestRewardPatternWindow(Quest quest, QuestRewardPattern pattern)
		{
			InitializeComponent();

			_quest = quest;
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

			var idBox = new TextBox() { Name = "IdBox", Text = _pattern.Id.ToString(), Margin = new Thickness(10), IsEnabled = false };
			_controls.Add(idBox.Name, idBox);

			var nameBox = new ComboBox() { Name = "NameBox", Margin = new Thickness(10) };
			nameBox.SelectionChanged += NameBox_SelectionChanged;
			_controls.Add(nameBox.Name, nameBox);

			var rewardTypeBox = new ComboBox() { Name = "RewardTypeBox", ItemsSource = Enum.GetValues(typeof(RewardType)), Margin = new Thickness(10) };
			rewardTypeBox.SelectionChanged += RewardTypeBox_SelectionChanged;
			rewardTypeBox.SelectedValue = _pattern.RewardType;

			var quantityBox = new TextBox() { Name = "QuantityBox", Text = _pattern.Quantity.ToString(), Margin = new Thickness(10) };

			// Set TextBox and ComboBox hints.
			HintAssist.SetHint(idBox, "ID");
			HintAssist.SetHint(nameBox, "Name");
			HintAssist.SetHint(rewardTypeBox, "RewardType");
			HintAssist.SetHint(quantityBox, "Quantity");

			_controls.Add(rewardTypeBox.Name, rewardTypeBox);
			_controls.Add(quantityBox.Name, quantityBox);

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

			switch (_pattern.RewardType)
			{
				case RewardType.Material:
					(_controls["IdBox"] as TextBox).Text = GameContent.Materials.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
					break;

				case RewardType.Recipe:
					(_controls["IdBox"] as TextBox).Text = GameContent.Recipes.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
					break;

				case RewardType.Artifact:
					(_controls["IdBox"] as TextBox).Text = GameContent.Artifacts.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
					break;

				case RewardType.Blessing:
					(_controls["IdBox"] as TextBox).Text = GameContent.Blessings.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
					break;

				case RewardType.Ingot:
					(_controls["IdBox"] as TextBox).Text = GameContent.Ingots.FirstOrDefault(x => x.Name == comboBox.SelectedValue.ToString()).Id.ToString();
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
					(_controls["NameBox"] as ComboBox).ItemsSource = GameContent.Materials.Select(x=>x.Name);
					break;

				case RewardType.Recipe:
					(_controls["NameBox"] as ComboBox).ItemsSource = GameContent.Recipes.Select(x=>x.Name);
					break;

				case RewardType.Artifact:
					(_controls["NameBox"] as ComboBox).ItemsSource = GameContent.Artifacts.Select(x=>x.Name);
					break;

				case RewardType.Blessing:
					(_controls["NameBox"] as ComboBox).ItemsSource = GameContent.Blessings.Select(x=>x.Name);
					break;

				case RewardType.Ingot:
					(_controls["NameBox"] as ComboBox).ItemsSource = GameContent.Ingots.Select(x=>x.Name);
					break;
			}

			_pattern.RewardType = (RewardType)Enum.Parse(typeof(RewardType), (sender as ComboBox).SelectedValue.ToString());

			(_controls["NameBox"] as ComboBox).SelectedIndex = 0;
		}

		private void UpdateQuestRewardPattern()
		{
			var oldPatternIndex = _quest.QuestRewardPatterns.IndexOf(_quest.QuestRewardPatterns.FirstOrDefault(x => x.Id == _pattern.Id));

			_pattern.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
			_pattern.RewardType = (RewardType)Enum.Parse(typeof(RewardType), (_controls["RewardTypeBox"] as ComboBox).SelectedValue.ToString());
			_pattern.Quantity = int.Parse((_controls["QuantityBox"] as TextBox).Text);

			if (oldPatternIndex == -1)
			{
				_quest.QuestRewardPatterns.Add(_pattern);
			}
			else
			{
				_quest.QuestRewardPatterns[oldPatternIndex] = _pattern;
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			UpdateQuestRewardPattern();
		}
	}
}
