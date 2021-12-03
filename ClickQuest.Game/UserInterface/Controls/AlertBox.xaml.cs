using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace ClickQuest.Game.UserInterface.Controls
{
    public partial class AlertBox : Window
    {
        private static MessageBoxResult _result = MessageBoxResult.OK;
        private static AlertBox _messageBox;

        public AlertBox()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        public static MessageBoxResult Show(string content, MessageBoxButton buttons = MessageBoxButton.YesNo)
        {
            // Display a single string message on the screen.

            _messageBox = new AlertBox
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
                        _messageBox.OkButton.Visibility = Visibility.Collapsed;
                        _messageBox.YesButton.Visibility = Visibility.Visible;
                        _messageBox.NoButton.Visibility = Visibility.Visible;
                    }
                    break;

                case MessageBoxButton.OK:
                    {
                        _messageBox.OkButton.Visibility = Visibility.Visible;
                        _messageBox.YesButton.Visibility = Visibility.Collapsed;
                        _messageBox.NoButton.Visibility = Visibility.Collapsed;
                    }
                    break;
            }

            _messageBox.ShowDialog();

            return _result;
        }

        public static MessageBoxResult Show(List<Run> textRuns, MessageBoxButton buttons = MessageBoxButton.YesNo)
        {
            // Display a complex message that supports coloring and styling on the screen.

            _messageBox = new AlertBox();

            _messageBox.ContentBox.Inlines.AddRange(textRuns);

            switch (buttons)
            {
                case MessageBoxButton.YesNo:
                    {
                        _messageBox.OkButton.Visibility = Visibility.Collapsed;
                        _messageBox.YesButton.Visibility = Visibility.Visible;
                        _messageBox.NoButton.Visibility = Visibility.Visible;
                    }
                    break;

                case MessageBoxButton.OK:
                    {
                        _messageBox.OkButton.Visibility = Visibility.Visible;
                        _messageBox.YesButton.Visibility = Visibility.Collapsed;
                        _messageBox.NoButton.Visibility = Visibility.Collapsed;
                    }
                    break;
            }

            _messageBox.ShowDialog();

            return _result;
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
            _result = MessageBoxResult.OK;
            _messageBox.Close();
            _messageBox = null;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Yes;
            _messageBox.Close();
            _messageBox = null;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.No;
            _messageBox.Close();
            _messageBox = null;
        }
    }
}