using System;
using System.Windows;
using ClickQuest.Data.GameData;
using ClickQuest.Entity;
using ClickQuest.Player;

namespace ClickQuest
{
	public partial class App : Application
	{
		protected void Application_Startup(object sender, StartupEventArgs e)
		{
			// Load JSONs and Entity.
			GameDataLoader.Load();
			// EntityOperations.CreateAndSeedDatabase();
			// EntityOperations.LoadGame();

			// Test
			Data.UserData.UserDataLoader.Load();

			// Save current time as the application's start time (for achievement tracking).
			User.SessionStartDate = DateTime.Now;
		}
	}
}