using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Entity;
using ClickQuest.Items;
using ClickQuest.Pages;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ClickQuest
{
	public partial class GameWindow : Window, INotifyPropertyChanged
	{
		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		#endregion INotifyPropertyChanged

		#region Private Fields
		private string _locationInfo;

		#endregion

		#region Properties

		public string LocationInfo
		{
			get
			{
				return _locationInfo;
			}
			set
			{
				_locationInfo = value;

				// If empty, set border thickness to 0 to make it invisible in main menu.
				if (_locationInfo == "")
				{
					LocationInfoBorder.BorderThickness = new Thickness(0);
				}
				else
				{
					LocationInfoBorder.BorderThickness = new Thickness(2);
				}

				OnPropertyChanged();
			}
		}
		#endregion

		public GameWindow()
		{
			InitializeComponent();

			this.DataContext = this;

			(Database.Pages["MainMenu"] as MainMenuPage).GenerateHeroButtons();
			(Application.Current.MainWindow as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["MainMenu"]);
		}

		#region Events

		protected override void OnClosing(CancelEventArgs e)
		{
			// Pause all blessings.
			Blessing.PauseBlessings();

			EntityOperations.SaveGame();

			base.OnClosing(e);
		}

		private void DragableTop_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				this.DragMove();
			}
		}

		private void ExitButton_Click(object sender, RoutedEventArgs e)
		{
			var result = AlertBox.Show($"Are you sure you want to quit?\nAll progress will be saved.");

			if (result == MessageBoxResult.OK)
			{
				Application.Current.Shutdown();
			}
		}

		#endregion
	}
}
