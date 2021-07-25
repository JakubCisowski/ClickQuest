using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ClickQuest.Player;

namespace ClickQuest.Windows
{
	public partial class AchievementsWindow : Window
	{
		public AchievementsWindow()
		{
			InitializeComponent();
			RefreshAchievementsPanel();
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void AchievementsWindow_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				DragMove();
			}
		}

		public void RefreshAchievementsPanel()
		{
			AchievementsList.Children.Clear();

			var achievements = User.Instance.Achievements;

			var time = achievements.TotalTimePlayed;
			AppendAchievementToAchievementsList("TotalTimePlayed", $"{Math.Floor(time.TotalHours)}h {time.Minutes}m {time.Seconds}s");

			foreach (var pair in achievements.NumericAchievementCollection)
			{
				string name = pair.Key.ToString();

				// Add spaces in between words.
				name = string.Concat(name.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

				string amount = pair.Value.ToString();

				AppendAchievementToAchievementsList(name, amount);
			}
		}

		public new void Show()
		{
			_instance.Visibility = Visibility.Visible;
		}

		#region Singleton

		private static AchievementsWindow _instance;

		public static AchievementsWindow Instance
		{
			get
			{
				if (_instance is null)
				{
					_instance = new AchievementsWindow();
				}

				return _instance;
			}
		}

		#endregion

		#region Events

		private void AchievementsWindow_Closing(object sender, CancelEventArgs e)
		{
			// If the window is closed, keep it open but hide it instead.
			e.Cancel = true;
			Visibility = Visibility.Hidden;
		}

		private void AppendAchievementToAchievementsList(string name, string amount)
		{
			// Create a TextBlock.
			var border = new Border
			{
				BorderThickness = new Thickness(0.5),
				BorderBrush = new SolidColorBrush(Colors.Gray),
				Padding = new Thickness(6),
				Margin = new Thickness(1),
				Background = FindResource("GameBackgroundAdditional") as SolidColorBrush
			};

			var grid = new Grid();

			var nameBlock = new TextBlock {FontSize = 18, HorizontalAlignment = HorizontalAlignment.Left, Text = name};

			var amountBlock = new TextBlock {FontSize = 18, HorizontalAlignment = HorizontalAlignment.Right, Text = amount};

			grid.Children.Add(nameBlock);
			grid.Children.Add(amountBlock);

			border.Child = grid;

			AchievementsList.Children.Add(border);
		}

		#endregion
	}
}