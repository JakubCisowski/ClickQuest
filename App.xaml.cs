using ClickQuest.Data;
using ClickQuest.Entity;
using System.Windows;
using System;
using ClickQuest.Player;

namespace ClickQuest
{
	public partial class App : Application
	{
		protected void Application_Startup(object sender, StartupEventArgs e)
		{
			EntityOperations.CreateAndSeedDatabase();

			// Load JSONs and Entity.
			Database.Load();
			EntityOperations.LoadGame();

			// Save current time as the application's start time (for achievement tracking).
			User.SessionStartDate = DateTime.Now;
		}
	}
}