using System;
using System.Windows;
using ClickQuest.Data;
using ClickQuest.Entity;
using ClickQuest.Player;

namespace ClickQuest
{
	public partial class App : Application
	{
		protected void Application_Startup(object sender, StartupEventArgs e)
		{
			// Load JSONs and Entity.
			DataLoader.Load();
			EntityOperations.CreateAndSeedDatabase();
			EntityOperations.LoadGame();

			// Save current time as the application's start time (for achievement tracking).
			User.SessionStartDate = DateTime.Now;
		}
	}
}