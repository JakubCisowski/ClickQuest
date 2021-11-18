using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace ClickQuest.Game.UserInterface.Controls
{
	public partial class AlertBox : Window
	{
		private static MessageBoxResult Result = MessageBoxResult.OK;
		private static AlertBox MessageBox;

		public AlertBox()
		{
			InitializeComponent();
			Owner = Application.Current.MainWindow;
		}

		public static MessageBoxResult Show(string content, MessageBoxButton buttons = MessageBoxButton.YesNo)
		{
			// Display a single string message on the screen.
			
			MessageBox = new AlertBox
			{
				ContentBox =
				{
					Text = content
				}
			};

			switch (buttons)
			{
				case MessageBoxButton.YesNo:
				{
					MessageBox.OkButton.Visibility = Visibility.Collapsed;
					MessageBox.YesButton.Visibility = Visibility.Visible;
					MessageBox.NoButton.Visibility = Visibility.Visible;
				}
					break;

				case MessageBoxButton.OK:
				{
					MessageBox.OkButton.Visibility = Visibility.Visible;
					MessageBox.YesButton.Visibility = Visibility.Collapsed;
					MessageBox.NoButton.Visibility = Visibility.Collapsed;
				}
					break;
			}

			MessageBox.ShowDialog();

			return Result;
		}

		public static MessageBoxResult Show(List<Run> textRuns, MessageBoxButton buttons = MessageBoxButton.YesNo)
		{
			// Display a complex message that supports coloring and styling on the screen.
			
			MessageBox = new AlertBox();

			MessageBox.ContentBox.Inlines.AddRange(textRuns);

			switch (buttons)
			{
				case MessageBoxButton.YesNo:
				{
					MessageBox.OkButton.Visibility = Visibility.Collapsed;
					MessageBox.YesButton.Visibility = Visibility.Visible;
					MessageBox.NoButton.Visibility = Visibility.Visible;
				}
					break;

				case MessageBoxButton.OK:
				{
					MessageBox.OkButton.Visibility = Visibility.Visible;
					MessageBox.YesButton.Visibility = Visibility.Collapsed;
					MessageBox.NoButton.Visibility = Visibility.Collapsed;
				}
					break;
			}

			MessageBox.ShowDialog();

			return Result;
		}


		private void AlertBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				DragMove();
			}
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.OK;
			MessageBox.Close();
			MessageBox = null;
		}

		private void YesButton_Click(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.Yes;
			MessageBox.Close();
			MessageBox = null;
		}

		private void NoButton_Click(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.No;
			MessageBox.Close();
			MessageBox = null;
		}
	}
}