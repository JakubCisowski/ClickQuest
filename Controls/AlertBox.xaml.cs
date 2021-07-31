using System.Windows;
using System.Windows.Input;

namespace ClickQuest.Controls
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
			MessageBox = new AlertBox {ContentBox = {Text = content}};

			switch (buttons)
			{
				case MessageBoxButton.YesNo:
				{
					MessageBox.OkButton2.Visibility = Visibility.Hidden;
					MessageBox.OkButton.Visibility = Visibility.Visible;
					MessageBox.CancelButton.Visibility = Visibility.Visible;
				}
					break;

				case MessageBoxButton.OK:
				{
					MessageBox.OkButton2.Visibility = Visibility.Visible;
					MessageBox.OkButton.Visibility = Visibility.Hidden;
					MessageBox.CancelButton.Visibility = Visibility.Hidden;
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

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.Cancel;
			MessageBox.Close();
			MessageBox = null;
		}
	}
}