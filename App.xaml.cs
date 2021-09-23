using System;
using System.Windows;
using ClickQuest.Data.GameData;
using ClickQuest.Data.UserData;
using ClickQuest.Pages;
using ClickQuest.Player;

namespace ClickQuest
{
	public partial class App : Application
	{
		protected void Application_Startup(object sender, StartupEventArgs e)
		{
			// Load JSONs.
			GameDataLoader.Load();

			var gameWindow = new GameWindow();
			Current.MainWindow = gameWindow;
			gameWindow.Show();

			UserDataLoader.Load();

			(GameData.Pages["MainMenu"] as MainMenuPage).UpdateSelectOrDeleteHeroButtons();

			// Save current time as the application's start time (for achievement tracking).
			User.SessionStartDate = DateTime.Now;
		}
	}
}