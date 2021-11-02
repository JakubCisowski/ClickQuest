using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.GameData.Models;
using ClickQuest.ContentManager.UserInterface.Windows;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ClickQuest.ContentManager.UserInterface.Panels
{
	public partial class PriestOfferPatternsPanel : UserControl
	{
		private VendorPattern _dataContext;
		private Dictionary<string, FrameworkElement> _controls = new Dictionary<string, FrameworkElement>();
		private StackPanel _currentPanel;

		public PriestOfferPatternsPanel()
		{
			InitializeComponent();

			PopulateContentSelectionBox();
		}

		private void PopulateContentSelectionBox()
		{
			ContentSelectionBox.ItemsSource = GameContent.PriestOffer.Select(x => x.Id);
		}

		public void RefreshStaticValuesPanel()
		{
			// Add controls to Dictionary for easier navigation.
			_controls.Clear();

			if (_currentPanel != null)
			{
				_currentPanel.Children.Clear();
				MainGrid.Children.Remove(_currentPanel);
			}

			double gridHeight = this.ActualHeight;
			double gridWidth = this.ActualWidth;
			var panel = new StackPanel() { Name = "StaticInfoPanel" };

			var selectedVendorPattern = _dataContext;

			var idBox = new TextBox() { Name = "IdBox", Text = selectedVendorPattern.Id.ToString(), Margin = new Thickness(10), IsEnabled = false };
			
			var referencedIdBox = new TextBox() { Name = "ReferencedIdBox", Text = selectedVendorPattern.VendorItemId.ToString(), Margin = new Thickness(10), IsEnabled = false };
			_controls.Add(referencedIdBox.Name, referencedIdBox);

			var nameBox = new ComboBox() { Name = "NameBox", Margin = new Thickness(10) };
			nameBox.SelectionChanged += NameBox_SelectionChanged;
			_controls.Add(nameBox.Name, nameBox);

			var rewardTypeBox = new ComboBox() { Name = "RewardTypeBox", ItemsSource = Enum.GetValues(typeof(RewardType)), Margin = new Thickness(10) };
			rewardTypeBox.SelectionChanged += RewardTypeBox_SelectionChanged;
			rewardTypeBox.SelectedValue = selectedVendorPattern.VendorItemType;

			var valueBox = new TextBox() { Name = "ValueBox", Text = selectedVendorPattern.Value.ToString(), Margin = new Thickness(10) };

			// Set TextBox and ComboBox hints.
			HintAssist.SetHint(idBox, "ID");
			HintAssist.SetHint(nameBox, "Name");
			HintAssist.SetHint(referencedIdBox, "ReferencedId");
			HintAssist.SetHint(rewardTypeBox, "RewardType");
			HintAssist.SetHint(valueBox, "Value");

			_controls.Add(idBox.Name, idBox);
			_controls.Add(valueBox.Name, valueBox);
			_controls.Add(rewardTypeBox.Name, rewardTypeBox);

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

			Grid.SetColumn(panel, 1);

			_currentPanel = panel;

			MainGrid.Children.Add(panel);
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

			_dataContext.VendorItemType = (RewardType)Enum.Parse(typeof(RewardType), (sender as ComboBox).SelectedValue.ToString());
			
			(_controls["NameBox"] as ComboBox).SelectedIndex = 0;
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

			switch (_dataContext.VendorItemType)
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

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			(sender as TextBox).CaretIndex = int.MaxValue;
		}

		public void Save()
		{
			var vendorPattern = _dataContext;

			if (vendorPattern is null)
			{
				return;
			}

			vendorPattern.Id = int.Parse((_controls["IdBox"] as TextBox).Text);
			vendorPattern.VendorItemId = int.Parse((_controls["ReferencedIdBox"] as TextBox).Text);
			vendorPattern.Value = int.Parse((_controls["ValueBox"] as TextBox).Text);
			vendorPattern.VendorItemType = (RewardType)Enum.Parse(typeof(RewardType), (_controls["RewardTypeBox"] as ComboBox).SelectedValue.ToString());

			// Check if this Id is already in the collection (modified).
			if (GameContent.PriestOffer.Select(x => x.Id).Contains(vendorPattern.Id))
			{
				int indexOfVendorPattern = GameContent.PriestOffer.FindIndex(x => x.Id == vendorPattern.Id);
				GameContent.PriestOffer[indexOfVendorPattern] = vendorPattern;
			}
			else
			{
				// If not, add it.
				GameContent.PriestOffer.Add(vendorPattern);
			}

			PopulateContentSelectionBox();
			ContentSelectionBox.SelectedValue = _dataContext.Id; // NARAZIE ID DLA TESTU
		}

		private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
		{
			Save();
			
			int nextId = GameContent.PriestOffer.Max(x => x.Id) + 1;
			_dataContext = new VendorPattern() { Id = nextId };
			ContentSelectionBox.SelectedIndex = -1;
			RefreshStaticValuesPanel();

			DeleteObjectButton.Visibility=Visibility.Visible;
		}

		private void DeleteObjectButton_Click(object sender, RoutedEventArgs e)
		{
			var objectToDelete = GameContent.PriestOffer.FirstOrDefault(x=>x.Id==int.Parse((_controls["IdBox"] as TextBox).Text));

			if (objectToDelete is null)
			{
				_currentPanel?.Children.Clear();
				DeleteObjectButton.Visibility=Visibility.Hidden;
				return;
			}

			var result = MessageBox.Show($"Are you sure you want to delete pattern of Id: {objectToDelete.Id}?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

			if (result == MessageBoxResult.No)
			{
				return;
			}

			GameContent.PriestOffer.Remove(objectToDelete);

			PopulateContentSelectionBox();
			ContentSelectionBox.SelectedIndex = -1;
			_currentPanel.Children.Clear();
			DeleteObjectButton.Visibility=Visibility.Hidden;
		}

		private void ContentSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedId = int.Parse((e.Source as ComboBox)?.SelectedValue?.ToString());

			if (selectedId == 0)
			{
				return;
			}

			if (_dataContext is not null)
			{
				Save();
			}

			_dataContext =  GameContent.PriestOffer.FirstOrDefault(x => x.Id == selectedId);
			RefreshStaticValuesPanel();
			DeleteObjectButton.Visibility=Visibility.Visible;
		}

	}
}