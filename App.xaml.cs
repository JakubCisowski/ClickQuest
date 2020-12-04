using System;
using System.Linq;
using System.Windows;
using ClickQuest.Entity;
using ClickQuest.Items;

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