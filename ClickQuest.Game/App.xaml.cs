using System;
using System.Windows;
using ClickQuest.Game.Data.GameData;
using ClickQuest.Game.Data.UserData;
using ClickQuest.Game.Pages;
using ClickQuest.Game.Player;

namespace ClickQuest.Game
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