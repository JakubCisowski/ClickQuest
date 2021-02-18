using ClickQuest.Data;
using ClickQuest.Entity;
using System.Windows;

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
		}
	}
}