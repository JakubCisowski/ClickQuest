using ClickQuest.Data.GameData;
using ClickQuest.Player;
using System;
using System.Windows;
using ClickQuest.Artifacts;
using ClickQuest.Pages;

namespace ClickQuest
{
	public partial class App : Application
	{
		protected void Application_Startup(object sender, StartupEventArgs e)
		{
			// Load JSONs.
			GameDataLoader.Load();

			var gameWindow = new GameWindow();
			Application.Current.MainWindow = gameWindow;
			gameWindow.Show();
			
			Data.UserData.UserDataLoader.Load();
			
			(GameData.Pages["MainMenu"] as MainMenuPage).UpdateSelectOrDeleteHeroButtons();
			
			// Save current time as the application's start time (for achievement tracking).
			User.SessionStartDate = DateTime.Now;
		}
	}
}