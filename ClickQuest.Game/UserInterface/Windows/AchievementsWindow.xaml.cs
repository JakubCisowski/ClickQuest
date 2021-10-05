using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.UserInterface.Windows
{
	public partial class AchievementsWindow : Window
	{
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

		public AchievementsWindow()
		{
			InitializeComponent();
			RefreshAchievementsPanel();
		}

		public new void Show()
		{
			_instance.Visibility = Visibility.Visible;
		}

		public void RefreshAchievementsPanel()
		{
			AchievementsList.Children.Clear();

			var achievements = User.Instance.Achievements;

			var time = achievements.TotalTimePlayed;
			AppendAchievementToAchievementsList("Total Time Played", $"{Math.Floor(time.TotalHours)}h {time.Minutes}m");

			foreach (var pair in achievements.NumericAchievementCollection)
			{
				string name = pair.Key.ToString();

				// Add spaces in between words.
				name = string.Concat(name.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

				string amount = pair.Value.ToString();

				AppendAchievementToAchievementsList(name, amount);
			}
		}

		private void AppendAchievementToAchievementsList(string name, string amount)
		{
			string nameWithoutSpaces = name.Replace(" ", "");

			var border = new Border
			{
				Name = nameWithoutSpaces + "Border",
				BorderThickness = new Thickness(0.5),
				BorderBrush = new SolidColorBrush(Colors.Gray),
				Padding = new Thickness(6),
				Margin = new Thickness(1),
				Background = FindResource("GameBackground3") as SolidColorBrush
			};

			var grid = new Grid();

			var nameBlock = new TextBlock
			{
				Name = nameWithoutSpaces + "Name",
				FontSize = 18,
				HorizontalAlignment = HorizontalAlignment.Left,
				Text = name
			};

			var amountBlock = new TextBlock
			{
				Name = nameWithoutSpaces + "Amount",
				FontSize = 18,
				HorizontalAlignment = HorizontalAlignment.Right,
				Text = amount
			};

			grid.Children.Add(nameBlock);
			grid.Children.Add(amountBlock);

			border.Child = grid;

			AchievementsList.Children.Add(border);
		}

		public void RefreshSingleAchievement(NumericAchievementType achievementType)
		{
			var achievements = User.Instance.Achievements;

			var achievementBorder = AchievementsList.Children.OfType<Border>().FirstOrDefault(x => x.Name == achievementType + "Border");

			if (achievementBorder != null)
			{
				var achievementAmountBlock = (achievementBorder.Child as Grid)?.Children.OfType<TextBlock>().FirstOrDefault(x => x.Name == achievementType + "Amount");

				if (achievementAmountBlock != null)
				{
					achievementAmountBlock.Text = achievements.NumericAchievementCollection[achievementType].ToString();
				}
			}
		}

		private void AchievementsWindow_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				DragMove();
			}
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void AchievementsWindow_Closing(object sender, CancelEventArgs e)
		{
			// If the window is closed, keep it open but hide it instead.
			e.Cancel = true;
			Visibility = Visibility.Hidden;
		}
	}
}