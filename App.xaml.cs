using ClickQuest.Data.GameData;
using ClickQuest.Player;
using System;
using System.Windows;
using ClickQuest.Artifacts;

namespace ClickQuest
{
	public partial class App : Application
	{
		protected void Application_Startup(object sender, StartupEventArgs e)
		{
			// Load JSONs and Entity.
			GameDataLoader.Load();

			var gameWindow = new GameWindow();
			Application.Current.MainWindow = gameWindow;
			gameWindow.Show();
			
			// Test
			Data.UserData.UserDataLoader.Load();
			
			// Save current time as the application's start time (for achievement tracking).
			User.SessionStartDate = DateTime.Now;
		}
	}
}