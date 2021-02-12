using System.Windows;
using System.Windows.Input;

namespace ClickQuest.Controls
{
    public partial class AlertBox : Window
    {
		#region Private Fields
        private static MessageBoxResult Result = MessageBoxResult.OK;
        private static AlertBox MessageBox;

		#endregion

        public AlertBox()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        public static MessageBoxResult Show(string content)
        {
            MessageBox = new AlertBox()
            {
                ContentBox = { Text = content }
            };

            MessageBox.ShowDialog();

            return Result;
        }

		#region Events

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

        private void AlertBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

		#endregion
    }
}