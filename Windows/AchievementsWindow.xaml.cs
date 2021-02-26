using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ClickQuest.Account;
using System.Reflection;

namespace ClickQuest.Windows
{
	public partial class AchievementsWindow : Window
	{
		public AchievementsWindow()
		{
			InitializeComponent();
			UpdateAchievements();
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void AchievementsWindow_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				this.DragMove();
			}
		}

		public void UpdateAchievements()
		{
			// Refresh achievements panel.
			AchievementsList.Children.Clear();

			var achievements = User.Instance.Achievements;

			var properties = typeof(Achievements).GetProperties();
			foreach (var property in properties)
			{
				var name = property.Name;
				name = string.Concat(name.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
				var amount = property.GetValue(achievements);

				// Create a TextBlock.
				var border = new Border()
				{
					BorderThickness = new Thickness(0.5),
					BorderBrush = new SolidColorBrush(Colors.Gray),
					Padding = new Thickness(6),
					Margin = new Thickness(4),
					Background = this.FindResource("GameBackgroundAdditional") as SolidColorBrush
				};

				var grid = new Grid();

				var nameBlock = new TextBlock()
				{
					FontSize = 18,
					HorizontalAlignment = HorizontalAlignment.Left,
					Text = name
				};

				var amountBlock = new TextBlock()
				{
					FontSize = 18,
					HorizontalAlignment = HorizontalAlignment.Right,
					Text = amount.ToString()
				};

				grid.Children.Add(nameBlock);
				grid.Children.Add(amountBlock);

				border.Child = grid;

				AchievementsList.Children.Add(border);

			}
		}
	}
}
