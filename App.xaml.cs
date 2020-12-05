using ClickQuest.Entity;
using System.Windows;

namespace ClickQuest
{
	public partial class App : Application
	{
		protected void Application_Startup(object sender, StartupEventArgs e)
		{
			using (var db = new UserContext())
			{
				db.Database.EnsureCreated();
			}

			Data.Database.Load();
			Entity.EntityOperations.LoadGame();
		}
	}
}