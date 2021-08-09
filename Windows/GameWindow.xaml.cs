using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ClickQuest.Controls;
using ClickQuest.Data.GameData;
using ClickQuest.Entity;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Pages;
using ClickQuest.Player;
using ClickQuest.Windows;

namespace ClickQuest
{
	public partial class GameWindow : Window, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private string _locationInfo;

		public string LocationInfo
		{
			get
			{
				return _locationInfo;
			}
			set
			{
				_locationInfo = value;
				SwitchLocationInfoBorderVisibility();
			}
		}

		public GameWindow()
		{
			InitializeComponent();

			DataContext = this;

			(GameData.Pages["MainMenu"] as MainMenuPage).UpdateCreateHeroButton();
			(GameData.Pages["MainMenu"] as MainMenuPage).UpdateSelectOrDeleteHeroButtons();

			InterfaceController.ChangePage(GameData.Pages["MainMenu"], "");
		}

		private void SwitchLocationInfoBorderVisibility()
		{
			LocationInfoBorder.Visibility = LocationInfo == "" ? Visibility.Hidden : Visibility.Visible;
		}

		private void DragableTop_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				DragMove();
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			User.Instance.Achievements.TotalTimePlayed += DateTime.Now - User.SessionStartDate;
			User.Instance.CurrentHero?.UpdateTimePlayed();
			User.Instance.CurrentHero?.PauseBlessing();

			// EntityOperations.SaveGame();

			Data.UserData.UserDataLoader.Save();

			base.OnClosing(e);
		}

		private void AchievementsButton_Click(object sender, RoutedEventArgs e)
		{
			AchievementsWindow.Instance.Show();
		}

		private void ExitButton_Click(object sender, RoutedEventArgs e)
		{
			var result = AlertBox.Show("Are you sure you want to quit?\nAll progress will be saved.");

			if (result == MessageBoxResult.OK)
			{
				Application.Current.Shutdown();
			}
		}
	}
}